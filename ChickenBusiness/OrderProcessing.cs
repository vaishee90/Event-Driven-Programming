using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChickenBusiness
{
    public delegate void OrderProcessedEventHandler(Int32 senderId, Int32 orderAmount, Int32 unitPrice, Int32 totalAmount, DateTime timeOfOrder);

    class OrderProcessing
    {
        public static event OrderProcessedEventHandler OrderProcessed;
        private Int32 shippingCost = 7;
        private Int32 tax = 5;
        private Int32 totalPrice;

        // method to check card validity and calculate total price of order
        public void processOrder(OrderClass order, Int32 unitPrice)
        {
            if(order.cardNo <= 7000 && order.cardNo >= 5000)
            {
                totalPrice = unitPrice * order.amount + (tax / 100) * (unitPrice * order.amount) + shippingCost;
                onOrderProcessed(order.senderId, order.amount, unitPrice, totalPrice, order.timeOfOrder);
            }
            else
            {
                Console.WriteLine("Invalid Card Number => {0}! Order could not be processed!", order.cardNo);
            }
        }

        // method to emit event when order is processed
        protected virtual void onOrderProcessed(Int32 Id, Int32 amount, Int32 unitP, Int32 totalP, DateTime time)
        {
            if(OrderProcessed != null) // there is at least one subscriber
            {
                OrderProcessed(Id, amount, unitP, totalP, time); // emit order processed event
            }
        }
    }
}
