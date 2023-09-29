# Working with Migrations.

### Making updates to the database

1. Update the DataModel by making the changes by adding/removing/updating the relevant properties in the Entity classes.
1. If you need to add new entities, add a new class under `Entities` folder (under DataAccess), and make sure you add a `DbSet` property in `SaaSKitContext.cs`
1. In developer PowerShell from the root of the solution (the  `src` folder) run a command to add the migration
    `dotnet-ef migrations add <migrationName> --project .\DataAccess\ --startup-project .\AdminSite\`
1. A new migration class is created under `DataAccess\Migrations` folder. 
1. If you want to pre-seed data, create a new class in the `DataAccess\Migrations\Custom` folder
    Ideally follow the same conventions as the other Seed classes. Have a static method that will insert / update / seed the database with what you need. Then update the migration class's `Up()` method to call the `Seed()` method that you created

### Apply the migration to the databse
Once the migration is created, you need to apply to the database.
**WARNING!!** the database you will apply this to, is determined by the active ConnectionString in the project you specify in the _--startup-project_ parameter of the command. Make sure you are not pointing to an important db if you are making breaking changes.

1. Apply the changes to the database to test
    `dotnet-ef database update --project .\DataAccess\ --startup-project .\AdminSite\`

**NOTE!** There is nothing else you need to do for more than this to deploy your changes.
- As part of the initial accelerator deployment (deploy.ps1) we automatically deploy all migrations to the database in one go so all new customers will get latest db version
- For existing customers, if they upgrade their database using the upgrade script (upgrade.ps1) the migrations will run as part of that process.

### Making additional changes after a migration was created

Ideally, you want to limit the number of migrations you execute in one PR. If for some reason, you need to make additional changes to the database after you generated a migration, it is best to update the migration than create a new one. To update a migration, you need to remove the one you create it and create it again (after you made more changes to the DataModel)

1. Remove the migration
   `dotnet-ef migrations remove --project .\DataAccess\ --startup-project .\AdminSite\`

1. If the migration was already applied to the database, you will get an error if you try to remove the migration so you need to first roll back the migration to the previous version. To roll back you need to apply an older migration (usually the one just before yours)
   `dotnet-ef database update 20221118211554_Baseline_v6 --project .\DataAccess\ --startup-project .\AdminSite\`

 