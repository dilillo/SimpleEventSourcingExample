# Agents

## Copilot Cloud Agent Setup

The repository contains a [Copilot setup steps workflow](.github/workflows/copilot-setup-steps.yml) that pre-configures the agent's ephemeral development environment before it starts working.

### What the setup does

| Step | Action |
|---|---|
| Checkout | Clones the repository (full history available to the agent). |
| Set up .NET | Installs the .NET Core 3.1 SDK via `actions/setup-dotnet@v4`. |
| Restore dependencies | Runs `dotnet restore` to download all NuGet packages so the agent can build and test immediately. |

### Runner

The agent runs on **`ubuntu-latest`** (standard GitHub-hosted runner).

### Permissions

The workflow requests only `contents: read`, following the principle of least privilege. The agent receives its own separate token for write operations (creating branches, commits, and pull requests).

### Customizing the environment

Edit `.github/workflows/copilot-setup-steps.yml` to:

- **Change the .NET SDK version** – update the `dotnet-version` value in the `actions/setup-dotnet` step.
- **Use a larger runner** – change `runs-on` to a larger GitHub-hosted runner label (e.g., `ubuntu-4-core`) or an ARC-managed self-hosted runner label.
- **Add extra tools** – append additional steps (e.g., install `dotnet-coverage`, set up Docker, etc.).

> The `copilot-setup-steps` job name must remain unchanged or the workflow will not be recognized by the agent.

## Further Reading

- [Customizing the Copilot cloud agent environment](https://docs.github.com/en/copilot/customizing-copilot/customizing-the-development-environment-for-copilot-coding-agent)
- [Copilot instructions file](.github/copilot-instructions.md)
