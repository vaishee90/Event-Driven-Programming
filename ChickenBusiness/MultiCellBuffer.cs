using System;
using System.Collections.Generic;
using System.Threading;

namespace ChickenBusiness
{
    class MultiCellBuffer
    {
        // buffer in the form of Queue
        public static Queue<string> buffer = new Queue<string>();
        private static Int32 bufferSize;
        private static Int32 cellsFilled = 0;
        private static Semaphore wLock = new Semaphore(3, 3);
        private static Semaphore rLock = new Semaphore(3, 3);

        // constructor to set buffer size
        public MultiCellBuffer(Int32 size)
        {
            bufferSize = size;
        }

        // method to add an order to the queue
        public void setOneCell(string order)
        {
            // writer lock acquired
            wLock.WaitOne();

            lock (this)
            {
                // if buffer is full, wait
                while(cellsFilled == bufferSize)
                {
                    Monitor.Wait(this);
                }

                // if buffer cells available, add the order to the queue
                buffer.Enqueue(order);
                cellsFilled++;

                // release writer lock
                wLock.Release();

                // allow next thread in ready queue to write
                Monitor.Pulse(this);
            }
        }

        // method to get an order from the queue
        public string getOneCell()
        {
            string orderFetched = "";

            // reader lock acquired
            rLock.WaitOne();

            lock (this)
            {
                // if buffer is empty, wait
                while(cellsFilled == 0)
                {
                    Monitor.Wait(this);
                }

                // else, get an order from queue
                orderFetched = buffer.Dequeue();
                cellsFilled--;

                // reader lock released
                rLock.Release();

                // allow next thread in ready queue to read
                Monitor.Pulse(this);
            }

            return orderFetched;
        }
    }
}
