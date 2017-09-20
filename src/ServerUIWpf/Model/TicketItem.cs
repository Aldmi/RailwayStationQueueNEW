
namespace ServerUi.Model
{
    public class TicketItem
    {
        public string TicketName { get; set; }
        public string CashierName { get; set; }
        public int CashierId { get; set; }


        public override string ToString()
        {
            return $"Талон {TicketName} Касса {CashierName}";
        }
    }
}
