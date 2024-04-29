using CommandLine;
using Octokit;
using System.Reflection;

namespace CreateReleaseByPrLabel;

internal class Program
{
    private static async Task<int> Main(string[] args)
    {
        return await Parser.Default.ParseArguments<ActionInputs>(args)
                                   .MapResult(o => RunActionAsync(o),
                                              _ => Task.FromResult(-1))
                                   .ConfigureAwait(false);
    }

    private static async Task<int> RunActionAsync(ActionInputs actionInputs)
    {
        Console.WriteLine($"{Assembly.GetExecutingAssembly().GetName().Name} v{Assembly.GetExecutingAssembly().GetName().Version} started...");

        string? repo = Environment.GetEnvironmentVariable("GITHUB_REPOSITORY");

        if (string.IsNullOrWhiteSpace(repo) || !repo.Contains('/'))
        {
            Console.WriteLine("Error: GITHUB_REPOSITORY environment variable not found.");
            return -2;
        }

        if (string.IsNullOrWhiteSpace(actionInputs.Token))
        {
            Console.WriteLine("Error: Authentication token not found, use either GITHUB_TOKEN or a Personal Access Token.");
            return -2;
        }

        try
        {
            string owner = repo.Split("/")[0];
            string repoName = repo.Split("/")[1];

            GitHubClient client = new(new ProductHeaderValue("create-release-by-pr-label"))
            {
                Credentials = new Credentials(actionInputs.Token)
            };

            var pullRequest = await client.Repository.PullRequest.Get(owner, repoName, actionInputs.PullRequestNumber);

            if(pullRequest == null)
            {
                Console.WriteLine($"Error: Could not find pull request with number: {actionInputs.PullRequestNumber}.");
                return -2;
            }

            var label = pullRequest.Labels.Where(x =>
                x.Name.ToLower() == "major" ||
                x.Name.ToLower() == "minor" ||
                x.Name.ToLower() == "patch").First();

            if (label == null)
            {
                Console.WriteLine("Error: Label not found.");
                return -2;
            }

            var release = await client.Repository.Release.GetLatest(owner, repoName);

            var currentVersion = release == null ? "0.0.0" : release.TagName;

            var updatedVersion = GetUpdatedVersionNumber(currentVersion, label.Name);

            var newRelease = new NewRelease($"v{updatedVersion}");
            newRelease.Prerelease = true;
            newRelease.Name = $"Release v{updatedVersion}";
            newRelease.GenerateReleaseNotes = true;

            var result = await client.Repository.Release.Create(owner, repoName, newRelease);

            if (result == null)
            {
                Console.WriteLine($"Error: Unable to create release.");
                return -2;
            }
            else
            {
                Console.WriteLine($"Release created.");
                return 0;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.GetType()} - {ex.Message} \n {ex}");
            return -3;
        }
    }

    private static string GetUpdatedVersionNumber(string currentVersion, string label)
    {
        currentVersion += currentVersion.Trim('v');

        var versionArray = currentVersion.Split('.');
        var major = Convert.ToInt16(versionArray[0]);
        var minor = Convert.ToInt16(versionArray[1]);
        var patch = Convert.ToInt16(versionArray[2]);

        switch (label.ToLower())
        {
            case "major":
                major++;
                minor = 0;
                patch = 0;
                break;
            case "minor":
                minor++;
                patch = 0;
                break;
            case "patch":
                patch++;
                break;
        }

        return $"{major}.{minor}.{patch}";
    }
}
