using System;
using System.Configuration;
using Akka.Actor;
using Akka.Streams;
using Akka.Streams.Dsl;

namespace Demo.SqlReader
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var system = ActorSystem.Create("system"))
            using(var materializer = system.Materializer())
            {
                var connectionString = ConfigurationManager.ConnectionStrings["Default"].ConnectionString;
                var query = @"select * from Products";

                var props = SqlActorPublisher<Product>.Props(query, connectionString);
                var source = Source.ActorPublisher<Product>(props);

                source
                    .RunForeach(product =>
                    {
                        Console.WriteLine($"ID: {product.ProductID}, Name: {product.ProductName}");
                    }, materializer).Wait();
                
                Console.ReadLine();
            }
        }
    }
}
