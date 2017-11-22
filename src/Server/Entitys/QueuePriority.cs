﻿using System;
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

        private object _locker = new object();//TODO: сделать критичеакие секции на доступ к Queue

        public string Name { get; set; }
        public List<Prefix> Prefixes { get; set; } // список типов очередей

        private ConcurrentQueue<TicketItem> Queue { get; set; } = new ConcurrentQueue<TicketItem>();
        public int Count => Queue.Count;
        public bool IsEmpty => Queue.IsEmpty;


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
            var item = new TicketItem {Prefix = prefix, Priority = 0};
            var priority = Prefixes.FirstOrDefault(p => p.Name == prefix)?.Priority;
            if (priority.HasValue)
            {
                item.Priority = priority.Value;
            }
        
            var items = new List<TicketItem>(Queue) { item };
            var ordered = items.OrderByDescending(t => t.Priority).ToList();
            return ordered.IndexOf(item);
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
            var items= new List<TicketItem>(Queue) {item};
            var ordered= items.OrderByDescending(t => t.Priority);
            Queue= new ConcurrentQueue<TicketItem>(ordered);
        }



        /// <summary>
        /// Показать первый элемент из очереди, по совпадению с элементами prefixes
        /// </summary>
        public TicketItem PeekByPriority(IList<string> prefixes)
        {
            var priorityItem = GetFirstPriorityItem(prefixes);
            return priorityItem;
        }


        /// <summary>
        /// Извлечь первый элемент из очереди, по совпадению с элементами prefixes
        /// </summary>
        public TicketItem DequeueByPriority(IList<string> prefixes)
        {
            var priorityItem = GetFirstPriorityItem(prefixes);
            if (priorityItem != null)
            {
                var items = new List<TicketItem>(Queue);
                items.Remove(priorityItem);
                Queue = new ConcurrentQueue<TicketItem>(items);
                return priorityItem;
            }
            return null;
        }


        private TicketItem GetFirstPriorityItem(IEnumerable<string> prefixes)
        {
            foreach (var pref in prefixes)
            {
                if (pref == "All")
                {
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