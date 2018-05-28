using System.Windows;
using Caliburn.Micro;
using Terminal.Service;

namespace TerminalUIWpf.ViewModels
{
    public class CheckPrinterStatusViewModel : Screen
    {
        private readonly PrintTicket _printTicketService;



        #region ctor

        public CheckPrinterStatusViewModel(PrintTicket printTicketService)
        {
            _printTicketService = printTicketService;
        }

        #endregion



        #region prop

        public string ErrorMessage { get; set; }

        #endregion




        #region Methode

        public bool CheckPrinterStatus()
        {
            var printerStat = _printTicketService.GetPrinterStatus();
            printerStat = PrinterStatus.IsOutOfPaper;  //DEBUG
            switch (printerStat)
            {
                case PrinterStatus.Ok:
                    return true;

                case PrinterStatus.QueueContainsElements:
                    ErrorMessage = "Очередь печати ПЕРЕПОЛНЕННА";
                    //MessageBox.Show("Очередь печати ПЕРЕПОЛНЕННА");
                    return false;

                case PrinterStatus.IsInError:
                    ErrorMessage = "ОШИБКА доступа к принтеру";
                    return false;

                case PrinterStatus.IsOutOfPaper:
                    ErrorMessage = "Бумага для печати талонов ЗАКОНЧИЛАСЬ";
                    return false;

                case PrinterStatus.IsPaperJammed:
                    ErrorMessage = "Бумага для печати талонов ЗАМЯТА";
                    return false;
            }
            return false;
        }


        public void BtnExit()
        {
            TryClose();
        }

        #endregion
    }
}