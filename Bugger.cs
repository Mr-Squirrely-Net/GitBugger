using System;
using System.Diagnostics;
using Octokit;

namespace GitBugger {
    public class Bugger {
        private static GitHubClient _client;
        private static readonly Lazy<Bugger> GitBugger = new Lazy<Bugger>(() => new Bugger());
        /// <summary>
        /// The Bugger Instance
        /// </summary>
        public static Bugger Instance => GitBugger.Value;

        private Bugger() { }
        /// <summary>
        /// Creates a new client using the given header and token
        /// </summary>
        /// <example>
        /// The token doesn't work anymore don't try
        /// Bugger.NewClient("MyApp", new Credentials("f25dccab1326d0b6a78bba91d1a501b4c7456cd6"));
        /// </example>
        /// <param name="header">The Product Header</param>
        /// <param name="token">Your developer token</param>
        public static void NewClient(string header, Credentials token) {
            try {
                _client = new GitHubClient(new ProductHeaderValue(header)) {Credentials = token};
            }
            catch (Exception ex) {
                Debug.Write(ex);
            }
        }
        /// <summary>
        /// Creates an issue with the label "bug"
        /// </summary>
        /// <example>
        /// Bugger.CreateBug("MrSquirrely", "UpdateJsons", Bugger.Issue("test1","test1"));
        /// </example>
        /// <param name="repoOwner">This is the owner of the repo</param>
        /// <param name="repo">This is the repo name</param>
        /// <param name="issue">Use Bugger.Issue to fill this out</param>
        public static async void CreateBug(string repoOwner, string repo, NewIssue issue) {
            issue.Labels.Add("bug");
            await _client.Issue.Create(repoOwner, repo, issue);
        }
        /// <summary>
        /// Creates an issue with the label "enhancement"
        /// </summary>
        /// <example>
        /// Bugger.CreateFeature("MrSquirrely", "UpdateJsons", Bugger.Issue("test2", "test2"));
        /// </example>
        /// <param name="repoOwner">This is the owner of the repo</param>
        /// <param name="repo">This is the repo name</param>
        /// <param name="issue">Use Bugger.Issue to fill this out</param>
        public static async void CreateFeature(string repoOwner, string repo, NewIssue issue) {
            issue.Labels.Add("enhancement");
            await _client.Issue.Create(repoOwner, repo, issue);
        }
        /// <summary>
        /// Creates an issue with the given label
        /// </summary>
        /// <example>
        /// Bugger.CreateIssue("MrSquirrely", "UpdateJsons", Bugger.Issue("test3", "test3"), "documentation");
        /// </example>
        /// <param name="repoOwner">This is the owner of the repo</param>
        /// <param name="repo">This is the repo name</param>
        /// <param name="issue">Use Bugger.Issue to fill this out</param>
        /// <param name="label">The label you want to use, The label needs to exist or it wont work</param>
        public static async void CreateIssue(string repoOwner, string repo, NewIssue issue, string label) {
            issue.Labels.Add(label);
            await _client.Issue.Create(repoOwner, repo, issue);
        }
        /// <summary>
        /// Creates an issue with the given labels
        /// </summary>
        /// <example>
        /// Bugger.CreateIssue("MrSquirrely", "UpdateJsons", Bugger.Issue("test3", "test3"), new []{"duplicate","good first issue", "invalid"});
        /// </example>
        /// <param name="repoOwner">This is the owner of the repo</param>
        /// <param name="repo">This is the repo name</param>
        /// <param name="issue">Use Bugger.Issue to fill this out</param>
        /// <param name="labels">The labels you want to use, The labels needs to exist or it wont work</param>
        public static async void CreateIssue(string repoOwner, string repo, NewIssue issue, string[] labels) {
            foreach (string label in labels) {
                issue.Labels.Add(label);
            }
            await _client.Issue.Create(repoOwner, repo, issue);
        }
        /// <summary>
        /// Returns "NewIssue" for ease
        /// </summary>
        /// <param name="title">The issue title</param>
        /// <param name="body">The body of the issue</param>
        /// <returns></returns>
        public static NewIssue Issue(string title, string body) => new NewIssue(title) { Body = body };
    }
}
