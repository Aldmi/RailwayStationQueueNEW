using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.AccessControl;
using Communication.Annotations;
using Server.Service;

namespace Server.Entitys
{
    public class QueuePriority : INotifyPropertyChanged
    {
        #region prop
        //Объект синхронизации.
        //Паралельное наполнение очереди (4 терминала)
        //Паралельное изьятие кассирами билетов (2 послед порта)
        private readonly object _locker = new object();

        public string Name { get; set; }
        public List<Prefix> Prefixes { get; set; } // список типов очередей

        private ConcurrentQueue<TicketItem> Queue { get; set; } = new ConcurrentQueue<TicketItem>();
        public int Count => Queue.Count;
        public bool IsEmpty => Queue.IsEmpty;
        public IEnumerable<TicketItem> GetQueueItems => Queue;

        public IEnumerable<TicketItem> SetQueueItems
        {
            set
            {
                if (value != null && value.Any())
                {
                    Queue = new ConcurrentQueue<TicketItem>(value);
                    OnPropertyChanged("QueuePriority");
                }
            }
        }


        public TicketFactory TicketFactory { get; set; } = new TicketFactory();
        public uint GetCurrentTicketNumber => TicketFactory.GetCurrentTicketNumber;

        public uint SetCurrentTicketNumber
        {
            set { TicketFactory.SetCurrentTicketNumber = value; }
        }

        #endregion




        #region ctor

        public QueuePriority(string name, List<Prefix> prefixes)
        {
            Name = name;
            Prefixes = prefixes;
        }

        #endregion




        #region Events

        public event PropertyChangedEventHandler PropertyChanged;
        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion




        #region Methode

        /// <summary>
        /// Определить место вставки элемента в очередь, исходя из приоритета.
        /// </summary>
        public int GetInseartPlace(string prefix)
        {
            return Queue.Count(t => t.Prefix == prefix);
        }


        public TicketItem CreateTicket(string prefix)
        {
            lock (_locker)
            {
                var inseartPlase = (ushort) GetInseartPlace(prefix);
                var ticket = TicketFactory.Create(inseartPlase, prefix);
                var priority = Prefixes.FirstOrDefault(p => p.Name == prefix)?.Priority;
                if (priority.HasValue)
                {
                    ticket.Priority = priority.Value;
                }
                return ticket;
            }
        }


        /// <summary>
        /// Добавить элемент в очередь, по приориету.
        /// </summary>
        public void Enqueue(TicketItem item)
        {
            lock (_locker)
            {
                var items = new List<TicketItem>(Queue) {item};
                var ordered = items.OrderByDescending(t => t.Priority).ThenBy(t=>t.AddedTime);
                Queue = new ConcurrentQueue<TicketItem>(ordered);
                OnPropertyChanged("QueuePriority");
            }
        }



        /// <summary>
        /// Показать первый элемент из очереди, по совпадению с элементами cachier.prefixes
        /// </summary>
        public TicketItem PeekByPriority(ICasher cachier)
        {
            lock (_locker)
            {
                var priorityItem = GetFirstPriorityItem(cachier);
                return priorityItem;
            }
        }


        /// <summary>
        /// Извлечь первый элемент из очереди, по совпадению с элементами cachier.prefixes
        /// </summary>
        public TicketItem DequeueByPriority(ICasher cachier)
        {
            lock (_locker)
            {
                var priorityItem = GetFirstPriorityItem(cachier);
                if (priorityItem != null)
                {
                    var items = new List<TicketItem>(Queue);
                    items.Remove(priorityItem);
                    Queue = new ConcurrentQueue<TicketItem>(items);
                    OnPropertyChanged("QueuePriority");
                    return priorityItem;
                }
                return null;
            }
        }


        private TicketItem GetFirstPriorityItem(ICasher cachier)
        {
            foreach (var pref in cachier.Prefixes)
            {
                if (pref == "All")
                {
                    //Поиск превого билета который не попадает под исключения
                    if (cachier.PrefixesExclude != null && cachier.PrefixesExclude.Any())
                    {
                        foreach (var item in Queue)
                        {
                            if (!cachier.PrefixesExclude.Contains(item.Prefix))
                                return item;
                        }
                        return null;
                    }

                    //Список исключенгий пуст, вернем первый элемент
                    return Queue.FirstOrDefault();
                }

                var priorityItem = Queue.FirstOrDefault(q => q.Prefix == pref);
                if (priorityItem != null)
                {
                    return priorityItem;
                }
            }
            return null;
        }

        #endregion
    }
}