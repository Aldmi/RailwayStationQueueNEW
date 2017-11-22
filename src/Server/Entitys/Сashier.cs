using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Communication.Annotations;
using System.Collections.Concurrent;


namespace Server.Entitys
{
    public class Сashier : INotifyPropertyChanged
    {
        #region Fields

        private readonly byte _maxCountTryHandin;
        private readonly QueuePriority _queueTicket;
        private readonly List<string> _prefixes;   //префиксы в порядке приоритета.
        private TicketItem _currentTicket;

        #endregion




        #region ctor

        public Сashier(byte id, List<string> prefixes, QueuePriority queueTicket, byte maxCountTryHanding)
        {
            Id = id;
            _prefixes = prefixes;
            _queueTicket = queueTicket;
            _maxCountTryHandin = maxCountTryHanding;
        }

        #endregion




        #region prop

        public TicketItem CurrentTicket
        {
            get { return _currentTicket; }
            set
            {
                if (Equals(value, _currentTicket))
                    return;
                _currentTicket = value;
                OnPropertyChanged();
            }
        }

        public byte Id { get; }

        #endregion




        #region Methode

        /// <summary>
        /// Показали 1-ый элемент в очереди (без извлечения из очереди)
        /// </summary>    
        public TicketItem StartHandling()
        {
            //Отправка синхронизации билета.
            //если кассир выключил устройство, не обработав приглашенного посетителя, то после включения ус-ва и нажатия кнопки "Следующий"
            //к кассиру придет текущий необработанный билет.
            if (CurrentTicket != null)
            {
                return CurrentTicket;
            }

            if (!_queueTicket.IsEmpty && CurrentTicket == null)
            {
                var newTicket = _queueTicket.PeekByPriority(_prefixes);
                if (newTicket != null)
                {
                    newTicket.Сashbox = Id;
                    return newTicket;
                }
            }
            return null;
        }


        /// <summary>
        /// Извлекли 1-ый элемент из очереди и сделали его текущим обрабатываемым
        /// </summary>   
        public void SuccessfulStartHandling()
        {
            if (!_queueTicket.IsEmpty && CurrentTicket == null)
            {
                TicketItem newTicket = _queueTicket.DequeueByPriority(_prefixes);
                if (newTicket != null)
                {
                    newTicket.Сashbox = Id;
                    CurrentTicket = newTicket;
                    CurrentTicket.CountTryHandling++;
                }
                //if (_queueTicket.TryDequeue(out newTicket))
                //{
                //    newTicket.Сashbox = Id;
                //    CurrentTicket = newTicket;
                //    CurrentTicket.CountTryHandling++;
                //}
            }
        }


        /// <summary>
        /// Успешная обработка клиента.
        /// </summary>
        public TicketItem SuccessfulHandling()
        {
            CurrentTicket = null;
            return CurrentTicket;
        }


        /// <summary>
        /// Клиент не обрабаотанн. Увеличиваем счетчик попыток обработки.
        /// Не обработанного клиента добавляем в конец очереди.
        /// </summary>
        public TicketItem ErrorHandling()
        {
            if (CurrentTicket?.CountTryHandling < _maxCountTryHandin)
              _queueTicket.Enqueue(CurrentTicket);               

            CurrentTicket = null;
            return CurrentTicket;
        }

        public void DisconectHandling()
        {
            if (CurrentTicket != null)
            {
                _queueTicket.Enqueue(CurrentTicket);
                CurrentTicket = null;
            }
        }

        #endregion




        #region Events

        public event PropertyChangedEventHandler PropertyChanged;
        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}