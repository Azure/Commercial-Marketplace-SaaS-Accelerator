# Client Data Access library

### Introduction

Enables persistence of plans, subscription, and other transaction data used with the SaaS Fulfillment API (v2) and Marketplace Metering Service API.	

### Description

The project contains the repositories to manage entities in the database.

Entities that are persisted in the Database

| Entity Name | Description |
| --- | --- | 
| Application Log | Tracks the Customer Web Application Sample activity |  
| Metered Dimensions | Tracks the metered information that will be submitted to the Billing API. The metered data is submitted by the Publisher. In the SDK, there is an interface in the Publisher Sample web application that will help generate this data.  |  
| Plans | Stores the SaaS Offer plan information. Plans are part of the SaaS offer and submitted via Partner Center. This table is populated after calls to the Fulfillment API|  
| Subscription Audit Log | This table is an audit trail for all the operations regarding Subscriptions. |  
| Subscription Licenses | This table tracks the associated licenses for a Subscription. This table is only relevant for the Blueprint: SaaS as a License Service |  
| Subscriptions | Tracks the Offer's Subscriptions. Associates Customers (users) with their selected Plan|  
| Users | Tracks the list of Customers (users that created subscriptions against the offer) |  

[Click here](Transactable-SaaS-SDK-Sample-Database.md) for more details on the sample database. 
### Source Code 

The Project is located in the **SaaS.SDK.Client.DataAccess** folder. The project is composed of the following sections: 

| Section Name | Description |
| --- | --- |  
| Dependencies | EntityFrameworkCore,  EntityFrameworkCore.SqlServer|
| Context | EntityFramework database context | 
| Contracts | Interface that defines the methods to manage the entities |
| Entities | Table entities | 
| Services | Implementations for the interfaces to manage the persistence and query over entities |
 


