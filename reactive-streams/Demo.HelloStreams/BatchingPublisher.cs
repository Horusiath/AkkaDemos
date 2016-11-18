using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Akka;
using Akka.Actor;
using Akka.Event;
using Akka.Streams.Actors;

namespace Demo.HelloStreams
{
    public abstract class BatchingPublisher<T> : ActorPublisher<T>
    {
        #region internal messages

        internal sealed class ElementsBatch
        {
            public readonly int Page;
            public readonly IReadOnlyList<T> Elements;

            public ElementsBatch(int page, IReadOnlyList<T> elements)
            {
                Page = page;
                Elements = elements;
            }

            public override string ToString() => $"BatchPage(page:{Page}, elementsCount:{Elements.Count})";
        }

        #endregion

        protected readonly ILoggingAdapter Log = Context.GetLogger();

        private readonly int chunkSize;
        private readonly Queue<T> buffer;
        private int lastRequestedPage = 0;
        private int lastFetchedPage = 0;

        protected BatchingPublisher(int chunkSize = 32, int bufferSize = 64)
        {
            this.chunkSize = chunkSize;
            this.buffer = new Queue<T>(bufferSize);
        }

        protected abstract Task<IReadOnlyList<T>> GetDataPage(int page, int pageSize);

        protected override bool Receive(object message) => message.Match()
            .With<ElementsBatch>(batch =>
            {
                Log.Info($"received {batch}");

                lastFetchedPage = Math.Max(lastFetchedPage, batch.Page);

                var elements = batch.Elements;
                var demand = TotalDemand > int.MaxValue ? int.MaxValue : (int)TotalDemand;

                if (demand > 0)
                {
                    for (int i = 0; i < elements.Count; i++)
                    {
                        var issue = elements[i];

                        if ((demand--) > 0) OnNext(issue);
                        else buffer.Enqueue(issue); // we can overcome a buffer size here by a little
                    }

                    if (demand > 0) FetchNextPage();
                }
                else
                {
                    foreach (var issue in elements)
                        buffer.Enqueue(issue);
                }
                
                if (elements.Count == 0 && batch.Page == lastFetchedPage) OnCompleteThenStop();
            })
            .With<Request>(request => DeliverBuffer())
            .With<Cancel>(_ => OnCompleteThenStop())
            .With<Status.Failure>(fail => Log.Error(fail.Cause, "Failed to fetch the data"))
            .WasHandled;

        private void DeliverBuffer()
        {
            var demand = TotalDemand > int.MaxValue ? int.MaxValue : (int)TotalDemand;
            while (demand > 0 && buffer.Count > 0)
            {
                OnNext(buffer.Dequeue());
                demand--;
            }

            if (demand > 0) FetchNextPage();
        }

        private void FetchNextPage()
        {
            if (lastRequestedPage <= lastFetchedPage)
            {
                var page = lastRequestedPage + 1;

                GetDataPage(page, chunkSize).PipeTo(Self, success: elements => new ElementsBatch(page, elements));

                lastRequestedPage = page;
            }
        }
    }
}