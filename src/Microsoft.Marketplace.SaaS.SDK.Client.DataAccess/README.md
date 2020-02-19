# Client Data Access library

### Introduction

Enables to persist the Plans, Subscriptions, and transactions with the fulfillment and Metered APIs.	

### Description

[TO DO]


Entities that are persisted in the Database

| Entity Name | Description |
| --- | --- | 
| Application Log | Tracks the Customer Web Application Sample activity |  
| Metered Dimensions | Tracks the metered information that will be submitted to the Billing API. The metered data is submitted by the Publisher. In the SDK, there is an interface in the Publisher Sample web application that will help generate this data.  |  
| Plans | Stores the SaaS Offer plan information. Plans are part of the SaaS offer and submitted via Partner Center. This table is populated after calls to the Fulfillment API|  
| Subscription Audit Log | This table is an audit trail for all the operations regarding Subscriptions. |  
| Subscription Licenses | This table tracks the associated licenses for a Subscription. This table is only relevant for the Blueprint: SaaS as a License Service |  
| Subscriptions | Tracks the Offer's Subscriptions. Associates Customers (users) with their selected Plan|  
| Users | Tracks the list of Customers |  

[Click here](Transactable-SaaS-SDK-Sample-Database.md) for more details on the sample database. 
### Source Code 

The Project is located in the ** Microsoft.Marketplace.SaaS.SDK.Client.DataAccess ** folder. The project is composed of the following sections: 

| Section Name | Description |
| --- | --- |  
| Dependencies | EntityFrameworkCore,  EntityFrameworkCore.SqlServer|
| Context | --- | 
| Contracts | --- |
| Entities | Table entities | 
| Services | --- |
 


