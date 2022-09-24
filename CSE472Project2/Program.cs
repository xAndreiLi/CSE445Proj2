using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSE472Project2
{
    public class Program
    {
        public static Random random = new Random();
        // Driver class for Cruise Ship Ticket Agent System
        static void Main(string[] args)
        {
            // Initialize Cruise Object/Thread
            Console.WriteLine("Starting cruises");
            int K = 2;  // No. of Cruises
            for(int i = 0; i < K; i++)
            {
                Cruise cruise = new Cruise($"cruise{i+1}");
                Thread cruiseThread = new Thread(cruise.StartCruise);
                cruiseThread.Start();
                cruiseThread.Name = cruise.ToString();
            }

            while (K > 0)
            {
                Thread.Sleep(1000);
            }
            Console.WriteLine("Cruises have finished");
        }
    }
}
