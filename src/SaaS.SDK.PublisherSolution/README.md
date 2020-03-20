# Publisher sample web application	

### Introduction

Showcases how to generate metered based transactions, persistence of those transactions and transmission of these transactions to the metered billing API.

### Description

The web application is for publisher to help them perform the following actions:

- Submit metered usage against subscriptions
- Assign / revoke licenses to / from subscriptions

### Source Code 

The Project is located in the **SaaS.SDK.PublisherSolution** folder. The project is composed of the following sections: 

| Section Name | Description |
| --- | --- |  
| Dependencies | Microsoft AspNet Core OpenID Authentication, EntityFramework and Logging extensions, SaaS.SDK.Client |
| Controllers | ASP.Net Core MVC controllers that are responsible to provide data  / views and handle the data posted back by the user |  
| Models | POCO classes for transferring data between the client and the endpoints | 
| Utilities | Constants and classes with utility methods | 
| Views | User interface components | 
