using System.Collections.Generic;
using System.ComponentModel;
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
            _model = new ServerModel();
            _model.PropertyChanged += _model_PropertyChanged;

            _model.LoadSetting();
            foreach (var devCashier in _model.DeviceCashiers)
            {
                devCashier.Cashier.PropertyChanged += Cashier_PropertyChanged;
            }

            if (_model.Listener != null)
            {
                _model.Listener.PropertyChanged += Listener_PropertyChanged;
                _mainTask = _model.Start();
            }
        }

        #endregion




        #region prop

        public BindableCollection<TicketItem> TicketItems { get; set; } = new BindableCollection<TicketItem>();


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
                        var ticket= new TicketItem
                        {
                            CashierId = сashier.Id,
                            CashierName = "Касса " + сashier.CurrentTicket.Сashbox,
                            TicketName = $"Талон {сashier.CurrentTicket.Prefix}{сashier.CurrentTicket.NumberElement.ToString("000")}"
                        };

                        FillTable4X4(ticket, Table4X41, Table4X42, Table4X43, Table4X44);


                        //TicketItems.Add(new TicketItem
                        //{
                        //    CashierId = сashier.Id,
                        //    CashierName = "Касса " + сashier.CurrentTicket.Сashbox,
                        //    TicketName = $"Талон {сashier.CurrentTicket.Prefix}{сashier.CurrentTicket.NumberElement.ToString("000")}"
                        //});

                        //var task = _model.LogTicket?.Add(сashier.CurrentTicket.ToString());
                        //if (task != null) await task;
                    }
                    else                                 //удалить элемент из списка
                    {
                        var removeItem = TicketItems.FirstOrDefault(elem => elem.CashierId == сashier.Id);
                        TicketItems.Remove(removeItem);
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
        /// Заполнить табло 4x4
        /// </summary>
        private void FillTable4X4(TicketItem item, IList<TicketItem> list1, IList<TicketItem> list2, IList<TicketItem> list3, IList<TicketItem> list4)
        {
            list1.Add(item);
            list2.Add(item);
        }



        /// <summary>
        /// Заполнить табло 8x2
        /// </summary>
        private void FillTable8X2(TicketItem item, IList<TicketItem> list1, IList<TicketItem> list2)
        {

        }



        protected override void OnDeactivate(bool close)
        {
            _model.Dispose();
            base.OnDeactivate(close);
        }

        #endregion




        #region DEBUG

        public void Add(int id)
        {
            for (int i = 0; i < 4; i++)
            {
                _model.DeviceCashiers[i].Cashier.StartHandling();
                _model.DeviceCashiers[i].Cashier.SuccessfulStartHandling();
            }

        }

        public void Dell(int id)
        {
            //for (int i = 0; i < 4; i++)
            //{
            //    _model.DeviceCashiers[i].Cashier.SuccessfulHandling();
            //}

            _model.DeviceCashiers[2].Cashier.SuccessfulHandling();
        }


        #endregion


    }
}