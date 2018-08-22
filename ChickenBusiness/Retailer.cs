using System;
using System.Threading;

namespace ChickenBusiness
{
    public delegate void OrderCreatedEventHandler();

    class Retailer
    {
        public static event OrderCreatedEventHandler OrderCreated;
        private Int32 oldChickenPrice = 20;
        private Int32 amountOfChicken = 10;
        private static Random randomNumberGenerator = new Random();

        // function called by the retailer thread
        public void retailerFunction()
        {
            // retailers run as long as chicken farm thread is not terminated
            while (!ChickenFarm.chickenFarmThreadTerminated)
            {
                Thread.Sleep(3000);
                //Console.WriteLine("Retailer selling chicken at market price: {0}",new ChickenFarm().getChickenPrice());
            }
        }

        // event handler for price cut event
        public void chickenOnSale(int price, int senderId)
        {
            Console.WriteLine("Chickens are on sale for price as low as ${0}! Store {1} is placing an order...", price, senderId);
            createOrder(senderId, price);
        }

        // creating order on price cut
        public void createOrder(int senderId, int price)
        {
            Int32 newChickenPrice = price;

            // create an order
            OrderClass order = new OrderClass();

            order.senderId = senderId;
            order.cardNo = randomNumberGenerator.Next(4500, 7000);
            amountOfChicken = getAmountOfChicken(oldChickenPrice, newChickenPrice, amountOfChicken);
            order.amount = amountOfChicken;
            order.timeOfOrder = DateTime.Now;

            Console.WriteLine("Order created for store {0}", senderId);

            //encode order
            string encodedOrder = EncodeDecodeOrder.EncodeOrder(order);

            // add order to Multi Cell Buffer
            Program.buffer.setOneCell(encodedOrder);

            // function for emitting event
            onOrderCreated();
        }

        // function to decide the amount of chicken based on the change in price
        public Int32 getAmountOfChicken(Int32 oldPrice, Int32 newPrice, Int32 amount)
        {
            Int32 newAmount = amount;
            if (newPrice < oldPrice)
            {
                newAmount += 2;
                if (newAmount >= 30)
                    newAmount = 30;
            }
            else
            {
                newAmount -= 2;
                if (newAmount <= 10)
                    newAmount = 10;
            }

            return newAmount;
        }

        // event handler when order is processed
        public void orderProcessed(Int32 senderId, Int32 orderAmount, Int32 unitPrice, Int32 totalPrice, DateTime timeOfOrder)
        {
            var timeDiff = DateTime.Now.Subtract(timeOfOrder);
            Console.WriteLine("\n\n******************** ORDER RECEIPT ********************\n"+
                "Order Processed for Retailer {0} \n"+
                "Unit price: ${1}\n"+
                "Amount of Chicken Puchased: {2}\n"+
                "Tax: 5%\n"+
                "Shipping Cost: $7\n"+
                "Total price to pay: ${3}\n"+
                "Time taken for order in (hh:mm:ss): {4}\n"+
                "******************** ORDER RECEIPT END ********************\n\n", 
                senderId, unitPrice, 
                orderAmount, totalPrice, 
                String.Format("{0}:{1}:{2}", timeDiff.Hours, timeDiff.Minutes, timeDiff.Seconds));
        }

        // function for emitting event
        protected virtual void onOrderCreated()
        {
            if(OrderCreated != null) // there is at least one subscriber
            {
                OrderCreated(); // wmit event
            }
        }
    }
}
