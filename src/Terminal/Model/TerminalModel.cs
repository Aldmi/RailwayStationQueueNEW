using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Communication.Annotations;
using Communication.Settings;
using Communication.TcpIp;
using Library.Xml;
using Terminal.Infrastructure;
using Terminal.Service;
using Terminal.Settings;


namespace Terminal.Model
{
    public class TerminalModel : INotifyPropertyChanged, IDisposable
    {
        #region prop

        public MasterTcpIp MasterTcpIp { get; set; }
        public PrintTicket PrintTicket { get; set; }

        private string _errorString;
        public string ErrorString
        {
            get { return _errorString; }
            set
            {
                if (value == _errorString) return;
                _errorString = value;
                OnPropertyChanged();
            }
        }

        #endregion




        #region Events

        public event Func<string, string, bool> ConfirmationAdded;
        private bool OnConfirmationAdded(string arg1, string arg2)
        {
            var res = ConfirmationAdded?.Invoke(arg1, arg2);
            return res != null && res.Value;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion





        #region Methods

        public void LoadSetting()
        {
            XmlMasterSettings xmlTerminal;
            XmlPrinterSettings xmlPrinter;
            try
            {
                var xmlFile = XmlWorker.LoadXmlFile("Settings", "Setting.xml");
                if (xmlFile == null)
                    return;

                xmlTerminal = XmlMasterSettings.LoadXmlSetting(xmlFile);
                xmlPrinter = XmlPrinterSettings.LoadXmlSetting(xmlFile);
            }
            catch (FileNotFoundException ex)
            {
                ErrorString = ex.ToString();
                return;
            }
            catch (Exception ex)
            {
                ErrorString = "ОШИБКА в узлах дерева XML файла настроек:  " + ex;
                return;
            }

            
            try
            {
                MasterTcpIp = new MasterTcpIp(xmlTerminal);
                PrintTicket = new PrintTicket(xmlPrinter); 
            }
            catch (Exception ex)
            {
                ErrorString = ex.ToString();
            }
        }


        public async Task Start()
        {
            if (MasterTcpIp != null)
            {
                  await MasterTcpIp.ReConnect();
            }
        }


        public async Task TrainSelection(byte numberQueue)
        {
            if (MasterTcpIp == null || !MasterTcpIp.IsConnect)
                return;

            //ЗАПРОС О СОСТОЯНИИ ОЧЕРЕДИ
            var provider = new Terminal2ServerExchangeDataProvider { InputData = new TerminalInData { NumberQueue = numberQueue, Action = TerminalAction.Info } };
            await MasterTcpIp.RequestAndRespoune(provider);

   
            if (provider.IsOutDataValid)
            {
                var prefix = provider.OutputData.NumberQueue == 1 ? "A" : "B";
                var ticketName = prefix + provider.OutputData.NumberElement.ToString("000");
                var countPeople = provider.OutputData.CountElement.ToString();

                var isAdded = OnConfirmationAdded(ticketName, countPeople);
                if (isAdded)
                {
                    //ЗАПРОС О ДОБАВЛЕНИИ ЭЛЕМЕНТА В ОЧЕРЕДЬ
                    provider = new Terminal2ServerExchangeDataProvider { InputData = new TerminalInData { NumberQueue = numberQueue, Action = TerminalAction.Add } };
                    await MasterTcpIp.RequestAndRespoune(provider);

                    if (provider.IsOutDataValid)
                    {
                        prefix = provider.OutputData.NumberQueue == 1 ? "A" : "B";
                        ticketName = prefix + provider.OutputData.NumberElement.ToString("000");
                        countPeople = provider.OutputData.CountElement.ToString();

                        PrintTicket.Print(ticketName, countPeople, provider.OutputData.AddedTime);
                    }
                }
                else
                {
                    // "НЕ добавлять"
                }
            }
        }

        #endregion




        #region IDisposable

        public void Dispose()
        {
            //MasterTcpIp?.Dispose();
            PrintTicket?.Dispose();
        }

        #endregion
    }
}