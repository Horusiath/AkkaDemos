using System;
using Akka.Actor;

namespace Demo.Lighthouse
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var system = ActorSystem.Create("system"))
            {
                Console.WriteLine("Press Enter to close the lighthouse...");
                Console.ReadLine();
            }
        }
    }
}
