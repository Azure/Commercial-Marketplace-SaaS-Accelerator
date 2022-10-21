# SaaS SDK Services

## Introduction

The project offers the service layer that is leveraged by the **Customer portal** and **Publisher portal**

## Source Code

The project is located in the **SaaS.SDK.Services** folder. The project is composed of the following sections:

| Section Name | Description |
| --- | --- |  
| Contracts| Interfaces and contracts for the services
| Helpers | Email helper |
| Dependencies | Microsoft.AspNetCore.Mvc.ViewFeatures, Microsoft.Azure.Management.ResourceManager, Microsoft.Extensions.Logging.Console, Microsoft.IdentityModel.Clients.ActiveDirectory and Microsoft.Rest.ClientRuntime.Azure.Authentication|
| Models |  POCO classes - instances participate as parameters / return types from the services
| Services | Implementation of the interfaces |
| Status Handlers | Status handlers to support notifications including: AbstractSubscription, ISubscription, Notification, PendingActivation, PendingFulfillment, Unsubscribe |
| Utilities| Utility classes |
