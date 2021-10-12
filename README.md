# Microsoft Commercial Marketplace - Community Sample Code and SDK for SaaS Applications

![.NET Core](https://github.com/Azure/Microsoft-commercial-marketplace-transactable-SaaS-offer-SDK/workflows/.NET%20Core/badge.svg)

## Introduction

The goal of this project is to provide a reference example with sample code for developers interested in publishing transactable, Software as a-Service (SaaS) offers in Microsoft's commercial marketplace.

The sample code uses a .NET-based SDK published as a part of this project. The SDK provides a framework for excercising the commercial marketplace billing system, including the [SaaS Fulfillment API (v2)](https://docs.microsoft.com/en-us/azure/marketplace/partner-center-portal/pc-saas-fulfillment-api-v2) and [Marketplace Metering Service API.](https://docs.microsoft.com/en-us/azure/marketplace/partner-center-portal/marketplace-metering-service-apis) Leveraging the SDK, the sample code demonstrates how a typical SaaS platform interacts with the marketplace APIs in order to provision subscriptions for customers, enable logging, and manage commercial marketplace subscriptions.

## Intended Use

The sample code and SDK in this project are for reference purposes only and are meant to complement the existing commercial marketplace documentation by demonstrating common API interactions required for publishing and managing SaaS application offers. The intent of this project is to accelerate the onboarding experience by providing code samples for developers. Although the sample code leverages the SDK, developers are encouraged to work with the SaaS Fulfillment API and Marketplace Metering Service API directly rather than rely on the SDK for production use.

Please note: this is not a Microsoft-supported Azure SDK project. Support for this project is community-based and contributions are welcome. Details on contributing can be found [below.](https://github.com/Azure/Microsoft-commercial-marketplace-transactable-SaaS-offer-SDK#contributing)

## Commercial Marketplace Documentation

Before using this sample code and SDK, please review the commercial marketplace documentation resources below to understand the important concepts, account setup, and offer configuration requirements for publishing SaaS SaaS application offers.

- [Commercial marketplace documentation.](https://docs.microsoft.com/azure/marketplace/) Getting started and top articles

- [SaaS applications in the commercial marketplace.](https://docs.microsoft.com/azure/marketplace/partner-center-portal/create-new-saas-offer) Overview of the SaaS SaaS application business policies, plus step-by step offer creation and configuration requirements.

- [SaaS fulfillment API (v2).](https://docs.microsoft.com/azure/marketplace/partner-center-portal/pc-saas-fulfillment-api-v2) API details for SaaS SaaS application subscription creation and management.

- [Marketplace metering service API.](https://docs.microsoft.com/azure/marketplace/partner-center-portal/marketplace-metering-service-apis) API details for the Marketplace Metering Service which, when used in conjunction with the SaaS Fulfillment API, enables event-based billing.

- [SaaS fulfillment API FAQ.](https://docs.microsoft.com/azure/marketplace/partner-center-portal/saas-fulfillment-apis-faq) Frequently-asked questions about the SaaS Fulfillment APIs.

## Installation

The documentation **(docs)** directory contains [installation instructions](./docs/Installation-Instructions.md) to help understand, implement, and deploy the sample code and SDK components.

## Terminology

- SDK: Software development kit. This refers to the SDK for the .NET language and incldues the Client and Data Access Libraries, as well as the Sample Web Applications used to excercise the SaaS Fulfillment API and Marketplace Metering Service API.

- Client library. This refers to a library (and associated tools, documentation, and samples) that customers/developers use to ease creating commercial marketplace SaaS SaaS application offers.

- Sample web application. This refers to source code that leverages the SDK and Client Libraries.

## Projects

The source **(src)** directory offers the following components:

| Project | Description | Directory Name |
| --- | --- | --- |
| [**Customer portal - Sample web application**](./src/SaaS.SDK.CustomerProvisioning) | Demonstrates how to register, provision, and activate the marketplace subscription. Implemented using ASP.Net Core 3.1, the sample web application uses the Services client library and data access library to invoke and persist API interactions and provides an example user interface to demonstrate how a customer would manage their subscriptions and plans. |SaaS.SDK.CustomerProvisioning|
| [**Publisher portal - Sample web application**](./src/SaaS.SDK.PublisherSolution) | Demonstrates how to generate usage events used in metered billing transactions, and how to emit these events to the Marketplace Metering Service API. |SaaS.SDK.PublisherSolution|
| [**Client data access library**](./src/SaaS.SDK.Client.DataAccess) | Demonstrates how to persist plans, marketplace subscriptions, and related transaction attributes when using the SaaS Fulfillment API (v2) and Marketplace Metering Service API. |SaaS.SDK.Client.DataAccess |
| [**Services client library**](./src/SaaS.SDK.Services) | Contains the services used by the Customer and Publisher portals, including the POCO classes to orchestrate calls to the marketplace APIs on [client library](https://github.com/microsoft/commercial-marketplace-client-dotnet) / database.|SaaS.SDK.Services |
| [**Unit tests project**](./src/SaaS.SDK.UnitTest) | Helps validate and test the SDKs codebase. | SaaS.SDK.UnitTest |

The sample code and SDK in this repository run in the Publisher's environment as illustrated below. The metering SDK ( .NET class library ) and a sample web application to report usage events for subscriptions against those plans that support metering ( have the dimensions defined and enabled ) and correlate to SaaS Metering and SaaS Service blocks in the below image, respectively.

![Usecase](./docs/images/sdk_overview.png)

## Technology and Versions

This SDK has been developed using the following technologies and versions:

- [.NET Core 3.1.1](https://dotnet.microsoft.com/download/dotnet-core/3.1)
- [ASP.NET Core Runtime 3.1.1](https://dotnet.microsoft.com/download/dotnet-core/3.1)
- [Entity Framework](https://docs.microsoft.com/ef/)

## Security

- The sample code and SDK have been scanned for vulnerabilities and use secure configurations. Versions have been reviewed to ensure compatibility with the lastest security guidelines.

## Prerequisites

Ensure the following prerequisites are met before getting started:

- You must have an active Azure subscription for development and testing purposes. Create an Azure subscription [here.](https://azure.microsoft.com/free/)
- You must have a Partner Center account enabled for use with the commercial marketplace. Create an account [here.](https://docs.microsoft.com/azure/marketplace/partner-center-portal/create-account)
- We recommend using an Integrated Development Environment (IDE):  [Visual Studio Code](https://code.visualstudio.com/),  [Visual Studio 2019](https://visualstudio.microsoft.com/thank-you-downloading-visual-studio/?sku=Community&rel=16#), etc...
- The SDK has been implemented using [.NET Core 3.1.1](https://dotnet.microsoft.com/download/dotnet-core/3.1)
- For data persistence we are using [Azure SQL Database](https://azure.microsoft.com/services/sql-database/) and [Entity Framework](https://docs.microsoft.com/ef/). However, feel free to use any data repository you are comfortable with.  

## Releases

- **February 2020 - v1.0** Current Release. It includes the full implementation of the Fulfillment V2 and metering service APIs with web applications that demonstrate customer and publisher portals.

## Contributing

This project welcomes contributions and suggestions.  Most contributions require you to agree to a
Contributor License Agreement (CLA) declaring that you have the right to, and actually do, grant us
the rights to use your contribution. For details, visit <https://cla.opensource.microsoft.com.>

When you submit a pull request, a CLA bot will automatically determine whether you need to provide
a CLA and decorate the PR appropriately (e.g., status check, comment). Simply follow the instructions
provided by the bot. You will only need to do this once across all repos using our CLA.

This project has adopted the [Microsoft Open Source Code of Conduct](https://opensource.microsoft.com/codeofconduct/).
For more information see the [Code of Conduct FAQ](https://opensource.microsoft.com/codeofconduct/faq/) or
contact [opencode@microsoft.com](mailto:opencode@microsoft.com) with any additional questions or comments.

### Developers

Code contributed should follow the C# specifications and best practices as documented [here](https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/inside-a-program/coding-conventions).

## License

This project is released under the [MIT License](LICENSE).
