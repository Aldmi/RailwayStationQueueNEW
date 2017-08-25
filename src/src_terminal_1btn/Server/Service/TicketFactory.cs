using System;
using Server.Entitys;

namespace Server.Service
{
    public class TicketFactory
    {
        #region field

        private const uint MaxTicketNumber = 999;
        private readonly string _ticketPrefix;
        private uint _ticketNumber;
        private int _currentDay;
    
        #endregion




        #region ctor

        public TicketFactory(string ticketPrefix)
        {
            _ticketPrefix = ticketPrefix;
            _currentDay = DateTime.Now.Day;
        }

        #endregion






        #region prop      

        public uint GetCurrentTicketNumber
        {
            get { return _ticketNumber; }
        }

        #endregion




        public TicketItem Create(ushort countElement)
        {
            if (++_ticketNumber >= MaxTicketNumber)
                _ticketNumber = 0;

            if (DateTime.Now.Day != _currentDay)           //Обнуление номера билета каждые сутки.
            {
                _ticketNumber = 0;
                _currentDay = DateTime.Now.Day;
            }

            return new TicketItem() { NumberElement = _ticketNumber, CountElement = countElement, AddedTime = DateTime.Now, Prefix = _ticketPrefix, Сashbox = null, CountTryHandling = 0 };
        }
    }
}