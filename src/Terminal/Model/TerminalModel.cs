using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Communication.Annotations;
using Communication.Settings;
using Communication.TcpIp;
using CSharpFunctionalExtensions;
using Library.Logs;
using Library.Xml;
using Terminal.Infrastructure;
using Terminal.Service;
using Terminal.Settings;


namespace Terminal.Model
{
    public class TerminalModel : INotifyPropertyChanged, IDisposable
    {
        private readonly Log _logger = new Log("Terminal.CommandAddItem");



        #region prop

        public MasterTcpIp MasterTcpIp { get; private set; }
        public PrintTicket PrintTicketService { get; private set; }
        public PrefixesConfig PrefixesConfig { get; private set; }

        public bool IsConnectTcpIp => (MasterTcpIp != null && MasterTcpIp.IsConnect);


        private string _errorString;
        public string ErrorString
        {
            get => _errorString;
            set
            {
                if (value == _errorString) return;
                _errorString = value;
                OnPropertyChanged();
            }
        }

        #endregion




        #region Events

        public event Func<string, string, string, bool> ConfirmationAdded;
        private bool OnConfirmationAdded(string arg1, string arg2, string arg3)
        {
            var res = ConfirmationAdded?.Invoke(arg1, arg2, arg3);
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
            XmlPrefixesConfigSetting xmlPrefixesConfig;
            try
            {
                var xmlFile = XmlWorker.LoadXmlFile("Settings", "Setting.xml");
                if (xmlFile == null)
                    return;

                xmlTerminal = XmlMasterSettings.LoadXmlSetting(xmlFile);
                xmlPrinter = XmlPrinterSettings.LoadXmlSetting(xmlFile);
                xmlPrefixesConfig = XmlPrefixesConfigSetting.LoadXmlSetting(xmlFile);
            }
            catch (FileNotFoundException ex)
            {
                ErrorString = ex.ToString();
                return;
            }
            catch (FormatException ex)
            {
                ErrorString = "ОШИБКА в формате тега в XML  файле:  "+ ex;
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
                PrintTicketService = new PrintTicket(xmlPrinter); 
                PrefixesConfig= new PrefixesConfig(xmlPrefixesConfig.PrefixDict);
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


        public async Task QueueSelection(string nameQueue, string prefixQueue, string descriptionQueue)
        {
            if(!IsConnectTcpIp)
                return;

            try
            {
                //ЗАПРОС О СОСТОЯНИИ ОЧЕРЕДИ
                var provider = new Terminal2ServerExchangeDataProvider { InputData = new TerminalInData { NameQueue = nameQueue, PrefixQueue = prefixQueue, Action = TerminalAction.Info } };
                await MasterTcpIp.RequestAndRespouneAsync(provider);

                if (provider.IsOutDataValid)
                {
                    var prefix = provider.OutputData.PrefixQueue;
                    var ticketName = prefix + provider.OutputData.NumberElement.ToString("000");
                    var countPeople = provider.OutputData.CountElement.ToString();

                    var isAdded = OnConfirmationAdded(ticketName, countPeople, descriptionQueue);
                    if (isAdded)
                    {
                        //ЗАПРОС О ДОБАВЛЕНИИ ЭЛЕМЕНТА В ОЧЕРЕДЬ
                        provider = new Terminal2ServerExchangeDataProvider { InputData = new TerminalInData { NameQueue = nameQueue, PrefixQueue = prefixQueue, Action = TerminalAction.Add } };
                        await MasterTcpIp.RequestAndRespouneAsync(provider);

                        if (provider.IsOutDataValid)
                        {
                            prefix = provider.OutputData.PrefixQueue;
                            ticketName = prefix + provider.OutputData.NumberElement.ToString("000");
                            countPeople = provider.OutputData.CountElement.ToString();

                            PrintTicketService.Print(ticketName, countPeople, provider.OutputData.AddedTime, descriptionQueue);

                            _logger.Info($"PrintTicket: {provider.OutputData.AddedTime}     {ticketName}    nameQueue= {nameQueue}   descriptionQueue= {descriptionQueue}");
                        }
                    }
                    else
                    {
                        // "НЕ добавлять"
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"TerminalModel/QueueSelection()=   {ex}");
            }
        }


        /// <summary>
        /// Проверка рабочего диапазона работы кассы
        /// </summary>
        /// <returns>false - запрет ограничения  true - ограничение</returns>
        public (PermitTime workTime, bool isPermited) CheckWorkPermitTime(string prefixQueue)
        {
            var (_, isFailure, value) = PrefixesConfig.GetConf(prefixQueue);
            if (isFailure)
                throw new Exception($"В настройки не внесен конфиг для префикса '{prefixQueue}'");

            var permitedTime= value.CheckPermitRange();
            return permitedTime == null ? (null, false) : (permitedTime, true);
        }

        #endregion




        #region IDisposable

        public void Dispose()
        {
            //MasterTcpIp?.Dispose();
            PrintTicketService?.Dispose();
        }

        #endregion


    }
}