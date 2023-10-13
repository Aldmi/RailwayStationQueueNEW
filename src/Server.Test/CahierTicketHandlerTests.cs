using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Server.Entitys;
using Xunit;

namespace Server.Test
{
    public class CahierTicketHandlerTests
    {
        private readonly QueuePriority _queue;
        public CahierTicketHandlerTests()
        {
            _queue = new QueuePriority("Main", new List<Prefix>
                {
                    new Prefix {Name = "A", Priority = 10},
                    new Prefix {Name = "B", Priority = 10},
                    new Prefix {Name = "C", Priority = 10},
                    new Prefix {Name = "D", Priority = 10}
                });
            
            _queue.Enqueue(new TicketItem(){});
        }


        private void InitQueue(IReadOnlyList<string> newTicketPrefixes)
        {
            foreach (var newTicketPrefix in newTicketPrefixes)
            {
                _queue.Enqueue(_queue.CreateTicket(newTicketPrefix));
            }
        }
        
        
        [Fact]
        public void StartHandling_Ok()
        {
            //Arrage
            InitQueue(new[] {"B","C","D","B","A","B","A","A"});
            var casher = new Сashier(1, new List<string>{"D", "B", "C"}, _queue, 2, "test");
            
            //Act Assert
            var ticket1= casher.StartHandling();
            var ticket2= casher.StartHandling();
            var ticket3= casher.StartHandling();
            
            //Assert
            ticket1.GetTicketName.Should().Be("D003");
            ticket2.Should().BeNull();
            ticket3.Should().BeNull();
        }
        
        
        
        [Fact]
        public void StartHandling_StartHandlingSuccessful_Ok()
        {
            //Arrage
            InitQueue(new[] {"B","C","D","B","A","B","A","A"});
            var casher = new Сashier(1, new List<string>{"D", "B", "C"}, _queue, 2, "test");
            
            //Act Assert
            var ticket1= casher.StartHandling();
            casher.StartHandlingSuccessful();
            
            var ticket2= casher.StartHandling();
            casher.StartHandlingSuccessful();
            
            var ticket3= casher.StartHandling();
            casher.StartHandlingSuccessful();
            
            //Assert
            ticket1.GetTicketName.Should().Be("D003");
            ticket2.GetTicketName.Should().Be("D003");
            ticket3.GetTicketName.Should().Be("D003");
        }
        
        
        [Fact]
        public void StartHandling_StartHandlingSuccessful_SuccessfulHandling_Ok()
        {
            //Arrage
            InitQueue(new[] {"B","C","D","B","A","B","A","A"});
            var casher = new Сashier(1, new List<string>{"D", "B", "C"}, _queue, 2, "test");
            
            //Act 
            var ticket1= casher.StartHandling();
            casher.StartHandlingSuccessful();
            casher.SuccessfulHandling();
            
            var ticket2= casher.StartHandling();
            casher.StartHandlingSuccessful();
            casher.SuccessfulHandling();
            
            var ticket3= casher.StartHandling();
            casher.StartHandlingSuccessful();
            casher.SuccessfulHandling();
            
            var ticket4= casher.StartHandling();
            casher.StartHandlingSuccessful();
            casher.SuccessfulHandling();
            
            var ticket5= casher.StartHandling();
            casher.StartHandlingSuccessful();
            casher.SuccessfulHandling();
            
            var ticket6= casher.StartHandling();
            casher.StartHandlingSuccessful();
            casher.SuccessfulHandling();
            
            //Элементы кончились добавим новые
            InitQueue(new[] {"B","A","A"});
            
            var ticket7= casher.StartHandling();
            casher.StartHandlingSuccessful();
            casher.SuccessfulHandling();
            
            //Assert
            ticket1.GetTicketName.Should().Be("D003");
            ticket2.GetTicketName.Should().Be("B001");
            ticket3.GetTicketName.Should().Be("B004");
            ticket4.GetTicketName.Should().Be("B006");
            ticket5.GetTicketName.Should().Be("C002");
            ticket6.Should().BeNull();
            ticket7.GetTicketName.Should().Be("B009");
        }



        /// <summary>
        /// 2 SerialPort - поэтому только 2 касира могут параллельно обратится к очереди.
        /// </summary>
        [Fact]
        public async Task StartHandling_StartHandlingSuccessful_SuccessfulHandling_Parallel_TWO_Cahier_Work_Ok()
        {
            //Arrage
            InitQueue(new[] {"B", "C", "D", "B", "A", "B", "A", "A"});
            var casher1 = new Сashier(1, new List<string> {"D", "B", "C"}, _queue, 2, "test");
            var casher2 = new Сashier(2, new List<string> {"A", "C"}, _queue, 2, "test");
            
            async Task TicketHandler(Сashier ch, ICollection<string> expectedTicketNamesList, TimeSpan startHandlingTransactionTime, TimeSpan workTime)
            {
                var ticket1 = ch.StartHandling();
                expectedTicketNamesList.Add(ticket1?.GetTicketName);
                 
                await Task.Delay(startHandlingTransactionTime); //Время на транзакцию билета до планшета
                ch.StartHandlingSuccessful();
                 
                await Task.Delay(workTime);                    //Время на обработку билета
                ch.SuccessfulHandling();
            }
            
            //Act 
            var expectedTicketNamesCashier1 = new List<string>();
            var task1 = Task.Run(async () =>
            {
                var countCashierInvoke = 6;
                foreach (var _ in Enumerable.Range(1,countCashierInvoke))
                {
                    await TicketHandler(casher1, expectedTicketNamesCashier1, TimeSpan.FromMilliseconds(10), TimeSpan.FromSeconds(0.2));
                }
            });
            
            var expectedTicketNamesCashier2 = new List<string>();
            var task2 = Task.Run(async () =>
            {
                var countCashierInvoke = 6;
                foreach (var _ in Enumerable.Range(1,countCashierInvoke))
                {
                    await TicketHandler(casher2, expectedTicketNamesCashier2, TimeSpan.FromMilliseconds(10), TimeSpan.FromSeconds(0.2));
                }
            });

            await Task.WhenAll(task1, task2);

            //Assert
            expectedTicketNamesCashier1.Should().BeEquivalentTo(new[] {"D003", "B001", "B004", "B006", null, null});
            expectedTicketNamesCashier2.Should().BeEquivalentTo(new[] {"A005", "A007", "A008", "C002", null, null});
        }

    }
}