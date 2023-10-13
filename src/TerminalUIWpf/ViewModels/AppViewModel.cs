using System;
using System.ComponentModel;
using System.Data;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Media;
using Caliburn.Micro;
using Communication.TcpIp;
using CSharpFunctionalExtensions;
using Library.Logs;
using Terminal.Model;
using Terminal.Service;


namespace TerminalUIWpf.ViewModels
{
    public class AppViewModel : Screen
    {
        #region field

        private readonly IWindowManager _windowManager;

        private readonly TerminalModel _model;
        private readonly Task _mainTask;

       // private readonly Log _logger = new Log("Terminal.CommandAddItem");//DEBUG

        private readonly Timer _timerDateTime;

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

            //обновелние времени раз в 10сек
            DateTimeNowStr = DateTime.Now.ToString("f");
            _timerDateTime = new Timer(10000) {AutoReset = true};
            _timerDateTime.Start();
            _timerDateTime.Elapsed += (sender, args) =>
            {
                DateTimeNowStr = DateTime.Now.ToString("f"); 
            };
        }

        #endregion




        #region prop

        private SolidColorBrush _colorBtn = Brushes.SlateGray;
        public SolidColorBrush ColorBtn
        {
            get => _colorBtn;
            set
            {
                _colorBtn = value;
                NotifyOfPropertyChange(() => ColorBtn);
            }
        }


        private bool _btnEnable = true;
        public bool BtnEnable
        {
            get => _btnEnable;
            set
            {
                _btnEnable = value;
                NotifyOfPropertyChange(() => BtnEnable);
            }
        }


        private bool _isConnect = true;
        public bool IsConnect
        {
            get => _isConnect;
            set
            {
                _isConnect = value;
                NotifyOfPropertyChange(() => IsConnect);
            }
        }


        private string _dateTimeNowStr = "";
        public string DateTimeNowStr
        {
            get => _dateTimeNowStr;
            set
            {
                _dateTimeNowStr = value;
                NotifyOfPropertyChange(() => DateTimeNowStr);
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
                    ColorBtn = master.IsConnect ? Brushes.SlateGray : ColorBtn = Brushes.DarkRed;
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


        private bool _model_ConfirmationAdded(string ticketName, string countPeople, string description)
        {
            var dialog = new DialogViewModel(_windowManager) { CountPeople = $"Впереди вас {countPeople} человек", TicketName = $"Номер вашего талона {ticketName}", Description = description};
            _windowManager.ShowDialog(dialog);
            return dialog.Act == Act.Ok;
        }

        #endregion




        #region Methode
        
        public async Task BtnBuyTicket()
        {
            const string descriptionQueue = "Купить билет оформить возврат";
            const string prefixQueue = "А";

            if (!_model.IsConnectTcpIp)
                return;

            if (!CheckPrinterStatus())
                return;

            if (!CheckWorkPermitTime(prefixQueue))
                return;

            var (_, isFailure, prefixeConf, error) = _model.PrefixesConfig.GetConf(prefixQueue);
            if (isFailure)
            {
                ViewErrorMessage4Staff(error);
                return;
            }

            await _model.QueueSelection(prefixeConf.QueueName, prefixQueue, descriptionQueue);
        }

        
        public async Task BtnPrivilegesTicket()
        {
            const string descriptionQueue = "Обслуживание маломобильных пассажиров и льготной категории граждан";
            const string prefixQueue = "М";

            if (!_model.IsConnectTcpIp)
                return;

            if (!CheckPrinterStatus())
                return;

            if (!CheckWorkPermitTime(prefixQueue))
                return;

            var (_, isFailure, prefixeConf, error) = _model.PrefixesConfig.GetConf(prefixQueue);
            if (isFailure)
            {
                ViewErrorMessage4Staff(error);
                return;
            }
            await _model.QueueSelection(prefixeConf.QueueName, prefixQueue, descriptionQueue);
        }

        
        public async Task BtnMilitaryPeopleTicket()
        {
            const string descriptionQueue = "Оформление по воинским перевозочным документам";
            const string prefixQueue = "В";

            if (!_model.IsConnectTcpIp)
                return;

            if (!CheckPrinterStatus())
                return;

            if (!CheckWorkPermitTime(prefixQueue))
                return;

            var (_, isFailure, prefixeConf, error) = _model.PrefixesConfig.GetConf(prefixQueue);
            if (isFailure)
            {
                ViewErrorMessage4Staff(error);
                return;
            }
            await _model.QueueSelection(prefixeConf.QueueName, prefixQueue, descriptionQueue);
        }
        
        public async Task BtnBaggageTicket()
        {
            const string descriptionQueue = "Оформление багажа и животных / выдача платных справок";
            const string prefixQueue = "Б";

            if (!_model.IsConnectTcpIp)
                return;

            if (!CheckPrinterStatus())
                return;

            if (!CheckWorkPermitTime(prefixQueue))
                return;

            var (_, isFailure, prefixeConf, error) = _model.PrefixesConfig.GetConf(prefixQueue);
            if (isFailure)
            {
                ViewErrorMessage4Staff(error);
                return;
            }
            await _model.QueueSelection(prefixeConf.QueueName, prefixQueue, descriptionQueue);
        }
        
        
        public async Task BtnMemberSvoTicket()
        {
            const string descriptionQueue = "Участник СВО";
            const string prefixQueue = "С";

            if (!_model.IsConnectTcpIp)
                return;

            if (!CheckPrinterStatus())
                return;

            if (!CheckWorkPermitTime(prefixQueue))
                return;

            var (_, isFailure, prefixeConf, error) = _model.PrefixesConfig.GetConf(prefixQueue);
            if (isFailure)
            {
                ViewErrorMessage4Staff(error);
                return;
            }
            await _model.QueueSelection(prefixeConf.QueueName, prefixQueue, descriptionQueue);
        }
        
        
        private bool CheckPrinterStatus()
        {
            var сheckPrinterStatusVm = new CheckPrinterStatusViewModel(_model.PrintTicketService);
            if (сheckPrinterStatusVm.CheckPrinterStatus())
                return true;

            _windowManager.ShowDialog(сheckPrinterStatusVm);
            return false;
        }


        private bool CheckWorkPermitTime(string prefixQueue)
        {
            var (workTime, isPermited) = _model.CheckWorkPermitTime(prefixQueue);
            if (!isPermited)
            {
                return true;
            }

            var checkWorkPermitTimeVm= new CheckWorkPermitTimeViewModel(_model, workTime.ToString());
            _windowManager.ShowDialog(checkWorkPermitTimeVm);
            return false;
        }


        private void ViewErrorMessage4Staff(string error)
        {
            MessageBox.Show(error, "ОШИБКА НАСТРОЙКИ ТЕРМИНАЛА", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.DefaultDesktopOnly);
        }


        protected override void OnDeactivate(bool close)
        {
            _timerDateTime.Stop();
            _timerDateTime.Close();
            _timerDateTime.Dispose();
            _model.Dispose();
            base.OnDeactivate(close);
        }

        #endregion
    }
}