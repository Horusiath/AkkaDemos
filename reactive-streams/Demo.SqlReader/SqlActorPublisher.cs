using System;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Akka;
using Akka.Actor;
using Akka.Dispatch;
using Akka.Streams.Actors;
using Dapper;

namespace Demo.SqlReader
{
    public sealed class SqlActorPublisher<T> : ActorPublisher<T>
    {
        public static Props Props(string query, string connectionString) =>
            Akka.Actor.Props.Create(() => new SqlActorPublisher<T>(query, connectionString)).WithDeploy(Deploy.Local);

        private readonly SqlCommand command;
        private SqlDataReader sqlReader;

        public SqlActorPublisher(string query, string connectionString)
        {
            var connection = new SqlConnection(connectionString);   
            connection.Open();
            this.command = new SqlCommand(query, connection);
        }

        protected override void PreStart()
        {
            base.PreStart();
            this.sqlReader = this.command.ExecuteReader();
        }

        protected override void PostStop()
        {
            base.PostStop();
            this.command?.Dispose();
        }

        protected override bool Receive(object message) => message.Match()
            .With<Request>(request =>
            {
                //Console.WriteLine("---");
                ActorTaskScheduler.RunTask(() => QueryAsync(request.Count));
            })
            .With<Cancel>(cancel => OnCompleteThenStop())
            .WasHandled;

        private async Task QueryAsync(long count)
        {
            var parser = sqlReader.GetRowParser<T>();
            for (var i = 0; i < count; i++)
            {
                try
                {
                    // read as many rows as demanded, and parse them into objects using Dapper
                    if (await this.sqlReader.ReadAsync())
                    {
                        var element = parser(this.sqlReader);
                        OnNext(element);
                    }
                    else
                    {
                        // if there are no more rows to read, complete the stream
                        OnCompleteThenStop();
                    }
                }
                catch (SqlException cause)
                {
                    // sql exceptions are hard database-related errors
                    OnErrorThenStop(cause);
                }
                catch (Exception cause)
                {
                    // other exceptions are more of logic errors
                    OnError(cause);
                }
            }
        }
    }
}
