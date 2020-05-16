 # Transactable SaaS Offer Fulfillment v2 and Metering SDK Instructions

   * [Overview](#overview)
    + [Features](#features)
  * [Prerequisites](#prerequisites)
  * [Set up web application resources in Azure](#set-up-web-application-resources-in-azure)
  * [Marketplace Provisioning Service](#marketplace-provisioning-service)
    + [Create marketplace offer](#create-marketplace-offer)
    + [Set up the sample client application locally](#set-up-the-sample-client-application-locally)
    + [Deploy the application to Azure](#deploy-the-application-to-azure)
      - [Using an ARM template and Azure CLI](#using-an-arm-template-and-azure-cli)
      - [Manual deployment using VS 2019](#manual-deployment-using-vs-2019)
    + [Landing page and Webhook settings in the Marketplace Offer](#landing-page-and-webhook-settings-in-the-marketplace-offer)
    + [Purchase the offer](#purchase-the-offer)
    + [Activate](#activate)
    + [Change plan](#change-plan)
    + [Unsubscribe](#unsubscribe)
    + [Change Quantity](#change-quantity)
    + [View activity log](#view-activity-log)
    + [Go to SaaS application](#go-to-saas-application)
  * [SaaS metering service](#saas-metering-service)
    + [Emit usage events](#emit-usage-events)
  * [License Manager](#license-manager)
    + [Publisher: Manage Licenses](#publisher--manage-licenses)
    + [Customer: View Licenses](#customer--view-licenses)
  * [Troubleshooting issues](#troubleshooting-issues)

## Overview

The SDK provides the components required for the implementations of the billing (fulfillment v2 and metered) APIs, and additional components that showcase how to build a customer provisioning interface, logging, and administration of the customer's subscriptions. These are the core projects in the SDK:  

- **Transactable SaaS Client Library** implements the fulfillment v2 and metered APIs and the Web-hook that handles messages from the Marketplace's E-commerce engine.
- **Customer provisioning sample web application** showcases how to register, provision, and activate the customer subscription. Implemented using ASP.Net Core 3.1, it uses the SaaS Client library and Data Access Library to to invoke and persists interactions with the fulfillment APIs. In addition, it provides interfaces for a customer to manage their subscriptions and plans. 
- **Publisher sample web application** showcases how to generate metered based transactions, persistence of those transactions and transmission of these transactions to the metered billing API. 
- **Client Data Access library** demonstrates how to persist the Plans, Subscriptions, and transactions with the fulfillment and Metered APIs.

The sample and the SDK in this repository cover the components that comprise the highlighted area in this architecture diagram:

![Usecase](./images/UseCaseSaaSAPIs.png)

### Features 

- The Azure Marketplace Metering SDK enables SaaS applications publish usage data to Azure so that customers are charged  according to non-standard units. 
- The metering SDK ( .NET class library ) and a sample web application to report usage events for subscriptions against those plans that support metering ( have the dimensions defined and enabled ) correlate to SaaS Metering and SaaS Service blocks in the below image, respectively.
- More details on the fulfillment APIs can be found [here](https://docs.microsoft.com/en-us/azure/marketplace/partner-center-portal/pc-saas-fulfillment-api-v2#update-a-subscription) 
- More details on the metering APIs can be found [here](https://docs.microsoft.com/en-us/azure/marketplace/partner-center-portal/marketplace-metering-service-apis).
- Steps to create a SaaS offer are available [here](https://docs.microsoft.com/en-us/azure/marketplace/partner-center-portal/create-new-saas-offer)

---

## Prerequisites

Ensure the following prerequisites are met before getting started:

- We recommend using an Integrated Development Environment (IDE):  [Visual Studio Code](https://code.visualstudio.com/),  [Visual Studio 2019](https://visualstudio.microsoft.com/thank-you-downloading-visual-studio/?sku=Community&rel=16#), etc...
- The SDK has been implemented using [.NET Core 3.1.1](https://dotnet.microsoft.com/download/dotnet-core/3.1)
- The Customer provisioning and Publisher web sample applications have been implemented using [ASP.NET Core Runtime 3.1.1](https://dotnet.microsoft.com/download/dotnet-core/3.1)
- For Persistence we are using [Azure SQL Database](https://azure.microsoft.com/en-us/services/sql-database/) and [Entity Framework](https://docs.microsoft.com/en-us/ef/). However, feel free to use any data repository you are comfortable with. The Database Schema is located in the **deployment/Database** folder. 

Besides, it is assumed that you have access to the following resources:
- [Azure subscription](https://ms.portal.azure.com/) - to host the SDK components and sample web applications.
- [Partner Center](https://partner.microsoft.com/en-US/) - to create and publish a marketplace offer. This can be used to create a SQL Server instance in Azure. 

---

## Set up web application resources in Azure

Follow the below steps to create a web application resource in an Azure subscription. The AMP SDK sample client application will be deployed against this resource. 

1. Log on to [Azure](https://portal.azure.com) 
2. Click **All Services** in the menu on the left
3. Click **App Services**
 ![AllServices](./images/Appservice.PNG)

4.  Click **Add** button to add a new **App Service**
 ![AllServices](./images/AppserviceNew.PNG)

5. Fill out the details for the new **App Service**
  - Select Subscription
  - Enter Name  of the instance 
  - Select RunTime stack - **.Net Core 3.1(LTS)**
  - Select **Region**
  - Select  **App Service Plan** 

![AllServices](./images/AddNewAppservice.PNG)

6. Click **Review + Create** to initiate the creation of the resource

7. Go to the details of the resource after it is successfully created. You can use the notification in the top right portion of the menu bar to get a link to the resource

8. Click **Overview** to see the details of the resource that is just created
![AllServices](./images/AppServiceOverview.PNG)

9. In the **Overview** tab, click **Get Publish Profile** button in the menu bar to download the publish profile to your local folder
![AllServices](./images/AppServicePublish.PNG)

> Note: We need to create two web application resources - one for the marketplace provisioning service and the other for SaaS service.

10. Create another Web App for the marketplace provisioning service, using the same process you used for the original Web App.

---

## Marketplace Provisioning Service

The marketplace provisioning service serves as an intermediary between Azure and the target SaaS application. In a real scenario, the intermediary would initiate the provisioning of the SaaS application and activate the subscription against the SaaS offer being purchased.

In this example, the sample client application allows the user to:
- Activate the subscription ( triggers the start of billing against the SaaS offer).
- Switch an existing subscription to another plan.
- Unsubscribe / delete an existing subscription.


### Create marketplace offer

For the purpose of the sample, a new marketplace offer is created and is made available in known tenants to test out the AMP SDK with the sample client application. More details on the creation of SaaS offers are available [here](https://docs.microsoft.com/en-us/azure/marketplace/partner-center-portal/create-new-saas-offer).

### Set up the sample client application locally

In this section, we will go over the steps to download the latest sources from the repository, build the application ready for deployment to Azure.

1. Clone or download the latest source code from [here](https://github.com/Azure/Microsoft-commercial-marketplace-transactable-SaaS-offer-SDK)
2. Open the solution **SaaS.SDK.sln** in Visual Studio 2019

![Solution Structure](./images/solution-structure-vs.png)

3. Right-click on the project named **SaaS.SDK.CustomerProvisioning** and click **Set as StartUp Project**.
4. Open the file **appsettings.json** under the project **SaaS.SDK.CustomerProvisioning** and update the values as follows:

    - **GrantType** - Leave this as `client_credentials`
    - **ClientId** - Azure Active Directory Application ID (as provided in the marketplace offer in Partner Center). Steps to create an Azure AD application for SaaS app can be found [here](https://docs.microsoft.com/en-us/azure/marketplace/partner-center-portal/pc-saas-registration)
    
    *Note:* Ensure that you have set the reply URL to the web application for the authentication to work properly.

5. Update the Application Registration in the AAD tenant.

   - Log on to [Azure](https://portal.azure.com)
   - Click **Azure Active Directory** in the left menu
   - Click **App Registrations** in the menu on the left
   - Locate the AD application and click to go to details
   - Click on the hyperlink next to **Redirect URIs**  
    ![Redirect URIs](./images/ad-app-redirect-uris.png)

    - Make sure you set https://localhost:44363/Home/Index as the redirect uri for the authentication to work when you run the app locally in Visual Studio.
    ![Redirect URI](./images/ad-app-redirect-uris-home-index.png)
    
    - Scroll down and check the box that reads **ID tokens** in the **Implicit grant** section
    ![ID Token](./images/id-token.png)

    - **ClientSecret** - Secret from the Azure Active Directory Application

    - **Resource** - Set this to *62d94f6c-d599-489b-a797-3e10e42fbe22*

    - **FulFillmentAPIBaseURL** - https://marketplaceapi.microsoft.com/api

    - **SignedOutRedirectUri** - Set the path to the page the user should be redirected to after signing out from the application

    - **TenantId** - Provide the tenant ID detail that was submitted in the. **Technical configuration** section of your marketplace offer in Partner Center.

    - **FulfillmentApiVersion** - Use 2018-09-15 for mock API and 2018-08-31 for the production version of the fulfilment APIs

    - **AdAuthenticationEndpoint** - https://login.microsoftonline.com
    
    - **SaaSAppUrl** - URL to the SaaS Metering service ( for this example. It should be the link to the SaaS application, in general)
    
    - **DefaultConnection** - Set the connection string to connect to the database    

After making all of the above changes, the **appSettings.json** would look like below sample.

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
    "FulFillmentAPIVersion": "2018-09-15",
    "AdAuthenticationEndPoint": "https://login.microsoftonline.com",
    "SaaSAppUrl" : "<Link-to-SaaS-Application>"
  },
  "connectionStrings" : {
    "DefaultConnection": "Data source=<server>;initial catalog=<database>;user id=<username>;password=<password>"
    },
  "AllowedHosts": "*"
}

```
> **Note**: When defining the keys in Azure App Service -> Configuration -> App Settings, refer to the below example for correctness:

|Name| Value|
|--|--|
|SaaSApiConfiguration__GrantType| client_credentials|_

> **Tip** __(double underscore) should be used to define the config items that appear as nested keys in appSettings.json

---

### Deploying the SQL Database

1. Click the button to start the deployment of SQL database 
<a href="https://portal.azure.com/#create/Microsoft.Template/uri/https%3A%2F%2Fraw.githubusercontent.com%2FSpektraSystems%2FAMP-SDK-Sample%2Fmaster%2Fdeploy%2Farm-deploy-v1.json" target="_blank">

    <img src="https://raw.githubusercontent.com/Azure/azure-quickstart-templates/master/1-CONTRIBUTION-GUIDE/images/deploytoazure.png"/> 
</a> 

2.  Fill out the details on the template deployment form as shown here
![Deploy database](./images/Deploy-Database.png) 

3. Click **Purchase** after agreeing to the terms and conditions by checking the box to start the deployment of the database by name **AMPSaaSDB**
4. Update the connection string property in **appSettings.json** with the details related to SQL Server name, database and the credentials to connect to the database.
   >**Note:** The application holds the configuration, feature flags and email templates in tables named **ApplicationConfiguration** and **EmailTemplate** tables. It is recommended that these tables are initialized and the values are validated by running the relevant SQL in **AMP-DB-2.0.sql**

### Alterntive SQL Database Setup

If you want to set up the database locally, you could create and initialize the database by following the steps given below:
1. Create a database named **AMPSaaSDB**
2. Switch to the database - **AMPSaaSDB**
3. Run the script - **AMP-DB-1.0.sql** to initalize the database
4. Run the script - **AMP-DB-2.0.sql** to update your existing database to 2.0
5. Add entries into KnownUsers table to allow login to **Publisher Portal**   
  > Note: If you already had created a database using an earlier version of the SDK, you just need to run the **AMP-DB-2.0.sql** 

---

### Run the Applicaiton Locally      

Press **Ctrl + F5** in Visual Studio 2019 to run the application locally.
*Note: Make sure that the home page url is listed in the **replyURLs** in the AD application for the authentication against Azure AD to work properly.*

---

### Deploy the application to Azure

#### Using an ARM template and Azure CLI

1. Click [![Deploy to Azure](https://raw.githubusercontent.com/Azure/azure-quickstart-templates/master/1-CONTRIBUTION-GUIDE/images/deploytoazure.png)](https://portal.azure.com/#create/Microsoft.Template/uri/https%3A%2F%2Fraw.githubusercontent.com%2FSpektraSystems%2FAMP-SDK-Sample%2Fmaster%2Fdeploy%2Fdeploy-webapp.json) to launch the template that deploys web applications to Azure
2. After navigating to the Azure portal, fill out the form that appears as below:

  ![ARM Template to deploy web apps](./images/deploy-webapp-template.png)

> **Web App Name Prefix** - The template creates two web applications and the prefix is used to prepend the word before the names of the web applications and the app service plan.
      -Eg: Web App Name Prefix  = **saasdemo**. App service plan is created with the name - **saasdemoAmpAppSvcPlan**, provisioning service is created as **saasdemo-portal.azurewebsites.net** and publisher application is created as **saasdemo-admin.azurewebsites.net**
1. Fill out the Tenant ID, AD Application ID and secret, accept the terms and click **Purchase** to start the deployment
2. Deploy the packages to the web apps using Azure CLI by following these steps after the deployment of web apps is completed
    
    - While you are on the Azure Portal, click on Azure CLI button in the top bar
    ![Launch Azure CLI](./images/azure-cli-button.png)
    - At the bottom of the window, a pane gets enabled with a request to choose the command shell. Click **Powershell** to proceed
    ![Azure CLI Powershell](./images/welcome-azure-cli.png)
    - Click **Create Storage** button for the Azure CLI to create a storage account to store media
    ![Azure CLI Storage](./images/azure-cli-create-storage.png)
    - Run the following commands in sequence to download and publish packages to web applications
    ```powershell
    azcopy copy https://msampbuilddbresources.blob.core.windows.net/msampcontainer/ProvisioningPortal.zip .

    Publish-AzWebapp -ResourceGroupName saas-demo-rg -Name saasdemo-portal  -ArchivePath ProvisioningPortal.zip

    azcopy copy https://msampbuilddbresources.blob.core.windows.net/msampcontainer/PublisherPortal.zip .

    Publish-AzWebapp -ResourceGroupName saas-demo-rg -Name saasdemo-admin -ArchivePath PublisherPortal.zip
    ```
  
  3. Navigate to the web application -> Configuration and add an item to connection strings with the detail to connect to the database. The below screenshot illustrates the place where the setting has to be added.
  ![Connection string](./images/webapp-connection-string.png)

> Note: DefaultConnection should be added to both the portals 

### Manual deployment using VS 2019

1. Open the solution in **Visual Studio 2019** and open **Solution Explorer**. Right click on **SaaS.SDK.CustomerProvisioning** Project and click **Publish ...**
![AllServices](./images/project-publish-menu.PNG)

2. Click **Import Profile ...** to browse and select the publish profile that was downloaded earlier
3. Click **Publish** to deploy the web application to Azure App Service
![AllServices](./images/VSPublishProfile.PNG).

4. Navigate to the  **URL (Instance Name)** to validate the deployment

> Note: The steps to set up the Publisher solution - **SaaS.SDK.PublisherSolution** locally are identical to the steps to set up the marketplace provisioning service.

---

### Landing page and Webhook settings in the Marketplace Offer

Suppose the names of the web applications deployed to Azure are as follows:
**Provisioning Service** - https://saaskit-portal.azurewebsites.net
**Publisher Application** - https://saaskit-admin.azurewebsites.net

The **Technical Configuration** section of the Marketplace offer with the values filled using the web app names would look like as shown here.

![Technical Configuration](./images/offer-technical-configuration.png)

|Field | Value |
|--|--|
|Landing page URL | Path to the Provisioning Service. Eg: https://saaskit-portal.azurewebsites.net
|Connection webhook | Path to the web hook API in the Provisioning Service. Eg: https://saaskit-portal.azurewebsites.net/api/AzureWebhook
|Azure Active Directory Tenant ID | Tenant where the AD application is created and configured to have the redirect URIs as explained above.
|Azure Active Directory Application ID | ID of the AD application with the redirect URIs configured as explained above

---

### Purchase the offer
 
Assuming that the SaaS offer was published and is available in the known tenants, follow the steps to try out a purchase against your SaaS offer.

1. Log on to [Azure](https://portal.azure.com) 
2. Click **All Services** menu option on the left.

![AllServices](./images/All-Services.png)

3. Search for resources of type **Software as a Service**. The page enlists all the SaaS offers that were previously purchased.
![SaaS Subscriptions](./images/CloudSaasOfferList.png)

4. Click **Add** to proceed to purchase a new SaaS offer.

> If you don't have prior subscriptions against SaaS offers, the list would be blank and you would get an option to **Create Software as a Service** button to help you proceed with the purchase.
![Create SaaS Subscription](./images/Create-SaaS-resource.png)

5. Clicking **Add** ( or **Create Software as a Service**) leads you to a page that lists down SaaS offers available for purchase.

6. Search for **Cloud SaaS** and locate our SaaS offer in the listing
![AMP SDK Sample Offer](./images/Search-Results-SaaS.png)

7. Click on the tile to view the details of the offer
![AMP SDK Sample Offer detail](./images/SaaS-Offer-Detail.png)

8. **Select a software plan** and click **Create**
9. Fill out the form and click **Subscribe**
![AMP SDK Sample Offer](./images/Subscribe-to-Plan.png)

- A new resource gets created and appears in the listing
![SaaS Subscriptions](./images/CloudSaasOfferList.png)

10. Click the text under **Name** to view the details of the resource
11. Click **Configure Account** option in the header bar. You will now be redirected to the SaaS offer landing page offered by the **AMP SDK Sample Client Application** in a new tab / window

The landing page presents the details of the offer that was purchased with an option to **Activate** the subscription.

> In a real scenario, the landing page would collect additional details relevant for provisioning the target SaaS application.

### Activate

> The below diagram illustrates the flow of information between Azure and the Azure marketplace SDK client application.
![Information flow between Azure and Provisioning application](https://docs.microsoft.com/en-us/azure/marketplace/partner-center-portal/media/saas-post-provisioning-api-v2-calls.png)

On the landing page, review the details presented and click **Activate**
![SaaS Subscriptions](./images/activate-subscription.png)

> The AMP SDK sample application calls the following AMP SDK API methods in the background

```csharp
// Determine the details of the offer using the marketplace token that is available in the URL during the redirect from Azure to the landing page.
Task<ResolvedSubscriptionResult> ResolveAsync(string marketPlaceAccessToken);

// Activates the subscription to trigger the start of billing 
Task<SubscriptionUpdateResult> ActivateSubscriptionAsync(Guid subscriptionId, string subscriptionPlanID);

```

- Upon successful activation of the subscription, the landing page switches to a view that enlists the subscriptions against the offer. 
> You can switch to Azure and note that the **Configure Account** button is replaced by **Manage Account** button indicating that the subscription has been materialized.

> **Note** If activation workflow is enabled, by turning on the flag - **IsAutomaticProvisioningSupported** in the ApplicationConfiguration table, the application would put the subscription in PendingActivation status and the Fulfillment API to activate the subscription is not called. Publisher has the option to activate the subscription via the action menu in the subscription listing in the Publisher Portal.

### Change plan

> The below diagram illustrates the flow of information between Azure and the Azure marketplace SDK client application.
![Update subscription](https://docs.microsoft.com/en-us/azure/marketplace/partner-center-portal/media/saas-update-api-v2-calls-from-saas-service-a.png)
1. Log on to [AMP SDK sample application]().
2. Click **Subscriptions** from the menu on the top, in case you are not on the page that shows you the list of subscriptions. The table on this page enlists all the subscriptions and their status.
3. Click **Change Plan** option in the dropdown menu that appears when the icon under the **Actions** column against any of the active subscriptions is clicked.
![SaaS Subscriptions](./images/customer-subscriptions.png)
A popup appears with a list of plans that you can switch to.

4. Select a desired plan and click **Change Plan**.
![SaaS Subscriptions](./images/change-plan.png)

> The AMP SDK sample application calls the following AMP SDK API methods in the background

```csharp
// Initiate the change plan process
Task<SubscriptionUpdateResult> ChangePlanForSubscriptionAsync(Guid subscriptionId, string subscriptionPlanID);

```
>The operation is asynchronous and the call to **change plan** comes back with an operation location that should be queried for status.

```csharp
// Get the latest status of the subscription due to an operation / action.
Task<OperationResult> GetOperationStatusResultAsync(Guid subscriptionId, Guid operationId);
```

> **Note** If activation workflow is enabled, by turning on the flag - **IsAutomaticProvisioningSupported** in the ApplicationConfiguration table, the option to **Change Plan** is disabled for customers. Publisher has the option to change the plan of the subscription via the action menu in the subscription listing in the Publisher Portal.

### Unsubscribe

1. Log on to AMP SDK sample application.
2. Click **Subscriptions** from the menu on the top, in case you are not on the page that shows you the list of subscriptions. The table on this page enlists all the subscriptions and their status.
3. Click **Unsubscribe** against an active subscription.
![SaaS Subscriptions](./images/unsubscribe.png)
4. Confirm your action to trigger the deletion of the subscription.

> The AMP SDK sample application calls the following AMP SDK API methods in the background.

```csharp
// Initiate the delete subscription process
Task<SubscriptionUpdateResult> DeleteSubscriptionAsync(Guid subscriptionId, string subscriptionPlanID);
```

> The operation is asynchronous and the call to **change plan** comes back with an operation location that should be queried for status.

```csharp
// Get the latest status of the subscription due to an operation / action.
Task<OperationResult> GetOperationStatusResultAsync(Guid subscriptionId, Guid operationId);
```
> **Note** If activation workflow is enabled, by turning on the flag - **IsAutomaticProvisioningSupported** in the ApplicationConfiguration table, the option to **Unsubscribe** is disabled for customers. Publisher has the option to delete the subscription via the action menu in the subscription listing in the Publisher Portal.

### Change Quantity

1. Log on to [AMP SDK sample application]().
2. Click **Subscriptions** from the menu on the top, in case you are not on the page that shows you the list of subscriptions. The table on this page enlists all the subscriptions and their status.
3. Click **Change quantity** in the menu as shown in the below picture
![Change quantity](./images/change-quantity-menu.png)

4. Provide the new quantity and click **Change Quantity** to update the quantity on the subscription
![Update quantity](./images/update-quantity-popup.png)

> Note: The update to quantity is applicable if only the subscription is against a Plan that is set to be billed per user
  ![Per user pricing](./images/per-user-plan-pricing.png)

> The AMP SDK sample application calls the following AMP SDK API methods in the background.

```csharp
Task<SubscriptionUpdateResult> ChangeQuantityForSubscriptionAsync(Guid subscriptionId, int? subscriptionQuantity);
```
> The operation is asynchronous and the call to **change plan** comes back with an operation location that should be queried for status.
```csharp
// Get the latest status of the subscription due to an operation / action.
Task<OperationResult> GetOperationStatusResultAsync(Guid subscriptionId, Guid operationId);
```

**Update Plan to indicate per user pricing**

Use the following script as an example / template to update the records in **Plans**

```sql
UPDATE Plans SET IsPerUser = 1 WHERE PlanId = '<ID-of-the-plan-as-in-the-offer-in-partner-center>'
```

The Plan ID is available in the **Plan overview** tab of the offer as shown here:

![Plan ID](./images/plan-id-for-metering.png)

### View activity log

1. Log on to [AMP SDK sample application]().
2. Click **Subscriptions** from the menu on the top, in case you are not on the page that shows you the list of subscriptions.
3. The table on this page enlists all the subscriptions and their status.
4. Click **Activity Log** to view the log of activity that happened against the subscription.
 ![SaaS Subscriptions](./images/activity-log-menu.png)
 ![SaaS Subscriptions](./images/activity-log-popup.png)

### Go to SaaS application

1. Log on to [AMP SDK sample application]().
2. Click **Subscriptions** from the menu on the top, in case you are not on the page that shows you the list of subscriptions.
3. The table on this page enlists all the subscriptions and their status.
4. Click **SaaSApp** from options menu under **Actions** to navigate to the target SaaS application.
![SaaS Subscriptions](./images/saas-app-menu.png)

## SaaS metering service

The **SaaS metering service** is the web application that helps ISVs to look at the subscriptions against the marketplace offer.

![List of subscriptions](./images/subscriptions-manage-usage.png)

For subscriptions against the plans that support metered billing, a button is enabled to post usage events against the subscription.

> Only one usage event is accepted for the hour interval. The hour interval starts at minute 0 and ends at minute 59. If more than one usage event is emitted for the same hour interval, any subsequent usage events are dropped as duplicates.

> Usage can be emitted with a delay and the maximum delay allowed between is 24 hours.
The usage / consumption is consolidated

### Emit usage events

The following interface in the **Saas metering service** allows the user to manual report the usage against a selected dimension.

> In this example, suppose the SaaS service is offering a notification service that helps its customers send out emails / text. Email and Text are modeled as dimensions and the plan in the marketplace offer captures the definition for charges by these dimensions.

![Report usage](./images/post-usage-event.png)

> Note: The option - Manage Usage is available against active subscriptions against a plan that supports metering. You are required to manually update the Plan record in the database to indicate that it supports metering. Besides, the meters for the plan should be initialized in the **MeteredDimensions** table
 
**Update Plan to indicate support for metering**

Use the following script as an example / template to update the records in **Plans**

```sql
UPDATE Plans SET IsmeteringSupported = 1 WHERE PlanId = '<ID-of-the-plan-as-in-the-offer-in-partner-center>'
```
The Plan ID is available in the **Plan overview** tab of the offer as shown here:

![Plan ID](./images/plan-id-for-metering.png)

**Initialize meters for plan**

Use the following script as an example / template to initialize meters in **MeteredDimensions** table

```sql
INSERT INTO MeteredDimensions ( Dimension, PlanId, Description, CreatedDate)
SELECT '<dimension-as-in-partner-center', '<id-of-the-plan>', '<description>', GETDATE()
```

The **Dimension** in the above example should be the attribute of a meter in the plan as shown in the below image:
![Meter dimension](./images/meter-dimension.png)


> The SaaS metering service calls the below API to emit usage events
```csharp
/// <summary>
/// Emits the usage event asynchronous.
/// </summary>
/// <param name="usageEventRequest">The usage event request.</param>
/// <returns></returns>
Task<MeteringUsageResult> EmitUsageEventAsync(MeteringUsageRequest usageEventRequest);
```

The service tracks the requests sent and the response received from the marketplace metering APIs for auditing purposes.

## License Manager

The license management feature in the SaaS metering service allows the Publisher to assign licenses to the active subscriptions. 
The intent here is to illustrate how the assignment can be done via the interface and how the customer user can consume this detail via the **SaaS Provisioning** application

### Publisher: Manage Licenses

1. Log on to [SaaS Metering Service application]()
2. Click **Licenses** menu at the top to view the list of subscriptions and licenses.
3. There is an option to **Revoke** an active license and **Activate** an already revoked license.
![View Licenses](./images/publisher-add-revoke-license.png)
4. Select a subscription, enter license key detail and hit **Add License** to assign a license.
![Add License](./images/publisher-add-revoke-license.png)


### Customer: View Licenses

1. Log on to [AMP SDK Sample application]()
2. Click **Licenses** menu at the top to view the list of subscriptions and licenses.
3. Use the **Copy** button to copy the license text to clipboard
![View Licenses](./images/customer-view-licenses.png)


## Troubleshooting issues

The Provisioning servie and the Publisher solution are configured to log the activity to console ( when running locally ). The logs are available via **Log Stream** when the applications are running in Azure as app services.
Logs in Azure can be viewed by following the below steps:

1. Log on to https://portal.azure.com
2. Navigate to the app service 
3. Click **App Service logs** and set the parameters as shown here:

![App service logs](./images/azure-application-logging.png)

4. Click **Log Stream** in the menu on the left to look at the logs output by the application. You could see the view refreshing every minute with the latest log information due to the activity in the application as you access the application in another browser window.

- You can download the logs from the FTP URL that is available in the **App Service Logs** interface.
- The credentials to access the FTP location are available in the **Publish Profile** of the web application.


 
