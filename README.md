# SyncOwe

## Getting Started

### Setting up database

1. Install [CosmosDB Emulator](https://learn.microsoft.com/en-us/azure/cosmos-db/local-emulator?tabs=ssl-netstd21)

1. Launch the emulator page in a web browser

1. In the Explorer tab click New Container

1. Under Database id select Create new and type in `syncowe`

1. Under Container id type in `Trips`

1. Under Partition key type in `/id`, then click OK

1. Select New Container again, but this time choose Use existing and select syncowe

1. Under Contianer id type in `Users`

1. Under Partition key type in `/email`

1. Add a unique key and enter `/email`, then click OK

### Connecting to local database

1. In the **Api** folder, create a `local.settings.json`. 

1. Insert the following into this file:
```
{
  "IsEncrypted": false,
  "Values": {
    "AzureWebJobsStorage": "",
    "FUNCTIONS_WORKER_RUNTIME": "dotnet-isolated"
  },
  "Host": {
    "LocalHttpPort": 7071,
    "CORS": "*",
    "CORSCredentials": false
  },
  "ConnectionStrings": {
    "CosmosDBConnection": {
      "ConnectionString": "TBD",
      "ProviderName": "System.Data.SqlClient"
    }
  }
}
```

1. Go to your cosmosDB emulator in a web browser, select the QuickStart tab.

1. Copy the Primary Connection String and paste it into the file where TBD is.

1. Follow steps for Visual Studio followed by Visual Studio Code

### Visual Studio 2022

Once you clone the project, open the solution in the latest release of [Visual Studio 2022](https://visualstudio.microsoft.com/vs/) with the Azure workload installed, and follow these steps:

1. Right-click on the solution and select **Set Startup Projects...**.

1. Select **Multiple startup projects** and set the following actions for each project:
    - *Api* - **Start**
    - *Client* - **Start**
    - *Shared* - None

1. Press **F5** to launch both the client application and the Functions API app.

### Visual Studio Code with Azure Static Web Apps CLI for a better development experience (Optional)

1. Install the [Azure Static Web Apps CLI](https://www.npmjs.com/package/@azure/static-web-apps-cli) and [Azure Functions Core Tools CLI](https://www.npmjs.com/package/azure-functions-core-tools).

1. Open the folder in Visual Studio Code.

1. In the VS Code terminal, run the following command to start the Static Web Apps CLI, along with the Blazor WebAssembly client application and the Functions API app:

    ```bash
    swa start http://localhost:5000 --api-location http://localhost:7071
    ```

    The Static Web Apps CLI (`swa`) starts a proxy on port 4280 that will forward static site requests to the Blazor server on port 5000 and requests to the `/api` endpoint to the Functions server. 

    If you get an error saying the file is not digitally signed run
    ```bash
    Set-ExecutionPolicy -Scope Process -ExecutionPolicy Bypass
    ```

1. Open a browser and navigate to the Static Web Apps CLI's address at `http://localhost:4280`. You'll be able to access both the client application and the Functions API app in this single address. When you navigate to the "Fetch Data" page, you'll see the data returned by the Functions API app.

1. Enter Ctrl-C to stop the Static Web Apps CLI.

## Template Structure

- **Client**: The Blazor WebAssembly sample application
- **Api**: A C# Azure Functions API, which the Blazor application will call
- **Shared**: A C# class library with a shared data model between the Blazor and Functions application
