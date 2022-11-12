# Client Data Access library

## Introduction

Enables persistence of plans, subscription, and other transaction data used with the **[SaaS Fulfillment API (v2)](https://docs.microsoft.com/en-us/azure/marketplace/partner-center-portal/pc-saas-fulfillment-api-v2)** and **[Marketplace Metering Service API](https://docs.microsoft.com/en-us/azure/marketplace/partner-center-portal/marketplace-metering-service-apis)**.

## Development

The project uses EF Core as an ORM layer to the database i. As of 6.0.2 the project uses Code-First design and Migrations to manage the data layer.

The database context and migrations are managed in a separate project (DataAccess).

## Prerequisites
In order to work with the migrations you need to install the dotnet ef tools on your machine. You can do that using:
`dotnet tool install --global dotnet-ef` or `dotnet tool update --global dotnet-ef`

The project has been tested to work with Tools version 6.0.0 or above.

[More info](https://learn.microsoft.com/en-us/ef/core/cli/dotnet)

## Development flow

Whenever changes are made to the database, a new migration has to be added to handle changes when deployed.
As a best practice, only create one migration per "feature" / branch. 

To run, the EF Core tools, need to be pointed at an "app" that uses the database. We will be using the Admin website as that startup project. In order for migrations to function, the statup project must be able to compile and start (ef core tools will do that in the background). most importantly, the startup project must be configured with a real connection string to an instance of the database. To that end, make sure your appsettings.development.json is always present and properly configured for the Admin Site to run.

Here are some examples of commands you will need to run (Open PowerShell terminal and ensure you are in the /src folder for the examples below to work)

**1. Add a new migration to the project**

Once changes to the Entites are complete, you will have to generate a new migration:

```powershell
dotnet ef migrations add "<NAME>" `
   --context SaasKitContext `
   --project .\DataAccess\DataAccess.csproj `
   --startup-project .\AdminSite\AdminSite.csproj
```

Details:
- `context` parameter represents the Type of the DbContext object we want to geenrate migration for
- `project` references the project where migrations are hosted (relative to the current directory in the terminal)
- `startup-project` reference to the project that executes the DataContext. This project must be correctly configured, and have the connection string set up to the target database you want to affect (usually a dev instance, or a local instance of the database). !IMPORTANT! this project must compile and be able to run 

Once the command runs, you will see the new migration in the `Migrations` folder. Each migration is made up of a class separated in 2 files. The main class has 2 methods `Up()` and `Down()` that you can potentially edit in advanced scenarios (see 4.). You should never edit the `.designer.cs` file.

See below for more info on how you can manipulate the migration once generated.

**2. Deploy migration to your dev database**

While in development, you might want to deploy your changes to a test database in order to verify your changes.

```powershell
dotnet ef database update `
   --context SaasKitContext `
   --project .\DataAccess\DataAccess.csproj `
   --startup-project .\AdminSite\AdminSite.csproj
```

Important! Ensure the AppSettings from the AdminSite project point to the correct database you want to update. 

**3. Revert migration**

While in development, you might want to revert changes you made, so that you can add more changes to the same migration instead of creating multiple migrations. 

If you deployed the migrations to the database (see 2.) then you have to revert the database first, and then remove the migration:

```powershell
dotnet ef database update "<PREV-MIGRATION-NAME>" `
   --context SaasKitContext `
   --project .\DataAccess\DataAccess.csproj `
   --startup-project .\AdminSite\AdminSite.csproj
```

There are a couple ways to determine the name of the migration to revert to. Either check the `Migrations` folder and get the name from the file name (i.e. "20221112150735_Subscriptions-OfferId" the migration name is "Subscriptions-OfferId") or you can run a command to list all migrations and choose the one you want to revert to:

```powershell
dotnet ef migrations list `
   --context SaasKitContext `
   --project .\DataAccess\DataAccess.csproj `
   --startup-project .\AdminSite\AdminSite.csproj
```

Once the database is migrated back, you can revert the migration using the following command:

```powershell
dotnet ef migrations remove `
   --context SaasKitContext `
   --project .\DataAccess\DataAccess.csproj `
   --startup-project .\AdminSite\AdminSite.csproj
```

Notes:
- The above command will remove the last migration in the stack. If you run this multiple times, it might remove more migrations, so make sure you use it correctly.
- If you have applied the database changes and have not run the rollback, you will not be able to remove the migration. Ensure you rolled back the target db first.
- Once these steps are complete, make the adjustments to the Entities and start at step1.

*IMPORTANT!!! Never remove migrations that have already been checked in and deployed. Even if you want to revemoe fields you need to add a new migration if the code has been deployed in production already*

**4. Advanced migration scenario**

If you migration requires other operations other than schema updates (i.e. data seeding or updates) you have the ability to update the migration code.

You can see an example of that in the Initial Migration where we are seeding the database with email templates and enum values.

The best practice for this is to create a new class under `Migrations/Custom` folder and call the methods in your new Migration `Up()` method.

**5. Deployment to production**

There is no need to do anything to deploy a migration to production once the code is complete and pushed to the repository.

As part of the `deploy-code.ps1` script we use another `dotnet-ef` command to generate the required script to update the database

```powershell 
dotnet-ef migrations script `
   --idempotent --output script.sql `
   --context SaasKitContext `
   --project .\DataAccess\DataAccess.csproj `
   --startup-project .\AdminSite\AdminSite.csproj
```

The above script generates a new file `script.sql` that contains the entire database generation script. Because of the `--idempotent` flag, the script is safe to applyto any database. The way it works is it uses the built-in `__EF-Migrations` table to determine the current state of the target database and only apply new chnages to it.



## Description

The project contains the repositories to manage entities in the database.

Entities that are persisted in the Database

| Entity Name | Description |
| --- | --- |
| Application Log | Tracks the Customer Web Application Sample activity |  
| Metered Dimensions | Tracks the metered information that will be submitted to the Marketplace Metering Service API. The metered data is submitted by the Publisher. In the SDK, there is an interface in the Publisher Sample web application that will help generate this data.  |
| Plans | Stores the SaaS Offer plan information. Plans are part of the SaaS offer and submitted via Partner Center. This table is populated after calls to the Fulfillment API|  
| Subscription Audit Log | This table is an audit trail for all the operations regarding Subscriptions. |  
| Subscriptions | Tracks the Offer's Subscriptions. Associates Customers (users) with their selected Plan|  
| Users | Tracks the list of Customers (users that created subscriptions against the offer) |  
| Application Configuration | Tracks the configurations required for SMTP and emails. |
| Database Version History | Tracks the database script versions. |
| Email Template | Tracks the emails templates html. |
| Events | Tracks the list of events available. |
| Known Users | Tracks the users that have access to login to the admin portal. |
| Metered Audit Logs | Tracks all the metering data posted to metering service API on a subscription. |
| Offers | Stores the SaaS offers information. |
| Offer Attributes | Tracks the list of all Custom fields that to be filled by customer after purchasing a subscription. |
| Plan Attribute Mapping | Tracks the list of Custom fields mapped per plan. |
| Plan Events Mapping | Tracks the events and email ids mapped per plan. |
| Roles | Tracks available roles in the system. |
| Subscription Attribute Values | Tracks the data filled by customer after purchasing the subscription. |
| Value Types | Tracks all the data types available to create offer parameters. |
| Web Job Subscription Status | Tracks the activity done by the web job. |
| Plan Attribute Output | Does not hold any data, used to map the result of the stored procedure spGetOfferParameters. |
| Plan Events Output | Does not hold any data, used to map the result of the stored procedure spGetPlanEvents. |

[Click here](Transactable-SaaS-SDK-Sample-Database.md) for more details on the sample database.

## Source Code

The Project is located in the **SaaS.SDK.Client.DataAccess** folder.
The project is composed of the following sections:

| Section Name | Description |
| --- | --- |  
| Dependencies | EntityFrameworkCore,  EntityFrameworkCore.SqlServer|
| Context | EntityFramework database context |
| Contracts | Interface that defines the methods to manage the entities |
| Entities | Table entities |
| Services | Implementations for the interfaces to manage the persistence and query over entities |
