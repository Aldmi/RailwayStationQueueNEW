using System;
using Server.Entitys;

namespace Server.Service
{
    public class TicketFactory
    {
        #region field

        private const uint MaxTicketNumber = 999;
        private uint _ticketNumber;
        private int _currentDay;
    
        #endregion




        #region ctor

        public TicketFactory()
        {
            _currentDay = DateTime.Now.Day;
        }

        #endregion




        #region prop      

        public uint GetCurrentTicketNumber => _ticketNumber;

        public uint SetCurrentTicketNumber         //Установка начального значения номера билета, после восстановления состояния очереди (перезагрузки)
        {
            set { _ticketNumber = value; }
        }

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

            return new TicketItem() { NumberElement = _ticketNumber, CountElement = countElement, AddedTime = DateTime.Now, Prefix = ticketPrefix, Cashbox = null, CountTryHandling = 0 };
        }
    }
}