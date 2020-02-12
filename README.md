# Introduction

This SDK's goal is to provide guidance and the components needed for Publishers (ISVs and StartUps) to create transactable Software as a-Service (SaaS) offers in the Azure and AppSource marketplaces.  

The SDK provides the components required for the implementations of the billing (fulfillment v2 and metered) APIs, and additional components that showcase how to build a customer provisioning interface, logging, and administration of the customer's subscriptions. These are the core projects in the SDK:  

- **Transactable SaaS Client Library** that implements the fulfillment v2 and metered APIs (.NET Core 3.1 LTS) and the Web-hook that handles messages from the Marketplace's E-commerce engine.
- **Customer provisioning sample web application** that showcases how to register, provision, and activate the customer subscription. Implemented using ASP.Net Core 3.1, it uses the SaaS Client library and Data Access Library to to invoke and persists interactions with the fulfillment APIs. In addition, it provides interfaces for a customer to manage their subscriptions and plans. 
- **Publisher sample web application** that showcases how to generate metered based transactions, persistence of those transactions and transmission of these transactions to the metered billing API. 
- **Client Data Access library** that demonstrates how to persist the Plans, Subscriptions, and transactions with the fulfillment and Metered APIs.


### Documentation 

The Documentation (doc) repository contains: 

- **[Installation instructions](./doc/fulfillment-metering-api-client-readme.md)** to help you deploy and implement the SDK  components.  
- **Review of the SaaS Offer BluePrints** the SDK provides extended functionality. In addition, to the implementation . Like using the SaaS offer a way to transact [Virtual Machines](https://docs.microsoft.com/en-us/azure/marketplace/cloud-partner-portal/virtual-machine/cpp-virtual-machine-offer)  and [Azure Application](https://docs.microsoft.com/en-us/azure/marketplace/cloud-partner-portal/azure-applications/cpp-azure-app-offer) offers with the use of software licenses (Scheduled for v.1.1 of this SDK) and for implementing Hybrid SaaS applications (scheduled for v1.2 of this SDK)

### Sources 

The source (src) repository offers the following components: 

- **Transactable SaaS Client Library** that implements the fulfillment v2 and metered APIs (.NET Core 3.1 LTS) and the Web-hook that handles messages from the Marketplace's E-commerce engine.
- **Customer provisioning sample web application** that showcases how to provision a customer (ASP.NET Core 3.1) that uses the SDK to invoke fulfillment APIs in order to manage the subscriptions against the SaaS offer in Azure.
- **Publisher sample web application** that showcases how to generate metered based transactions, persistence of those transactions and transmission of these transactions to the metered billing API. 
- **Client Data Access library** to persist the Plans, Subscriptions, and transactions with the fulfillment and Metered APIs.
- **Unit Tests project** to help validate and test the SDK's codebase. 

The sample and the SDK in this repository cover the components that comprise the highlighted area in the below picture

Azure Marketplace Metering SDK enables SaaS applications publish usage data to Azure so that customers are charged  according to non-standard units. 

The metering SDK ( .NET class library ) and a sample web application to report usage events for subscriptions against those plans that support metering ( have the dimensions defined and enabled ) correlate to SaaS Metering and SaaS Service blocks in the below image, respectively.

![Usecase](./docs/images/UseCaseSaaSAPIs.png)


## Resources

- Details on the fulfillment v2 API can be found [here](https://docs.microsoft.com/en-us/azure/marketplace/partner-center-portal/pc-saas-fulfillment-api-v2#update-a-subscription) 

- Details on the metering APIs can be found [here](https://docs.microsoft.com/en-us/azure/marketplace/partner-center-portal/marketplace-metering-service-apis).

- Steps to create a SaaS offer are available [here](https://docs.microsoft.com/en-us/azure/marketplace/partner-center-portal/create-new-saas-offer)

## Prerequisites

Ensure the following prerequisites are met before getting started:

- We recommend using an Integrated Development Environment (IDE):  [Visual Studio Code](https://code.visualstudio.com/),  [Visual Studio 2019](https://visualstudio.microsoft.com/thank-you-downloading-visual-studio/?sku=Community&rel=16#), etc...
- The SDK has been implemented using [.NET Core 3.1 LTS](https://download.visualstudio.microsoft.com/download/pr/639f7cfa-84f8-48e8-b6c9-82634314e28f/8eb04e1b5f34df0c840c1bffa363c101/dotnet-sdk-3.1.100-win-x64.exe)
- For Persistence we are using [Azure SQL Database](https://azure.microsoft.com/en-us/services/sql-database/) and [Entity Framework](https://docs.microsoft.com/en-us/ef/). However, feel free to use any data repository you are comfortable with.  

Besides, it is assumed that you have access to the following resources:
- Azure subscription - to host the AMP SDK sample client application.
- Partner Center - to create and publish a marketplace offer.

## Roadmap

The following is the proposed roadmap for this SDK: 

- **February 2020 - v1.0** Current Release. It includes the full implementation of the Fulfillment V2 and metered APIs with web applications that demonstrate customer provisioning and publisher solutions. 
- **March 2020 - v1.1** Add the **SaaS Offer as a License Manager Application** blueprint: This is targeted for Publishers that would like to use a combination of two Azure Marketplace offers to transact their SaaS Solution.  The first offer is a SaaS offer that would be use as a mechanism to sell a license for the solution on specific terms (defined by Plan) and a Virtual Machine or an Azure Application offer that will deploy the solution in the Customer Subscription as a BYOL offer.   
- **May 2020 - v1.2** Add the **SaaS Offer that deploys in the Customer's Azure Subscription** (also refereed as the Hybrid Model) blueprint: Targeted to publishers that:
	- Need use the billing capabilities of the SaaS offer for the Virtual Machine and Azure Application (Solution Template) offer
	- Partners that would like to deploy solutions in the customer’s subscriptions that use technologies (VM + Container, Kubernetes, etc…) that are not currently supported by other offers in Marketplace
	- Partners that need to deploy solutions in the customer’s subscriptions that use Azure Services that cannot be fully automated via Azure Resource Manager Deployments (need manual steps to complete the full deployment). 
	- Partners that have a SaaS deployment solution that needs to have both a SaaS Application running into their subscription and Azure resources deployed into the customers Azure subscription. A combination of both is required to enable their solution to work.


# Contributing

This project welcomes contributions and suggestions.  Most contributions require you to agree to a
Contributor License Agreement (CLA) declaring that you have the right to, and actually do, grant us
the rights to use your contribution. For details, visit https://cla.opensource.microsoft.com.

When you submit a pull request, a CLA bot will automatically determine whether you need to provide
a CLA and decorate the PR appropriately (e.g., status check, comment). Simply follow the instructions
provided by the bot. You will only need to do this once across all repos using our CLA.

This project has adopted the [Microsoft Open Source Code of Conduct](https://opensource.microsoft.com/codeofconduct/).
For more information see the [Code of Conduct FAQ](https://opensource.microsoft.com/codeofconduct/faq/) or
contact [opencode@microsoft.com](mailto:opencode@microsoft.com) with any additional questions or comments.
