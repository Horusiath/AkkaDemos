using System.Collections.Generic;
using Akka.Actor;
using Akka.Cluster;
using Akka.Cluster.Tools.PublishSubscribe;
using Demo.ClusterShared;
using Demo.Web.Hubs;
using Microsoft.AspNet.SignalR;

namespace Demo.Web.Actors
{
    public class SignalRActor : ReceiveActor
    {
        private readonly IActorRef mediator = DistributedPubSub.Get(Context.System).Mediator;

        public SignalRActor()
        {
            var chat = GlobalHost.ConnectionManager.GetHubContext<VehicleHub>();

            Receive<VehicleState>(p =>
            {
                chat.Clients.All.positionChanged(p);
            });
        }

        protected override void PreStart()
        {
            base.PreStart();
            mediator.Tell(new Subscribe(Topics.VehicleTracking, Self));
        }

        protected override void PostStop()
        {
            mediator.Tell(new Unsubscribe(Topics.VehicleTracking, Self));
            base.PostStop();
        }
    }
}