using System.Collections.Generic;
using Newtonsoft.Json;

namespace GitLabApiClient.Models.Projects.Requests
{
    public class CreateCommitRequest
    {
        [JsonProperty("branch")] public string Branch { get; set; }

        [JsonProperty("commit_message")] public string CommitMessage { get; set; }

        [JsonProperty("start_branch")] public string StartBranch { get; set; }

        [JsonProperty("author_email")] public string AuthorEmail { get; set; }

        [JsonProperty("author_name")] public string AuthorName { get; set; }

        [JsonProperty("stats")] public bool IncludeCommitStats { get; set; }

        [JsonProperty("actions")] public List<CommitAction> Actions { get; set; } = new List<CommitAction>();
    }

    public abstract class CommitAction
    {
        [JsonProperty("action")] public abstract string Action { get; set; }

        [JsonProperty("file_path")] public string FilePath { get; set; }

        [JsonProperty("previous_path")] public string PreviousPath { get; set; }
    }

    public sealed class CreateAction : CommitAction
    {
        public override string Action { get; set; } = "create";

        [JsonProperty("content")] public string Content { get; set; }

        [JsonProperty("encoding")] public string Encoding { get; set; } = "text";
    }

    public sealed class UpdateAction : CommitAction
    {
        public override string Action { get; set; } = "update";

        [JsonProperty("content")] public string Content { get; set; }

        [JsonProperty("encoding")] public string Encoding { get; set; } = "text";

        [JsonProperty("last_commit_id")] public string LastCommitId { get; set; }
    }

    public sealed class MoveAction : CommitAction
    {
        public override string Action { get; set; } = "move";

        [JsonProperty("last_commit_id")] public string LastCommitId { get; set; }
    }

    public sealed class DeleteAction : CommitAction
    {
        public override string Action { get; set; } = "delete";

        [JsonProperty("last_commit_id")] public string LastCommitId { get; set; }
    }

    public sealed class ChmodAction : CommitAction
    {
        public override string Action { get; set; } = "chmod";

        [JsonProperty("execute_filemode")] public string ExecuteFilemode { get; set; }
    }
}