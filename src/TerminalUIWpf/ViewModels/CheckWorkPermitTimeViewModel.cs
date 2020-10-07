using System.Threading.Tasks;
using Terminal.Model;
using TerminalUIWpf.BaseViewModels;

namespace TerminalUIWpf.ViewModels
{
    public class CheckWorkPermitTimeViewModel : AutoClouseByTimerBaseVewModel
    {
        private readonly TerminalModel _model;

        #region prop
        public string Text { get; }
        #endregion


        #region ctor
        public CheckWorkPermitTimeViewModel(TerminalModel model, string text) : base(20000)
        {
            _model = model;
            Text = text;
        }
        #endregion


        #region Methode
        /// <summary>
        /// Открыть окно покупки билетов
        /// </summary>
        public async Task BtnBuyTicket()
        {
            if (!_model.IsConnectTcpIp)
                return;

            const string descriptionQueue = "Кассы";
            const string prefixQueue = "К";
            const string nameQueue = "Main";
            await _model.QueueSelection(nameQueue, prefixQueue, descriptionQueue);
            BtnExit();
        }


        /// <summary>
        /// Администратор
        /// </summary>
        public async Task BtnAdmin()
        {
            if (!_model.IsConnectTcpIp)
                return;

            const string descriptionQueue = "Администратор";
            const string prefixQueue = "А";
            const string nameQueue = "Admin";
            await _model.QueueSelection(nameQueue, prefixQueue, descriptionQueue);
            BtnExit();
        }


        public void BtnExit()
        {
            TryClose();
        }
        #endregion
    }
}