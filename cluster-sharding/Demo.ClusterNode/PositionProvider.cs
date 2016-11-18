using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Cluster.Sharding;
using Akka.Cluster.Tools.PublishSubscribe;
using Akka.Event;
using Demo.ClusterShared;
using Newtonsoft.Json;

namespace Demo.ClusterNode
{
    public class PositionProvider : ReceiveActor
    {
        #region internal messages

        // singleton
        sealed class FetchTick
        {
            public static readonly FetchTick Instance = new FetchTick();
            private FetchTick() { }
        }

        #endregion

        const string Url = "http://reseplanerare.vasttrafik.se/bin/query.exe/dny?&look_minx=0&look_maxx=99999999&look_miny=0&look_maxy=99999999&tpl=trains2json&performLocating=1";
        
        private readonly WebClient webClient = new WebClient();

        public PositionProvider()
        {
            var interval = TimeSpan.FromSeconds(0.5);
            var log = Context.GetLogger();
            var shard = ClusterSharding.Get(Context.System).ShardRegion(nameof(Vehicle));

            Receive<FetchTick>(_ =>
            {
                var self = Self;
                var scheduler = Context.System.Scheduler;
                Fetch()
                .ContinueWith(task =>
                {
                    scheduler.ScheduleTellOnce(interval, self, FetchTick.Instance, ActorRefs.NoSender);
                    return task.Result;
                })
                .PipeTo(Self);
            });
            
            Receive<IEnumerable<IdentifiedPosition>>(positions =>
            {
                foreach (var value in positions)
                {
                    shard.ShardedTell(value.Identifier, value.Position);
                }
            });

            Receive<Status.Failure>(fail =>
            {
                log.Error(fail.Cause, "Failed to fetch the data");
            });
        }

        protected override void PreStart()
        {
            base.PreStart();
            Self.Tell(FetchTick.Instance);
        }

        protected override void PostStop()
        {
            webClient.Dispose();
            base.PostStop();
        }

        private async Task<IEnumerable<IdentifiedPosition>> Fetch()
        {
            var json = await webClient.DownloadStringTaskAsync(Url);
            dynamic res = JsonConvert.DeserializeObject(json);
            
            var positions = new List<IdentifiedPosition>();
            foreach (var vehicle in res.look.trains)
            {
                string id = vehicle.trainid;
                double lat = vehicle.y / 1000000d;
                double lon = vehicle.x / 1000000d;

                positions.Add(new IdentifiedPosition(id, new Position(lon, lat)));
            }

            Console.WriteLine($"Received {positions.Count} vehicle positions");

            return positions;
        }
    }
}