<p align="center">
    <img src="https://github.com/user-attachments/assets/e7750a92-fe96-4742-8327-a34978d587fe" width="200" height="200" />
</p>

[![Build and Publish](https://github.com/kris701/ActFlow/actions/workflows/dotnet.yml/badge.svg)](https://github.com/kris701/ActFlow/actions/workflows/dotnet-desktop.yml)
![Nuget](https://img.shields.io/nuget/v/ActFlow)
![Nuget](https://img.shields.io/nuget/dt/ActFlow)
![GitHub last commit (branch)](https://img.shields.io/github/last-commit/kris701/ActFlow/main)
![GitHub commit activity (branch)](https://img.shields.io/github/commit-activity/m/kris701/ActFlow)
![Static Badge](https://img.shields.io/badge/Platform-Windows-blue)
![Static Badge](https://img.shields.io/badge/Platform-Linux-blue)
![Static Badge](https://img.shields.io/badge/Framework-dotnet--10.0-green)

# ActFlow
ActFlow is a simple, no-code, workflow system that takes in a set of activities and strings them together as workflows.
The intention for this is to enable you to run complex business logic by means of script files instead of hardwired integrations.
This makes it significantly easier to make workflows such as one for waiting and answering emails, keep a database updated with data an LLM found from an email, integrity check data in a database, etc.
The ActFlow engine in of itself is very independent and does not require any complex scheduling system to run.

By its core, it consists of a set of workers and activities.
Workers execute activities, while activities define some input data for the workers.

All workers and activities are JSON serializable

> [!IMPORTANT]
> To make this work, you must add a modifier to the default TypeInfoResolver for a JsonSerializer instance like this: `JsonSerializerOptions(){ TypeInfoResolver = new DefaultJsonTypeInfoResolver().WithAddedModifier(JsonExtensions.AddNativePolymorphicTypInfo) }`

## How to use
Start by installing the nuget package `ActFlow` into your project.
This package contains the actual engine used to run the workflows.
You can add whichever integration nuget package afterwards, to give you workers and activities (I will assume you have `ActFlow.Integrations.Core` installed for the following).
You can then setup the ActFlow engine by doing the following:
```csharp
var engine = new ActFlowEngine(new List<IWorker>()
{
	new NoActivityWorker("a"),
    new CreateContextWorker("b")
});
```
This has now defined an engine instance with two workers, `NoActivityWorker` with the ID `a` and a `CreateContextWorker` with the id `b`.
The IDs are important, since they are used by activities to know what worker to run (this also means you can have multiple workers with different configurations).

You can then run the following script (represented in JSON):
```json
{
	"Name":"test workflow",
	"Globals":{ "noactivityworker":"a", "createcontextworker":"b" },
	"Activities": [
		{
			"$type":"NoActivity",
			"WorkerID":"${{noactivityworker}}"
		},
		{
			"$type":"CreateContextActivity",
			"WorkerID":"${{createcontextworker}}",
			"Context": {
				"$type":"StringContext",
				"Text":"abc"
			}
		}
	]
}
```
You can execute is as:
```csharp
var state = await engine.Execute(
    JsonSerializer.Deserialize<Workflow>(
        ..., 
        JsonSerializerOptions() { 
            TypeInfoResolver = 
                new DefaultJsonTypeInfoResolver()
                    .WithAddedModifier(JsonExtensions.AddNativePolymorphicTypInfo) 
            }
        )
    );
```
This will run the script, and return the final state of the execution.
The result of each step will be visible through the `ContextStore` property.
As an example, the resulting context of the last activity will be accessible through the key `b.Text` where it will give the result "abc".

Using this simple system syntax, you can combine many different activities to create complex business logic.

## Integrations

Here is a list of integrations that you can use in your ActFlow workflows.

![Static Badge](https://img.shields.io/badge/Integration-Core-green) ![Nuget](https://img.shields.io/nuget/v/ActFlow.Integrations.Core) ![Nuget](https://img.shields.io/nuget/dt/ActFlow.Integrations.Core)

Contains some core activities that you typically see used in workflows. 
Take a look in the Integrations folder under Core to inspect the schema of the activities.

The activities are:
* **Conditional If**
    * Compares two string values against each other. True or false redirects to different activity indexes in the workflow file.
* **Conditional (user) If**
    * Same as the conditional if, but where one of the values is a user input.
* **Create Context**
    * Directly creates a new context.
* **Insert Globals**
    * Insert some global values that can be used across the entire workflow.
* **No Action**
    * A placeholder activity that does nothing.

![Static Badge](https://img.shields.io/badge/Integration-DatabaseSharp-green) ![Nuget](https://img.shields.io/nuget/v/ActFlow.Integrations.DatabaseSharp) ![Nuget](https://img.shields.io/nuget/dt/ActFlow.Integrations.DatabaseSharp)

Integrates [DatabaseSharp](https://github.com/kris701/DatabaseSharp) as activities.
Take a look in the Integrations folder under DatabaseSharp to inspect the schema of the activities.

The activities are:
* **Fetch Items From Database**
    * Gets a set of items from a database and returns them as a Dictionary context.
* **Insert Item Into Database**
    * Insert some data into a database.
* **Insert Workflow From Databaser**
    * Fetch a workflow from a database and insert it after this activity.

![Static Badge](https://img.shields.io/badge/Integration-EMail-green) ![Nuget](https://img.shields.io/nuget/v/ActFlow.Integrations.EMail) ![Nuget](https://img.shields.io/nuget/dt/ActFlow.Integrations.EMail)

Integrates Outlook email handling as activities.
Take a look in the Integrations folder under EMail to inspect the schema of the activities.

The activities are:
* **Reply to Email**
    * Reply to a given email
* **Send Email**
    * Send a new email
* **Wait for Email**
    * Wait for a reply on a given email

![Static Badge](https://img.shields.io/badge/Integration-JSON-green) ![Nuget](https://img.shields.io/nuget/v/ActFlow.Integrations.JSON) ![Nuget](https://img.shields.io/nuget/dt/ActFlow.Integrations.JSON)

Integrates some JSON handling as activities.
Take a look in the Integrations folder under JSON to inspect the schema of the activities.

The activities are:
* **Extract Value From JSON**
    * Given some text, extract some data by means of a JSONPath

![Static Badge](https://img.shields.io/badge/Integration-ML.NET-green) ![Nuget](https://img.shields.io/nuget/v/ActFlow.Integrations.ML.NET) ![Nuget](https://img.shields.io/nuget/dt/ActFlow.Integrations.ML.NET)

Integrates [ML.NET](https://dotnet.microsoft.com/en-us/apps/ai/ml-dotnet) handling as activities.
Take a look in the Integrations folder under ML.NET to inspect the schema of the activities.

The activities are:
* **Train Text Classitifer**
    * Train a text classifier with some data
* **Classify Text**
    * Classify some text

![Static Badge](https://img.shields.io/badge/Integration-OpenWebUISharp-green) ![Nuget](https://img.shields.io/nuget/v/ActFlow.Integrations.OpenWebUISharp) ![Nuget](https://img.shields.io/nuget/dt/ActFlow.Integrations.OpenWebUISharp)

Integrates [DatabaseSharp](https://github.com/kris701/OpenWebUISharp) as activities.
Take a look in the Integrations folder under OpenWebUISharp to inspect the schema of the activities.

The activities are:
* **Extract Data From Text**
    * Extract data from text using a LLM
* **Extract Data From Text (RAG)**
    * Extract data from text using a LLM (RAG)
* **Query**
    * Query an LLM

