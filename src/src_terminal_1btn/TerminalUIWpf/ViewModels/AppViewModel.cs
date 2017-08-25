using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using Caliburn.Micro;
using Communication.TcpIp;
using Terminal.Model;


namespace TerminalUIWpf.ViewModels
{
    public class AppViewModel : Screen
    {
        #region field

        private readonly IWindowManager _windowManager;

        private readonly TerminalModel _model;
        private readonly Task _mainTask;

        #endregion




        #region ctor

        public AppViewModel(IWindowManager windowManager)
        {
            _windowManager = windowManager;

            _model = new TerminalModel();
            _model.PropertyChanged += _model_PropertyChanged;
            _model.ConfirmationAdded += _model_ConfirmationAdded;

            _model.LoadSetting();
            if (_model.MasterTcpIp != null)
            {
                _model.MasterTcpIp.PropertyChanged += _model_MasterTcpIp_PropertyChanged;
                _mainTask = _model.Start();
            }
        }

        #endregion




        #region prop

        private SolidColorBrush _colorBtn = Brushes.SlateGray;
        public SolidColorBrush ColorBtn
        {
            get { return _colorBtn; }
            set
            {
                _colorBtn = value;
                NotifyOfPropertyChange(() => ColorBtn);
            }
        }


        private bool _btnEnable = true;
        public bool BtnEnable
        {
            get { return _btnEnable; }
            set
            {
                _btnEnable = value;
                NotifyOfPropertyChange(() => BtnEnable);
            }
        }


        private bool _isConnect = true;
        public bool IsConnect
        {
            get { return _isConnect; }
            set
            {
                _isConnect = value;
                NotifyOfPropertyChange(() => IsConnect);
            }
        }

        #endregion




        #region EventHandler

        private void _model_MasterTcpIp_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var master = sender as MasterTcpIp;
            if (master != null)
            {
                if (e.PropertyName == "IsConnect")
                {
                    IsConnect = master.IsConnect;
                    ColorBtn = master.IsConnect ? Brushes.SlateGray : ColorBtn = Brushes.Magenta;
                }
                else if (e.PropertyName == "IsRunDataExchange")
                {
                    BtnEnable = !master.IsRunDataExchange;
                }
            }
        }


        private void _model_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var terminal = sender as TerminalModel;
            if (terminal != null)
            {
                if (e.PropertyName == "ErrorString")
                {
                    MessageBox.Show(terminal.ErrorString); //TODO: как вызвать MessageBox
                }
            }
        }


        private bool _model_ConfirmationAdded(string ticketName, string countPeople)
        {
            var dialog = new DialogViewModel { CountPeople = $"Впереди вас {countPeople} человек", TicketName = $"Номер вашего билета {ticketName}" };
            _windowManager.ShowDialog(dialog);
            return dialog.Act == Act.Ok;
        }

        #endregion




        #region Methode

        public async Task BtnVilage()
        {
            const byte numberQueue = 1;
            await _model.TrainSelection(numberQueue);
        }


        public async Task BtnLongRoad()
        {
            const byte numberQueue = 2;
            await _model.TrainSelection(numberQueue);
        }


        protected override void OnDeactivate(bool close)
        {
            _model.Dispose();
            base.OnDeactivate(close);
        }

        #endregion
    }
}