using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace ChickenBusiness
{
    public delegate void priceCutEventHandler(Int32 price, Int32 senderId);

    class ChickenFarm
    {
        private static Random randomPriceGenerator = new Random();
        private static Int32 threadNumber = 0;
        private static Int32 currentChickenPrice = 20;
        private static Int32 numberOfPriceCuts = 0;
        public static bool chickenFarmThreadTerminated = false;

        public static event priceCutEventHandler priceCut;

        // to get current price of chicken
        public Int32 getChickenPrice()
        {
            return currentChickenPrice;
        }

        // method called on chicken farm thread start
        public void farmerFunction()
        {
            // while price cut limit not reached, check for drop in prices
            while (numberOfPriceCuts < 10)
            {
                Thread.Sleep(1000);
                pricingModel();
            }

            // if price cut limit reached, abort chicken farm thread
            if (numberOfPriceCuts >= 10)
            {
                chickenFarmThreadTerminated = true;
                Thread.Sleep(2000);
                Console.WriteLine("\nPrice cut limit reached. Aborting Chicken Farm... Enter any key to exit...");
                Thread.Sleep(3000);
                Console.ReadLine();
                Thread.CurrentThread.Abort();
            }
        }

        // pricing model which decides the market price of chicken
        public void pricingModel()
        {
            int newChickenPrice = randomPriceGenerator.Next(5, 20);

            if (newChickenPrice != currentChickenPrice)
                Console.WriteLine("New Price in Pricing Model of Farm: ${0}", newChickenPrice);

            if (newChickenPrice < currentChickenPrice)
            {
                Console.WriteLine("Price dropped from ${0} to ${1}",currentChickenPrice,newChickenPrice);
                onPriceCut(newChickenPrice); // method emitting price cut event called
            }
            currentChickenPrice = newChickenPrice;
        }

        // method emitting price cut event
        protected virtual void onPriceCut(int newChickenPrice)
        {
            if(priceCut != null) // if there is at least one subscriber
            {
                numberOfPriceCuts++; // increment number of price cuts
                priceCut(newChickenPrice, Convert.ToInt32(Program.retailers[(threadNumber++)%5].Name)); // emit event
            }
        }

        // event handler when order is created by a retailer and placed in buffer
        public void getOrder()
        {
            string orderReceived = Program.buffer.getOneCell();
            string[] orderDetails = new EncoderDecoderService.ServiceClient().Decrypt(orderReceived).Split(',');

            OrderClass order = new OrderClass();
            Console.WriteLine("Order Received from store {0}", orderDetails[0]);
            order.senderId = Convert.ToInt32(orderDetails[0]);
            order.cardNo = Convert.ToInt32(orderDetails[1]);
            order.amount = Convert.ToInt32(orderDetails[2]);
            order.timeOfOrder = Convert.ToDateTime(orderDetails[3]);

            OrderProcessing op = new OrderProcessing();

            // thread created and started to process order fetched from buffer
            Thread processThread = new Thread(delegate () { op.processOrder(order, getChickenPrice()); });
            processThread.Start();

        }
    }
}
