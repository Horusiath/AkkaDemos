## Setup

This example uses an SQL Server as a persistent backend for Akka.Persistence. Default connection string is: `Server=.;Database=akka-demo;Trusted_Connection=True;`

To run the example, run at least 3 nodes:

- Demo.Lighthouse - dummy actor system used as a randevouz for all other nodes to establish a cluster.
- Demo.ClusterNode - nodes responsible for hosting persistent actors (entities) and singleton actor used to pull taxi positions. This project can be started as multiple processes: this way cluster sharding will dispatch messages to entities, even if they live on more than one node.
- Demo.Web - ASP.NET application with actor system used to get data from the rest of the cluster and publishing it through SignalR.

In order to be able to use Google Maps API, you need to specify your key in [index reference](./Demo.Web/index.html#L33).