# Microsoft Commercial Marketplace - Community Sample Code and SDK for SaaS App Offers
![.NET Core](https://github.com/Azure/Microsoft-commercial-marketplace-transactable-SaaS-offer-SDK/workflows/.NET%20Core/badge.svg)


## Introduction

The goal of this project is to provide a reference example with sample code for developers interested publishing transactable, Software as a-Service (SaaS) offers in Microsoft's Commercial Marketplace.

The sample code uses a .NET-based SDK published as a part of this project. The SDK provides a framework for excercising the Commercial Marketplace billing system, including the [SaaS Fulfillment API (v2)](https://docs.microsoft.com/en-us/azure/marketplace/partner-center-portal/pc-saas-fulfillment-api-v2) and [Marketplace Metering Service API.](https://docs.microsoft.com/en-us/azure/marketplace/partner-center-portal/marketplace-metering-service-apis) Leveraging the SDK, the sample code demonstrates how a typical SaaS platform interacts with these APIs in order to provision customers, enable logging, and manage Commercial Marketplace subscriptions.


## Intended Use

The sample code and SDK in this project are for reference purposes only and are meant to complement the existing Commercial Marketplace documentation by demonstrating common API interactions required to use the SaaS App offer type. The intent of this project is to accelerate the onboarding experience by providing code samples for developers. Although the sample code leverages the SDK, developers are encouraged to work with the SaaS Fulfillment API and Marketplace Metering Service API directly rather than rely on the SDK for production use.

Please note: this is not a Microsoft-supported Azure SDK project. Support for this project is community-based and contributions are welcomed. Details on contributing can be found [below.](https://github.com/Azure/Microsoft-commercial-marketplace-transactable-SaaS-offer-SDK#contributing)


## Commercial Marketplace Documentation

Before using this sample code and SDK, please review the Commercial Marketplace documentation resources below to understand the important concepts, account setup, and offer configuration requirements for publishing SaaS App offers.

- [Commercial Marketplace Overview.](https://docs.microsoft.com/en-us/azure/marketplace/marketplace-publishers-guide) Overview of the Commercial Marketplace.

- [SaaS Apps in the Commercial Marketplace.](https://docs.microsoft.com/en-us/azure/marketplace/partner-center-portal/create-new-saas-offer) Overview of the SaaS App business policies, plus step-by step offer creation and configuration requirements.

- [SaaS Fulfillment API (v2).](https://docs.microsoft.com/en-us/azure/marketplace/partner-center-portal/pc-saas-fulfillment-api-v2) API details for SaaS App Marketplace subscription creation and management.

- [Marketplace Metering Service API.](https://docs.microsoft.com/en-us/azure/marketplace/partner-center-portal/marketplace-metering-service-apis) API details for the Marketplace Metering Service which, when used in conjunction with the SaaS Fulfillment API, enables event-based billing. 

- [SaaS Fulfillment API FAQ.](https://docs.microsoft.com/en-us/azure/marketplace/partner-center-portal/saas-fulfillment-apis-faq) Frequently-asked questions about the SaaS Fulfillment APIs.


## Sample Code and SDK Documentation 

The documentation **(docs)** directory contains [installation instructions](./docs/Installation-Instructions.md) to help understand, implement, and deploy the sample code and SDK components. 


## Terminology

- SDK: Software Development Kit. This refers to the SDK for the .NET language and incldues the Client and Data Access Libraries, as well as the Sample Web Applications used to excercise the SaaS Fulfillment API and Marketplace Metering Service API.

- Client Library. This refers to a library (and associated tools, documentation, and samples) that customers/developers use to ease creating Commercial Markeptlace SaaS App offers.

- Sample Web Application. This refers to source code that leverages the SDK and Client Libraries.   


## Projects 

The source **(src)** directory offers the following components: 


| Project | Description | Directory Name |
| --- | --- | --- |
| **Transactable SaaS Client Library** | Implements the SaaS Fulfillment API (v2), Marketplace Metering Service API, and the Web-hook that handles messages from the Marketplace billing system. |SaaS.SDK.Client|
| **Customer Provisioning - Sample web application** | Demonstrates how to register, provision, and activate the Marketplace subscription. Implemented using ASP.Net Core 3.1, the sample web application uses the SaaS Client Library and Data Access Library to invoke and persist API interactions and provides an example user interface to demonstrate how a customer would manage their subscriptions and plans. |SaaS.SDK.CustomerProvisioning|
| **Publisher Solution (metered billing)- Sample web application** | Demonstrates how to generate usage events used in metered billing transactions, and how to emit these events to the Marketplace Metering Service API. |SaaS.SDK.PublisherSolution|
| **Client Data Access library** | Demonstrates how to persist Plans, Marketplace subscriptions, and related transaction attributes when using the SaaS Fulfillment API (v2) and Marketplace Metering Service API. |SaaS.SDK.Client.DataAccess |
| **[Unit Tests Project](./src/SaaS.SDK.UnitTest)** | Helps validate and test the SDKs codebase. | SaaS.SDK.UnitTest |


The sample code and SDK in this repository run in the Publisher's environment as illustrated below. The metering SDK ( .NET class library ) and a sample web application to report usage events for subscriptions against those plans that support metering ( have the dimensions defined and enabled ) correlate to SaaS Metering and SaaS Service blocks in the below image, respectively.

![Usecase](./docs/images/UseCaseSaaSAPIs.png)


## Technology and Versions

This SDK has been developed using the following technologies and versions: 

- [.NET Core 3.1.1](https://dotnet.microsoft.com/download/dotnet-core/3.1)
- [ASP.NET Core Runtime 3.1.1](https://dotnet.microsoft.com/download/dotnet-core/3.1)
- [Entity Framework](https://docs.microsoft.com/en-us/ef/)


## Security
- The sample code and SDK have been scanned for vulnerabilities and use secure configurations. Versions have been reviewed to ensure compatibility with the lastest security guidelines.


## Prerequisites

Ensure the following prerequisites are met before getting started:

- You must have an active Azure subscription for development and testing purposes. Creat an Azure subscription [here.](https://azure.microsoft.com/en-us/free/)
- You must have a Partner Center account enabled for use with the Commercial Marketplace. Create an account [here.](https://docs.microsoft.com/en-us/azure/marketplace/partner-center-portal/create-account)
- We recommend using an Integrated Development Environment (IDE):  [Visual Studio Code](https://code.visualstudio.com/),  [Visual Studio 2019](https://visualstudio.microsoft.com/thank-you-downloading-visual-studio/?sku=Community&rel=16#), etc...
- The SDK has been implemented using [.NET Core 3.1.1](https://dotnet.microsoft.com/download/dotnet-core/3.1)
- For data persistence we are using [Azure SQL Database](https://azure.microsoft.com/en-us/services/sql-database/) and [Entity Framework](https://docs.microsoft.com/en-us/ef/). However, feel free to use any data repository you are comfortable with.  


## Releases

- **February 2020 - v1.0** Current Release. It includes the full implementation of the Fulfillment V2 and metered APIs with web applications that demonstrate customer provisioning and publisher solutions. 


## Contributing

This project welcomes contributions and suggestions.  Most contributions require you to agree to a
Contributor License Agreement (CLA) declaring that you have the right to, and actually do, grant us
the rights to use your contribution. For details, visit https://cla.opensource.microsoft.com.

When you submit a pull request, a CLA bot will automatically determine whether you need to provide
a CLA and decorate the PR appropriately (e.g., status check, comment). Simply follow the instructions
provided by the bot. You will only need to do this once across all repos using our CLA.

This project has adopted the [Microsoft Open Source Code of Conduct](https://opensource.microsoft.com/codeofconduct/).
For more information see the [Code of Conduct FAQ](https://opensource.microsoft.com/codeofconduct/faq/) or
contact [opencode@microsoft.com](mailto:opencode@microsoft.com) with any additional questions or comments.

## License

This project is released under the [MIT License](LICENSE).
