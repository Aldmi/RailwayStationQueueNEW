using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Media;
using Caliburn.Micro;
using Communication.TcpIp;
using Library.Logs;
using Server.Entitys;
using Server.Model;
using ServerUi.Model;
using Sound;
using Brushes = System.Windows.Media.Brushes;
using Screen = Caliburn.Micro.Screen;
using TicketItem = ServerUi.Model.TicketItem;
using Brush = System.Windows.Media.Brush;
using MessageBox = System.Windows.MessageBox;


namespace ServerUi.ViewModels
{
    public class AppViewModel : Screen
    {
        #region field

        private readonly ServerModel _model;
        private readonly Task _mainTask;
        private readonly Log _logger;

        private const int LimitRowTable8X21 = 6;
        private const int LimitRowTable8X22 = 6;

        private const int LimitRowTable4X41 = 3;
        private const int LimitRowTable4X42 = 3;
        private const int LimitRowTable4X43 = 3;
        private const int LimitRowTable4X44 = 3;

        private const string SettingUiNameFile = @"Settings\settingsUi.dat";

        #endregion




        #region ctor

        public AppViewModel()
        {
            _logger = new Log("Server.Main");

            _model = new ServerModel();
            _model.PropertyChanged += _model_PropertyChanged;

            _model.LoadSetting();
            foreach (var devCashier in _model.DeviceCashiers)
            {
                devCashier.Cashier.PropertyChanged+= Cashier_PropertyChanged;
                devCashier.PropertyChanged+= DevCashierOnPropertyChanged;
            }

            if (_model.Listener != null)
            {
                _model.Listener.PropertyChanged += Listener_PropertyChanged;
                _mainTask = _model.Start();
            }

            _model.SoundQueue.PropertyChanged += SoundQueue_PropertyChanged;
            _model.SoundQueue.StartQueue();


           var queueMain= _model.QueuePriorities.FirstOrDefault(q => q.Name == "Main");
           if (queueMain != null)
           {
              queueMain.PropertyChanged += QueueMain_PropertyChanged;
           }
 
           _model.LoadStates();//DEBUG



            //ЗАГРУЗКА НАСТРОЕК ТАБЛО
            HeaderBackgroundColor = new SolidColorBrush(Colors.DarkRed);
            HeaderFontColor = new SolidColorBrush(Colors.Black);
            ColorListRows = new SolidColorBrush(Colors.Azure);
            ColorListBackground = new SolidColorBrush(Colors.CadetBlue);
            ListFontColor = new SolidColorBrush(Colors.Black);

            CurrentFontCash = new FontSetting { FontHeader = null, FontRow = new Font(System.Drawing.FontFamily.GenericMonospace, 10), PaddingHeader = 0, PaddingRow = 0};
            CurrentFont8X2 = new FontSetting  { FontHeader = new Font(System.Drawing.FontFamily.GenericMonospace, 10),
                                               FontRow = new Font(System.Drawing.FontFamily.GenericMonospace, 10),
                                               PaddingHeader = 0,
                                               PaddingRow = 0 };

            CurrentFont4X4 = new FontSetting { FontHeader = new Font(System.Drawing.FontFamily.GenericMonospace, 10),
                                               FontRow = new Font(System.Drawing.FontFamily.GenericMonospace, 10),
                                               PaddingHeader = 0,
                                               PaddingRow = 0 };

            var settingUi= LoadSettingUi();
            ApplySetting(settingUi);


            //DEBUG-------------
            //TimerAutoTest.Elapsed += TimerAutoTest_Elapsed;   
            //TimerAutoTest.Start();
        }


        private void SoundQueue_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var soundQueue = sender as SoundQueue;
            if (soundQueue != null)
            {
                if (e.PropertyName == "Queue")
                {
                    SoundTemplates.Clear();
                    SoundTemplates.AddRange(soundQueue.GetQueue);
                }
            }
        }


        private void QueueMain_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var queuePriority = sender as QueuePriority;
            if (queuePriority != null)
            {
                if (e.PropertyName == "QueuePriority")
                {
                    QueuePriority.Clear();
                    QueuePriority.AddRange(queuePriority.GetQueueItems);
                }
            }
        }

        #endregion




        #region prop


        #region БИЛЕТЫ ПО КАССИРАМ

        private TicketItem _cashierTicket1;
        public TicketItem CashierTicket1
        {
            get { return _cashierTicket1; }
            set
            {
                _cashierTicket1 = value;
                NotifyOfPropertyChange(() => CashierTicket1);
            }
        }

        private TicketItem _cashierTicket2;
        public TicketItem CashierTicket2
        {
            get { return _cashierTicket2; }
            set
            {
                _cashierTicket2 = value;
                NotifyOfPropertyChange(() => CashierTicket2);
            }
        }

        private TicketItem _cashierTicket3;
        public TicketItem CashierTicket3
        {
            get { return _cashierTicket3; }
            set
            {
                _cashierTicket3 = value;
                NotifyOfPropertyChange(() => CashierTicket3);
            }
        }

        private TicketItem _cashierTicket4;
        public TicketItem CashierTicket4
        {
            get { return _cashierTicket4; }
            set
            {
                _cashierTicket4 = value;
                NotifyOfPropertyChange(() => CashierTicket4);
            }
        }

        private TicketItem _cashierTicket5;
        public TicketItem CashierTicket5
        {
            get { return _cashierTicket5; }
            set
            {
                _cashierTicket5 = value;
                NotifyOfPropertyChange(() => CashierTicket5);
            }
        }

        private TicketItem _cashierTicket6;
        public TicketItem CashierTicket6
        {
            get { return _cashierTicket6; }
            set
            {
                _cashierTicket6 = value;
                NotifyOfPropertyChange(() => CashierTicket6);
            }
        }

        private TicketItem _cashierTicket7;
        public TicketItem CashierTicket7
        {
            get { return _cashierTicket7; }
            set
            {
                _cashierTicket7 = value;
                NotifyOfPropertyChange(() => CashierTicket7);
            }
        }

        private TicketItem _cashierTicket8;
        public TicketItem CashierTicket8
        {
            get { return _cashierTicket8; }
            set
            {
                _cashierTicket8 = value;
                NotifyOfPropertyChange(() => CashierTicket8);
            }
        }

        private TicketItem _cashierTicket9;
        public TicketItem CashierTicket9
        {
            get { return _cashierTicket9; }
            set
            {
                _cashierTicket9 = value;
                NotifyOfPropertyChange(() => CashierTicket9);
            }
        }

        private TicketItem _cashierTicket10;
        public TicketItem CashierTicket10
        {
            get { return _cashierTicket10; }
            set
            {
                _cashierTicket10 = value;
                NotifyOfPropertyChange(() => CashierTicket10);
            }
        }

        private TicketItem _cashierTicket11;
        public TicketItem CashierTicket11
        {
            get { return _cashierTicket11; }
            set
            {
                _cashierTicket11 = value;
                NotifyOfPropertyChange(() => CashierTicket11);
            }
        }

        private TicketItem _cashierTicket12;
        public TicketItem CashierTicket12
        {
            get { return _cashierTicket12; }
            set
            {
                _cashierTicket12 = value;
                NotifyOfPropertyChange(() => CashierTicket12);
            }
        }

        private TicketItem _cashierTicket13;
        public TicketItem CashierTicket13
        {
            get { return _cashierTicket13; }
            set
            {
                _cashierTicket13 = value;
                NotifyOfPropertyChange(() => CashierTicket13);
            }
        }

        private TicketItem _cashierTicket14;
        public TicketItem CashierTicket14
        {
            get { return _cashierTicket14; }
            set
            {
                _cashierTicket14 = value;
                NotifyOfPropertyChange(() => CashierTicket14);
            }
        }

        private TicketItem _cashierTicket15;
        public TicketItem CashierTicket15
        {
            get { return _cashierTicket15; }
            set
            {
                _cashierTicket15 = value;
                NotifyOfPropertyChange(() => CashierTicket15);
            }
        }

        private TicketItem _cashierTicket16;
        public TicketItem CashierTicket16
        {
            get { return _cashierTicket16; }
            set
            {
                _cashierTicket16 = value;
                NotifyOfPropertyChange(() => CashierTicket16);
            }
        }

        private TicketItem _cashierTicket17;
        public TicketItem CashierTicket17
        {
            get { return _cashierTicket17; }
            set
            {
                _cashierTicket17 = value;
                NotifyOfPropertyChange(() => CashierTicket17);
            }
        }

        #endregion




        #region ТАБЛО

        //ТАБЛО 4X4 - 1
        public BindableCollection<TicketItem> Table4X41 { get; set; } = new BindableCollection<TicketItem>();
        public BindableCollection<TicketItem> Table4X42 { get; set; } = new BindableCollection<TicketItem>();
        public BindableCollection<TicketItem> Table4X43 { get; set; } = new BindableCollection<TicketItem>();
        public BindableCollection<TicketItem> Table4X44 { get; set; } = new BindableCollection<TicketItem>();

        //ТАБЛО 8X2 - 1
        public BindableCollection<TicketItem> Table8X21 { get; set; } = new BindableCollection<TicketItem>();
        public BindableCollection<TicketItem> Table8X22 { get; set; } = new BindableCollection<TicketItem>();

        //ТАБЛО 8X2 - 2
        public BindableCollection<TicketItem> Table8X23 { get; set; } = new BindableCollection<TicketItem>();
        public BindableCollection<TicketItem> Table8X24 { get; set; } = new BindableCollection<TicketItem>();


        #endregion




        #region СВЯЗЬ С КАССИРАМИ

        private SolidColorBrush _colorBackgroundCashierTicket1 = Brushes.SlateGray;
        public SolidColorBrush ColorBackgroundCashierTicket1
        {
            get { return _colorBackgroundCashierTicket1; }
            set
            {
                _colorBackgroundCashierTicket1 = value;
                NotifyOfPropertyChange(() => ColorBackgroundCashierTicket1);
            }
        }

        private SolidColorBrush _colorBackgroundCashierTicket2 = Brushes.SlateGray;
        public SolidColorBrush ColorBackgroundCashierTicket2
        {
            get { return _colorBackgroundCashierTicket2; }
            set
            {
                _colorBackgroundCashierTicket2 = value;
                NotifyOfPropertyChange(() => ColorBackgroundCashierTicket2);
            }
        }

        private SolidColorBrush _colorBackgroundCashierTicket3 = Brushes.SlateGray;
        public SolidColorBrush ColorBackgroundCashierTicket3
        {
            get { return _colorBackgroundCashierTicket3; }
            set
            {
                _colorBackgroundCashierTicket3 = value;
                NotifyOfPropertyChange(() => ColorBackgroundCashierTicket3);
            }
        }

        private SolidColorBrush _colorBackgroundCashierTicket4 = Brushes.SlateGray;
        public SolidColorBrush ColorBackgroundCashierTicket4
        {
            get { return _colorBackgroundCashierTicket4; }
            set
            {
                _colorBackgroundCashierTicket4 = value;
                NotifyOfPropertyChange(() => ColorBackgroundCashierTicket4);
            }
        }

        private SolidColorBrush _colorBackgroundCashierTicket5 = Brushes.SlateGray;
        public SolidColorBrush ColorBackgroundCashierTicket5
        {
            get { return _colorBackgroundCashierTicket5; }
            set
            {
                _colorBackgroundCashierTicket5 = value;
                NotifyOfPropertyChange(() => ColorBackgroundCashierTicket5);
            }
        }

        private SolidColorBrush _colorBackgroundCashierTicket6 = Brushes.SlateGray;
        public SolidColorBrush ColorBackgroundCashierTicket6
        {
            get { return _colorBackgroundCashierTicket6; }
            set
            {
                _colorBackgroundCashierTicket6 = value;
                NotifyOfPropertyChange(() => ColorBackgroundCashierTicket6);
            }
        }

        private SolidColorBrush _colorBackgroundCashierTicket7 = Brushes.SlateGray;
        public SolidColorBrush ColorBackgroundCashierTicket7
        {
            get { return _colorBackgroundCashierTicket7; }
            set
            {
                _colorBackgroundCashierTicket7 = value;
                NotifyOfPropertyChange(() => ColorBackgroundCashierTicket7);
            }
        }

        private SolidColorBrush _colorBackgroundCashierTicket8 = Brushes.SlateGray;
        public SolidColorBrush ColorBackgroundCashierTicket8
        {
            get { return _colorBackgroundCashierTicket8; }
            set
            {
                _colorBackgroundCashierTicket8 = value;
                NotifyOfPropertyChange(() => ColorBackgroundCashierTicket8);
            }
        }

        private SolidColorBrush _colorBackgroundCashierTicket9 = Brushes.SlateGray;
        public SolidColorBrush ColorBackgroundCashierTicket9
        {
            get { return _colorBackgroundCashierTicket9; }
            set
            {
                _colorBackgroundCashierTicket9 = value;
                NotifyOfPropertyChange(() => ColorBackgroundCashierTicket9);
            }
        }

        private SolidColorBrush _colorBackgroundCashierTicket10 = Brushes.SlateGray;
        public SolidColorBrush ColorBackgroundCashierTicket10
        {
            get { return _colorBackgroundCashierTicket10; }
            set
            {
                _colorBackgroundCashierTicket10 = value;
                NotifyOfPropertyChange(() => ColorBackgroundCashierTicket10);
            }
        }

        private SolidColorBrush _colorBackgroundCashierTicket11 = Brushes.SlateGray;
        public SolidColorBrush ColorBackgroundCashierTicket11
        {
            get { return _colorBackgroundCashierTicket11; }
            set
            {
                _colorBackgroundCashierTicket11 = value;
                NotifyOfPropertyChange(() => ColorBackgroundCashierTicket11);
            }
        }

        private SolidColorBrush _colorBackgroundCashierTicket12 = Brushes.SlateGray;
        public SolidColorBrush ColorBackgroundCashierTicket12
        {
            get { return _colorBackgroundCashierTicket12; }
            set
            {
                _colorBackgroundCashierTicket12 = value;
                NotifyOfPropertyChange(() => ColorBackgroundCashierTicket12);
            }
        }

        private SolidColorBrush _colorBackgroundCashierTicket13 = Brushes.SlateGray;
        public SolidColorBrush ColorBackgroundCashierTicket13
        {
            get { return _colorBackgroundCashierTicket13; }
            set
            {
                _colorBackgroundCashierTicket13 = value;
                NotifyOfPropertyChange(() => ColorBackgroundCashierTicket13);
            }
        }

        private SolidColorBrush _colorBackgroundCashierTicket14 = Brushes.SlateGray;
        public SolidColorBrush ColorBackgroundCashierTicket14
        {
            get { return _colorBackgroundCashierTicket14; }
            set
            {
                _colorBackgroundCashierTicket14 = value;
                NotifyOfPropertyChange(() => ColorBackgroundCashierTicket14);
            }
        }

        private SolidColorBrush _colorBackgroundCashierTicket15 = Brushes.SlateGray;
        public SolidColorBrush ColorBackgroundCashierTicket15
        {
            get { return _colorBackgroundCashierTicket15; }
            set
            {
                _colorBackgroundCashierTicket15 = value;
                NotifyOfPropertyChange(() => ColorBackgroundCashierTicket15);
            }
        }

        private SolidColorBrush _colorBackgroundCashierTicket16 = Brushes.SlateGray;
        public SolidColorBrush ColorBackgroundCashierTicket16
        {
            get { return _colorBackgroundCashierTicket16; }
            set
            {
                _colorBackgroundCashierTicket16 = value;
                NotifyOfPropertyChange(() => ColorBackgroundCashierTicket16);
            }
        }

        private SolidColorBrush _colorBackgroundCashierTicket17 = Brushes.SlateGray;
        public SolidColorBrush ColorBackgroundCashierTicket17
        {
            get { return _colorBackgroundCashierTicket17; }
            set
            {
                _colorBackgroundCashierTicket17 = value;
                NotifyOfPropertyChange(() => ColorBackgroundCashierTicket17);
            }
        }


        #endregion




        #region ОПРОС КАССИРОВ

        private SolidColorBrush _colorForegroundCashierTicket1 = Brushes.Black;
        public SolidColorBrush ColorForegroundCashierTicket1
        {
            get { return _colorForegroundCashierTicket1; }
            set
            {
                _colorForegroundCashierTicket1 = value;
                NotifyOfPropertyChange(() => ColorForegroundCashierTicket1);
            }
        }

        private SolidColorBrush _colorForegroundCashierTicket2 = Brushes.Black;
        public SolidColorBrush ColorForegroundCashierTicket2
        {
            get { return _colorForegroundCashierTicket2; }
            set
            {
                _colorForegroundCashierTicket2 = value;
                NotifyOfPropertyChange(() => ColorForegroundCashierTicket2);
            }
        }

        private SolidColorBrush _colorForegroundCashierTicket3 = Brushes.Black;
        public SolidColorBrush ColorForegroundCashierTicket3
        {
            get { return _colorForegroundCashierTicket3; }
            set
            {
                _colorForegroundCashierTicket3 = value;
                NotifyOfPropertyChange(() => ColorForegroundCashierTicket3);
            }
        }

        private SolidColorBrush _colorForegroundCashierTicket4 = Brushes.Black;
        public SolidColorBrush ColorForegroundCashierTicket4
        {
            get { return _colorForegroundCashierTicket4; }
            set
            {
                _colorForegroundCashierTicket4 = value;
                NotifyOfPropertyChange(() => ColorForegroundCashierTicket4);
            }
        }

        private SolidColorBrush _colorForegroundCashierTicket5 = Brushes.Black;
        public SolidColorBrush ColorForegroundCashierTicket5
        {
            get { return _colorForegroundCashierTicket5; }
            set
            {
                _colorForegroundCashierTicket5 = value;
                NotifyOfPropertyChange(() => ColorForegroundCashierTicket5);
            }
        }

        private SolidColorBrush _colorForegroundCashierTicket6 = Brushes.Black;
        public SolidColorBrush ColorForegroundCashierTicket6
        {
            get { return _colorForegroundCashierTicket6; }
            set
            {
                _colorForegroundCashierTicket6 = value;
                NotifyOfPropertyChange(() => ColorForegroundCashierTicket6);
            }
        }

        private SolidColorBrush _colorForegroundCashierTicket7 = Brushes.Black;
        public SolidColorBrush ColorForegroundCashierTicket7
        {
            get { return _colorForegroundCashierTicket7; }
            set
            {
                _colorForegroundCashierTicket7 = value;
                NotifyOfPropertyChange(() => ColorForegroundCashierTicket7);
            }
        }

        private SolidColorBrush _colorForegroundCashierTicket8 = Brushes.Black;
        public SolidColorBrush ColorForegroundCashierTicket8
        {
            get { return _colorForegroundCashierTicket8; }
            set
            {
                _colorForegroundCashierTicket8 = value;
                NotifyOfPropertyChange(() => ColorForegroundCashierTicket8);
            }
        }

        private SolidColorBrush _colorForegroundCashierTicket9 = Brushes.Black;
        public SolidColorBrush ColorForegroundCashierTicket9
        {
            get { return _colorForegroundCashierTicket9; }
            set
            {
                _colorForegroundCashierTicket9 = value;
                NotifyOfPropertyChange(() => ColorForegroundCashierTicket9);
            }
        }

        private SolidColorBrush _colorForegroundCashierTicket10 = Brushes.Black;
        public SolidColorBrush ColorForegroundCashierTicket10
        {
            get { return _colorForegroundCashierTicket10; }
            set
            {
                _colorForegroundCashierTicket10 = value;
                NotifyOfPropertyChange(() => ColorForegroundCashierTicket10);
            }
        }

        private SolidColorBrush _colorForegroundCashierTicket11 = Brushes.Black;
        public SolidColorBrush ColorForegroundCashierTicket11
        {
            get { return _colorForegroundCashierTicket11; }
            set
            {
                _colorForegroundCashierTicket11 = value;
                NotifyOfPropertyChange(() => ColorForegroundCashierTicket11);
            }
        }

        private SolidColorBrush _colorForegroundCashierTicket12 = Brushes.Black;
        public SolidColorBrush ColorForegroundCashierTicket12
        {
            get { return _colorForegroundCashierTicket12; }
            set
            {
                _colorForegroundCashierTicket12 = value;
                NotifyOfPropertyChange(() => ColorForegroundCashierTicket12);
            }
        }

        private SolidColorBrush _colorForegroundCashierTicket13 = Brushes.Black;
        public SolidColorBrush ColorForegroundCashierTicket13
        {
            get { return _colorForegroundCashierTicket13; }
            set
            {
                _colorForegroundCashierTicket13 = value;
                NotifyOfPropertyChange(() => ColorForegroundCashierTicket13);
            }
        }

        private SolidColorBrush _colorForegroundCashierTicket14 = Brushes.Black;
        public SolidColorBrush ColorForegroundCashierTicket14
        {
            get { return _colorForegroundCashierTicket14; }
            set
            {
                _colorForegroundCashierTicket14 = value;
                NotifyOfPropertyChange(() => ColorForegroundCashierTicket14);
            }
        }

        private SolidColorBrush _colorForegroundCashierTicket15 = Brushes.Black;
        public SolidColorBrush ColorForegroundCashierTicket15
        {
            get { return _colorForegroundCashierTicket15; }
            set
            {
                _colorForegroundCashierTicket15 = value;
                NotifyOfPropertyChange(() => ColorForegroundCashierTicket15);
            }
        }

        private SolidColorBrush _colorForegroundCashierTicket16 = Brushes.Black;
        public SolidColorBrush ColorForegroundCashierTicket16
        {
            get { return _colorForegroundCashierTicket16; }
            set
            {
                _colorForegroundCashierTicket16 = value;
                NotifyOfPropertyChange(() => ColorForegroundCashierTicket16);
            }
        }

        private SolidColorBrush _colorForegroundCashierTicket17 = Brushes.Black;
        public SolidColorBrush ColorForegroundCashierTicket17
        {
            get { return _colorForegroundCashierTicket17; }
            set
            {
                _colorForegroundCashierTicket17 = value;
                NotifyOfPropertyChange(() => ColorForegroundCashierTicket17);
            }
        }

        #endregion




        #region СВЯЗЬ С ТЕРМИНАЛАМИ

        public BindableCollection<string> TerminalsIp { get; set; } = new BindableCollection<string>();

        #endregion



        private SolidColorBrush _colorBackground = Brushes.SlateGray;
        public SolidColorBrush ColorBackground
        {
            get { return _colorBackground; }
            set
            {
                _colorBackground = value;
                NotifyOfPropertyChange(() => ColorBackground);
            }
        }


        public BindableCollection<SoundTemplate> SoundTemplates { get; set; } = new BindableCollection<SoundTemplate>();  //Звуковые шалоны в очереди

        public BindableCollection<Server.Entitys.TicketItem> QueuePriority { get; set; } = new BindableCollection<Server.Entitys.TicketItem>(); //Основная очередь

        #endregion





        #region EventHandler

        private void Cashier_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var сashier = sender as Сashier;
            if (сashier != null)
            {
                if (e.PropertyName == "CurrentTicket")
                {
                    if (сashier.CurrentTicket != null)     //добавить элемент к списку
                    {
                        var ticket = new TicketItem
                        {
                            CashierId = сashier.Id,
                            CashierName = сashier.CurrentTicket.Cashbox.ToString(),
                            TicketName = $"{сashier.CurrentTicket.Prefix}{сashier.CurrentTicket.NumberElement:000}",
                        };

                        var ticketPrefix = ticket.TicketName.Substring(0, 1);
                        var ticketNumber = ticket.TicketName.Substring(1, 3);
                        var formatStr= $"Талон {ticketPrefix} {ticketNumber} Касса {ticket.CashierName}";
                        _model.SoundQueue.AddItem(new SoundTemplate(formatStr));

                        FillTableCashier(сashier.Id, ticket);
                        FillTable4X4(ticket, Table4X41, Table4X42, Table4X43, Table4X44);
                        FillTable8X2(ticket, Table8X21, Table8X22);
                        FillTable8X2(ticket, Table8X23, Table8X24);

                        //LOG
                        _logger.Info(сashier.CurrentTicket.ToString());
                    }
                    else                                 //удалить элемент из списка
                    {
                        FillTableCashier(сashier.Id, null);
                        ClearTable4X4(сashier.Id, Table4X41, Table4X42, Table4X43, Table4X44);
                        ClearTable8X2(сashier.Id, Table8X21, Table8X22);
                        ClearTable8X2(сashier.Id, Table8X23, Table8X24);
                    }
                }
            }
        }


        /// <summary>
        /// Обработка события IsConnect
        /// </summary>
        private async void DevCashierOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var deviceCashier = sender as DeviceCashier;
            if (deviceCashier != null)
            {
                //отобразить подключение для кассиров
                if (e.PropertyName == "IsConnect")
                {
                    switch (deviceCashier.Cashier.Id)
                    {
                        case 1:
                            ColorBackgroundCashierTicket1 = deviceCashier.IsConnect ? Brushes.Green : Brushes.SlateGray;               
                            break;

                        case 2:
                            ColorBackgroundCashierTicket2 = deviceCashier.IsConnect ? Brushes.Green : Brushes.SlateGray;
                            break;

                        case 3:
                            ColorBackgroundCashierTicket3 = deviceCashier.IsConnect ? Brushes.Green : Brushes.SlateGray;
                            break;

                        case 4:
                            ColorBackgroundCashierTicket4 = deviceCashier.IsConnect ? Brushes.Green : Brushes.SlateGray;
                            break;

                        case 5:
                            ColorBackgroundCashierTicket5 = deviceCashier.IsConnect ? Brushes.Green : Brushes.SlateGray;
                            break;

                        case 6:
                            ColorBackgroundCashierTicket6 = deviceCashier.IsConnect ? Brushes.Green : Brushes.SlateGray;
                            break;

                        case 7:
                            ColorBackgroundCashierTicket7 = deviceCashier.IsConnect ? Brushes.Green : Brushes.SlateGray;
                            break;

                        case 8:
                            ColorBackgroundCashierTicket8 = deviceCashier.IsConnect ? Brushes.Green : Brushes.SlateGray;
                            break;

                        case 9:
                            ColorBackgroundCashierTicket9 = deviceCashier.IsConnect ? Brushes.Green : Brushes.SlateGray;
                            break;

                        case 10:
                            ColorBackgroundCashierTicket10 = deviceCashier.IsConnect ? Brushes.Green : Brushes.SlateGray;
                            break;

                        case 11:
                            ColorBackgroundCashierTicket11 = deviceCashier.IsConnect ? Brushes.Green : Brushes.SlateGray;
                            break;

                        case 12:
                            ColorBackgroundCashierTicket12 = deviceCashier.IsConnect ? Brushes.Green : Brushes.SlateGray;
                            break;

                        case 13:
                            ColorBackgroundCashierTicket13 = deviceCashier.IsConnect ? Brushes.Green : Brushes.SlateGray;
                            break;

                        case 14:
                            ColorBackgroundCashierTicket14 = deviceCashier.IsConnect ? Brushes.Green : Brushes.SlateGray;
                            break;

                        case 15:
                            ColorBackgroundCashierTicket15 = deviceCashier.IsConnect ? Brushes.Green : Brushes.SlateGray;
                            break;

                        case 16:
                            ColorBackgroundCashierTicket16 = deviceCashier.IsConnect ? Brushes.Green : Brushes.SlateGray;
                            break;

                        case 17:
                            ColorBackgroundCashierTicket17 = deviceCashier.IsConnect ? Brushes.Green : Brushes.SlateGray;
                            break;
                    }

                    //LOG
                    _logger.Debug($"Кассиры на связи Id: {deviceCashier.Cashier.Id}    IsConnect: {deviceCashier.IsConnect}");
                }


                //отобразить (миганием) процесс обмена с кассами
                if (e.PropertyName == "DataExchangeSuccess")
                {
                    switch (deviceCashier.Cashier.Id)
                    {
                        case 1:
                            ColorForegroundCashierTicket1 = Brushes.Red;
                            await Task.Delay(100);
                            ColorForegroundCashierTicket1 = deviceCashier.DataExchangeSuccess ? Brushes.White : Brushes.Black;
                            break;

                        case 2:
                            ColorForegroundCashierTicket2 = Brushes.Red;
                            await Task.Delay(100);
                            ColorForegroundCashierTicket2 = deviceCashier.DataExchangeSuccess ? Brushes.White : Brushes.Black;
                            break;

                        case 3:
                            ColorForegroundCashierTicket3 = Brushes.Red;
                            await Task.Delay(100);
                            ColorForegroundCashierTicket3 = deviceCashier.DataExchangeSuccess ? Brushes.White : Brushes.Black;
                            break;

                        case 4:
                            ColorForegroundCashierTicket4 = Brushes.Red;
                            await Task.Delay(100);
                            ColorForegroundCashierTicket4 = deviceCashier.DataExchangeSuccess ? Brushes.White : Brushes.Black;
                            break;

                        case 5:
                            ColorForegroundCashierTicket5 = Brushes.Red;
                            await Task.Delay(100);
                            ColorForegroundCashierTicket5 = deviceCashier.DataExchangeSuccess ? Brushes.White : Brushes.Black;
                            break;

                        case 6:
                            ColorForegroundCashierTicket6 = Brushes.Red;
                            await Task.Delay(100);
                            ColorForegroundCashierTicket6 = deviceCashier.DataExchangeSuccess ? Brushes.White : Brushes.Black;
                            break;

                        case 7:
                            ColorForegroundCashierTicket7 = Brushes.Red;
                            await Task.Delay(100);
                            ColorForegroundCashierTicket7 = deviceCashier.DataExchangeSuccess ? Brushes.White : Brushes.Black;
                            break;

                        case 8:
                            ColorForegroundCashierTicket8 = Brushes.Red;
                            await Task.Delay(100);
                            ColorForegroundCashierTicket8 = deviceCashier.DataExchangeSuccess ? Brushes.White : Brushes.Black;
                            break;

                        case 9:
                            ColorForegroundCashierTicket9 = Brushes.Red;
                            await Task.Delay(100);
                            ColorForegroundCashierTicket9 = deviceCashier.DataExchangeSuccess ? Brushes.White : Brushes.Black;
                            break;

                        case 10:
                            ColorForegroundCashierTicket10 = Brushes.Red;
                            await Task.Delay(100);
                            ColorForegroundCashierTicket10 = deviceCashier.DataExchangeSuccess ? Brushes.White : Brushes.Black;
                            break;

                        case 11:
                            ColorForegroundCashierTicket11 = Brushes.Red;
                            await Task.Delay(100);
                            ColorForegroundCashierTicket11 = deviceCashier.DataExchangeSuccess ? Brushes.White : Brushes.Black;
                            break;

                        case 12:
                            ColorForegroundCashierTicket12 = Brushes.Red;
                            await Task.Delay(100);
                            ColorForegroundCashierTicket12 = deviceCashier.DataExchangeSuccess ? Brushes.White : Brushes.Black;
                            break;

                        case 13:
                            ColorForegroundCashierTicket13 = Brushes.Red;
                            await Task.Delay(100);
                            ColorForegroundCashierTicket13 = deviceCashier.DataExchangeSuccess ? Brushes.White : Brushes.Black;
                            break;

                        case 14:
                            ColorForegroundCashierTicket14 = Brushes.Red;
                            await Task.Delay(100);
                            ColorForegroundCashierTicket14 = deviceCashier.DataExchangeSuccess ? Brushes.White : Brushes.Black;
                            break;

                        case 15:
                            ColorForegroundCashierTicket15 = Brushes.Red;
                            await Task.Delay(100);
                            ColorForegroundCashierTicket15 = deviceCashier.DataExchangeSuccess ? Brushes.White : Brushes.Black;
                            break;

                        case 16:
                            ColorForegroundCashierTicket16 = Brushes.Red;
                            await Task.Delay(100);
                            ColorForegroundCashierTicket16 = deviceCashier.DataExchangeSuccess ? Brushes.White : Brushes.Black;
                            break;

                        case 17:
                            ColorForegroundCashierTicket17 = Brushes.Red;
                            await Task.Delay(100);
                            ColorForegroundCashierTicket17 = deviceCashier.DataExchangeSuccess ? Brushes.White : Brushes.Black;
                            break;
                    }
                }
            }
        }



        private void Listener_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var listener = sender as ListenerTcpIp;
            if (listener != null)
            {
                if (e.PropertyName == "IsConnect")
                {
                    ColorBackground = listener.IsConnect ? Brushes.SlateGray : Brushes.Magenta;
                }
                else
                if (e.PropertyName == "GetClients")
                {
                  var ipTcpClients=  listener.GetClients.Select(c=>c.Ip).ToList();
                  TerminalsIp.Clear();
                  TerminalsIp.AddRange(ipTcpClients);

                  //LOG
                  var strb = new StringBuilder("Терминалы на свзяи: ");
                  foreach (var c in ipTcpClients)
                  {
                     strb.Append(c).Append("; ");
                  }
                  _logger.Debug(strb.ToString());
                }
            }
        }



        private void _model_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var server = sender as ServerModel;
            if (server != null)
            {
                if (e.PropertyName == "ErrorString")
                {
                    _logger.Error($"ServerModel/ErrorString= {server.ErrorString}");
                }
            }
        }

        #endregion




        #region Methode
        /// <summary>
        /// Заполнить табло кассиров
        /// </summary>
        private void FillTableCashier(int cashierId, TicketItem item)
        {
            switch (cashierId)
            {
                case 1:
                    CashierTicket1 = item;
                    break;

                case 2:
                    CashierTicket2 = item;
                    break;

                case 3:
                    CashierTicket3 = item;
                    break;

                case 4:
                    CashierTicket4 = item;
                    break;

                case 5:
                    CashierTicket5 = item;
                    break;

                case 6:
                    CashierTicket6 = item;
                    break;

                case 7:
                    CashierTicket7 = item;
                    break;

                case 8:
                    CashierTicket8 = item;
                    break;

                case 9:
                    CashierTicket9 = item;
                    break;

                case 10:
                    CashierTicket10 = item;
                    break;

                case 11:
                    CashierTicket11 = item;
                    break;

                case 12:
                    CashierTicket12 = item;
                    break;

                case 13:
                    CashierTicket13 = item;
                    break;

                case 14:
                    CashierTicket14 = item;
                    break;

                case 15:
                    CashierTicket15 = item;
                    break;

                case 16:
                    CashierTicket16 = item;
                    break;

                case 17:
                    CashierTicket17 = item;
                    break;
            }
        }


        /// <summary>
        /// Заполнить табло 4x4
        /// </summary>
        private void FillTable4X4(TicketItem item, IList<TicketItem> list1, IList<TicketItem> list2, IList<TicketItem> list3, IList<TicketItem> list4)
        {
            if (list1.Count < LimitRowTable4X41)
            {
                list1.Add(item);
            }
            else
            if (list2.Count < LimitRowTable4X42)
            {
                list2.Add(item);
            }
            else
            if (list3.Count < LimitRowTable4X43)
            {
                list3.Add(item);
            }
            else
            if (list4.Count < LimitRowTable4X44)
            {
                list4.Add(item);
            }
        }


        /// <summary>
        /// Очистить табло 4x4
        /// </summary>
        private void ClearTable4X4(int removeTicketId, IList<TicketItem> list1, IList<TicketItem> list2, IList<TicketItem> list3, IList<TicketItem> list4)
        {
            bool isDelete = false;

            // Удалить элемент из нужного списка
            var removeTicket = list1.FirstOrDefault(elem => elem.CashierId == removeTicketId);
            if (removeTicket != null)
            {
                list1.Remove(removeTicket);
                isDelete = true;
            }
           
            removeTicket = list2.FirstOrDefault(elem => elem.CashierId == removeTicketId);
            if (removeTicket != null)
            {
                list2.Remove(removeTicket);
                isDelete = true;
            }
           
            removeTicket = list3.FirstOrDefault(elem => elem.CashierId == removeTicketId);
            if (removeTicket != null)
            {
                list3.Remove(removeTicket);
                isDelete = true;
            }

            removeTicket = list4.FirstOrDefault(elem => elem.CashierId == removeTicketId);
            if (removeTicket != null)
            {            
                list4.Remove(removeTicket);
                isDelete = true;
            }

            //Перезаполнить список, если элемент был удален
            if (isDelete)
            {
                var sumList = list1.Union(list2).Union(list3).Union(list4).ToList();
                list1.Clear();
                list2.Clear();
                list3.Clear();
                list4.Clear();
                foreach (var item in sumList)
                {
                    FillTable4X4(item, list1, list2, list3, list4);
                }
            }

        }


        /// <summary>
        /// Заполнить табло 8x2
        /// </summary>
        private void FillTable8X2(TicketItem item, IList<TicketItem> list1, IList<TicketItem> list2)
        {
            if (list1.Count < LimitRowTable8X21) 
            {
                list1.Add(item);
            }
            else
            if (list2.Count < LimitRowTable8X22)
            {
                list2.Add(item);
            }
        }


        /// <summary>
        /// Очистить табло 4x4
        /// </summary>
        private void ClearTable8X2(int removeTicketId, IList<TicketItem> list1, IList<TicketItem> list2)
        {
            bool isDelete = false;

            // Удалить элемент из нужного списка
            var removeTicket = list1.FirstOrDefault(elem => elem.CashierId == removeTicketId);
            if (removeTicket != null)
            {
                list1.Remove(removeTicket);
                isDelete = true;
            }

            removeTicket = list2.FirstOrDefault(elem => elem.CashierId == removeTicketId);
            if (removeTicket != null)
            {
                list2.Remove(removeTicket);
                isDelete = true;
            }

            //Перезаполнить список, если элемент был удален
            if (isDelete)
            {            
                var sumList = list1.Union(list2).ToList();
                list1.Clear();
                list2.Clear();
                foreach (var item in sumList)
                {
                    FillTable8X2(item, list1, list2);
                }
            }
        }



        private string _btnStartStopSoundQueueName = "СТОП";
        public string BtnStartStopSoundQueueName
        {
            get { return _btnStartStopSoundQueueName; }
            set
            {
                _btnStartStopSoundQueueName = value;
                NotifyOfPropertyChange(() => BtnStartStopSoundQueueName);
            }
        }
        public void StartStopSoundQueue()
        {
            switch (BtnStartStopSoundQueueName)
            {
                case "СТОП":
                    _model.SoundQueue.StopQueue();
                    BtnStartStopSoundQueueName = "СТАРТ";
                    break;

                case "СТАРТ":
                    _model.SoundQueue.StartQueue();
                    BtnStartStopSoundQueueName = "СТОП";
                    break;
            }
        }


        public void CleanSoundQueue()
        {
            _model.SoundQueue.Clear();
        }




        #region Настройки табло

        //Цвет фона заголовка
        private Brush _headerBackgroundColor;
        public Brush HeaderBackgroundColor 
        {
            get { return _headerBackgroundColor; }
            set
            {
                _headerBackgroundColor = value;
                NotifyOfPropertyChange(() => HeaderBackgroundColor);
            }
        }


        //Цвет шрифта заголовка
        private Brush _headerFontColor;
        public Brush HeaderFontColor
        {
            get { return _headerFontColor; }
            set
            {
                _headerFontColor = value;
                NotifyOfPropertyChange(() => HeaderFontColor);
            }
        }


        //Цвет строк списка
        private Brush _colorListRows;
        public Brush ColorListRows
        {
            get { return _colorListRows; }
            set
            {
                _colorListRows = value;
                NotifyOfPropertyChange(() => ColorListRows);
            }
        }


        //Цвет фона списка
        private Brush _colorListBackground;
        public Brush ColorListBackground
        {
            get { return _colorListBackground; }
            set
            {
                _colorListBackground = value;
                NotifyOfPropertyChange(() => ColorListBackground);
            }
        }


        //Цвет шрифта списка
        private Brush _listFontColor;
        public Brush ListFontColor
        {
            get { return _listFontColor; }
            set
            {
                _listFontColor = value;
                NotifyOfPropertyChange(() => ListFontColor);
            }
        }




        private FontSetting _currentFontCash;
        public FontSetting CurrentFontCash
        {
            get { return _currentFontCash; }
            set
            {
                _currentFontCash = value;
                NotifyOfPropertyChange(() => CurrentFontCash);
            }
        }


        private FontSetting _currentFont8X2;
        public FontSetting CurrentFont8X2
        {
            get { return _currentFont8X2; }
            set
            {
                _currentFont8X2 = value;
                NotifyOfPropertyChange(() => CurrentFont8X2);
            }
        }

        private FontSetting _currentFont4X4;
        public FontSetting CurrentFont4X4
        {
            get { return _currentFont4X4; }
            set
            {
                _currentFont4X4 = value;
                NotifyOfPropertyChange(() => CurrentFont4X4);
            }
        }



        public void FontChoser(string table)
        {
            Font currentFont = null;
            switch (table)
            {
                case "кассы шрифт строк":
                    currentFont = CurrentFontCash?.FontRow;
                    break;

                case "8X2 шрифт заголовка":
                    currentFont = CurrentFont8X2?.FontHeader;
                    break;

                case "8X2 шрифт строк":
                    currentFont = CurrentFont8X2?.FontRow;
                    break;

                case "4X4 шрифт заголовка":
                    currentFont = CurrentFont4X4?.FontHeader;
                    break;

                case "4X4 шрифт строк":
                    currentFont = CurrentFont4X4?.FontRow;
                    break;
            }


            var fontDialog = new FontDialog { Font = currentFont };
            if (fontDialog.ShowDialog() == DialogResult.OK)
            {
                currentFont = fontDialog.Font;
                switch (table)
                {
                    case "кассы шрифт строк":
                        CurrentFontCash.FontRow = currentFont;
                        break;

                    case "8X2 шрифт заголовка":
                        CurrentFont8X2.FontHeader= currentFont;
                        break;

                    case "8X2 шрифт строк":
                        CurrentFont8X2.FontRow = currentFont;
                        break;

                    case "4X4 шрифт заголовка":
                        CurrentFont4X4.FontHeader = currentFont;
                        break;

                    case "4X4 шрифт строк":
                        CurrentFont4X4.FontRow = currentFont;
                        break;
                }
                //TODO:Сохранить шрифты на диск CurrentFont8X2, CurrentFont4X4
            }
        }


        public void SaveTableSetting()
        {
            SaveSettingUi();
        }


        public void LoadTableSetting()
        {
            var settingUi = LoadSettingUi();
            ApplySetting(settingUi);
        }

        #endregion




        private void SaveSettingUi()
        {
            try
            {
                var settingsUi = new SettingsUi(HeaderBackgroundColor, HeaderFontColor, ColorListRows, ColorListBackground, ListFontColor, CurrentFontCash, CurrentFont8X2, CurrentFont4X4);
                var formatter = new BinaryFormatter();
                using (FileStream fs = new FileStream(SettingUiNameFile, FileMode.OpenOrCreate))
                {
                    formatter.Serialize(fs, settingsUi);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }


        private SettingsUi LoadSettingUi()
        {
            try
            {
                var formatter = new BinaryFormatter();
                using (FileStream fs = new FileStream(SettingUiNameFile, FileMode.OpenOrCreate))
                {
                    return (SettingsUi)formatter.Deserialize(fs);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

            return null;
        }


        private void ApplySetting(SettingsUi settingUi)
        {
            if (settingUi != null)
            {
                HeaderBackgroundColor = settingUi.ConvertString2Brush(settingUi.HeaderBackgroundColorString);
                HeaderFontColor = settingUi.ConvertString2Brush(settingUi.HeaderFontColorString);
                ColorListRows = settingUi.ConvertString2Brush(settingUi.ColorListRowsString);
                ColorListBackground = settingUi.ConvertString2Brush(settingUi.ColorListBackgroundString);
                ListFontColor = settingUi.ConvertString2Brush(settingUi.ListFontColorString);

                CurrentFontCash.FontRow = settingUi.ConvertString2Font(settingUi.FontCashierRowString);
                CurrentFontCash.PaddingRow = settingUi.FontCashierRowPadding;

                CurrentFont8X2.FontHeader = settingUi.ConvertString2Font(settingUi.Font8X2HeaderString);
                CurrentFont8X2.FontRow = settingUi.ConvertString2Font(settingUi.Font8X2RowString);
                CurrentFont8X2.PaddingRow = settingUi.Font8X2RowPadding;
                CurrentFont8X2.PaddingHeader = settingUi.Font8X2HeaderPadding;

                CurrentFont4X4.FontHeader = settingUi.ConvertString2Font(settingUi.Font4X4HeaderString);
                CurrentFont4X4.FontRow = settingUi.ConvertString2Font(settingUi.Font4X4RowString);
                CurrentFont4X4.PaddingRow = settingUi.Font4X4RowPadding;
                CurrentFont4X4.PaddingHeader = settingUi.Font4X4HeaderPadding;
            }
        }


        protected override void OnDeactivate(bool close)
        {
            _model.PropertyChanged -= _model_PropertyChanged;
            foreach (var devCashier in _model.DeviceCashiers)
            {
                devCashier.Cashier.PropertyChanged -= Cashier_PropertyChanged;
                devCashier.PropertyChanged -= DevCashierOnPropertyChanged;
            }
            _model.Listener.PropertyChanged -= Listener_PropertyChanged;
            _model.SoundQueue.PropertyChanged -= SoundQueue_PropertyChanged;


            _model.Dispose();
            base.OnDeactivate(close);
        }

        #endregion




        #region DEBUG

        public void Add(int idCashier)
        {
            _model.DeviceCashiers[idCashier-1].Cashier.StartHandling();
            _model.DeviceCashiers[idCashier-1].Cashier.SuccessfulStartHandling();
        }


        public void Dell(int idCashier)
        {
            //_model.DeviceCashiers[idCashier - 1].Cashier.SuccessfulHandling();

            _model.DeviceCashiers[idCashier - 1].Cashier.ErrorHandling();
        }


        public void Redirect(int idCashier)
        {
            if (_model.AdminCasher != null)
            {
                var redirectTicket = _model.DeviceCashiers[idCashier - 1].Cashier.CurrentTicket;
                if (redirectTicket != null)
                {
                    _model.AdminCasher.Cashier.AddRedirectedTicket(redirectTicket);
                }
            }

            _model.DeviceCashiers[idCashier - 1].Cashier.SuccessfulHandling();
        }



        //Авто тест-------------------------------------------------------------------
        //public Timer TimerAutoTest { get; set; } = new Timer(15000);
        //private void TimerAutoTest_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        //{
        //    TimerAutoTest.Stop();
        //    for (int i = 1 ; i < 13; i++)
        //    {
        //        Add(i);   
        //        Thread.Sleep(12000);
        //        Dell(i);
        //    }
        //    TimerAutoTest.Start();
        //}

        #endregion

    }
}