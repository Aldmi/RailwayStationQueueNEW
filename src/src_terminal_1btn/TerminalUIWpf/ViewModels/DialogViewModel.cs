using Caliburn.Micro;

namespace TerminalUIWpf.ViewModels
{
    public enum Act
    {
        Ok, Cancel, Undefined
    }

    public class DialogViewModel : Screen
    {
        #region prop

        public string TicketName { get; set; }
        public string CountPeople { get; set; }

        public Act Act { get; set; }

        #endregion




        #region Methode

        public void BtnOk()
        {
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