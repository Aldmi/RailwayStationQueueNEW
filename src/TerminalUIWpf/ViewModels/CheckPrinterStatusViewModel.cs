using System.Threading;
using System.Windows;
using Caliburn.Micro;
using Library.Logs;
using Terminal.Service;

namespace TerminalUIWpf.ViewModels
{
    public class CheckPrinterStatusViewModel : Screen
    {
        private readonly PrintTicket _printTicketService;
        private readonly Log _logger = new Log("Terminal.CheckPrintStatus");

  
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
                    _logger.Error($"ErrorPrinterStatus: {ErrorMessage} ");
                    return false;

                case PrinterStatus.IsInError:
                    ErrorMessage = "ОШИБКА доступа к принтеру";
                    _logger.Error($"ErrorPrinterStatus: {ErrorMessage} ");
                    return false;

                case PrinterStatus.IsOutOfPaper:
                    ErrorMessage = "Бумага для печати талонов ЗАКОНЧИЛАСЬ";
                    _logger.Error($"ErrorPrinterStatus: {ErrorMessage} ");
                    return false;

                case PrinterStatus.IsPaperJammed:
                    ErrorMessage = "Бумага для печати талонов ЗАМЯТА";
                    _logger.Error($"ErrorPrinterStatus: {ErrorMessage} ");
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