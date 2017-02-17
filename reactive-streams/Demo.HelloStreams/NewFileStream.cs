using System;
using System.IO;
using System.Text;
using Akka;
using Akka.IO;
using Akka.Streams;
using Akka.Streams.Dsl;

namespace Demo.HelloStreams
{
    public static class NewFileStream
    {
        public static void Example(IMaterializer materializer)
        {
            Source<Tuple<string, WatcherChangeTypes>, NotUsed> directoryTrigger = null;

            var delimeter = ByteString.FromString("\r\n");
            var getLines =
                Flow.Create<ByteString>()
                .Via(Framing.Delimiter(delimeter, maximumFrameLength: 256, allowTruncation: true))
                .Select(bytes => bytes.DecodeString());

            var newFilesLines =
                directoryTrigger
                .Where(t => t.Item2 == WatcherChangeTypes.Created)
                .Select(t => t.Item1)
                .MergeMany(breadth: 100, flatten: fileName =>
                    FileIO.FromFile(new FileInfo(fileName))
                    .Via(getLines)
                    .MapMaterializedValue(_ => NotUsed.Instance));
        }
    }
}