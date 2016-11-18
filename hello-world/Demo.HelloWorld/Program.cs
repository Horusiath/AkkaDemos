using System;
using System.Threading.Tasks;
using Akka.Actor;

namespace Demo.HelloWorld
{
    class Program
    {
        static void Main(string[] args)
        {
            Task.Run(RunAsync).Wait();

            Console.ReadLine();
        }

        private static async Task RunAsync()
        {
            using (var system = ActorSystem.Create("system"))
            {
                #region Example 1 - printer

                Example1(system);

                #endregion

                #region Example 2 - counter

                //await Example2(system);

                #endregion

                #region Example 3 - user

                //await Example3(system);

                #endregion
            }
        }

        private static async Task Example3(ActorSystem system)
        {
            var userRef = system.ActorOf(Props.Create(() => new User("Bartek", Guid.NewGuid().ToString("N"))), "user-1");
            
            userRef.Tell(new GetAccountNr()); // unhandled - user not authorized
            userRef.Tell(new LogIn("Maciek")); // unhandled - nick doesn't match

            userRef.Tell(new LogIn("Bartek"));
            var accountNr = await userRef.Ask<string>(new GetAccountNr());
            Console.WriteLine($"User account number is: {accountNr}");
            userRef.Tell(new LogOut());
        }

        private static async Task Example2(ActorSystem system)
        {
            var counterRef = system.ActorOf(Props.Create<Counter>(), "counter");

            counterRef.Tell(CounterMessage.Increment);
            counterRef.Tell(CounterMessage.Increment);
            counterRef.Tell(CounterMessage.Increment);
            counterRef.Tell(CounterMessage.Decrement);

            var count = await counterRef.Ask<int>(CounterMessage.GetState);
            Console.WriteLine($"{counterRef} state is: {count}");
        }

        private static void Example1(ActorSystem system)
        {
            var printerRef = system.ActorOf<Printer>("printer");
            printerRef.Tell("Białystok");
        }
    }
}
