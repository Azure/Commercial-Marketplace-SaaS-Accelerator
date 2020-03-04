# Microsoft SaaS SDK Solution


### Introduction

The Transactable SaaS SDK's solution contains the following components:


| Project | Description | Directory Name |
| --- | --- | --- |
|  **Transactable SaaS Client Library** |Implements the fulfillment v2 and metered APIs and the Web-hook that handles messages from the Marketplace's E-commerce engine. |Microsoft.Marketplace.SaaS.SDK.Client|
| **Customer provisioning sample web application** | Showcases how to provision a customer (ASP.NET Core 3.1) that uses the SDK to invoke fulfillment APIs in order to manage the subscriptions against the SaaS offer in Azure. This project also includes the script for the **[Sample Database](./Microsoft.Marketplace.SaaS.SDK.CustomerProvisioning/Database/README.md)** |Microsoft.Marketplace.SaaS.SDK.CustomerProvisioning|
| **Publisher sample web application** | Showcases how to generate metered based transactions, persistence of those transactions and transmission of these transactions to the metered billing API. |Microsoft.Marketplace.SaaS.SDK.PublisherSolution|
| **Client Data Access library** | Enables to persist the Plans, Subscriptions, and transactions with the fulfillment and Metered APIs. |Microsoft.Marketplace.SaaS.SDK.Client.DataAccess |
| **[Unit Tests project](./Microsoft.Marketplace.SaaS.SDK.UnitTest/README.md)** | Helps validate and test the SDK's codebase. | Microsoft.Marketplace.SaaS.SDK.UnitTest |




