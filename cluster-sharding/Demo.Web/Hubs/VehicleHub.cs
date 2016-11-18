using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Demo.ClusterShared;
using Demo.Web.Actors;
using Microsoft.AspNet.SignalR;

namespace Demo.Web.Hubs
{
    public class VehicleHub : Hub
    {
        public void Send(IdentifiedPosition identifiedPosition)
        {
            Clients.All.positionChanged(identifiedPosition);
        }

        public async Task<VehicleHistory> GetHistory(string vehicleId)
        {
            if (ActorBoostrapper.IsInitialized)
            {
                var id = vehicleId;//Uri.UnescapeDataString(vehicleId);
                var result = await ActorBoostrapper.VehicleShard.ShardedAsk<VehicleHistory>(id, ClusterShared.GetHistory.Instance);
                return result;
            }
            else return new VehicleHistory(string.Empty, new PositionChanged[0]);
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            Console.WriteLine($"Client {Context.ConnectionId} disconnected");
            return base.OnDisconnected(stopCalled);
        }

        public override Task OnConnected()
        {
            Console.WriteLine($"Client {Context.ConnectionId} connected");
            return base.OnConnected();
        }
    }
}