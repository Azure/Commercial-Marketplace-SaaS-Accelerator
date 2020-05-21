# Projects

| Project | Description | Directory Name |
| --- | --- | --- |
| **[Transactable SaaS client library](./src/SaaS.SDK.Client)** | Implements the SaaS Fulfillment API (v2), Marketplace Metering Service API and the webhook that handles messages from the Marketplace billing system. |SaaS.SDK.Client|
| **[Customer portal - Sample web application](./src/SaaS.SDK.CustomerProvisioning)** | Demonstrates how to register, provision, and activate the Marketplace subscription. Implemented using ASP.Net Core 3.1, the sample web application uses the SaaS Client Library and Data Access Library to invoke and persist API interactions and provides an example user interface to demonstrate how a customer would manage their subscriptions and plans. |SaaS.SDK.CustomerProvisioning|
| **[Publisher portal - Sample web application](./src/SaaS.SDK.PublisherSolution)** | Demonstrates how to generate usage events used in metered billing transactions and how to emit these events to the Marketplace Metering Service API. |SaaS.SDK.PublisherSolution|
| **[Client data access library](./src/SaaS.SDK.Client.DataAccess)** | Demonstrates how to persist Plans, Marketplace subscriptions, and related transaction attributes when using the SaaS Fulfillment API (v2) and Marketplace Metering Service API. |SaaS.SDK.Client.DataAccess |
| **[Services client library](./src/SaaS.SDK.Services)** | The project contains the service layer that is leveraged by the Customer portal and Publisher portal. It includes the POCO classes for the solution. |SaaS.SDK.Services |
| **[Unit tests project](./src/SaaS.SDK.UnitTest)** | Helps validate and test the SDKs codebase. | SaaS.SDK.UnitTest |
