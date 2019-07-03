using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Server.Entitys;

namespace Server.SerializableModel
{
    [Serializable]
    public class QueuePrioritysModelSerializable
    {
        public List<QueuePriorityModelSerializable> Queues { get; set; } = new List<QueuePriorityModelSerializable>();
        public List<СashierModelSerializable> Cashiers { get; set; } = new List<СashierModelSerializable>();
    }



    [Serializable]
    public class QueuePriorityModelSerializable
    {
        public string Name { get; set; }
        public uint CurrentTicketNumber { get; set; }
        public List<TicketItem> Queue { get; set; } = new List<TicketItem>();
    }



    [Serializable]
    public class СashierModelSerializable
    {
        public int Id { get; set; }
        public TicketItem CurrenTicketItem { get; set; }
    }


}