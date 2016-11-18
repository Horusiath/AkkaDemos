using Akka.Actor;

namespace Demo.HelloWorld
{
    public sealed class GetAccountNr { }
    public sealed class LogOut { }
    public sealed class LogIn
    {
        public readonly string Nick;
        public LogIn(string nick)
        {
            Nick = nick;
        }
    }

    public class User : ReceiveActor
    {
        private readonly string userName;
        private readonly string accountNr;

        public User(string userName, string accountNr)
        {
            this.userName = userName;
            this.accountNr = accountNr;

            Unauthorized();
        }

        private void Unauthorized()
        {
            Receive<LogIn>(_ => Become(Authorized), shouldHandle: login => login.Nick == userName);
        }

        private void Authorized()
        {
            Receive<GetAccountNr>(_ => Sender.Tell(accountNr));
            Receive<LogOut>(_ => Become(Unauthorized));
        }
    }
}