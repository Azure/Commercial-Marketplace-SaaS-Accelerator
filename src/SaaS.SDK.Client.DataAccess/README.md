# Client Data Access library

## Introduction

Enables persistence of plans, subscription, and other transaction data used with the **[SaaS Fulfillment API (v2)](https://docs.microsoft.com/en-us/azure/marketplace/partner-center-portal/pc-saas-fulfillment-api-v2)** and **[Marketplace Metering Service API](https://docs.microsoft.com/en-us/azure/marketplace/partner-center-portal/marketplace-metering-service-apis)**.	

## Description

The project contains the repositories to manage entities in the database.

Entities that are persisted in the Database

| Entity Name | Description |
| --- | --- |
| Application Log | Tracks the Customer Web Application Sample activity |  
| Metered Dimensions | Tracks the metered information that will be submitted to the Marketplace Metering Service API. The metered data is submitted by the Publisher. In the SDK, there is an interface in the Publisher Sample web application that will help generate this data.  | 
| Plans | Stores the SaaS Offer plan information. Plans are part of the SaaS offer and submitted via Partner Center. This table is populated after calls to the Fulfillment API|  
| Subscription Audit Log | This table is an audit trail for all the operations regarding Subscriptions. |  
| Subscriptions | Tracks the Offer's Subscriptions. Associates Customers (users) with their selected Plan|  
| Users | Tracks the list of Customers (users that created subscriptions against the offer) |  
| Application Configuration | Tracks the configurations required for SMTP and emails. |
| Database Version History | Tracks the database script versions. |
| Email Template | Tracks the emails templates html. |
| Events | Tracks the list of events available. |
| Known Users | Tracks the users that have access to login to the admin portal. |
| Metered Audit Logs | Tracks all the metering data posted to metering service API on a subscription. |
| Offers | Stores the SaaS offers information. |
| Offer Attributes | Tracks the list of all Custom fields that to be filled by customer after purchasing a subscription. |
| Plan Attribute Mapping | Tracks the list of Custom fields mapped per plan. |
| Plan Events Mapping | Tracks the events and email ids mapped per plan. |
| Roles | Tracks available roles in the system. |
| Subscription Attribute Values | Tracks the data filled by customer after purchasing the subscription. |
| Value Types | Tracks all the data types available to create offer parameters. |
| Web Job Subscription Status | Tracks the activity done by the web job. |
| Plan Attribute Output | Does not hold any data, used to map the result of the stored procedure spGetOfferParameters. |
| Plan Events Output | Does not hold any data, used to map the result of the stored procedure spGetPlanEvents. |

[Click here](Transactable-SaaS-SDK-Sample-Database.md) for more details on the sample database.

## Source Code

The Project is located in the **SaaS.SDK.Client.DataAccess** folder.
The project is composed of the following sections:

| Section Name | Description |
| --- | --- |  
| Dependencies | EntityFrameworkCore,  EntityFrameworkCore.SqlServer|
| Context | EntityFramework database context |
| Contracts | Interface that defines the methods to manage the entities |
| Entities | Table entities |
| Services | Implementations for the interfaces to manage the persistence and query over entities |
