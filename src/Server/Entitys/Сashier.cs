using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Communication.Annotations;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Library.Logs;


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
        private readonly string _logName;
        private readonly QueuePriority _queueTicket;

        // Внутренняя очередь кассира, хранит Перенаправленные билеты, самая приоритетная на извлечение.
        private readonly Queue<TicketItem> _internalQueueTicket = new Queue<TicketItem>();
        private readonly Log _loggerCashierInfo;

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

        public TicketItem PreviousTicket { get; set; }

        public bool CanHandling => (CurrentTicket != null);

        #endregion




        #region ctor

        public Сashier(byte id, List<string> prefixes, QueuePriority queueTicket, byte maxCountTryHanding, string logName)
        {
            Id = id;

            _queueTicket = queueTicket;
            _maxCountTryHandin = maxCountTryHanding;
            _logName = logName;
            _loggerCashierInfo = new Log(_logName);

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
        /// Успешная обработка клиента.
        /// </summary>
        public async Task<TicketItem> SuccessfulHandlingAsync(CancellationToken ct)
        {
            _loggerCashierInfo.Info($"Команда от кассира: \"SuccessfulHandling (Успешная обработка клиента.)\"  Id= {Id}  NameTicket= {(CurrentTicket != null ? CurrentTicket.Prefix + CurrentTicket.NumberElement.ToString("000") : string.Empty)}");//LOG
            PreviousTicket = CurrentTicket;
            CurrentTicket = null;
            return CurrentTicket;
        }




        /// <summary>
        /// Показали 1-ый элемент в очереди (без извлечения из очереди)
        /// </summary>    
        public TicketItem StartHandling()
        {
            //Отправка синхронизации билета.
            //если кассир выключил устройство, не обработав приглашенного посетителя, то после включения ус-ва и нажатия кнопки "Следующий"
            //к кассиру придет текущий необработанный билет.
            if (CurrentTicket != null)//TODO:???
            {
                CurrentTicket.Cashbox = Id;
                _loggerCashierInfo.Info($"Команда от кассира: \"StartHandling. (Отправка синхронизации билета)\"  Id= {Id}  NameTicket= {(CurrentTicket != null ? CurrentTicket.Prefix + CurrentTicket.NumberElement.ToString("000") : string.Empty )}");//LOG
                return CurrentTicket;
            }

            //Если внутреняя очередь не пуста, работаем с ней
            if (_internalQueueTicket != null && _internalQueueTicket.Any())
            {
                var newTicket = _internalQueueTicket.Peek();
                newTicket.Cashbox = Id;
                _loggerCashierInfo.Info($"Команда от кассира [НАЧАЛО ТРАНЗАЦИИ]: \"StartHandling (внутреняя очередь)\"  Id= {Id}  NameTicket= NameTicket= {newTicket.Prefix + newTicket.NumberElement.ToString("000")}");//LOG
                return newTicket;
            }

            //Работаем с внешней очередью
            if (!_queueTicket.IsEmpty && CurrentTicket == null)
            {
                var newTicket = _queueTicket.PeekByPriority(this);
                if (newTicket != null)
                {
                    if (newTicket.Cashbox != null) //Билет из очереди не должен быть в обработке кассиром.
                    {
                        _loggerCashierInfo.Info($"Команда от кассира: \"StartHandling (!!!!!!!!!!!Билет из очереди не должен быть в обработке кассиром)!!!!!!!!!!\"  Id= {Id}  NameTicket= {newTicket.Prefix + newTicket.NumberElement.ToString("000")}");//LOG
                        //return null;//TODO: Прверить лог на возниконверние этого условия
                    }

                    newTicket.Cashbox = Id;
                    _loggerCashierInfo.Info($"Команда от кассира [НАЧАЛО ТРАНЗАЦИИ]: \"StartHandling (Работаем с внешней очередью)\"  Id= {Id}  NameTicket= {newTicket.Prefix + newTicket.NumberElement.ToString("000")}");//LOG
                    return newTicket;
                }
            }
            return null;
        }


        /// <summary>
        /// Извлекли 1-ый элемент из очереди и сделали его текущим обрабатываемым (CurrentTicket)
        /// </summary>   
        public void SuccessfulStartHandling()
        {
            //Ответ на Отправка синхронизации билета.
            if (CurrentTicket != null) 
            {
                CurrentTicket.Cashbox = Id;
                CurrentTicket.CountTryHandling++;
                _loggerCashierInfo.Info($"Команда от кассира [КОНЕЦ ТРАНЗАЦИИ]: \"SuccessfulStartHandling (Ответ на Отправка синхронизации билета)\"  Id= {Id}  NameTicket= {(CurrentTicket != null ? CurrentTicket.Prefix + CurrentTicket.NumberElement.ToString("000") : string.Empty)}");//LOG
                return;
            }

            //Если внутреняя очередь не пуста, работаем с ней
            if (_internalQueueTicket != null && _internalQueueTicket.Any())
            {
                var newTicket = _internalQueueTicket.Dequeue();
                newTicket.Cashbox = Id;
                CurrentTicket = newTicket;
                CurrentTicket.CountTryHandling++;
                _loggerCashierInfo.Info($"Команда от кассира [КОНЕЦ ТРАНЗАЦИИ]: \"SuccessfulStartHandling (внутреняя очередь)\"  Id= {Id}  NameTicket= {(CurrentTicket != null ? CurrentTicket.Prefix + CurrentTicket.NumberElement.ToString("000") : string.Empty)}");//LOG
                return;
            }

            //Работаем с внешней очередью
            if (!_queueTicket.IsEmpty && CurrentTicket == null)
            {
                TicketItem newTicket = _queueTicket.DequeueByPriority(this);
                if (newTicket != null)
                {
                    newTicket.Cashbox = Id;
                    CurrentTicket = newTicket;
                    CurrentTicket.CountTryHandling++;
                    _loggerCashierInfo.Info($"Команда от кассира [КОНЕЦ ТРАНЗАЦИИ]: \"SuccessfulStartHandling (Работаем с внешней очередью)\"  Id= {Id}  NameTicket= {(CurrentTicket != null ? CurrentTicket.Prefix + CurrentTicket.NumberElement.ToString("000") : string.Empty)}");//LOG
                }
            }
        }


        /// <summary>
        /// Успешная обработка клиента.
        /// </summary>
        public TicketItem SuccessfulHandling()
        {
            _loggerCashierInfo.Info($"Команда от кассира: \"SuccessfulHandling (Успешная обработка клиента.)\"  Id= {Id}  NameTicket= {(CurrentTicket != null ? CurrentTicket.Prefix + CurrentTicket.NumberElement.ToString("000") : string.Empty)}");//LOG
            PreviousTicket = CurrentTicket;
            CurrentTicket = null;
            return CurrentTicket;
        }


        /// <summary>
        /// Клиент не обрабаотанн. Увеличиваем счетчик попыток обработки.
        /// Не обработанного клиента добавляем в конец очереди.
        /// </summary>
        public TicketItem ErrorHandling()
        {
            if (CurrentTicket == null)
                return null;

            CurrentTicket.Cashbox= null;
            CurrentTicket.AddedTime= DateTime.Now;
            if (CurrentTicket?.CountTryHandling < _maxCountTryHandin)
              _queueTicket.Enqueue(CurrentTicket);

            _loggerCashierInfo.Info($"Команда от кассира: \"ErrorHandling (Клиент не обрабаотанн.)\"  Id= {Id}    CountTryHandling= {CurrentTicket?.CountTryHandling}  NameTicket= {(CurrentTicket != null ? CurrentTicket.Prefix + CurrentTicket.NumberElement.ToString("000") : string.Empty)}");//LOG
            PreviousTicket = CurrentTicket;
            CurrentTicket = null;
            return CurrentTicket;
        }


        public void DisconectHandling()
        {
            if (CurrentTicket != null)
            {
                _queueTicket.Enqueue(CurrentTicket);
                PreviousTicket = CurrentTicket;
                CurrentTicket = null;
            }
        }


        /// <summary>
        /// Добавить перенаправленный билет
        /// </summary>
        public void AddRedirectedTicket(TicketItem ticket)
        {
            _loggerCashierInfo.Info($"Команда от кассира: \"AddRedirectedTicket (Добавить перенаправленный билет.)\"  Id= {Id}  NameTicket= {(CurrentTicket != null ? CurrentTicket.Prefix + CurrentTicket.NumberElement.ToString("000") : string.Empty)}");//LOG
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