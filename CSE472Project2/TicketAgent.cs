using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace CSE472Project2
{
    public class TicketAgent
    {
        /*  Defines TicketAgent object that runs a thread
         *  Passive object that listens to Cruise PriceCut events
         *  On PriceCut, save new price and send OrderObject to MultiCellBuffer
         */
        
        private static double ticketPrice = 120.00;
        private static ManualResetEventSlim work = new ManualResetEventSlim(false);

        private string name;
        private Cruise cruise;
        private int cardNo;

        public TicketAgent(string name, Cruise cruise)
        {
            this.name = name;
            this.cruise = cruise;
            this.cruise.PriceCut += cruise_PriceCut;
            cardNo = Convert.ToInt16(Program.random.NextSingle() * 8999 + 1000);
        }

        public void StartAgent()
        {
            /* Starts thread that lasts until all Cruise objects have terminated.
             * Uses ManualResetEventSlim to block thread until PriceCut event
             */
            double lastPrice;
            double demand;
            int quantity;
            OrderClass.OrderObject order;
            while (true)
            {
                //  Initialize data before each PriceCut
                demand = Program.random.NextDouble() * 10;
                lastPrice = ticketPrice;
                work.Wait();
                //  Increase demand by a ratio of ticket prices
                demand = demand * ((ticketPrice - lastPrice)/ticketPrice);
                quantity = Convert.ToInt16(demand);
                order = OrderClass.Order(name, cardNo, cruise.ToString(), quantity, ticketPrice);
            }
        }

        public static void cruise_PriceCut(object? sender, double newPrice)
        {
            // Event Handler for PriceCut event from Cruise
            ticketPrice = newPrice;
            Console.WriteLine(sender.ToString() + " has cut its ticket price. New Price: " + newPrice);
            work.Set();
        }
    }
}
