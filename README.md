# Akka.NET demos

1. Basic demos:
    - Simple hello world example
    - Counter example
    - User example - a simple state machine
2. Akka.Streams demos:
    - Custom reactive-stream compliant source using Octokit to retrieve github issues
    - Web crawler as a flow shape - an Akka.Streams graph as a bot used to crawl through the web
3. [Akka.Cluster.Sharding demo](https://github.com/Horusiath/AkkaDemos/tree/master/cluster-sharding):
    - Live taxi tracker - clustered system, which uses cluster singleton actor to retrieve taxi positions. Then, positions are sent to peristent entities (actors backed by eventsourcing using Akka.Persistence.SqlServer and managed by cluster sharding). Once positions are stored in persistent backend, they are published using distributed pub/sub mechanism. Events published this way are received on the frontend node, passed through SignalR to user's browser and then displayed and updated live on Google Maps API. Additionally clicking a taxi marker on map will display history of position updates for related taxi.