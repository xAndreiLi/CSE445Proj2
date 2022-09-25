using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CSE472Project2
{
    public class MultiCellBuffer
    {

        private OrderClass.OrderObject[] buffer;    // Array containing OrderObjects from TicketAgents for Cruise
        private List<ReaderWriterLockSlim> readerWriterLocks; // Array of size of buffer allowing for two writers to write to different cells
        // private ReaderWriterLockSlim readWriteLock; 
        private SemaphoreSlim semaphore;    // No. of available cells for writing, incremented upon write, decremented upon read.
        private int size;
        private string name;
        private int wrote;
        private int read;
        public MultiCellBuffer(string name, int size)
        {
            buffer = new OrderClass.OrderObject[size];
            readerWriterLocks = new List<ReaderWriterLockSlim>();
            for(int i = 0; i < size; i++)
            {
                readerWriterLocks.Add(new ReaderWriterLockSlim());
            }
            //readWriteLock = new ReaderWriterLockSlim();
            semaphore = new SemaphoreSlim(initialCount: size, maxCount: size);
            this.size = size;
            this.name = name;
            wrote = -1;
            read = 0;
        }

        public int WriteCell(OrderClass.OrderObject order)
        {
            // Called by TicketAgent Threads to write an OrderObject to a cell
            // Block thread until there is an available cell
            
            semaphore.Wait();
            wrote++;
            // Enter cells in order till full
            for (int i = 0; i<size; i++)
            {
                if (readerWriterLocks[i].TryEnterWriteLock(5))
                {
                    buffer[i] = order;
                    readerWriterLocks[i].ExitWriteLock();
                    // Release lock but do not release semaphore (wait for reader)
                    return i;
                }
            }
            return -1;
        }

        public OrderClass.OrderObject ReadCell(int index)
        {
            // Called by Cruise for OrderProcess Thread to read buffer
            readerWriterLocks[index].EnterReadLock();
            try
            {
                return buffer[index];
            }
            finally
            {
                readerWriterLocks[index].ExitReadLock();
                read++;
                // Finally release semaphore after reading all cells
                if (read == size)
                {
                    Console.WriteLine("Readers Finished");
                    read = 0;
                    wrote = -1;
                    semaphore.Release();
                    Thread.Sleep(10);
                    semaphore.Release();
                    Thread.Sleep(10);
                    semaphore.Release();
                }
            }
        }
    }
}
