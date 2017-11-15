using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Timers;
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
using System.Collections.Concurrent;

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

        public ConcurrentQueue<TicketItem> QueueGetHelp { get; set; } = new ConcurrentQueue<TicketItem>();
        public ConcurrentQueue<TicketItem> QueueBuyTicket { get; set; } = new ConcurrentQueue<TicketItem>();
        public ConcurrentQueue<TicketItem> QueueBuyInterstateTicket { get; set; } = new ConcurrentQueue<TicketItem>();
        public ConcurrentQueue<TicketItem> QueueBaggageCheckout { get; set; } = new ConcurrentQueue<TicketItem>();
        public ConcurrentQueue<TicketItem> QueueAdmin { get; set; } = new ConcurrentQueue<TicketItem>();

        public List<QueuePriority> QueuePriorities { get; set; }= new List<QueuePriority>();



        public Log LogTicket { get; set; }

        public ListenerTcpIp Listener { get; set; }
        public IExchangeDataProvider<TerminalInData, TerminalOutData> ProviderTerminal { get; set; }

        public SoundQueue SoundQueue { get; set; } = new SoundQueue(new SoundPlayer(), new SoundNameService(), 100);


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
            List<XmlQueuesSettings> xmlQueues;
            try
            {
                var xmlFile = XmlWorker.LoadXmlFile("Settings", "Setting.xml"); //все настройки в одном файле
                if (xmlFile == null)
                    return;

                xmlListener = XmlListenerSettings.LoadXmlSetting(xmlFile);
                xmlSerials = XmlSerialSettings.LoadXmlSetting(xmlFile).ToList();
                xmlLog = XmlLogSettings.LoadXmlSetting(xmlFile);
                xmlCashier = XmlCashierSettings.LoadXmlSetting(xmlFile);
                xmlQueues = XmlQueuesSettings.LoadXmlSetting(xmlFile);
            }
            catch (FileNotFoundException ex)
            {
                ErrorString = ex.ToString();
                return;
            }
            catch (Exception ex)
            {
                ErrorString= "ОШИБКА в узлах дерева XML файла настроек:  " + ex;
                return;
            }


            //СОЗДАНИЕ ЛОГА--------------------------------------------------------------------------
            LogTicket= new Log("TicketLog.txt", xmlLog);


            //СОЗДАНИЕ ОЧЕРЕДИ-----------------------------------------------------------------------
            foreach (var xmlQueue in xmlQueues)
            {
                var queue= new QueuePriority (xmlQueue.Name, xmlQueue.Prefixes);
                QueuePriorities.Add(queue);
            }


          //  var queuePriority  = new QueuePriority("Main", new List<Prefix> { new Prefix { Name = "C", Priority = 9 }, new Prefix { Name = "T", Priority = 5 }, new Prefix { Name = "M", Priority = 1 } });
      


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
                        provider.OutputData = provider.OutputData ?? new TerminalOutData();

                        //Найдем очередь к которой обращен запрос
                        var prefixQueue = ((char)provider.InputData.NumberQueue).ToString();
                        var nameQueue = provider.InputData.NameQueue;
                        var queue= QueuePriorities.FirstOrDefault(q => string.Equals(q.Name, nameQueue, StringComparison.InvariantCultureIgnoreCase));

                        if (queue == null)
                           return;

                        switch (provider.InputData.Action)
                        {
                            //ИНФОРМАЦИЯ ОБ ОЧЕРЕДИ
                            case TerminalAction.Info:
                                provider.OutputData.NumberQueue = provider.InputData.NumberQueue;
                                provider.OutputData.CountElement = (ushort)queue.GetInseartPlace(prefixQueue);
                                provider.OutputData.NumberElement = (ushort)(queue.GetCurrentTicketNumber + 1);
                                provider.OutputData.AddedTime = DateTime.Now;
                                break;

                            //ДОБАВИТЬ БИЛЕТ В ОЧЕРЕДЬ
                            case TerminalAction.Add:
                                var ticket = queue.CreateTicket(prefixQueue);

                                provider.OutputData.NumberQueue = provider.InputData.NumberQueue;
                                provider.OutputData.CountElement = ticket.CountElement;
                                provider.OutputData.NumberElement = (ushort)ticket.NumberElement;
                                provider.OutputData.AddedTime = ticket.AddedTime;

                                queue.Enqueue(ticket);
                                break;
                        }
                    }
                }
            };

            //DEBUG------ИНИЦИАЛИЗАЦИЯ ОЧЕРЕДИ---------------------
            for (int i = 0; i < 100; i++)
            {
                var ticket2 = TicketFactoryGetHelp.Create((ushort)QueueGetHelp.Count);
                QueueGetHelp.Enqueue(ticket2);

                ticket2 = TicketFactoryBuyTicket.Create((ushort)QueueBuyTicket.Count);
                QueueBuyTicket.Enqueue(ticket2);

                ticket2 = TicketFactoryBaggageCheckout.Create((ushort)QueueBaggageCheckout.Count);
                QueueBaggageCheckout.Enqueue(ticket2);

                ticket2 = TicketFactoryAdmin.Create((ushort)QueueAdmin.Count);
                QueueAdmin.Enqueue(ticket2);

                ticket2 = TicketFactoryBuyInterstateTicket.Create((ushort)QueueBuyInterstateTicket.Count);
                QueueBuyInterstateTicket.Enqueue(ticket2);
            }
            //DEBUG------------------------------



            //СОЗДАНИЕ КАССИРОВ------------------------------------------------------------------------------------------------
            foreach (var xmlCash in xmlCashier)
            {
                ConcurrentQueue<TicketItem> queueTicket = null;
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