using System;
using Akka.Actor;

namespace Demo.HelloWorld
{
    public class Printer : ReceiveActor
    {
        public Printer()
        {
            Receive<string>(whom => Console.WriteLine($"Hello from {whom}!"));
        }
    }
}