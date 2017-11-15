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

        public string Name { get; set; }
        public List<Prefix> Prefixes { get; set; } // список типов очередей

        private ConcurrentQueue<TicketItem> Queue { get; set; } = new ConcurrentQueue<TicketItem>();
        public int Count => Queue.Count;

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



        public void Enqueue(TicketItem item)
        {
            var items= new List<TicketItem>(Queue) {item};
            var ordered= items.OrderByDescending(t => t.Priority);

            Queue= new ConcurrentQueue<TicketItem>(ordered);
        }


        public TicketItem Dequeue()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}