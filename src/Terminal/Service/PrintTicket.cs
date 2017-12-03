﻿using System;
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
            //string filePath = Path.Combine(Directory.GetCurrentDirectory(), "Picture", "RZD_logo.jpg");
            //if (File.Exists(filePath))
            //    e.Graphics.DrawImage(Image.FromFile(filePath), 10, 5);

            //ПЕЧАТЬ ТЕКСТА
            string printText = $"              Уважаемые пассажиры!\r\n" +
                               $"    Вы можете через транзакционные\r\n" +
                               $"терминалы самообслуживания (ТТС)\r\n" +
                               $"         распечатать билеты, ранее\r\n" +
                               $"    оформленные через Интернет или\r\n" +
                               $"  самостоятельно приобрести билеты\r\n" +
                               $"    при условии наличия банковской\r\n" +
                               $"карты и если до отправления поезда\r\n" +
                               $" осталось не менее 30 минут, кроме\r\n" +
                               $"     льготных категорий пассажиров\r\n";

            Font printFont = new Font("Times New Roman", 4, FontStyle.Regular, GraphicsUnit.Millimeter);
            e.Graphics.DrawString(printText, printFont, Brushes.Black, 0, 2);

            e.Graphics.DrawLine(new Pen(Color.Black), 5, 185, 245, 185);

            //ПЕЧАТЬ ТЕКСТА
            printText = $"{_ticketName}\r\n";
            printFont = new Font("Times New Roman", 20, FontStyle.Regular, GraphicsUnit.Millimeter);
            e.Graphics.DrawString(printText, printFont, Brushes.Black, 33, 190);//9,150

            printText =$"перед вами {_countPeople} чел.\r\n";
            printFont = new Font("Times New Roman", 7, FontStyle.Regular, GraphicsUnit.Millimeter);
            e.Graphics.DrawString(printText, printFont, Brushes.Black, 12, 285); //9,260

            printText = "\r\n \r\n ";
            printText += $"{_dateAdded.ToString("T")}            {_dateAdded.ToString("d")}";
            printFont = new Font("Times New Roman", 5, FontStyle.Regular, GraphicsUnit.Millimeter);
            e.Graphics.DrawString(printText, printFont, Brushes.Black, 5, 300);
        }


        /// <summary>
        /// Печать с логотипом РЖД
        /// </summary>
        //private void Pd_PrintPage(object sender, PrintPageEventArgs e)
        //{
        //    //ПЕЧАТЬ ЛОГОТИПА
        //    string filePath = Path.Combine(Directory.GetCurrentDirectory(), "Picture", "RZD_logo.jpg");
        //    if (File.Exists(filePath))
        //        e.Graphics.DrawImage(Image.FromFile(filePath), 10, 5);

        //    e.Graphics.DrawLine(new Pen(Color.Black), 5, 130, 245, 130);

        //    //ПЕЧАТЬ ТЕКСТА
        //    string printText = $"{_ticketName}\r\n";
        //    Font printFont = new Font("Times New Roman", 20, FontStyle.Regular, GraphicsUnit.Millimeter);
        //    e.Graphics.DrawString(printText, printFont, Brushes.Black, 33, 150);//9,150

        //    printText = $"перед вами {_countPeople} чел.\r\n";
        //    printFont = new Font("Times New Roman", 7, FontStyle.Regular, GraphicsUnit.Millimeter);
        //    e.Graphics.DrawString(printText, printFont, Brushes.Black, 12, 260); //9,260

        //    printText = "\r\n \r\n ";
        //    printText += $"{_dateAdded.ToString("T")}            {_dateAdded.ToString("d")}";
        //    printFont = new Font("Times New Roman", 5, FontStyle.Regular, GraphicsUnit.Millimeter);
        //    e.Graphics.DrawString(printText, printFont, Brushes.Black, 5, 300);
        //}

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