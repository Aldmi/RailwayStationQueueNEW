using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Printing;
using System.Text;
using System.Windows.Forms;
using Terminal.Settings;

namespace Terminal.Service
{
    public enum PrinterStatus
    {
        Ok,                                // Все хорошо
        QueueContainsElements,             // Очередь содержит элементы
        IsInError,                         // Ошибка принтера
        IsOutOfPaper,                      // Отсутсвует бумага
        IsPaperJammed,                     // Замята бумага
        HasPaperProblem                    // Проблема с бумагой
    }


    public class PrintTicket : IDisposable
    {
        #region Field

        private readonly PrintDocument _printDocument;

        private string _ticketName;
        private string _countPeople;
        private DateTime _dateAdded;
        private string _descriptionQueue;

        private readonly PrintServer _printServer;
        private readonly PrintQueue _printQueue;

        #endregion



        #region ctor

        public PrintTicket(string printerName)
        {
            var printersNames = PrinterSettings.InstalledPrinters;

            if (printersNames == null || printersNames.Count == 0)
                throw new Exception("ПРИНТЕРЫ НЕ НАЙДЕННЫ В СИСТЕМЕ");

            bool isFind = false;
            for (int i = 0; i < printersNames.Count; i++)
            {
                if (printersNames[i] == printerName)
                {
                    isFind = true;
                    break;
                }
            }
            if (!isFind)
                throw new Exception($"ПРИНТЕРА С ИМЕНЕМ {printerName} НЕ НАЙДЕННО В СИСТЕМЕ");

            PrinterSettings ps = new PrinterSettings { PrinterName = printerName };
            _printDocument = new PrintDocument { PrinterSettings = ps };
            _printDocument.PrintPage += Pd_PrintPage;

            _printServer = new PrintServer();
            _printQueue = _printServer.GetPrintQueues().FirstOrDefault(printer => printer.FullName == printerName);
            if (_printQueue == null)
                throw new Exception($"ОЧЕРЕДЬ ПЕЧАТИ ДЛЯ ПРИНТЕРА С ИМЕНЕМ {printerName} НЕ НАЙДЕННО В СИСТЕМЕ");
        }

        public PrintTicket(XmlPrinterSettings settings) : this(settings.PrinterName)
        {

        }

        #endregion




        #region Event
        //$"качеству обслуживания, Вы можете\r\n" - самая широкая строка
        private void Pd_PrintPage(object sender, PrintPageEventArgs e)
        {
            //ПЕЧАТЬ Название офиса
            var printText = "Новосибирск-Главный";
            var printFont = new Font("Times New Roman", 4, FontStyle.Regular, GraphicsUnit.Millimeter);
            e.Graphics.DrawString(printText, printFont, Brushes.Black, 33, 2);

            //ПЕЧАТЬ Номера билета
            printText = $"{_ticketName}\r\n";
            printFont = new Font("Times New Roman", 18, FontStyle.Regular, GraphicsUnit.Millimeter);
            e.Graphics.DrawString(printText, printFont, Brushes.Black, 23, 10);

            //ПЕЧАТЬ Название операции
            var listStrings= _descriptionQueue.SubstringWithWholeWords(42).ToList();
            printText = listStrings.Aggregate((s, s1) => s+ "\r\n" +s1);
            printFont = new Font("Times New Roman", 3, FontStyle.Bold, GraphicsUnit.Millimeter);
            e.Graphics.DrawString(printText, printFont, Brushes.Black, 8, 81);

            //ПЕЧАТЬ Памятки1
            printText = $"При возникновении вопросов по качеству\r\n" +
                        $"обслуживания или конфликтных ситуаций\r\n" +
                        $"      вы можете обратиться в кассу №10\r\n" +
                        $"   \"Администратор\" или по телефону:\r\n";
            printFont = new Font("Times New Roman", (float)2.6, FontStyle.Regular, GraphicsUnit.Millimeter);
            e.Graphics.DrawString(printText, printFont, Brushes.Black, 20, 140);

            //ПЕЧАТЬ Телефона
            printText = "+7 (913) 901-61-67";
            printFont = new Font("Times New Roman", (float)3.5, FontStyle.Regular, GraphicsUnit.Millimeter);
            e.Graphics.DrawString(printText, printFont, Brushes.Black, 45, 188);

            //ПЕЧАТЬ QR кода
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), "Pictures", "QR_Rzd.jpg");
            if (File.Exists(filePath))
                e.Graphics.DrawImage(Image.FromFile(filePath), 7, 218);

            //ПЕЧАТЬ памятки 2
            printText = $" Купить билет самостоятельно,\r\n" +
                        $"     быстро и без ожидания Вы\r\n" +
                        $"             можете  на сайте\r\n" +
                        $"   www.rzd.ru или в мобильном\r\n" +
                        $"приложении \"РЖД Пассажирам\"\r\n" +
                        $" для платформ Android и iOS\"\r\n";

            printFont = new Font("Times New Roman", (float)2.4, FontStyle.Regular, GraphicsUnit.Millimeter);
            //e.Graphics.DrawString(printText, printFont, Brushes.Black, 112, 252);
            e.Graphics.DrawString(printText, printFont, Brushes.Black, 130, 243);


            //printText = $"перед вами {_countPeople} чел.\r\n";
            //printFont = new Font("Times New Roman", 7, FontStyle.Regular, GraphicsUnit.Millimeter);
            //e.Graphics.DrawString(printText, printFont, Brushes.Black, 12, 310); //9,260

            //printText = "\r\n \r\n ";
            //printText += $"{_dateAdded.ToString("T")}            {_dateAdded.ToString("d")}";
            //printFont = new Font("Times New Roman", 5, FontStyle.Regular, GraphicsUnit.Millimeter);
            //e.Graphics.DrawString(printText, printFont, Brushes.Black, 5, 300);
        }

        #endregion




        #region Methode

        //private PrinterStatus _lastStatus;
        public PrinterStatus GetPrinterStatus()
        {
            PrinterStatus status = PrinterStatus.Ok;
            var queue = _printQueue.GetPrintJobInfoCollection();
            var count = queue.Count();
            //MessageBox.Show($"Count={count}   NumberOfJobs={_printQueue.NumberOfJobs}   QueueStatus= { _printQueue.QueueStatus}");//DEBUG
            if (count > 0)
            {
                status = PrinterStatus.QueueContainsElements;
            }
            else
            if (_printQueue.IsInError)
                status = PrinterStatus.IsInError;
            else
            if (_printQueue.IsOutOfPaper)
                status = PrinterStatus.IsOutOfPaper;
            else
            if (_printQueue.IsPaperJammed)
                status = PrinterStatus.IsOutOfPaper;
            else
            if (_printQueue.HasPaperProblem)
                status = PrinterStatus.HasPaperProblem;

            return status;
        }





        public void Print(string ticketName, string countPeople, DateTime dateAdded, string descriptionQueue)
        {
            _ticketName = ticketName;
            _countPeople = countPeople;
            _dateAdded = dateAdded;
            _descriptionQueue = descriptionQueue;

            _printDocument.Print();
        }


        #endregion




        #region IDisposable

        public void Dispose()
        {
            _printDocument?.Dispose();
            _printServer?.Dispose();
            _printQueue?.Dispose();
        }

        #endregion
    }
}