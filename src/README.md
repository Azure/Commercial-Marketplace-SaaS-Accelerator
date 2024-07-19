# Projects

The following components are included in this directory:

| Project | Description | Directory Name |
| --- | --- | --- |
| [**Customer portal - Sample web application**](./CustomerSite) | Demonstrates how to register, provision, and activate the marketplace subscription. Implemented using ASP.Net Core 8.0, the sample web application uses the Services client library and data access library to invoke and persist API interactions and provides an example user interface to demonstrate how a customer would manage their subscriptions and plans. |CustomerSite |
| [**Publisher portal - Sample web application**](./AdminSite) | Demonstrates how to generate usage events used in metered billing transactions, and how to emit these events to the Marketplace Metering Service API. | AdminSite |
| [**Client data access library**](./DataAccess) | Demonstrates how to persist plans, marketplace subscriptions, and related transaction attributes when using the SaaS Fulfillment API (v2) and Marketplace Metering Service API. | DataAccess |
| [**Services client library**](./Services) | Contains the services used by the Customer and Publisher portals, including the POCO classes to orchestrate calls to the marketplace APIs on [client library](https://github.com/microsoft/commercial-marketplace-client-dotnet) / database.| Services |
| [**Unit tests project**](./src/Service.UnitTest) | Helps validate and test the Services codebase. | Services.UnitTest |
