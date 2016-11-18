using System.Collections.Generic;
using System.Threading.Tasks;
using Akka;
using Akka.Actor;
using Octokit;

namespace Demo.HelloStreams
{
    public class OctokitIssuesPublisher : BatchingPublisher<Issue>, IWithUnboundedStash
    {
        public static Props Props(string owner, string repositoryName, Credentials credentials) =>
            Akka.Actor.Props.Create(() => new OctokitIssuesPublisher(owner, repositoryName, credentials));

        public IStash Stash { get; set; }

        private readonly IGitHubClient github;
        private Repository repository;
        public OctokitIssuesPublisher(string owner, string repositoryName, Credentials credentials)
        {
            github = new GitHubClient(new ProductHeaderValue("akka-streams-demo"));
            #region credentials
            github.Connection.Credentials = credentials;
            #endregion
            github.Repository.Get(owner, repositoryName).PipeTo(Self);

            BecomeStacked(Initializing);
        }

        private bool Initializing(object message) => message.Match()
            .With<Repository>(repo =>
            {
                repository = repo;
                Log.Info($"Repo {repo.Name} has {repo.OpenIssuesCount} open issues");
                UnbecomeStacked();
                Stash.UnstashAll();
            })
            .With<Status.Failure>(fail => Log.Error(fail.Cause, "Failed to fetch a repository"))
            .Default(msg => Stash.Stash())
            .WasHandled;

        protected override Task<IReadOnlyList<Issue>> GetDataPage(int page, int pageSize) =>
            github.Issue.GetAllForRepository(repository.Id, new ApiOptions
            {
                PageCount = 1,
                PageSize = pageSize,
                StartPage = page
            });
    }
}