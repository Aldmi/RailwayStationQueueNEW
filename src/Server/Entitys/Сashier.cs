using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Communication.Annotations;
using System.Collections.Concurrent;


namespace Server.Entitys
{
    /// <summary>
    /// Узкий интрефейс кассира
    /// </summary>
    public interface ICasher : INotifyPropertyChanged
    {
        List<string> Prefixes { get; }
        List<string> PrefixesExclude { get; }
    }



    public class Сashier : ICasher
    {
        #region Fields

        private readonly byte _maxCountTryHandin;
        private readonly QueuePriority _queueTicket;

        // Внутренняя очередь кассира, хранит Перенаправленные билеты, самая приоритетная на извлечение.
        private readonly Queue<TicketItem> _internalQueueTicket = new Queue<TicketItem>(); 

        #endregion




        #region prop
        public byte Id { get; }
        public List<string> Prefixes { get; }
        public List<string> PrefixesExclude { get; }

        private TicketItem _currentTicket;
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

        #endregion



        
        #region ctor

        public Сashier(byte id, List<string> prefixes, QueuePriority queueTicket, byte maxCountTryHanding)
        {
            Id = id;

            _queueTicket = queueTicket;
            _maxCountTryHandin = maxCountTryHanding;

           //Выделить префиксы идущие за All.
           var prefixExclude= prefixes.SkipWhile(p => p != "All").ToList();
           if (prefixExclude.Count > 1)
           {
               PrefixesExclude = prefixExclude.Skip(1).ToList();
           }

            //Выделить префиксы идущие перед All.
            Prefixes = (PrefixesExclude) == null ? prefixes : prefixes.Except(PrefixesExclude).ToList();
        }

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

            //Если внутреняя очередь не пуста, работаем с ней
            if (_internalQueueTicket != null && _internalQueueTicket.Any())
            {
                var newTicket = _internalQueueTicket.Peek();
                newTicket.Сashbox = Id;
                return newTicket;
            }

            //Работаем с внешней очередью
            if (!_queueTicket.IsEmpty && CurrentTicket == null)
            {
                var newTicket = _queueTicket.PeekByPriority(this);
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
            //Если внутреняя очередь не пуста, работаем с ней
            if (_internalQueueTicket != null && _internalQueueTicket.Any())
            {
                var newTicket = _internalQueueTicket.Dequeue();
                newTicket.Сashbox = Id;
                CurrentTicket = newTicket;
                CurrentTicket.CountTryHandling++;
                return;
            }

            //Работаем с внешней очередью
            if (!_queueTicket.IsEmpty && CurrentTicket == null)
            {
                TicketItem newTicket = _queueTicket.DequeueByPriority(this);
                if (newTicket != null)
                {
                    newTicket.Сashbox = Id;
                    CurrentTicket = newTicket;
                    CurrentTicket.CountTryHandling++;
                }
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


        /// <summary>
        /// Добавить перенаправленный билет
        /// </summary>
        public void AddRedirectedTicket(TicketItem ticket)
        {
            _internalQueueTicket.Enqueue(ticket);
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