using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSE472Project2
{
    public class OrderClass
    {
        // Class used to create OrderObject objects
        public static OrderObject Order(string senderId, int cardNo, string receiverId, int quantity, double unitPrice)
        {
            // Called by class to create and return a read-only OrderObject
            return new OrderObject(senderId, cardNo, receiverId, quantity, unitPrice);
        }
        public class OrderObject
        {
            /* Defines object to be returned by OrderClass
             * Private fields can only be read after the constructor call
             * first initialized by OrderClass using constructor
             */
            private string senderId;
            private int cardNo;
            private string receiverId;
            private int quantity;
            private double unitPrice;
            public OrderObject(string senderId, int cardNo, string receiverId, int quantity, double unitPrice)
            {
                this.senderId = senderId;
                this.cardNo = cardNo;
                this.receiverId = receiverId;
                this.quantity = quantity;
                this.unitPrice = unitPrice;
            }
            public string GetSenderId() { return senderId; }
            public int GetCardNo() { return cardNo; }
            public string GetReceiverId() { return receiverId; }
            public int GetQuantity() { return quantity; }
            public double GetUnitPrice() { return unitPrice; }
        }
    }
}
