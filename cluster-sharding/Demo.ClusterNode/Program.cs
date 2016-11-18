using System;
using Akka.Actor;
using Demo.ClusterShared;

namespace Demo.ClusterNode
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var system = ActorSystem.Create("system"))
            {
                var shardRegion = system.BootstrapShard<Vehicle>(role: Roles.Sharding);
                var positionProvider = system.BootstrapSingleton<PositionProvider>(role: Roles.Sharding);
                
                Console.ReadLine();
            }
        }
    }
}
