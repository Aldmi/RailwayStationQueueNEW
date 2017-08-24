using System;

namespace Server.Entitys
{
    public class TicketItem
    {
        public string Prefix { get; set; }          // строковый префикс элемента (A или B)
        public uint NumberElement { get; set; }     // номер в очереди на момент добавления
        public ushort CountElement { get; set; }    // кол-во клиентов в очереди на момент добавления
        public DateTime AddedTime{ get; set; }      // дата добавления
        public int? Сashbox { get; set; }           // номер кассира
        public byte CountTryHandling { get; set; }  // количество попыток обработки этого билета кассиром

        public override string ToString()
        {
            var ticketName = Prefix + NumberElement.ToString("000");
            return $" Дата поступления в обработку: {AddedTime}       Номер билета: {ticketName}      Номер кассира:   {  Сashbox?.ToString() ?? "неизвестный кассир" } ";
        }
    }
}