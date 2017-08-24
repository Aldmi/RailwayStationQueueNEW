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
                    if (сashier.CurrentTicket != null)      //добавить элемент к списку
                    {

                        TicketItems.Add(new TicketItem { CashierName = "Касса " + сashier.CurrentTicket.Сashbox,
                                                         TicketName =  $"Талон {сashier.CurrentTicket.Prefix}{сashier.CurrentTicket.NumberElement.ToString("000")}" });
                        var task = _model.LogTicket?.Add(сashier.CurrentTicket.ToString());
                        if (task != null) await task;
                    }
                    else                             //удалить элемент из списка
                    {
                        var removeItem = TicketItems.FirstOrDefault(elem => elem.CashierName.Contains(сashier.Id.ToString()));
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
                    MessageBox.Show(server.ErrorString); //TODO: как вызвать MessageBox
                }
            }
        }

        #endregion




        #region Methode

        protected override void OnDeactivate(bool close)
        {
            _model.Dispose();
            base.OnDeactivate(close);
        }

        #endregion

    }
}