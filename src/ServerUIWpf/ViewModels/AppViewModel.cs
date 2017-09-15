using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using Caliburn.Micro;
using Communication.TcpIp;
using Server.Entitys;
using Server.Model;
using TicketItem = ServerUi.Model.TicketItem;

namespace ServerUi.ViewModels
{
    public class AppViewModel : Screen
    {
        #region field

        private readonly ServerModel _model;
        private readonly Task _mainTask;

        #endregion




        #region ctor

        public AppViewModel()
        {
            DurationAnimationListView= new Duration(TimeSpan.FromMilliseconds(1000));
            AnimationListViewFrom = 0.8;

            _model = new ServerModel();
            _model.PropertyChanged += _model_PropertyChanged;

            _model.LoadSetting();
            foreach (var devCashier in _model.DeviceCashiers)
            {
                devCashier.Cashier.PropertyChanged += Cashier_PropertyChanged;
                devCashier.PropertyChanged+= DevCashierOnPropertyChanged;
            }

            if (_model.Listener != null)
            {
                _model.Listener.PropertyChanged += Listener_PropertyChanged;
                _mainTask = _model.Start();
            }

            //_model.SoundPlayer.Volume = 100;
            //_model.SoundPlayer.Play(@"D:/music.mp3");


            //DEBUG-----------------
            List<string> IstFonts= new List<string>();
            foreach (FontFamily fontFamily in Fonts.SystemFontFamilies)
            {
                IstFonts.Add(fontFamily.Source);
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


        private Duration _durationAnimationListView;         //DEBUG
        public Duration DurationAnimationListView
        {
            get { return _durationAnimationListView; }
            set
            {
                _durationAnimationListView = value;
                NotifyOfPropertyChange(() => DurationAnimationListView);
            }
        }


        private double? _animationListViewFrom;              //DEBUG
        public double? AnimationListViewFrom
        {
            get { return _animationListViewFrom; }
            set
            {
                _animationListViewFrom = value;
                NotifyOfPropertyChange(() => AnimationListViewFrom);
            }
        }

        #endregion





        #region EventHandler

        private async void Cashier_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var сashier = sender as Сashier;
            if (сashier != null)
            {
                if (e.PropertyName == "CurrentTicket")
                {
                    if (сashier.CurrentTicket != null)     //добавить элемент к списку
                    {
                        //var ticket= new TicketItem
                        //{
                        //    CashierId = сashier.Id,
                        //    CashierName = "Касса " + сashier.CurrentTicket.Сashbox,
                        //    TicketName = $"Талон {сashier.CurrentTicket.Prefix}{сashier.CurrentTicket.NumberElement:000}"
                        //};

                        var ticket = new TicketItem
                        {
                            CashierId = сashier.Id,
                            CashierName = сashier.CurrentTicket.Сashbox.ToString(),
                            TicketName = $"{сashier.CurrentTicket.Prefix}{сashier.CurrentTicket.NumberElement:000}"
                        };

                        FillTableCashier(сashier.Id, ticket);
                        FillTable4X4(ticket, Table4X41, Table4X42, Table4X43, Table4X44);
                        FillTable8X2(ticket, Table8X21, Table8X22);
                        FillTable8X2(ticket, Table8X23, Table8X24);

                        //var task = _model.LogTicket?.Add(сashier.CurrentTicket.ToString());
                        //if (task != null) await task;
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
        private void DevCashierOnPropertyChanged(object sender, PropertyChangedEventArgs e)
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
                    }


                    //TODO: добавить IsConnect на каждого кассира
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
                    //  MessageBox.Show(server.ErrorString); //TODO: как вызвать MessageBox
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
            }
        }


        /// <summary>
        /// Заполнить табло 4x4
        /// </summary>
        private void FillTable4X4(TicketItem item, IList<TicketItem> list1, IList<TicketItem> list2, IList<TicketItem> list3, IList<TicketItem> list4)
        {
            if (list1.Count < 4)
            {
                list1.Add(item);
            }
            else
            if (list2.Count < 4)
            {
                list2.Add(item);
            }
            else
            if (list3.Count < 4)
            {
                list3.Add(item);
            }
            else
            if (list4.Count < 4)
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
            if (list1.Count < 8)
            {
                list1.Add(item);
            }
            else
            if (list2.Count < 8)
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




        protected override void OnDeactivate(bool close)
        {
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
            _model.DeviceCashiers[idCashier - 1].Cashier.SuccessfulHandling();
        }

        #endregion

    }
}