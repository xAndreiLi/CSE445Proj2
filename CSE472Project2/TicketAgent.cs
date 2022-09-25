using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace CSE472Project2
{
    public class AgentEventArgs : EventArgs
    {
        public int index { get; set; }
        public TicketAgent agent { get; set; }
        public ICruise cruise { get; set; }
        public DateTimeOffset startTime { get; set; }
    }
    public class TicketAgent
    {
        /*  Defines TicketAgent object that runs a thread
         *  Passive object that listens to Cruise PriceCut events
         *  On PriceCut, save new price and send OrderObject to MultiCellBuffer
         */
        public EventHandler<AgentEventArgs>? OrderSent;     // OrderSent event raised when TicketAgent Thread writes to buffer
        private static double ticketPrice = 120.00;
        private static ManualResetEventSlim work = new ManualResetEventSlim(false);
        private static SemaphoreSlim semaphore = new SemaphoreSlim(initialCount: 0, maxCount: 5);
        
        private static string lastCruise;

        private string name;
        private int cardNo;
        private int delay;

        public TicketAgent(string name, int delay)
        {
            this.name = name;
            this.delay = delay;
            // Listen for Pricecuts from all cruises
            foreach(ICruise cruise in Program.cruiseList)
            {
                cruise.PriceCut += cruise_PriceCut;
            }      
            cardNo = Convert.ToInt16(Program.random.NextSingle() * 8999 + 1000);
        }

        public void StartAgent()
        {
            /* Starts thread that lasts until all Cruise objects have terminated.
             * Uses ManualResetEventSlim to block thread until PriceCut event
             */
            Console.WriteLine($"{name} started working.");
            double lastPrice;
            double demand;
            int quantity;
            OrderClass.OrderObject order;
            while (Program.K > 0)
            {
                //  Initialize data before each PriceCut
                demand = Program.random.NextDouble() * 10;
                lastPrice = ticketPrice;
                semaphore.Wait();
                Thread.Sleep(delay);
                //  Increase demand by a ratio of ticket prices
                demand = demand * ((ticketPrice - lastPrice)/ticketPrice);
                quantity = Convert.ToInt16(demand);
                order = OrderClass.Order(name, cardNo, lastCruise, quantity, ticketPrice);
                var timeStamp = new DateTimeOffset(DateTime.UtcNow).ToLocalTime();
                var data = new AgentEventArgs();
                data.index = Program.buffer.WriteCell(order);
                data.agent = this;
                data.startTime = timeStamp;
                OnOrderSent(data);
            }
        }

        protected virtual void OnOrderSent(AgentEventArgs e)
        {
            // OrderSent Event Handler Invocation, sends index in Buffer to read to Cruise
            OrderSent?.Invoke(this, e);
        }

        public static void cruise_PriceCut(object sender, CruiseEventArgs e)
        {
            // Event Handler for PriceCut event from Cruise
            ticketPrice = e.price;
            lastCruise = e.cruise.ToString();
            semaphore.Release();
        }

        public static void proc_OrderReceived(OrderEventArgs e)
        {
            // Event Handler for OrderRecieved event from Cruise
            var x = e.endTime - e.startTime;
        }

        public override string ToString()
        {
            return name;
        }
    }
}
