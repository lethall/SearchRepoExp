using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Octokit;

namespace SearchRepoExp {
    public class SearchExperiment {
        private GitHubClient client;
        public SearchExperiment (string baseUri, string personalAccessToken) {
            this.client = new GitHubClient (
                new ProductHeaderValue ("Experimental-Search"),
                new Uri (baseUri)
            );
            this.client.Credentials = new Credentials (personalAccessToken);
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