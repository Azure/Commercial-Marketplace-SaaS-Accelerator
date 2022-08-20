# Projects
The following components are included in this directory:

| Project | Description | Directory Name |
| --- | --- | --- |
| [**Customer portal - Sample web application**](./SaaSAccelerator.CustomerSite) | Demonstrates how to register, provision, and activate the marketplace subscription. Implemented using ASP.Net Core 6.0, the sample web application uses the Services client library and data access library to invoke and persist API interactions and provides an example user interface to demonstrate how a customer would manage their subscriptions and plans. |SaaSAccelerator.CustomerSite|
| [**Publisher portal - Sample web application**](./SaaSAccelerator.PublisherSite) | Demonstrates how to generate usage events used in metered billing transactions, and how to emit these events to the Marketplace Metering Service API. |SaaSAccelerator.PublisherSite|
| [**Client data access library**](./SaaSAccelerator.DataAccess) | Demonstrates how to persist plans, marketplace subscriptions, and related transaction attributes when using the SaaS Fulfillment API (v2) and Marketplace Metering Service API. |SaaSAccelerator.DataAccess |
| [**Services client library**](./SaaSAccelerator.Services) | Contains the services used by the Customer and Publisher portals, including the POCO classes to orchestrate calls to the marketplace APIs on [client library](https://github.com/microsoft/commercial-marketplace-client-dotnet) / database.|SaaSAccelerator.Services |
| [**Unit tests project**](./src/SaaSAccelerator.UnitTest.Services) | Helps validate and test the Accelerator's services. | SaaSAccelerator.UnitTest.Services |
