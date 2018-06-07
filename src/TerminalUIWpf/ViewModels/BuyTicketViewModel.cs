using System.Threading.Tasks;
using System.Timers;
using Caliburn.Micro;
using Terminal.Model;


namespace TerminalUIWpf.ViewModels
{
    public class BuyTicketViewModel : Screen
    {
        #region field

        private readonly TerminalModel _model;
        private readonly IWindowManager _windowManager;

        private const double TimerPeriod = 15000;// Таймер закрытия окна. Автосброс, если нажата любая кнопка.
        private readonly Timer _timer;

        #endregion




        #region ctor

        public BuyTicketViewModel(TerminalModel model, IWindowManager windowManager)
        {
            _model = model;
            _windowManager = windowManager;
            _timer = new Timer(TimerPeriod);
            _timer.Elapsed += _timer_AutoCloseWindow;
        }

        #endregion




        #region EventHandler

        protected override void OnInitialize()
        {
            StartTimer();
            base.OnInitialize();
        }


        protected override void OnDeactivate(bool close)
        {
            _timer.Stop();
            _timer.Close();
        }


        private void StartTimer()
        {
            _timer.Enabled = true;
            _timer.Start();
        }

        private void StopTimer()
        {
            _timer.Stop();
        }

        private void ResetTimer()
        {
            _timer.Interval = TimerPeriod;
            _timer.Start();
        }


        private void _timer_AutoCloseWindow(object sender, ElapsedEventArgs e)
        {
            TryClose();
        }

        #endregion




        #region Methode

        /// <summary>
        /// Купить билет
        /// </summary>
        public async Task BtnBuyTicket()
        {
            StopTimer();
            var сheckPrinterStatusVm = new CheckPrinterStatusViewModel(_model.PrintTicketService);
            if (!сheckPrinterStatusVm.CheckPrinterStatus())
            {
                _windowManager.ShowDialog(сheckPrinterStatusVm);
                return;
            }  
            const string descriptionQueue = "Купить билет / Возврат билета / замена персональных данных в билете / переоформление билетов / оформление багажа";
            const string prefixQueue = "К";
            const string nameQueue = "Main";
            await _model.QueueSelection(nameQueue, prefixQueue, descriptionQueue);
            ResetTimer();
        }

        /// <summary>
        /// Купить билет в международном сообщении
        /// </summary>
        public async Task BtnBuyInterstateTicket()
        {
            StopTimer();
            var сheckPrinterStatusVm = new CheckPrinterStatusViewModel(_model.PrintTicketService);
            if (!сheckPrinterStatusVm.CheckPrinterStatus())
            {
                _windowManager.ShowDialog(сheckPrinterStatusVm);
                return;
            }
            const string descriptionQueue = "Купить билет в страны Европы, Монголию, Китай";
            const string prefixQueue = "М";
            const string nameQueue = "Main";
            await _model.QueueSelection(nameQueue, prefixQueue, descriptionQueue);
            ResetTimer();
        }

        /// <summary>
        /// Оформление групп пассажиров
        /// </summary>
        public async Task BtnGroupsTicket()
        {
            StopTimer();
            var сheckPrinterStatusVm = new CheckPrinterStatusViewModel(_model.PrintTicketService);
            if (!сheckPrinterStatusVm.CheckPrinterStatus())
            {
                _windowManager.ShowDialog(сheckPrinterStatusVm);
                return;
            }
            const string descriptionQueue = "Оформление организованных групп пассажиров (по предварительным заявкам)";
            const string prefixQueue = "Г";
            const string nameQueue = "Main";
            await _model.QueueSelection(nameQueue, prefixQueue, descriptionQueue);
            ResetTimer();
        }

        /// <summary>
        /// Оформление маломобильных пассажиров
        /// </summary>
        public async Task BtnLowMobilityTicket()
        {
            StopTimer();
            var сheckPrinterStatusVm = new CheckPrinterStatusViewModel(_model.PrintTicketService);
            if (!сheckPrinterStatusVm.CheckPrinterStatus())
            {
                _windowManager.ShowDialog(сheckPrinterStatusVm);
                return;
            }
            const string descriptionQueue = "Оформление маломобильных пассажиров";
            const string prefixQueue = "И";
            const string nameQueue = "Main";
            await _model.QueueSelection(nameQueue, prefixQueue, descriptionQueue);
            ResetTimer();
        }

        /// <summary>
        /// Возврат
        /// </summary>
        public async Task BtnReturnTicket()
        {
            StopTimer();
            var сheckPrinterStatusVm = new CheckPrinterStatusViewModel(_model.PrintTicketService);
            if (!сheckPrinterStatusVm.CheckPrinterStatus())
            {
                _windowManager.ShowDialog(сheckPrinterStatusVm);
                return;
            }
            const string descriptionQueue = "В";
            const string prefixQueue = "В";
            const string nameQueue = "Main";
            await _model.QueueSelection(nameQueue, prefixQueue, descriptionQueue);
            ResetTimer();
        }

        /// <summary>
        /// Переоформление
        /// </summary>
        public async Task BtnReformTicket()
        {
            StopTimer();
            var сheckPrinterStatusVm = new CheckPrinterStatusViewModel(_model.PrintTicketService);
            if (!сheckPrinterStatusVm.CheckPrinterStatus())
            {
                _windowManager.ShowDialog(сheckPrinterStatusVm);
                return;
            }
            const string descriptionQueue = "П";
            const string prefixQueue = "П";
            const string nameQueue = "Main";
            await _model.QueueSelection(nameQueue, prefixQueue, descriptionQueue);
            ResetTimer();
        }

        /// <summary>
        /// Восстановление утерянных, испорченных билетов
        /// </summary>
        public async Task BtnRestoreTicket()
        {
            StopTimer();
            var сheckPrinterStatusVm = new CheckPrinterStatusViewModel(_model.PrintTicketService);
            if (!сheckPrinterStatusVm.CheckPrinterStatus())
            {
                _windowManager.ShowDialog(сheckPrinterStatusVm);
                return;
            }
            const string descriptionQueue = "У";
            const string prefixQueue = "У";
            const string nameQueue = "Main";
            await _model.QueueSelection(nameQueue, prefixQueue, descriptionQueue);
            ResetTimer();
        }

        /// <summary>
        /// Замена персональных данных в билете
        /// </summary>
        public async Task BtnReplacementPersonalData()
        {
            StopTimer();
            var сheckPrinterStatusVm = new CheckPrinterStatusViewModel(_model.PrintTicketService);
            if (!сheckPrinterStatusVm.CheckPrinterStatus())
            {
                _windowManager.ShowDialog(сheckPrinterStatusVm);
                return;
            }
            const string descriptionQueue = "З";
            const string prefixQueue = "З";
            const string nameQueue = "Main";
            await _model.QueueSelection(nameQueue, prefixQueue, descriptionQueue);
            ResetTimer();
        }


        public void BtnClouseWindow()
        {
            TryClose();
        }

        #endregion
    }
}