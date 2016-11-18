using Akka.Actor;
using Akka.Cluster.Sharding;
using Akka.Cluster.Tools.Singleton;
using Demo.ClusterShared;

namespace Demo.ClusterNode
{
    public static class Bootstrapping
    {
        public static IActorRef BootstrapSingleton<T>(this ActorSystem system, string role = null) where T : ActorBase
        {
            var props = ClusterSingletonManager.Props(
                singletonProps: Props.Create<T>(),
                settings: ClusterSingletonManagerSettings.Create(system).WithRole(role));

            return system.ActorOf(props, typeof(T).Name);
        }

        public static IActorRef BootstrapShard<T>(this ActorSystem system, string role = null) where T : ActorBase
        {
            var clusterSharding = ClusterSharding.Get(system);
            return clusterSharding.Start(
                typeName: typeof(T).Name,
                entityProps: Props.Create<T>(),
                settings: ClusterShardingSettings.Create(system)
                    .WithRole(role),
                messageExtractor: new CustomMessageExtractor());
        }
    }
}