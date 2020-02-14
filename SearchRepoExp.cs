using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GitHubJwt;
using Octokit;

namespace SearchRepoExp {
    public class SearchExperiment {
        private GitHubClient client;
        public SearchExperiment (string baseUri, string appKeyFile) {
            this.client = new GitHubClient (
                new ProductHeaderValue ("Experimental-Search"),
                new Uri (baseUri)
            );
            GitHubJwtFactory generator = new GitHubJwtFactory (new FilePrivateKeySource (appKeyFile),
                new GitHubJwtFactoryOptions { AppIntegrationId = 53, ExpirationSeconds = 600 });
            this.client.Credentials = new Credentials (generator.CreateEncodedJwtToken (), AuthenticationType.Bearer);
            var app = this.client.GitHubApps.GetCurrent ().Result;
            var installations = this.client.GitHubApps.GetAllInstallationsForCurrent ().Result;
            var response = this.client.GitHubApps.CreateInstallationToken (installations[0].Id).Result;
            this.client.Credentials = new Credentials (response.Token);
        }

        public async Task<List<Repository>> RequestUserRepositories (string gitUser) {
            var request = new SearchRepositoriesRequest () {
                Fork = ForkQualifier.IncludeForks,
                User = gitUser,
                SortField = RepoSearchSort.Stars,
            };
            SearchRepositoryResult initialResult = await client.Search.SearchRepo (request);
            List<Repository> results = initialResult.Items.ToList ();

            int maxPages = (int) Math.Ceiling ((double) initialResult.TotalCount / 100);
            if (maxPages > 1) {
                for (int i = 2; i <= maxPages; i++) {
                    request.Page = i;
                    SearchRepositoryResult result = await client.Search.SearchRepo (request);
                    results = results.Concat (result.Items.ToList ()).ToList ();
                }
            }
            return results;
        }
    }
}