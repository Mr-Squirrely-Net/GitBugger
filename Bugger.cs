using System;
using Octokit;

namespace GitBugger {
    public class Bugger {
        private readonly GitHubClient _client;
        public Bugger(string headerValue, Credentials token) {
            try {
                _client = new GitHubClient(new ProductHeaderValue(headerValue)){Credentials = token};
            }
            catch (Exception) {
                // ignored
            }
        }

        public async void CreateIssue(string repoOwner, string repo, NewIssue issue, Label label) {
            switch (label) {
                case Label.Bug:
                    issue.Labels.Add("bug");
                    break;
                case Label.Enhancement:
                    issue.Labels.Add("enhancement");
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(label), label, null);
            }

            await _client.Issue.Create(repoOwner, repo, issue);
        }

        public NewIssue Issue(string title, string body) => new NewIssue(title){Body = body};

        public enum Label {
            Bug,
            Enhancement
        }
    }
}
