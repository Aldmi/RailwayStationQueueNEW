using TerminalUIWpf.BaseViewModels;

namespace TerminalUIWpf.ViewModels
{
    public class CheckWorkPermitTimeViewModel : AutoClouseByTimerBaseVewModel
    {
        #region prop
        public string Text { get; }
        #endregion


        #region ctor
        public CheckWorkPermitTimeViewModel(string text) : base(10000)
        {
            Text = text;
        }
        #endregion


        #region Methode
        public void BtnExit()
        {
            TryClose();
        }
        #endregion
    }
}