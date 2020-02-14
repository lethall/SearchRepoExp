using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SearchRepoExp {
    class Program {
        static void Main (string[] args) {
            if (args.Length != 2) {
                Console.WriteLine ("usage: Program <certfile> <owner>");
                return;
            }
            SearchExperiment experiment = new SearchExperiment ("https://github.dxc.com/", args[0]);
            List<Octokit.Repository> repositories = experiment.RequestUserRepositories (args[1]).Result;
            foreach (var r in repositories) {
                Console.WriteLine (r.CloneUrl);
            }
        }
    }
}