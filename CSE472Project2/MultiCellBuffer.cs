using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSE472Project2
{
    public class MultiCellBuffer
    {
        public EventHandler<int>? OrderSent;     // OrderSent event raised when TicketAgent Thread writes to buffer

        private OrderClass.OrderObject[] buffer;    // Array containing OrderObjects from TicketAgents for Cruise
        private ReaderWriterLockSlim[] readWriteLocks; // Array of size of buffer allowing for two writers to write to different cells
        private SemaphoreSlim semaphore;    // No. of available cells for writing, decremented upon write, incremented upon read.
        private int size;
        private string name;
        public MultiCellBuffer(string name, int size)
        {
            buffer = new OrderClass.OrderObject[size];
            readWriteLocks = new ReaderWriterLockSlim[size];
            for(int i = 0; i < size; i++)
            {
                readWriteLocks[i] = new ReaderWriterLockSlim();
            }
            semaphore = new SemaphoreSlim(initialCount: 0, maxCount: 2);
            this.size = size;
            this.name = name;   
        }

        public void WriteCell(OrderClass.OrderObject order)
        {
            // Called by TicketAgent Threads to write an OrderObject to a cell

            int toRelease = 0;  // Remember which cell was written to
            // Block thread until there is an available cell
            semaphore.Wait();
            // Check if cell i is available
            for(int i = 0; i < size; i++)
            {
                if (readWriteLocks[i].TryEnterWriteLock(1))
                {
                    // If so, write order to cell
                    toRelease = i;
                    buffer[0] = order;
                }
                // If not, loop to cell i+1
            }
            // Semaphore makes sure that there is a cell available
            readWriteLocks[toRelease].ExitWriteLock();
            // Release lock but do not release semaphore (wait for reader)
        }

        public OrderClass.OrderObject ReadCell(int index)
        {
            // Called by Cruise for OrderProcess Thread to read buffer
            readWriteLocks[index].EnterReadLock();
            try
            {
                return buffer[index];
            }
            finally
            {
                // Finally release semaphore after reading
                semaphore.Release();
                readWriteLocks[index].ExitReadLock();
            }
        }

        protected virtual void OnOrderSent(int index)
        {
            // OrderSent Event Handler Invocation, sends index in Buffer to read to Cruise
            OrderSent?.Invoke(this, index);
        }
    }
}
