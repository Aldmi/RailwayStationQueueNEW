using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Communication.Annotations;
using Communication.Interfaces;
using Communication.SerialPort;
using Communication.Settings;
using Communication.TcpIp;
using Library.Logs;
using Library.Xml;
using Server.Entitys;
using Server.Infrastructure;
using Server.Service;
using Server.Settings;
using Terminal.Infrastructure;

namespace Server.Model
{
    public class ServerModel : INotifyPropertyChanged, IDisposable
    {
        #region prop

        public TicketFactory TicketFactoryVilage { get; } = new TicketFactory("A");
        public TicketFactory TicketFactoryLong { get; } = new TicketFactory("B");

        public Queue<TicketItem> QueueVilage { get; set; } = new Queue<TicketItem>();
        public Queue<TicketItem> QueueLong { get; set; } = new Queue<TicketItem>();

        public Log LogTicket { get; set; }

        public ListenerTcpIp Listener { get; set; }
        public IExchangeDataProvider<TerminalInData, TerminalOutData> ProviderTerminal { get; set; }

        public MasterSerialPort MasterSerialPort { get; set; }
        public List<DeviceCashier> DeviceCashiers { get; set; } = new List<DeviceCashier>();
        public CashierExchangeService CashierExchangeService { get; set; }

        public List<Task> BackGroundTasks { get; set; } = new List<Task>();

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
            //ЗАГРУЗКА НАСТРОЕК----------------------------------------------------------------
            XmlListenerSettings xmlListener;
            XmlSerialSettings xmlSerial;
            XmlLogSettings xmlLog;
            List<XmlCashierSettings> xmlCashier;
            try
            {
                var xmlFile = XmlWorker.LoadXmlFile("Settings", "Setting.xml"); //все настройки в одном файле
                if (xmlFile == null)
                    return;

                xmlListener = XmlListenerSettings.LoadXmlSetting(xmlFile);
                xmlSerial = XmlSerialSettings.LoadXmlSetting(xmlFile);
                xmlLog = XmlLogSettings.LoadXmlSetting(xmlFile);
                xmlCashier = XmlCashierSettings.LoadXmlSetting(xmlFile);
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


            //СОЗДАНИЕ ЛОГА--------------------------------------------------------------------------
            LogTicket = new Log("TicketLog.txt", xmlLog);


            //СОЗДАНИЕ СЛУШАТЕЛЯ ДЛЯ ТЕРМИНАЛОВ-------------------------------------------------------
            Listener = new ListenerTcpIp(xmlListener);
            ProviderTerminal = new Server2TerminalExchangeDataProvider();
            ProviderTerminal.PropertyChanged += (o, e) =>
            {
                var provider = o as Server2TerminalExchangeDataProvider;
                if (provider != null)
                {
                    if (e.PropertyName == "InputData")
                    {
                        TicketItem ticket;
                        provider.OutputData = provider.OutputData ?? new TerminalOutData();
                        switch (provider.InputData.NumberQueue)
                        {
                            //ПРИГОРОДНЫЕ КАСС
                            case 1:
                                switch (provider.InputData.Action)
                                {
                                    //ИНФОРМАЦИЯ ОБ ОЧЕРЕДИ
                                    case TerminalAction.Info:
                                        provider.OutputData.NumberQueue = provider.InputData.NumberQueue;
                                        provider.OutputData.CountElement = (ushort)QueueVilage.Count;
                                        provider.OutputData.NumberElement = (ushort)(TicketFactoryVilage.GetCurrentTicketNumber + 1);
                                        provider.OutputData.AddedTime = DateTime.Now;
                                        break;

                                    //ДОБАВИТЬ БИЛЕТ В ОЧЕРЕДЬ
                                    case TerminalAction.Add:
                                        ticket = TicketFactoryVilage.Create((ushort)QueueVilage.Count);

                                        provider.OutputData.NumberQueue = provider.InputData.NumberQueue;
                                        provider.OutputData.CountElement = ticket.CountElement;
                                        provider.OutputData.NumberElement = (ushort)ticket.NumberElement;
                                        provider.OutputData.AddedTime = ticket.AddedTime;

                                        QueueVilage.Enqueue(ticket);
                                        break;
                                }
                                break;

                            //ДАЛЬНЕГО СЛЕДОВАНИЯ КАССЫ
                            case 2:
                                switch (provider.InputData.Action)
                                {
                                    //ИНФОРМАЦИЯ ОБ ОЧЕРЕДИ
                                    case TerminalAction.Info:
                                        provider.OutputData.NumberQueue = provider.InputData.NumberQueue;
                                        provider.OutputData.CountElement = (ushort)QueueLong.Count;
                                        provider.OutputData.NumberElement = (ushort)(TicketFactoryLong.GetCurrentTicketNumber + 1);
                                        provider.OutputData.AddedTime = DateTime.Now;
                                        break;

                                    //ДОБАВИТЬ БИЛЕТ В ОЧЕРЕДЬ
                                    case TerminalAction.Add:
                                        ticket = TicketFactoryLong.Create((ushort)QueueLong.Count);

                                        provider.OutputData.NumberQueue = provider.InputData.NumberQueue;
                                        provider.OutputData.CountElement = ticket.CountElement;
                                        provider.OutputData.NumberElement = (ushort)ticket.NumberElement;
                                        provider.OutputData.AddedTime = ticket.AddedTime;

                                        QueueLong.Enqueue(ticket);
                                        break;
                                }
                                break;
                        }
                    }
                }
            };


            //СОЗДАНИЕ КАССИРОВ------------------------------------------------------------------------------------------------
            foreach (var xmlCash in xmlCashier)
            {
                var casher = new Сashier(xmlCash.Id, (xmlCash.Prefix == "A") ? QueueVilage : QueueLong, xmlCash.MaxCountTryHanding);
                DeviceCashiers.Add(new DeviceCashier(casher));
            }


            //СОЗДАНИЕ ПОСЛЕД. ПОРТА ДЛЯ ОПРОСА КАССИРОВ-----------------------------------------------------------------------
            MasterSerialPort = new MasterSerialPort(xmlSerial);
            CashierExchangeService = new CashierExchangeService(DeviceCashiers, xmlSerial.TimeRespoune);
            MasterSerialPort.AddFunc(CashierExchangeService.ExchangeService);
            MasterSerialPort.PropertyChanged += (o, e) =>
            {
                var port = o as MasterSerialPort;
                if (port != null)
                {
                    if (e.PropertyName == "StatusString")
                    {
                        ErrorString = port.StatusString;
                    }
                }
            };

        }


        public async Task Start()
        {
            //ЗАПУСК СЛУШАТЕЛЯ ДЛЯ ТЕРМИНАЛОВ---------------------------------------------------------
            if (Listener != null)
            {
                var taskListener = Listener.RunServer(ProviderTerminal);
                BackGroundTasks.Add(taskListener);
            }

            //ЗАПУСК ОПРОСА КАССИРОВ-------------------------------------------------------------------
            if (MasterSerialPort != null)
            {
                var taskSerialPort = Task.Factory.StartNew(async () =>
                {
                    if (await MasterSerialPort.CycleReConnect())
                    {
                        var taskCashierEx = MasterSerialPort.RunExchange();
                        BackGroundTasks.Add(taskCashierEx);
                    }
                });
                BackGroundTasks.Add(taskSerialPort);
            }

            //КОНТРОЛЬ ФОНОВЫХ ЗАДАЧ
            var taskFirst = await Task.WhenAny(BackGroundTasks);
            if (taskFirst.Exception != null)                           //критическая ошибка фоновой задачи
                ErrorString = taskFirst.Exception.ToString();
        }

        #endregion




        #region IDisposable

        public void Dispose()
        {
            Listener?.Dispose();
            MasterSerialPort?.Dispose();
        }

        #endregion
    }
}