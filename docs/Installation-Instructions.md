 # Installation instructions

  - [Overview](#overview)
  - [Create an Azure SQL Database single database, an Azure Storage account, a storage queue and prepare](#create-an-azure-sql-database-single-database-an-azure-storage-account-a-storage-queue-and-prepare)
    - [Azure SQL Database](#azure-sql-database)
    - [Azure Storage Account](#azure-storage-account)
  - [Change configuration](#change-configuration)
  - [Create Web Apps, WebJobs on Azure and deploy the code](#create-web-apps-webjobs-on-azure-and-deploy-the-code)
    - [Running the solution locally](#running-the-solution-locally)
  - [Landing page and webhook settings for the SaaS offer on Partner Center](#landing-page-and-webhook-settings-for-the-saas-offer-on-partner-center)
    - [Next steps](#next-steps)

## Overview

This document describes how to implement the required components to enable the SDK for the SaaS Fulfillment API (v2), Marketplace Metering Service API, and additional components that demonstrate how to build a customer provisioning interface, logging, and administration of the customer's subscriptions.

Learn more about what's included and how to-use the SDK [here.](https://github.com/Azure/Microsoft-commercial-marketplace-transactable-SaaS-offer-SDK/blob/master/README.md)

**Note: before you start, reminder that this SDK is community-supported. If you need help or have questions using this SDK, please create a GitHub issue. Do not contact the marketplace pubisher support alias directly regarding use of this SDK. Thank you.**

## Create an Azure SQL Database single database, an Azure Storage account, a storage queue and prepare
 
### Azure SQL Database
 - Create a single database following the instructions on the SQL Database service [quickstart](https://docs.microsoft.com/en-us/azure/sql-database/sql-database-single-database-get-started?tabs=azure-portal) document.

 - Run the script **AMP-DB-2.1.sql** to initialize the database using your favorite SQL management tool, such as [SQL Server Management Studio](https://docs.microsoft.com/en-us/sql/ssms/download-sql-server-management-studio-ssms?view=sql-server-ver15), or [Azure Data Studio](https://docs.microsoft.com/en-us/sql/azure-data-studio/download-azure-data-studio?view=sql-server-ver15). The scripts are in [deployment/database](../deployment/Database) folder.

 - Add the email for the Azure Active Directory user you are planning to log in to the solution to **KnownUsers** table on the database, with value "1" for the RoleId column. For example, if the user is expected to login with **user@contoso.com** run the following script in your favorite management tool.

 ``` sql
  INSERT INTO KnownUsers (UserEmail, RoleId) VALUES ('user@contoso.com', 1)
 ```

### Azure Storage Account
 - Create an Azure Storage account following the instructions in [the article](https://docs.microsoft.com/en-us/azure/storage/common/storage-account-create), using your favorite method in the article. 

 - Create a queue with name **saas-provisioning-queue** on the new storage account following the instructions in this [article](https://docs.microsoft.com/en-us/azure/storage/queues/storage-quickstart-queues-portal). Using this queue name is important, as the WebJob will be listening on this queue.



## Change configuration

Open the files **appsettings.json** under the project **SaaS.SDK.CustomerProvisioning** and **SaaS.SDK.PublisherSolution** update the values as follows:
- **GrantType** - Leave this as `client_credentials`
- **ClientId** - Azure Active Directory Application ID (the value for marketplace offer in Partner Center, under technical configuration tab). Steps to register an Azure AD application are [here](https://docs.microsoft.com/en-us/azure/marketplace/partner-center-portal/pc-saas-registration)
- **ClientSecret** - Secret from the Azure Active Directory Application
- **Resource** - Set this to *62d94f6c-d599-489b-a797-3e10e42fbe22* **this value is important, it is the resource ID for the fulfillment API**
- **FulFillmentAPIBaseURL** - https://marketplaceapi.microsoft.com/api
- **SignedOutRedirectUri** - Set the path to the page the user should be redirected to after signing out from the application
- **TenantId** - Provide the tenant ID detail that was submitted in the. **Technical configuration** section of your marketplace offer in Partner Center.
- **FulfillmentApiVersion** - Use 2018-08-31 for the production version of the fulfillment APIs
- **AdAuthenticationEndpoint** - https://login.microsoftonline.com
- **SaaSAppUrl** - URL to the SaaS Metering service ( for this example. It should be the link to the SaaS application, in general)
- **DefaultConnection** - Set the connection string to connect to the database.     
- **AzureWebJobsStorage** - Connection string to the Azure storage queue. Adding a message to this queue would trigger the Provisioning webjob that monitors the queue for messages

After making all of the above changes, the **appSettings.json** would look like sample below.

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning",
      "Microsoft.Hosting.Lifetime": "Information"
    }
  },
   // Comment the sections - SaaSApiConfiguration and Connection strings when deploying to Azure
  "SaaSApiConfiguration": {
    "GrantType": "client_credentials",
    "ClientId": "<Azure-AD-Application-ID>",
    "ClientSecret": "******",
    "Resource": "62d94f6c-d599-489b-a797-3e10e42fbe22",
    "FulFillmentAPIBaseURL": "https://marketplaceapi.microsoft.com/api",
    "SignedOutRedirectUri": "<provisioning_or_publisher_web_app_base_path>/Home/Index",
    "TenantId": "<TenantID-of-AD-Application>",
    "FulFillmentAPIVersion": "2018-08-31",
    "AdAuthenticationEndPoint": "https://login.microsoftonline.com",
    "SaaSAppUrl" : "<Link-to-SaaS-Application>"
  },
  "connectionStrings" : {
    "DefaultConnection": "Data source=<server>;initial catalog=<database>;user id=<username>;password=<password>"
    },
  "AllowedHosts": "*",
  "AzureWebJobsStorage": "<Connection String for storage queue. Enqueueing a message to this queue triggers the webjob>"  
}

```
## Create Web Apps, WebJobs on Azure and deploy the code

The sample has two web apps and an Azure WebKpb to demonstrate the activation of a subscription for a SaaS offer, and potential scenarios for managing subscriptions and users. 

There are many ways to create App Service resources on [App Service](https://docs.microsoft.com/en-us/azure/app-service/) and deploy the code,
- Using Azure portal
- Using command line tools, [Azure CLI](https://docs.microsoft.com/en-us/azure/app-service/samples-cli), [Azure PowerShell](https://docs.microsoft.com/en-us/azure/app-service/samples-powershell) and [Resource Manager (ARM) templates](https://docs.microsoft.com/en-us/azure/app-service/samples-resource-manager-templates)
- [Using Visual Studio Code](https://docs.microsoft.com/en-us/azure/app-service/app-service-web-get-started-nodejs#deploy-the-app-to-azure), the example on this link is showing a Node.js app, but the same principles apply for a .NET solution.
- [Using Visual Studio](https://docs.microsoft.com/en-us/azure/app-service/app-service-web-get-started-dotnet#publish-your-web-app), this example demonstrates how to create a new web app on the Azure App Service, and deploy the code to it. 
- [Continuos deployment](https://docs.microsoft.com/en-us/azure/app-service/deploy-continuous-deployment)

You can use any of the methods above to create the web apps and deploy the code, but for the rest of this document, let's assume the use of [Visual Studio method](https://docs.microsoft.com/en-us/azure/app-service/app-service-web-get-started-dotnet#publish-your-web-app) to deploy the following two apps. Give appropriate names to indicate the applications' roles, for example, **\<yourname\>provisioning**, and **\<yourname\>publisher**. Please remember that these names will be the dns prefix for the host names of your applications and will eventually be available as yournameprovisioning.azurewebsites.net and yournamepublisher.azurewebsites.net.
1. **Customer provisioning sample web application**, create and deploy the provisioning sample web application project in folder [src/SaaS.SDK.CustomerProvisioning](../src/SaaS.SDK.CustomerProvisioning)
1. **Publisher sample web application**, create and deploy the publisher sample web application project in folder [src/SaaS.SDK.CustomerProvisioning](../src/SaaS.SDK.PublisherSolution)
1. **SaaS.SDK.Provisioning.Webjob**, create and deploy the WebJob project in folder **src/SaaS.SDK.Provisioning.Webjob**. Plesae see the steps in this [article](https://docs.microsoft.com/en-us/azure/app-service/webjobs-dotnet-deploy-vs) with Visual Studio to deploy the WebJob.
  
Deploying the debug release, and choosing "self-contained" deployment mode is useful for the initial deployments.
![publisoptions](./images/VSPublish.png)

**_Important_**, Add the redirect uri on the Azure AD app registration after deploying the publisher solution following the steps [here](https://docs.microsoft.com/en-us/azure/active-directory/develop/quickstart-configure-app-access-web-apis#add-redirect-uris-to-your-application). The value should be https://\<yourappname\>.azurewebsites.net/Home/Index


### Running the solution locally

Make sure both **SaaS.SDK.CustomerProvisioning** and **SaaS.SDK.CustomerProvisioning** are set up to run on the dotnet development web server (Kestrel), by selecting the host. For details, please see Kestrel [documentation](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/servers/kestrel?view=aspnetcore-3.1).

![RunTarget](./images/RunTarget.png)

Right click on the solution, in Visual Studio as shown in the figure below.

![SetStartupProjects](./images/SetStartupProjects.png)

Select the following projects with "Start" action.

![SelectStartupProjects](./images/SelectStartupProjects.png)

Press **F5** in Visual Studio 2019 to run the application locally. After pressing "F5", you should see two browser window opening, one loading a page from https://localhost:5001, and another https://localhost:5081, if both of the browsers open with port 5001, change one of them to 5081. The application running on port 5001 is the customer provisioning application, and 5081 is the publisher solution.

> **_Important_**, Add the redirect uris on the Azure AD app registration before the solutions following the steps [here](https://docs.microsoft.com/en-us/azure/active-directory/develop/quickstart-configure-app-access-web-apis#add-redirect-uris-to-your-application). The values should be 
- https://localhost:5001/Home/Index
- https://localhost:5081/Home/Index

> **_Important_**, the actual flow of subscribing to an offer on the Azure marketplace and managing the relevant lifetime events of the subscription, such as activation, cancellation and upgrade is only possible for the provisioning solution deployed to a location accessible on the internet.

## Landing page and webhook settings for the SaaS offer on Partner Center

The landing page and the webhook endpoint are implemented in the **SaaS.SDK.CustomerProvisioning** application. 

The landing page is the home page of the solution, for example, if you have deployed the solution to \<yourappname\>, the landing page value should be **https://\<yourappname\>.azurewebsites.net**.

Webhook endpoint is at **https://\<yourappname\>.azurewebsites.net/AzureWebhook**

The **Technical Configuration** section of the Marketplace offer with the values filled using the web app names would look like as shown here.

![Technical Configuration](./images/offer-technical-configuration.png)

|Field | Value |
|--|--|
|Landing page URL | Path to the Provisioning Service. Eg: https://saaskit-portal.azurewebsites.net
|Connection webhook | Path to the web hook API in the Provisioning Service. Eg: https://saaskit-portal.azurewebsites.net/api/AzureWebhook
|Azure Active Directory Tenant ID | Tenant where the AD application is registered.
|Azure Active Directory Application ID | ID of the registered AD application

### Next steps

* [Customer purchase experience](./Customer-Experience.md)
* [Publisher experience](./Publisher-Experience.md)
