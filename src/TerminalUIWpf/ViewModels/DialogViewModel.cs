using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using Caliburn.Micro;
using TerminalUIWpf.BaseViewModels;
using Timer = System.Timers.Timer;

namespace TerminalUIWpf.ViewModels
{
    public enum Act
    {
        Ok, Cancel, Undefined
    }

    public class DialogViewModel : AutoClouseByTimerBaseVewModel
    {
        #region field
        private readonly IWindowManager _windowManager;
        #endregion


        #region prop
        public string TicketName { get; set; }
        public string CountPeople { get; set; }
        public string Description { get; set; }
        #endregion


        #region ctor
        public DialogViewModel(IWindowManager windowManager) : base(8000)
        {
            _windowManager = windowManager;
        }
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