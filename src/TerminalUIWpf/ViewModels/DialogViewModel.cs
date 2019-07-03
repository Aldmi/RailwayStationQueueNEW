using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using Caliburn.Micro;
using Timer = System.Timers.Timer;

namespace TerminalUIWpf.ViewModels
{
    public enum Act
    {
        Ok, Cancel, Undefined
    }

    public class DialogViewModel : Screen
    {
        #region field

        private readonly IWindowManager _windowManager;
        private const double TimerPeriod = 8000; // Таймер закрытия окна
        private readonly Timer _timer;

        #endregion




        #region ctor

        public DialogViewModel(IWindowManager windowManager)
        {
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



        private void _timer_AutoCloseWindow(object sender, ElapsedEventArgs e)
        {
            Act = Act.Cancel;
            TryClose();
        }

        #endregion





        #region prop

        public string TicketName { get; set; }
        public string CountPeople { get; set; }
        public string Description { get; set; }

        public Act Act { get; set; }

        #endregion




        #region Methode

        public void BtnOk()
        {
            var dialog = new PrintMessageViewModel();
            _windowManager.ShowWindow(dialog);

            Act = Act.Ok;
            TryClose();
        }

        public void BtnCancel()
        {
            Act = Act.Cancel;
            TryClose();
        }

        #endregion
    }
}