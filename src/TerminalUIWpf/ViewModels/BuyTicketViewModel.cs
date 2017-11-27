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
            const string prefixQueue = "К";
            const string nameQueue = "Main";
            await _model.QueueSelection(nameQueue, prefixQueue);
        }

        /// <summary>
        /// Купить билет в международном сообщении
        /// </summary>
        public async Task BtnBuyInterstateTicket()
        {
            const string prefixQueue = "М";
            const string nameQueue = "Main";
            await _model.QueueSelection(nameQueue, prefixQueue);
        }

        /// <summary>
        /// Оформление групп пассажиров
        /// </summary>
        public async Task BtnGroupsTicket()
        {
            const string prefixQueue = "Г";
            const string nameQueue = "Main";
            await _model.QueueSelection(nameQueue, prefixQueue);
        }

        /// <summary>
        /// Оформление маломобильных пассажиров
        /// </summary>
        public async Task BtnLowMobilityTicket()
        {
            const string prefixQueue = "И";
            const string nameQueue = "Main";
            await _model.QueueSelection(nameQueue, prefixQueue);
        }

        /// <summary>
        /// Возврат
        /// </summary>
        public async Task BtnReturnTicket()
        {
            const string prefixQueue = "В";
            const string nameQueue = "Main";
            await _model.QueueSelection(nameQueue, prefixQueue);
        }

        /// <summary>
        /// Переоформление
        /// </summary>
        public async Task BtnReformTicket()
        {
            const string prefixQueue = "П";
            const string nameQueue = "Main";
            await _model.QueueSelection(nameQueue, prefixQueue);
        }

        /// <summary>
        /// Восстановление утерянных, испорченных билетов
        /// </summary>
        public async Task BtnRestoreTicket()
        {
            const string prefixQueue = "У";
            const string nameQueue = "Main";
            await _model.QueueSelection(nameQueue, prefixQueue);
        }

        /// <summary>
        /// Замена персональных данных в билете
        /// </summary>
        public async Task BtnReplacementPersonalData()
        {
            const string prefixQueue = "З";
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