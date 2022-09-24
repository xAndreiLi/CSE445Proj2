using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSE472Project2
{
    public class Cruise
    {
        /*  Cruise class defines a Thread object that represents a Cruise Ship
         *  Acts as the parent class for derived Cruise{1:K} classes
         *  Periodically changes its ticket price, notifying TicketAgent objects if the price is cut
         *  Recieves orders from MultiCellBuffer and processes by starting OrderProcessing Thread
         */
        public event EventHandler<double>? PriceCut;   // Price Cut event raised when ticketPrice is lowered

        private string name;    // Name of Cruise object
        private double ticketPrice; // Price of Cruise Tickets (between 40.00-200.00)
        private int t;      // Counts no. of price cuts made (Terminates thread at 20)
        
        public Cruise(string name)
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
            double lastPrice;
            while (t < 20)
            {
                lastPrice = PricingModel();
                if(ticketPrice < lastPrice)
                {
                    t++;   // Increment t on PriceCut
                    OnPriceCut(ticketPrice);
                }
                ticketPrice = lastPrice;
            }
        }

        public double PricingModel()
        {
            // Generates random number between 40-200 and rounds to 2 decimal places
            return Math.Round(Program.random.NextDouble() * (200.00 - 40.00) + 40.00, 2);
        }

        protected virtual void OnPriceCut(double newPrice)
        {
            //  PriceCut Event Handler Invocation Method, sends ticket price as a parameter
            PriceCut?.Invoke(this, newPrice);
        }

        public double OrderProcessing(OrderClass.OrderObject order)
        {
            /*  Starts thread for Cruise Object order processing
             *  Called when an order needs to be processed
             *  Checks validity of credit card number (int between 1000 - 9999)
             */
            if((1000 <= order.GetCardNo()) && (order.GetCardNo() <= 9999))
            {
                // Final amount charged is Ticket Cost * Quantity + 7% Sales Tax
                return order.GetUnitPrice() * order.GetQuantity() * 1.07;
            }
            else
            {
                return double.NaN;
            }
        }

        public static void buff_OrderSent()
        {
            // Event Handler for OrderSent event from MultiCellBuffer

        }

        override public string ToString()
        {
            //  Prints name when Cruise object is printed
            return name;
        }
    }
}
