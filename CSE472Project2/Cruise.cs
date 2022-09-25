using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSE472Project2
{
    public class CruiseEventArgs : EventArgs
    {
        public ICruise cruise { get; set; }
        public double price { get; set; }
    }
    public abstract class ICruise 
    {
        /*  Cruise class defines a Thread object that represents a Cruise Ship
         *  Acts as the abstract parent class for derived Cruise{1:K} classes
         *  Defines base functionality of derived classes with abstract method PricingModel()
         *  Periodically changes its ticket price, notifying TicketAgent objects if the price is cut
         *  Recieves orders from MultiCellBuffer and processes by starting OrderProcessing Thread
         *  
         */
        public event EventHandler<CruiseEventArgs>? PriceCut;   // Price Cut event raised when ticketPrice is lowered
        private string name;    // Name of Cruise object
        private double ticketPrice; // Price of Cruise Tickets (between 40.00-200.00)
        private int t;      // Counts no. of price cuts made (Terminates thread at 20)


        private static OrderProcessing[] processes = { };
        private static int procNo = 0;

        public ICruise(string name)
        {
            this.name = name;
            ticketPrice = 120.00;
            t = 0;
        }

        public void StartCruise()
        {
            /*  Starts thread for Cruise object ticket pricing
             *  Continually generate new ticket prices
             *  If ticket price is lowered, raise PriceCut event
             */
            foreach (TicketAgent agent in Program.agentList)
            {
                agent.OrderSent += agent_OrderSent;
            }
            double lastPrice;
            while (t < 20)
            {
                Thread.Sleep(50);
                lastPrice = PricingModel();
                if (ticketPrice < lastPrice)
                {
                    t++;   // Increment t on PriceCut
                    Console.WriteLine($"{name} emits PriceCut");
                    var data = new CruiseEventArgs();
                    data.cruise = this;
                    data.price = ticketPrice;
                    OnPriceCut(data);
                }
                ticketPrice = lastPrice;
            }
            // Decrements total Cruise threads
            Program.K--;
        }

        public abstract double PricingModel();
        protected virtual void OnPriceCut(CruiseEventArgs e)
        {
            //  PriceCut Event Handler Invocation Method, sends ticket price as a parameter
            PriceCut?.Invoke(this, e);
            Console.WriteLine(name + " has cut its ticket price. New Price: " + e.price);

        }

        public static void agent_OrderSent(object? sender, AgentEventArgs e)
        {
            // Event Handler for OrderSent event from MultiCellBuffer
            procNo++;
            Console.WriteLine($"{e.agent.ToString()} sent an order to Cruise");
            OrderProcessing processor1 = new OrderProcessing($"Proc{procNo}", e.index);
            processor1.PassData(e);
            Thread processThread = new Thread(()=>processor1.StartProcessing());
            processThread.Name = $"Proc{procNo}";
            processes.Append(processor1);
            processThread.Start();
            
            
        }

        override public string ToString()
        {
            //  Prints name when Cruise object is printed
            return name;
        }
    }

    public class Cruise1 : ICruise
    {
        public Cruise1(string name)
            : base(name) { }

        public override double PricingModel()
        {
            // Generates random number between 40-200 and rounds to 2 decimal places
            return Math.Round(Program.random.NextDouble() * (200.00 - 40.00) + 40.00, 2);
        }
    }
}
