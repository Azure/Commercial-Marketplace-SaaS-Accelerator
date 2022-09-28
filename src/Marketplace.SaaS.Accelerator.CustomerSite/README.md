# Customer provisioning sample web application

## Introduction

Demonstrates how to provision a customer (ASP.NET Core 3.1) using the SDK to invoke the APIs to create and manage marketplace SaaS subscriptions.

## Description

The web application demonstrates the end-user experience covering the following scenarios: activation of a subscription, change plan, and unsubscribe. The application also has a view of the licenses assigned by the publisher against subscriptions.

## Source Code

The Project is located in the **Marketplace.SaaS.Accelerator.CustomerSite** folder. The project is composed of the following sections:

| Section Name | Description |
| --- | --- |  
| Dependencies | Microsoft AspNet Core OpenID Authentication, EntityFramework and Logging extensions.  |
| Controllers | ASP.Net Core MVC controllers that are responsible to provide data  / views and handle the data posted back by the user |
| Database | Scripts to set up and initialize the database |
| Models | POCO classes for transferring data between the client and the endpoints |
| Services | Facade classes that orchestrate calls to the Fullfillment V2 and metering service APIs and read / write operations to database|
| Utilities | Constants and classes with utility methods |
| Views | User interface components |
| WebHook | Controller that handles the notifications from Azure due to user action on the SaaS subscriptions |
