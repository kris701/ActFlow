# ActFlow CLI

A simple CLI to interact with the [ActFlow](https://github.com/kris701/ActFlow) system.
The syntax are as follows for running workflows directly:
```
actflow run <workflow file> -c <config file>
```
* `-c|--config` a path to the config file to use.
* `-o|--output` path to output the result to.
* `--limiter` limiter on how many activities are allowed to run pr workflow.
* `--persistent` Directory to keep persistent data in.
* `--runner` Directory to keep active workflow runs in

The syntax are as follows for running a small HTTP server that can run workflows:
```
actflow serve -c <config file>
```
* `-c|--config` a path to the config file to use.
* `-h|--host` host o serve the HTTP server on.
* `--lifetime` Amount of time (seconds) before completed workflows gets removed from memory.
* `--limiter` limiter on how many activities are allowed to run pr workflow.
* `--persistent` Directory to keep persistent data in.
* `--runner` Directory to keep active workflow runs in

The syntax are as follows for managing installed plugins.
```
actflow plugins add <plugin name> <plugin version>
actflow plugins remove <plugin name> <plugin version>
actflow plugins list
```

## How to Use
Generally, you want to install plugins first.
These plugins are the NuGet packages you can find in this repo.
As an example, to install the "Core" integration, enter:

```
actflow plugins add ActFlow.Integrations.Core 1.0.1
```

All the installed plugins are loaded at runtime.

You can only have one version of a given plugin installed at the same time!


### The `serve` command

When you run the `serve` command, a small web server starts that has the following endpoints:
* `/run`: Runs a workflow and returns the result
* `/queue`: Starts a workflow, but doesnt wait for the result. It just returns the run ID.
* `/status`: Gets a simplified list of all currently active workflows.
* `/status?id=<run ID>`: Gets the result of a given workflow run based on the run ID.

