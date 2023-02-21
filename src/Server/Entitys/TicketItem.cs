using System;

namespace Server.Entitys
{
    [Serializable]
    public class TicketItem
    {
        public string Prefix { get; set; }          // строковый префикс элемента
        public uint NumberElement { get; set; }     // номер в очереди на момент добавления
        public ushort CountElement { get; set; }    // кол-во клиентов в очереди на момент добавления
        public DateTime AddedTime{ get; set; }      // дата добавления в очередь
        public DateTime StartProcessingTime { get; set; }  // дата добавления в обработку
        public DateTime EndProcessingTime { get; set; }    // дата окончания обработки
        public TimeSpan ProcessingTime { get; set; }      // Время обработки (EndProcessingTime - StartProcessingTime)
        public TimeSpan ServiceTime { get; set; }        // Время обслуживания (EndProcessingTime - AddedTime)
        public TimeSpan WaitingTime { get; set; }        // Время ожидания (StartProcessingTime - AddedTime)

        public int? CashboxId { get; set; }           // номер кассира
        public byte CountTryHandling { get; set; }  // количество попыток обработки этого билета кассиром
        public int Priority { get; set; }           // приоритет билета в очереди



        /// <summary>
        /// Вычислить TimeProcessing, ServiceTime, WaitingTime
        /// </summary>
        public void CalculateTime()
        {
            ProcessingTime = EndProcessingTime - StartProcessingTime;
            ServiceTime = EndProcessingTime - AddedTime;
            WaitingTime = StartProcessingTime - AddedTime;
        }

     
        public string GetTicketName=> Prefix + NumberElement.ToString("000");
        public override string ToString() => $";  Дата добавления в очередь: {AddedTime};  Дата поступления в обработку: {StartProcessingTime};  Дата окончания обработки: {EndProcessingTime};  Номер билета: {GetTicketName};  Номер кассира: {CashboxId?.ToString() ?? "неизвестный кассир" } ";
    }
}