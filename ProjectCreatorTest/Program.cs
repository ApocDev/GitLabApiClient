using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GitLabApiClient;
using GitLabApiClient.Models.Groups.Requests;
using GitLabApiClient.Models.Projects.Requests;

namespace ProjectCreatorTest
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            const string baseUrl = "http://localhost";
            const string accessToken = "eyjHuQoFdCE_bP1yXuYN";

            // Gitlab has GraphQL support :)
            // It sucks, but it's there.
            //await _client.PostAsync("http://localhost/api/v4/features/graphql", new StringContent("value=100"));

            var projectName = "Test5";
            var projectGroup = "TestProjects";
            var projectDescription = "Test Project";
            var groupDescription = "Test Group";

            var client = new GitLabClient(baseUrl, accessToken);

            var groups = await client.Groups.GetAsync();
            var group = groups.FirstOrDefault(g =>
                string.Equals(g.Name, projectGroup, StringComparison.OrdinalIgnoreCase));
            if (group == null)
            {
                Console.WriteLine("Creating group " + projectGroup);
                group = await client.Groups.CreateAsync(new CreateGroupRequest(projectGroup, projectGroup)
                {
                    LfsEnabled = true,
                    RequestAccessEnabled = true,
                    Visibility = GroupsVisibility.Internal,
                    Description = groupDescription
                });
            }

            var projects = await client.Projects.GetAsync();
            var project = projects.FirstOrDefault(p =>
                p.Namespace.Kind == "group" &&
                p.Namespace.Name.Equals(projectGroup, StringComparison.OrdinalIgnoreCase) &&
                p.Name.Equals(projectName, StringComparison.OrdinalIgnoreCase));

            if (project == null)
            {
                Console.WriteLine("Creating project " + projectName);
                var req = CreateProjectRequest.FromName(projectName);

                req.NamespaceId = group.Id;
                req.Description = projectDescription;
                req.EnableContainerRegistry = false;
                req.EnableIssues = true;
                req.EnableWiki = true;
                req.EnableJobs = true;
                req.EnableLfs = true;
                req.EnableMergeRequests = true;
                req.EnableRequestAccess = true;
                req.EnableSharedRunners = true;
                req.EnableSnippets = true;
                req.EnablePrintingMergeRequestLink = true;
                req.OnlyAllowMergeIfAllDiscussionsAreResolved = true;
                req.OnlyAllowMergeIfPipelineSucceeds = true;
                req.PublicBuilds = false;
                req.Visibility = ProjectVisibilityLevel.Internal;

                project = await client.Projects.CreateAsync(req);
            }
            else
            {
                Console.Error.WriteLine($"Project {projectName} already exists at {project.WebUrl}");
                Console.ReadLine();
                return;
            }


            Console.WriteLine("Committing starter readme to master branch");

            // TODO: Pull the readme...
            await client.Projects.CreateCommit(project.Id, new CreateCommitRequest
            {
                Branch = "master",
                CommitMessage = "Initial Commit",
                Actions = new List<CommitAction>
                {
                    new CreateAction
                    {
                        // TODO: Replace this with a boilerplate "project instructions" readme
                        Content = "# Project Instructions",
                        FilePath = "README.md",
                    }
                }
            });

            Console.WriteLine("Creating develop branch");

            await client.Projects.CreateBranch(project.Id, "master", "develop");

            // Update the default branch to point to develop now that it exists
            await client.Projects.UpdateAsync(new UpdateProjectRequest(project.Id.ToString(), projectName)
            {
                DefaultBranch = "develop"
            });

            // Console.WriteLine("Protecting master branch");
            // await client.Projects.ProtectBranch(project.Id, "master");
            Console.WriteLine("Protecting develop branch");
            await client.Projects.ProtectBranch(project.Id, "develop", AccessLevel.Maintainer, AccessLevel.Maintainer,
                AccessLevel.Maintainer);

            Console.WriteLine("Finished creating project. Visit " + project.WebUrl);
            Console.ReadLine();
        }
    }
}
