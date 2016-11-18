using Akka.Actor;
using Akka.Persistence;
using Demo.ClusterShared;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Akka.Cluster.Tools.PublishSubscribe;

namespace Demo.ClusterNode
{
    public sealed class Vehicle : ReceivePersistentActor
    {
        #region internal messages

        #endregion

        private readonly LinkedList<PositionChanged> history;

        public Position CurrentPosition => history.Count == 0 ? Position.Zero : history.First.Value.Position;
        public override string PersistenceId { get; }

        public Vehicle()
        {
            PersistenceId = Uri.UnescapeDataString(Self.Path.Name);
            Console.WriteLine($"Vehicle {PersistenceId} started");

            history = new LinkedList<PositionChanged>();
            var mediator = DistributedPubSub.Get(Context.System).Mediator;

            Command<Position>(position =>
            {
                if (CurrentPosition != position)
                {
                    Persist(new PositionChanged(position, DateTime.UtcNow), e =>
                    {
                        UpdateState(e);
                        mediator.Tell(new Publish(Topics.VehicleTracking, new VehicleState(PersistenceId, e.Position, e.When)));
                    });
                }
            });

            Command<GetCurrentState>(_ =>
            {
                var lastPosition = history.First.Value;
                Sender.Tell(new VehicleState(PersistenceId, lastPosition.Position, lastPosition.When));
            });
            Command<GetHistory>(_ => Sender.Tell(new VehicleHistory(PersistenceId, history)));
            Recover<PositionChanged>(changed => UpdateState(changed));
        }

        private void UpdateState(PositionChanged changed)
        {
            history.AddFirst(changed);
        }
    }
}