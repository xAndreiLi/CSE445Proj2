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
        public static int K = 1, N = 5;  // No. of Cruises / Agents
        public static MultiCellBuffer buffer = new MultiCellBuffer("buffer1", 3);

        public static List<ICruise> cruiseList = new List<ICruise>();
        public static List<TicketAgent> agentList = new List<TicketAgent>();

        // Driver class for Cruise Ship Ticket Agent System
        static void Main(string[] args)
        {
            // Initialize Cruise Object/Thread
            Console.WriteLine("Starting cruises");
            Cruise1 cruise1 = new Cruise1("Cruise1");
            Thread cruiseThread1 = new Thread(cruise1.StartCruise);
            cruiseThread1.Name = cruise1.ToString();
            cruiseList.Add(cruise1);

            for(int i = 0; i < N; i++)
            {
                TicketAgent agent = new TicketAgent($"TicketAgent{i + 1}", i*5);
                agentList.Add(agent);
                Thread agentThread = new Thread(agent.StartAgent);
                agentThread.Name = $"TicketAgent{i + 1}";
                agentThread.Start();
            }

            cruiseThread1.Start();


            while (K > 0)
            {
                Thread.Sleep(1000);
            }
            Console.WriteLine("Cruises have finished");
        }
    }
}
