using System.Timers;
using Caliburn.Micro;
using TerminalUIWpf.ViewModels;

namespace TerminalUIWpf.BaseViewModels
{
    public abstract class AutoClouseByTimerBaseVewModel : Screen
    {
        #region field
        private readonly double _time;
        private const double TimerPeriod = 8000; // Таймер закрытия окна
        private readonly Timer _timer;
        #endregion


        #region prop
        public Act Act { get; protected set; }
        #endregion


        #region ctor
        protected AutoClouseByTimerBaseVewModel(double time)
        {
            _time = time;
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

        private void _timer_AutoCloseWindow(object sender, ElapsedEventArgs e)
        {
            Act = Act.Cancel;
            TryClose();
        }
        #endregion
    }
}