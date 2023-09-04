using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace SquirrelUtils.GitBugger {
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    [SuppressMessage("ReSharper", "SwitchStatementHandlesSomeKnownEnumValuesWithDefault")]
    public class Bugger {
        private const string GithubUrl = "https://api.github.com/repos/";
        private const int GithubWaitForbidden = 60;
        public string? Token;

        private HttpClient? _client;
        
        public Bugger(string token) => Token = token;

        private HttpClient GetClient() {
            if (_client != null) {
                return _client;
            }
            _client = new HttpClient();
            _client.DefaultRequestHeaders.Add("Connection", "close");
            _client.DefaultRequestHeaders.Add("User-Agent", "GitBugger-Client");
            _client.DefaultRequestHeaders.Add("Accept", "application/vnd.github.v3+json");
            _client.DefaultRequestHeaders.Add("Authorization", $"token {Token}");
            return _client;
        }

        /// <summary>
        /// Submits and issue to give repo
        /// </summary>
        /// <param name="owner">The repo owner</param>
        /// <param name="repo">The repo</param>
        /// <param name="issue">The GithubIssue used</param>
        public async Task SubmitIssue(string owner, string repo, GithubIssue issue) {
            while (true) {
                HttpClient httpClient = GetClient();
                HttpResponseMessage response = await httpClient.PostAsync($"{GithubUrl}{owner}/{repo}/issues", JsonContent.Create(issue));
                switch (response.StatusCode) {
                    case HttpStatusCode.Created:
                        Console.WriteLine("Done");
                        break;
                    case HttpStatusCode.Forbidden:
                        Console.Write("Too many requests. Github is mad. Will retry in 60 seconds...");
                        await Task.Delay(GithubWaitForbidden);
                        continue;
                    default: {
                        string errorMessage = await response.Content.ReadAsStringAsync();
                        Console.WriteLine(errorMessage);
                        break;
                    }
                }

                break;
            }
        }

        /// <summary>
        /// The record used in creating a new Issue
        /// </summary>
        /// <param name="Title">The title of the issue</param>
        /// <param name="Body">The body of the issue</param>
        public record GithubIssue(string Title, string Body) {
            public List<string> Labels { get; set; } = new();
            public string? Milestone { get; set; }
            public List<string> Assignees { get; set; } = new();

            /// <summary>
            /// Used to add a label to an issue
            /// The label needs to exist in the Github repo or Github will just ignore the label
            /// </summary>
            /// <param name="label">The label to add</param>
            /// <returns>GithubIssue</returns>
            public GithubIssue AddLabel(string label) {
                Labels.Add(label);
                return this;
            }

            /// <summary>
            /// Used to add labels to an issue
            /// The labels need to exist in the Github repo or Github will just ignore the labels
            /// </summary>
            /// <param name="labels">The labels to add</param>
            /// <returns>GithubIssue</returns>
            public GithubIssue AddLabels(string[] labels) {
                Labels.AddRange(labels);
                return this;
            }

            /// <summary>
            /// Used to add labels to an issue
            /// The labels need to exist in the Github repo or Github will just ignore the labels
            /// </summary>
            /// <param name="labels">The labels to add</param>
            /// <returns>GithubIssue</returns>
            public GithubIssue AddLabels(List<string> labels) {
                Labels.AddRange(labels);
                return this;
            }

            /// <summary>
            /// Used to add a milestone label to an issue
            /// The Milestone needs to exist in the repo or Github will just ignore this
            /// </summary>
            /// <param name="milestone">The milestone to add</param>
            /// <returns>GithubIssue</returns>
            public GithubIssue AddMilestone(string? milestone) {
                Milestone = milestone;
                return this;
            }

            /// <summary>
            /// Used to add a milestone label to an issue
            /// The Milestone needs to exist in the repo or Github will just ignore this
            /// </summary>
            /// <param name="milestone">the milestone to add</param>
            /// <returns>GithubIssue</returns>
            public GithubIssue AddMilestone(int milestone) {
                Milestone = milestone.ToString();
                return this;
            }

            /// <summary>
            /// Used to add an assignee to an issue
            /// </summary>
            /// <param name="assignee">The Github user to add</param>
            /// <returns>GithubIssue</returns>
            public GithubIssue AddAssignee(string assignee) {
                Assignees.Add(assignee);
                return this;
            }

            /// <summary>
            /// Used to add assignees to an issue
            /// </summary>
            /// <param name="assignees">The Github users to add</param>
            /// <returns>GithubIssue</returns>
            public GithubIssue AddAssignees(List<string> assignees) {
                Assignees.AddRange(assignees);
                return this;
            }

            /// <summary>
            /// Used to add assignees to an issue
            /// </summary>
            /// <param name="assignees">The Github users to add</param>
            /// <returns>GithubIssue</returns>
            public GithubIssue AddAssignees(string[] assignees) {
                Assignees.AddRange(assignees);
                return this;
            }

        }

    }
}
