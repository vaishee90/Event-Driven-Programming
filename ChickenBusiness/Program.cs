using System;
using System.Threading;

namespace ChickenBusiness
{
    class Program
    {
        // initialise buffer with size
        public static MultiCellBuffer buffer = new MultiCellBuffer(2);

        // retailer threads
        public static Thread[] retailers = new Thread[5];

        static void Main(string[] args)
        {
            ChickenFarm farm = new ChickenFarm();
            Retailer chickenStore = new Retailer();

            // new thread created and started for Chicken Farm
            Thread farmThread = new Thread((new ThreadStart(farm.farmerFunction)));
            Console.WriteLine("Farm thread started...");
            farmThread.Start();

            // Retailers subscribing to Price cut event; Retailer is notified when there is a price cut
            ChickenFarm.priceCut += new priceCutEventHandler(chickenStore.chickenOnSale);

            // Chicken Farm subscribing to Order created event; Farm gets notified when an order is created
            Retailer.OrderCreated += new OrderCreatedEventHandler(farm.getOrder);

            // Retailers subscribing to Order processed event; Retailers notified when an order is processed
            OrderProcessing.OrderProcessed += new OrderProcessedEventHandler(chickenStore.orderProcessed);

            // Create and start 5 retailer threads
            for(Int32 i = 0; i < 5; i++)
            {
                retailers[i] = new Thread(new ThreadStart(chickenStore.retailerFunction));
                retailers[i].Name = (i+1).ToString();

                Console.WriteLine("Retailer Thread {0} started", retailers[i].Name);
                retailers[i].Start();
            }
        }
    }
}
