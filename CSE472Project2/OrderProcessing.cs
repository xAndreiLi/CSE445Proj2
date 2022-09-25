using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSE472Project2
{
    public class OrderEventArgs : EventArgs
    {
        public DateTimeOffset startTime { get; set; }
        public DateTimeOffset endTime { get; set; }
        public ICruise cruise { get; set; }
        public TicketAgent agent { get; set; }
    }
    public class OrderProcessing
    {
        // Event Handlers
        public EventHandler<OrderEventArgs>? OrderReceived; // OrderRecived event raised when OrderProcessing is confirmed

        // Instance Variables
        private string name;
        private int index;  // Index of cell to read
        private OrderEventArgs data = new OrderEventArgs();

        public OrderProcessing(string name, int index)
        {
            this.name = name;
            this.index = index;
        }
        public void StartProcessing()
        {
            /*  Starts thread for OrderProcessing Object
             *  Called when an order needs to be processed by Cruise
             *  Checks validity of credit card number (int between 1000 - 9999)
             *  Raises OrderRecieved event after validation
             */
            Console.WriteLine($"{name} starts processing");
            double charge;
            OrderClass.OrderObject order = Program.buffer.ReadCell(index);
            if ((1000 <= order.GetCardNo()) && (order.GetCardNo() <= 9999))
            {
                // Final amount charged is Ticket Cost * Quantity + 7% Sales Tax
                charge = order.GetUnitPrice() * order.GetQuantity() * 1.07;
            }
            else
            {
                charge = double.NaN;
            }
            DateTimeOffset timeStamp = new DateTimeOffset(DateTime.UtcNow).ToLocalTime();
            
            data.endTime = timeStamp;
            OnOrderConfirmed(data);
        }

        public void PassData(AgentEventArgs e)
        {
            data.cruise = e.cruise;
            data.agent = e.agent;
            data.startTime = e.startTime;
        }

        protected virtual void OnOrderConfirmed(OrderEventArgs e)
        {
            // Confirms Event Handler Invocation from cruise.
            OrderReceived?.Invoke(this, e);
        }
    }
}
