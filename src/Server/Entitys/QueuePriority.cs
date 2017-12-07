using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using Server.Service;

namespace Server.Entitys
{
    public class QueuePriority
    {
        #region prop
        //Объект синхронизации.
        // Паралельное наполнение очереди (4 терминала)
        //Паралельное изьятие кассирами билетов (2 послед порта)
        private readonly object _locker = new object();

        public string Name { get; set; }
        public List<Prefix> Prefixes { get; set; } // список типов очередей

        private ConcurrentQueue<TicketItem> Queue { get; set; } = new ConcurrentQueue<TicketItem>();
        public int Count => Queue.Count;
        public bool IsEmpty => Queue.IsEmpty;
        public IEnumerable<TicketItem> GetQueueItems => Queue.ToList();

        public IEnumerable<TicketItem> SetQueueItems
        {
            set
            {
                if (value != null && value.Any())
                {
                    Queue = new ConcurrentQueue<TicketItem>(value);
                }
            }
        }


        public TicketFactoryNew TicketFactory { get; set; } = new TicketFactoryNew();
        public uint GetCurrentTicketNumber => TicketFactory.GetCurrentTicketNumber;

        #endregion




        #region ctor

        public QueuePriority(string name, List<Prefix> prefixes)
        {
            Name = name;
            Prefixes = prefixes;
        }

        #endregion





        #region Methode

        /// <summary>
        /// Определить место вставки элемента в очередь, исходя из приоритета.
        /// </summary>
        public int GetInseartPlace(string prefix)
        {
            lock (_locker)
            {
                var item = new TicketItem {Prefix = prefix, Priority = 0};
                var priority = Prefixes.FirstOrDefault(p => p.Name == prefix)?.Priority;
                if (priority.HasValue)
                {
                    item.Priority = priority.Value;
                }

                var items = new List<TicketItem>(Queue) {item};
                var ordered = items.OrderByDescending(t => t.Priority).ToList();
                return ordered.IndexOf(item);
            }
        }


        public TicketItem CreateTicket(string prefix)
        {
            var inseartPlase= (ushort)GetInseartPlace(prefix);
            var ticket = TicketFactory.Create(inseartPlase, prefix);
            var priority = Prefixes.FirstOrDefault(p => p.Name == prefix)?.Priority;
            if (priority.HasValue)
            {
                ticket.Priority = priority.Value;
            }
            return ticket;
        }


        /// <summary>
        /// Добавить элемент в очередь
        /// </summary>
        public void Enqueue(TicketItem item)
        {
            lock (_locker)
            {
                var items = new List<TicketItem>(Queue) {item};
                var ordered = items.OrderByDescending(t => t.Priority);
                Queue = new ConcurrentQueue<TicketItem>(ordered);
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