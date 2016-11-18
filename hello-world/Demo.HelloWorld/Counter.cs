using Akka.Actor;

namespace Demo.HelloWorld
{
    public enum CounterMessage
    {
        Increment,
        Decrement,
        GetState
    }

    public class Counter : ReceiveActor
    {
        private int counter = 0;

        public Counter()
        {
            Receive<CounterMessage>(msg =>
            {
                switch (msg)
                {
                    case CounterMessage.Increment: counter++; break;
                    case CounterMessage.Decrement: counter--; break;
                    case CounterMessage.GetState: Sender.Tell(counter); break;
                }
            });
        }
    }
}