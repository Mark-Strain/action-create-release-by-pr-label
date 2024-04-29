# Create Release By PR Label

<div align="center">

![.NET 6.0](https://img.shields.io/badge/Version-.NET%208.0-informational?style=flat&logo=dotnet)
&nbsp;
![Built With Docker](https://img.shields.io/badge/Built_With-Docker-informational?style=flat&logo=docker)
&nbsp;

</div>

GitHub action that will create a new release and assign it with a version number that is based on a PR label. [Major, Minor or Patch]

As a Docker based action Edit Release requires a Linux runner, see [Types of Action](https://docs.github.com/en/actions/creating-actions/about-custom-actions#types-of-actions).

## Inputs

#### `token`

**Required**

Authentication token, use either `GITHUB_TOKEN` or a Personal Access Token.

#### `pullRequestNumber`

**Required**

Pull Request Number, use the number for the triggering pull request. e.g. `github.event.pull_request.number`.

## Outputs

Action has no outputs other than console messages.
