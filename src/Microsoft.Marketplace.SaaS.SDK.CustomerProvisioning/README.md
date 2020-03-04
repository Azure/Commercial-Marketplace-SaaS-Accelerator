# Customer provisioning sample web application

### Introduction

Showcases how to provision a customer (ASP.NET Core 3.1) that uses the SDK to invoke fulfillment APIs in order to manage the subscriptions against the SaaS offer in Azure.	

### Description

The web application demonstrates the end-user experience covering the scenarios - activation of a subscription, change plan and unsubscribe.
The application also has a viewer over the licenses assigned by the publisher against subscriptions.

### Source Code

The Project is located in the ** Microsoft.Marketplace.SaaS.SDK.CustomerProvisioning** folder. The project is composed of the following sections: 

| Section Name | Description |
| --- | --- |  
| Dependencies | Microsoft AspNet Core OpenID Authentication, EntityFramework and Logging extensions, Microsoft.Marketplace.SaaS.SDK.Client  |
| Controllers | ASP.Net Core MVC controllers that are responsible to provide data  / views and handle the data posted back by the user | 
| Database | Scripts to set up and initialize the database |
| Models | POCO classes for transferring data between the client and the endpoints | 
| Services | Facade classes that orchestrate calls to the Fullfillment V2 and Metered APIs and read / write operations to database| 
| Utilities | Constants and classes with utility methods | 
| Views | User interface components |
| WebHook | Controller that handles the notifications from Azure due to user action on the SaaS subscriptions | 
