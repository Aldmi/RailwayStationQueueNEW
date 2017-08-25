using System;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using Terminal.Settings;

namespace Terminal.Service
{
    public class PrintTicket : IDisposable
    {
        #region Field

        private readonly PrintDocument _printDocument;

        private string _ticketName;
        private string _countPeople;
        private DateTime _dateAdded;
        #endregion




        #region ctor

        public PrintTicket(string printerName)
        {
            var printersNames = PrinterSettings.InstalledPrinters;

           if(printersNames == null || printersNames.Count == 0)
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
            if(!isFind)
                throw new Exception($"ПРИНТЕРА С ИМЕНЕМ {printerName} НЕ НАЙДЕННО В СИСТЕМЕ");

            PrinterSettings ps = new PrinterSettings {PrinterName = printerName};
            _printDocument = new PrintDocument {PrinterSettings = ps};
            _printDocument.PrintPage += Pd_PrintPage;
        }


        public PrintTicket(XmlPrinterSettings settings) : this(settings.PrinterName)
        { 
        }

        #endregion




        #region Event

        private void Pd_PrintPage(object sender, PrintPageEventArgs e)
        {
            //ПЕЧАТЬ ЛОГОТИПА
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), "Picture", "RZD_logo.jpg");
            if (File.Exists(filePath))
                e.Graphics.DrawImage(Image.FromFile(filePath), 5, 5);

            e.Graphics.DrawLine(new Pen(Color.Black), 5, 130, 245, 130);

            //ПЕЧАТЬ ТЕКСТА
            string printText = $"{_ticketName}\r\n";
            Font printFont = new Font("Times New Roman", 20, FontStyle.Regular, GraphicsUnit.Millimeter);
            e.Graphics.DrawString(printText, printFont, Brushes.Black, 9, 150);

            printText =$"перед вами {_countPeople} чел.\r\n";
            printFont = new Font("Times New Roman", 7, FontStyle.Regular, GraphicsUnit.Millimeter);
            e.Graphics.DrawString(printText, printFont, Brushes.Black, 9, 260);

            printText = "\r\n \r\n ";
            printText += $"{_dateAdded.ToString("T")}         {_dateAdded.ToString("d")}";
            printFont = new Font("Times New Roman", 5, FontStyle.Regular, GraphicsUnit.Millimeter);
            e.Graphics.DrawString(printText, printFont, Brushes.Black, 5, 300);
        }

        #endregion




        #region Methode

        public void Print(string ticketName, string countPeople, DateTime dateAdded)
        {
            _ticketName = ticketName;
            _countPeople = countPeople;
            _dateAdded = dateAdded;

            _printDocument.Print();
        }

        #endregion




        #region IDisposable

        public void Dispose()
        {
            _printDocument?.Dispose();
        }

        #endregion
    }
}