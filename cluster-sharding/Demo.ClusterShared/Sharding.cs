using System;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Cluster.Sharding;

namespace Demo.ClusterShared
{

    public static class ShardingExtensions
    {
        public static void ShardedTell(this IActorRef shardRegion, string entityId, object message) =>
               shardRegion.Tell(new ShardEnvelope(entityId, message));

        public static Task<T> ShardedAsk<T>(this IActorRef shardRegion, string entityId, object message) =>
               shardRegion.Ask<T>(new ShardEnvelope(entityId, message), TimeSpan.FromSeconds(5));
    }

    public sealed class ShardEnvelope
    {
        public readonly string Recipient;
        public readonly object Message;

        public ShardEnvelope(string recipient, object message)
        {
            Recipient = recipient;
            Message = message;
        }
    }

    public class CustomMessageExtractor : HashCodeMessageExtractor
    {
        public CustomMessageExtractor() : base(maxNumberOfShards: 100) { }
        public override string EntityId(object message) => (message as ShardEnvelope)?.Recipient;
        public override object EntityMessage(object message) => (message as ShardEnvelope)?.Message ?? message;
    }
}