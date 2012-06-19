using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SchedulingSimulator
{
    class Program
    {
        static void Main(string[] args)
        {
            Scheduling simulator = new Scheduling("Joblist.txt");
            simulator.ExecuteFCFS();
            simulator.ExecuteRoundRobin();
            simulator.ExecuteSPN();
            simulator.ExecuteSRN();

            Pause();

        }

        private static void Pause()
        {
            Console.WriteLine("\nPress [Enter] to continue...");
            Console.ReadLine();
        }
    }
}
