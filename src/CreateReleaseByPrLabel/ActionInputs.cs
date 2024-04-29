using CommandLine;
namespace CreateReleaseByPrLabel;

public class ActionInputs
{
    [Option(longName: "token", Required = true, HelpText = "Authentication token, use either GITHUB_TOKEN or a Personal Access Token.")]
    public string Token { get; set; } = null!;

    [Option(longName: "pullRequestNumber", Required = true, HelpText = "Pull Request Number, use the number for the triggering pull request.")]
    public int PullRequestNumber { get; set; }
}
