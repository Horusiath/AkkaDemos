using System;
using Demo.Web.Actors;
using Microsoft.Owin.Hosting;

namespace Demo.Web
{
    class Program
    {
        static void Main(string[] args)
        {
            var baseUrl = "http://localhost:9000";

            using (WebApp.Start<Startup>(baseUrl))
            using (var system = ActorBoostrapper.Start())
            {
                Console.WriteLine($"Listenting on {baseUrl}");
                Console.ReadLine();
            }
        }
    }
}
