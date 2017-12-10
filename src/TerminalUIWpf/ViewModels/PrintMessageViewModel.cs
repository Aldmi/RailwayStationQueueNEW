using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using Caliburn.Micro;
using Timer = System.Timers.Timer;

namespace TerminalUIWpf.ViewModels
{
    public class PrintMessageViewModel : Screen
    {
        #region field

        private const double TimerPeriod = 2000; // Таймер закрытия окна
        private readonly Timer _timer;

        #endregion




        #region ctor

        public PrintMessageViewModel()
        {
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
            TryClose();
        }

        #endregion
    }
}
