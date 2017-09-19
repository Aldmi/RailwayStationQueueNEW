using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.IO.Ports;
using System.Linq;
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
using Sound;
using Terminal.Infrastructure;

namespace Server.Model
{
    public class ServerModel : INotifyPropertyChanged, IDisposable
    {
        #region prop

        public TicketFactory TicketFactoryGetHelp { get; } = new TicketFactory("C");              //Получить справку
        public TicketFactory TicketFactoryBuyTicket { get; } = new TicketFactory("T");            //Купить билет
        public TicketFactory TicketFactoryBuyInterstateTicket { get; } = new TicketFactory("M");  //Купить билет меж государственного сообщения
        public TicketFactory TicketFactoryBaggageCheckout { get; } = new TicketFactory("B");      //Оформить багаж
        public TicketFactory TicketFactoryAdmin { get; } = new TicketFactory("A");                //Администратор

        public Queue<TicketItem> QueueGetHelp { get; set; } = new Queue<TicketItem>();
        public Queue<TicketItem> QueueBuyTicket { get; set; } = new Queue<TicketItem>();
        public Queue<TicketItem> QueueBuyInterstateTicket { get; set; } = new Queue<TicketItem>();
        public Queue<TicketItem> QueueBaggageCheckout { get; set; } = new Queue<TicketItem>();
        public Queue<TicketItem> QueueAdmin { get; set; } = new Queue<TicketItem>();

        public Log LogTicket { get; set; }

        public ListenerTcpIp Listener { get; set; }
        public IExchangeDataProvider<TerminalInData, TerminalOutData> ProviderTerminal { get; set; }

        //public SoundPlayer SoundPlayer { get; set; } = new SoundPlayer();

        public List<MasterSerialPort> MasterSerialPorts { get; set; } = new List<MasterSerialPort>();
        public List<DeviceCashier> DeviceCashiers { get; set; } = new List<DeviceCashier>();
        public List<CashierExchangeService> CashierExchangeServices { get; set; } = new List<CashierExchangeService>();

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
            IList<XmlSerialSettings> xmlSerials;
            XmlLogSettings xmlLog;
            List<XmlCashierSettings> xmlCashier;
            try
            {
                var xmlFile = XmlWorker.LoadXmlFile("Settings", "Setting.xml"); //все настройки в одном файле
                if (xmlFile == null)
                    return;

                xmlListener = XmlListenerSettings.LoadXmlSetting(xmlFile);
                xmlSerials = XmlSerialSettings.LoadXmlSetting(xmlFile).ToList();
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
                        switch ((char)provider.InputData.NumberQueue)
                        {
                            //ПОЛУЧИТЬ СПРАВКУ---------------------------------------------------------------------
                            case 'C':
                                switch (provider.InputData.Action)
                                {
                                    //ИНФОРМАЦИЯ ОБ ОЧЕРЕДИ
                                    case TerminalAction.Info:
                                        provider.OutputData.NumberQueue = provider.InputData.NumberQueue;
                                        provider.OutputData.CountElement = (ushort)QueueGetHelp.Count;
                                        provider.OutputData.NumberElement = (ushort)(TicketFactoryGetHelp.GetCurrentTicketNumber + 1);
                                        provider.OutputData.AddedTime = DateTime.Now;
                                        break;

                                    //ДОБАВИТЬ БИЛЕТ В ОЧЕРЕДЬ
                                    case TerminalAction.Add:
                                        ticket = TicketFactoryGetHelp.Create((ushort)QueueGetHelp.Count);

                                        provider.OutputData.NumberQueue = provider.InputData.NumberQueue;
                                        provider.OutputData.CountElement = ticket.CountElement;
                                        provider.OutputData.NumberElement = (ushort)ticket.NumberElement;
                                        provider.OutputData.AddedTime = ticket.AddedTime;

                                        QueueGetHelp.Enqueue(ticket);
                                        break;
                                }
                                break;

                            //КУПИТЬ БИЛЕТ--------------------------------------------------------------------------
                            case 'T':
                                switch (provider.InputData.Action)
                                {
                                    //ИНФОРМАЦИЯ ОБ ОЧЕРЕДИ
                                    case TerminalAction.Info:
                                        provider.OutputData.NumberQueue = provider.InputData.NumberQueue;
                                        provider.OutputData.CountElement = (ushort)QueueBuyTicket.Count;
                                        provider.OutputData.NumberElement = (ushort)(TicketFactoryBuyTicket.GetCurrentTicketNumber + 1);
                                        provider.OutputData.AddedTime = DateTime.Now;
                                        break;

                                    //ДОБАВИТЬ БИЛЕТ В ОЧЕРЕДЬ
                                    case TerminalAction.Add:
                                        ticket = TicketFactoryBuyTicket.Create((ushort)QueueBuyTicket.Count);

                                        provider.OutputData.NumberQueue = provider.InputData.NumberQueue;
                                        provider.OutputData.CountElement = ticket.CountElement;
                                        provider.OutputData.NumberElement = (ushort)ticket.NumberElement;
                                        provider.OutputData.AddedTime = ticket.AddedTime;

                                        QueueBuyTicket.Enqueue(ticket);
                                        break;
                                }
                                break;

                            //КУПИТЬ БИЛЕТ МЕЖ ГОСУДАРСТВЕННОГО СООБЩЕНИЯ--------------------------------------------------------------------------
                            case 'M':
                                switch (provider.InputData.Action)
                                {
                                    //ИНФОРМАЦИЯ ОБ ОЧЕРЕДИ
                                    case TerminalAction.Info:
                                        provider.OutputData.NumberQueue = provider.InputData.NumberQueue;
                                        provider.OutputData.CountElement = (ushort)QueueBuyInterstateTicket.Count;
                                        provider.OutputData.NumberElement = (ushort)(TicketFactoryBuyInterstateTicket.GetCurrentTicketNumber + 1);
                                        provider.OutputData.AddedTime = DateTime.Now;
                                        break;

                                    //ДОБАВИТЬ БИЛЕТ В ОЧЕРЕДЬ
                                    case TerminalAction.Add:
                                        ticket = TicketFactoryBuyInterstateTicket.Create((ushort)QueueBuyInterstateTicket.Count);

                                        provider.OutputData.NumberQueue = provider.InputData.NumberQueue;
                                        provider.OutputData.CountElement = ticket.CountElement;
                                        provider.OutputData.NumberElement = (ushort)ticket.NumberElement;
                                        provider.OutputData.AddedTime = ticket.AddedTime;

                                        QueueBuyInterstateTicket.Enqueue(ticket);
                                        break;
                                }
                                break;

                            //ОФОРМИТЬ БАГАЖ--------------------------------------------------------------------------
                            case 'B':
                                switch (provider.InputData.Action)
                                {
                                    //ИНФОРМАЦИЯ ОБ ОЧЕРЕДИ
                                    case TerminalAction.Info:
                                        provider.OutputData.NumberQueue = provider.InputData.NumberQueue;
                                        provider.OutputData.CountElement = (ushort)QueueBaggageCheckout.Count;
                                        provider.OutputData.NumberElement = (ushort)(TicketFactoryBaggageCheckout.GetCurrentTicketNumber + 1);
                                        provider.OutputData.AddedTime = DateTime.Now;
                                        break;

                                    //ДОБАВИТЬ БИЛЕТ В ОЧЕРЕДЬ
                                    case TerminalAction.Add:
                                        ticket = TicketFactoryBaggageCheckout.Create((ushort)QueueBaggageCheckout.Count);

                                        provider.OutputData.NumberQueue = provider.InputData.NumberQueue;
                                        provider.OutputData.CountElement = ticket.CountElement;
                                        provider.OutputData.NumberElement = (ushort)ticket.NumberElement;
                                        provider.OutputData.AddedTime = ticket.AddedTime;

                                        QueueBaggageCheckout.Enqueue(ticket);
                                        break;
                                }
                                break;

                            //АДМИНИСТРАТОР--------------------------------------------------------------------------
                            case 'A':
                                switch (provider.InputData.Action)
                                {
                                    //ИНФОРМАЦИЯ ОБ ОЧЕРЕДИ
                                    case TerminalAction.Info:
                                        provider.OutputData.NumberQueue = provider.InputData.NumberQueue;
                                        provider.OutputData.CountElement = (ushort)QueueAdmin.Count;
                                        provider.OutputData.NumberElement = (ushort)(TicketFactoryAdmin.GetCurrentTicketNumber + 1);
                                        provider.OutputData.AddedTime = DateTime.Now;
                                        break;

                                    //ДОБАВИТЬ БИЛЕТ В ОЧЕРЕДЬ
                                    case TerminalAction.Add:
                                        ticket = TicketFactoryAdmin.Create((ushort)QueueAdmin.Count);

                                        provider.OutputData.NumberQueue = provider.InputData.NumberQueue;
                                        provider.OutputData.CountElement = ticket.CountElement;
                                        provider.OutputData.NumberElement = (ushort)ticket.NumberElement;
                                        provider.OutputData.AddedTime = ticket.AddedTime;

                                        QueueAdmin.Enqueue(ticket);
                                        break;
                                }
                                break;
                        }
                    }
                }
            };

            //DEBUG------ИНИЦИАЛИЗАЦИЯ ОЧЕРЕДИ---------------------
            for (int i = 0; i < 100; i++)
            {
                var ticket2 = TicketFactoryGetHelp.Create((ushort) QueueGetHelp.Count);
                QueueGetHelp.Enqueue(ticket2);
            }
            //DEBUG------------------------------



            //СОЗДАНИЕ КАССИРОВ------------------------------------------------------------------------------------------------
            foreach (var xmlCash in xmlCashier)
            {
                Queue<TicketItem> queueTicket = null;
                switch (xmlCash.Prefix)
                {
                    case "C":
                        queueTicket = QueueGetHelp;
                        break;

                    case "T":
                        queueTicket = QueueBuyTicket;
                        break;

                    case "M":
                        queueTicket = QueueBuyInterstateTicket;
                        break;

                    case "B":
                        queueTicket = QueueBaggageCheckout;
                        break;

                    case "A":
                        queueTicket = QueueAdmin;
                        break;
                }

                var casher = new Сashier(xmlCash.Id, queueTicket, xmlCash.MaxCountTryHanding);
                DeviceCashiers.Add(new DeviceCashier(casher, xmlCash.Port));
            }


            //СОЗДАНИЕ ПОСЛЕД. ПОРТА ДЛЯ ОПРОСА КАССИРОВ-----------------------------------------------------------------------
            var cashersGroup = DeviceCashiers.GroupBy(d => d.Port).ToDictionary(group => group.Key, group => group.ToList());  //принадлежность кассира к порту
            foreach (var xmlSerial in xmlSerials)
            {
                var sp= new MasterSerialPort(xmlSerial);
                var cashiers= cashersGroup[xmlSerial.Port];
                var cashierExch= new CashierExchangeService(cashiers, xmlSerial.TimeRespoune);
                sp.AddFunc(cashierExch.ExchangeService);
                sp.PropertyChanged+= (o, e) =>
                {
                    var port = o as MasterSerialPort;
                    if (port != null)
                    {
                        if (e.PropertyName == "StatusString")
                        {
                            ErrorString = port.StatusString;                     //TODO: РАЗДЕЛЯЕМЫЙ РЕСУРС возможно нужна блокировка
                        }
                    }
                };
                MasterSerialPorts.Add(sp);
                CashierExchangeServices.Add(cashierExch);
            }
        }


        public async Task Start()
        {
            //ЗАПУСК СЛУШАТЕЛЯ ДЛЯ ТЕРМИНАЛОВ----------------------------------------------------------
            if (Listener != null)
            {
                var taskListener = Listener.RunServer(ProviderTerminal);
                BackGroundTasks.Add(taskListener);
            }

            //ЗАПУСК ОПРОСА КАССИРОВ-------------------------------------------------------------------
            if (MasterSerialPorts.Any())
            {
                foreach (var sp in MasterSerialPorts)
                {
                    var taskSerialPort = Task.Factory.StartNew(async () =>
                    {
                        if (await sp.CycleReConnect())
                        {
                            var taskCashierEx = sp.RunExchange();
                            BackGroundTasks.Add(taskCashierEx);
                        }
                    });
                    BackGroundTasks.Add(taskSerialPort);
                }
            }


            //КОНТРОЛЬ ФОНОВЫХ ЗАДАЧ----------------------------------------------------------------------
            var taskFirst = await Task.WhenAny(BackGroundTasks);
            if (taskFirst.Exception != null)                           //критическая ошибка фоновой задачи
                ErrorString = taskFirst.Exception.ToString();
        }

        #endregion




        #region IDisposable

        public void Dispose()
        {
            Listener?.Dispose();
            foreach (var sp in MasterSerialPorts)
            {
                sp?.Dispose();
            }
        }

        #endregion
    }
}