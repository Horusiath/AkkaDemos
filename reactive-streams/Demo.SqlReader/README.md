## Generic SQL reader-based publisher

This example presents an actor publisher, that can be used as a source for Akka.Streams graph. It uses [Dapper](https://github.com/StackExchange/Dapper) for mapping sql rows into POCO objects.

Please keep in mind, that underneat it's using a SqlDataReader which maintains a database connection.

This example uses a popular [Northwind](https://northwinddatabase.codeplex.com/) database as a data source.