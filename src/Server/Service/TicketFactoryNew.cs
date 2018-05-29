using System;
using Server.Entitys;

namespace Server.Service
{
    public class TicketFactoryNew
    {
        #region field

        private const uint MaxTicketNumber = 999;
        private uint _ticketNumber;
        private int _currentDay;
    
        #endregion




        #region ctor

        public TicketFactoryNew()
        {
            _currentDay = DateTime.Now.Day;
        }

        #endregion




        #region prop      

        public uint GetCurrentTicketNumber => _ticketNumber;

        #endregion




        public TicketItem Create(ushort countElement, string ticketPrefix)
        {
            if (++_ticketNumber >= MaxTicketNumber)
                _ticketNumber = 0;

            if (DateTime.Now.Day != _currentDay)           //Обнуление номера билета каждые сутки.
            {
                _ticketNumber = 0;
                _currentDay = DateTime.Now.Day;
            }

            return new TicketItem() { NumberElement = _ticketNumber, CountElement = countElement, AddedTime = DateTime.Now, Prefix = ticketPrefix, Сashbox = null, CountTryHandling = 0 };
        }
    }
}