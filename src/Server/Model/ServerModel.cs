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
using System.Text;
using Server.SerializableModel;
using System.Runtime.Serialization.Formatters.Binary;

namespace Server.Model
{
    public class ServerModel : INotifyPropertyChanged, IDisposable
    {
        #region prop

        public List<QueuePriority> QueuePriorities { get; set; }= new List<QueuePriority>();

        public Log LogTicket { get; set; }

        public ListenerTcpIp Listener { get; set; }
        public IExchangeDataProvider<TerminalInData, TerminalOutData> ProviderTerminal { get; set; }

        public SoundQueue SoundQueue { get; set; } = new SoundQueue(new SoundPlayer(), new SoundNameService(), 100);

        public List<MasterSerialPort> MasterSerialPorts { get; } = new List<MasterSerialPort>();
        public List<DeviceCashier> DeviceCashiers { get; } = new List<DeviceCashier>();
        public DeviceCashier AdminCasher { get; private set; } //Ссылка на администратора кассира (сам кассир находится в DeviceCashiers)
        public List<CashierExchangeService> CashierExchangeServices { get; } = new List<CashierExchangeService>();

        public List<Task> BackGroundTasks { get; } = new List<Task>();

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
            List<XmlCashierSettings> xmlCashier;
            List<XmlQueuesSettings> xmlQueues;
            try
            {
                var xmlFile = XmlWorker.LoadXmlFile("Settings", "Setting.xml"); //все настройки в одном файле
                if (xmlFile == null)
                    return;

                xmlListener = XmlListenerSettings.LoadXmlSetting(xmlFile);
                xmlSerials = XmlSerialSettings.LoadXmlSetting(xmlFile).ToList();
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

       
            //СОЗДАНИЕ ОЧЕРЕДИ-----------------------------------------------------------------------
            foreach (var xmlQueue in xmlQueues)
            {
                var queue= new QueuePriority (xmlQueue.Name, xmlQueue.Prefixes);
                QueuePriorities.Add(queue);
            }


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
                        var prefixQueue = provider.InputData.PrefixQueue;
                        var nameQueue = provider.InputData.NameQueue;
                        var queue= QueuePriorities.FirstOrDefault(q => string.Equals(q.Name, nameQueue, StringComparison.InvariantCultureIgnoreCase));

                        if (queue == null)
                           return;

                        switch (provider.InputData.Action)
                        {
                            //ИНФОРМАЦИЯ ОБ ОЧЕРЕДИ
                            case TerminalAction.Info:
                                provider.OutputData.PrefixQueue = provider.InputData.PrefixQueue;
                                provider.OutputData.CountElement = (ushort)queue.GetInseartPlace(prefixQueue);
                                provider.OutputData.NumberElement = (ushort)(queue.GetCurrentTicketNumber + 1);
                                provider.OutputData.AddedTime = DateTime.Now;
                                break;

                            //ДОБАВИТЬ БИЛЕТ В ОЧЕРЕДЬ
                            case TerminalAction.Add:
                                var ticket = queue.CreateTicket(prefixQueue);

                                provider.OutputData.PrefixQueue = provider.InputData.PrefixQueue;
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
            //var queueTemp = QueuePriorities.FirstOrDefault(q => string.Equals(q.Name, "Main", StringComparison.InvariantCultureIgnoreCase));
            //var queueAdmin = QueuePriorities.FirstOrDefault(q => string.Equals(q.Name, "Admin", StringComparison.InvariantCultureIgnoreCase));
            //for (int i = 0; i < 5; i++)
            //{
            //    var ticketAdmin = queueTemp.CreateTicket("А");
            //    //queueAdmin.Enqueue(ticketAdmin);


            //    var ticket = queueTemp.CreateTicket("К");
            //    queueTemp.Enqueue(ticket);

            //    ticket = queueTemp.CreateTicket("М");
            //    queueTemp.Enqueue(ticket);

            //    ticket = queueTemp.CreateTicket("Г");
            //    queueTemp.Enqueue(ticket);

            //    ticket = queueTemp.CreateTicket("И");
            //    queueTemp.Enqueue(ticket);

            //    ticket = queueTemp.CreateTicket("В");
            //    queueTemp.Enqueue(ticket);

            //    ticket = queueTemp.CreateTicket("П");
            //    queueTemp.Enqueue(ticket);

            //    ticket = queueTemp.CreateTicket("У");
            //    queueTemp.Enqueue(ticket);

            //    ticket = queueTemp.CreateTicket("З");
            //    queueTemp.Enqueue(ticket);

            //    ticket = queueTemp.CreateTicket("С");
            //    queueTemp.Enqueue(ticket);

            //    ticket = queueTemp.CreateTicket("Б");
            //    queueTemp.Enqueue(ticket);
            //}
            //DEBUG----------------------------------------------


            //СОЗДАНИЕ КАССИРОВ------------------------------------------------------------------------------------------------
            foreach (var xmlCash in xmlCashier)
            {
               var queue= QueuePriorities.FirstOrDefault(q => q.Name == xmlCash.NameQueue);
               if (queue != null)
               {
                   var casher = new Сashier(xmlCash.Id, xmlCash.Prefixs, queue, xmlCash.MaxCountTryHanding);
                   DeviceCashiers.Add(new DeviceCashier(xmlCash.AddressDevice, casher, xmlCash.Port));
               }
            }
            AdminCasher = DeviceCashiers.FirstOrDefault(d => d.Cashier.Prefixes.Contains("А"));


            //ВОССТАНОВЛЕНИЕ СОСТОЯНИЯ ОБЪЕКТОВ (сохранялись на момент закрытия программы)----------------------------------------------------------------------------------------
            //LoadStates();


            //СОЗДАНИЕ ПОСЛЕД. ПОРТА ДЛЯ ОПРОСА КАССИРОВ-----------------------------------------------------------------------
            var cashersGroup = DeviceCashiers.GroupBy(d => d.Port).ToDictionary(group => group.Key, group => group.ToList());  //принадлежность кассира к порту
            foreach (var xmlSerial in xmlSerials)
            {
                var sp= new MasterSerialPort(xmlSerial);
                var cashiers= cashersGroup[xmlSerial.Port];
                var cashierExch= new CashierExchangeService(cashiers, AdminCasher, xmlSerial.TimeRespoune);
                sp.AddFunc(cashierExch.ExchangeService);
                sp.PropertyChanged += (o, e) =>
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


        /// <summary>
        /// Сохранить состояние: билеты в очереди, текущие обрабаываемые билеты.
        /// </summary>
        private void SaveStates()
        {
            var model = new QueuePrioritysModelSerializable();

            //сохранить элементы очереди
            foreach (var queuePriority in QueuePriorities)
            {
                var queue = new QueuePriorityModelSerializable
                {
                    Name = queuePriority.Name,
                    CurrentTicketNumber = queuePriority.GetCurrentTicketNumber,
                    Queue = queuePriority.GetQueueItems.ToList(),
                };
              
                model.Queues.Add(queue);
            }

            //сохранить текущие обрабатываемые билеты кассирами
            foreach (var deviceCashier in DeviceCashiers)
            {
                var cashier = new СashierModelSerializable
                {
                    Id = deviceCashier.Cashier.Id,
                    CurrenTicketItem = deviceCashier.Cashier.CurrentTicket
                };
                model.Cashiers.Add(cashier);
            }

            try
            {
                // создаем объект BinaryFormatter
                var formatter = new BinaryFormatter();
                using (var fs = new FileStream("States.dat", FileMode.OpenOrCreate))
                {
                    formatter.Serialize(fs, model);
                }
            }
            catch (Exception ex)
            {

            }
        }


        /// <summary>
        /// Загрузить состояние: билеты в очереди, текущие обрабаываемые билеты попадают в приоритетеную очередь кассира и будут обработанны касииром снова
        /// </summary>
        public void LoadStates()
        {
            try
            {
                if (File.Exists("States.dat"))
                {
                    var formatter = new BinaryFormatter();
                    using (var fs = new FileStream("States.dat", FileMode.Open))
                    {
                        var model = (QueuePrioritysModelSerializable)formatter.Deserialize(fs);

                        //восстановить элементы очереди
                        foreach (var newQueue in model.Queues)
                        {
                            var queue = QueuePriorities.FirstOrDefault(q => q.Name == newQueue.Name);
                            if (queue != null)
                            {
                                queue.SetQueueItems = newQueue.Queue;
                                queue.SetCurrentTicketNumber = newQueue.CurrentTicketNumber;
                            }
                        }

                        //восстановить текущие обрабатываемые билеты кассирами
                        foreach (var newCashier in model.Cashiers)
                        {
                            if (newCashier.CurrenTicketItem == null)
                                continue;

                            var cashier = DeviceCashiers.FirstOrDefault(d => d.Cashier.Id == newCashier.Id);
                            cashier?.Cashier.AddRedirectedTicket(newCashier.CurrenTicketItem);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }



        #endregion




        #region IDisposable

        public void Dispose()
        {
            SaveStates();

            Listener?.Dispose();
            foreach (var sp in MasterSerialPorts)
            {
                sp?.Dispose();
            }
        }

        #endregion
    }
}