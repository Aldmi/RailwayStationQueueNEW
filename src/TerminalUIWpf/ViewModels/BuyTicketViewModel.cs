using System.Threading.Tasks;
using Caliburn.Micro;
using Terminal.Model;

namespace TerminalUIWpf.ViewModels
{
    public class BuyTicketViewModel : Screen
    {
        #region field

        private readonly TerminalModel _model;

        #endregion




        #region ctor

        public BuyTicketViewModel(TerminalModel model)
        {
            _model = model;
        }

        #endregion





        #region Methode

        /// <summary>
        /// Купить билет
        /// </summary>
        public async Task BtnBuyTicket()
        {
            const string prefixQueue = "Я";
            const string nameQueue = "Main";
            await _model.QueueSelection(nameQueue, prefixQueue);
        }



        public void BtnClouseWindow()
        {
            TryClose();
        }


        #endregion
    }
}