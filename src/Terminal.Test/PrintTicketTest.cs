using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terminal.Service;
using Xunit;

namespace Terminal.Test
{
    public class PrintTicketTest
    {
        [Fact]
        public void Print2Pdf()
        {
            //Arrage
            var printTicket = new PrintTicket("Microsoft Print to PDF");
            var ticketName = "Б019";
            var countPeople = "0";
            var dateAdded = DateTime.Parse("19.09.2019 15:39:52");
            //var descriptionQueue = "Оформление багажа и живности";
            var descriptionQueue = "Администратор: идентификация 14-значного номера электронного билета, восстановление утраченных и испорченных билетов, вопросы по работе билетных касс";
            //var descriptionQueue = "Оформление, возврат, переоформление, прерывание поездки, опоздание на поезд дальнего следования - внутреннее и межгосударственное сообщения"; ;
          

            //Act
            printTicket.Print(ticketName, countPeople, dateAdded, descriptionQueue);

            //Assert
        }
    }
}
