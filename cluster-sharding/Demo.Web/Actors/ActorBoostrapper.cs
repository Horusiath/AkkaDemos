using System.Collections.Generic;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Cluster;
using Akka.Cluster.Sharding;
using Demo.ClusterShared;

namespace Demo.Web.Actors
{
    public class ActorBoostrapper
    {
        public static ActorSystem System { get; private set; }
        public static IActorRef VehicleShard { get; private set; }
        public static IActorRef SignalRRef { get; private set; }

        public static bool IsInitialized { get; private set; }

        public static ActorSystem Start()
        {
            var system = ActorSystem.Create("system");
            var cluster = Cluster.Get(system);
            cluster.RegisterOnMemberUp(() =>
            {
                var clusterSharding = ClusterSharding.Get(system);
                VehicleShard = clusterSharding.StartProxy("Vehicle", Roles.Sharding, new CustomMessageExtractor());
                SignalRRef = system.ActorOf<SignalRActor>("signalr");
            });

            System = system;
            system.RegisterOnTermination(() => IsInitialized = false);
            IsInitialized = true;
            return system;
        }
    }
}