# Client Data Access library

### Introduction

Enables to persist the Plans, Subscriptions, and transactions with the fulfillment and Metered APIs.	

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
| Application Configuration | Tracks the configurations required for SMTP and emails. |
| ARM Templates | Tracks the list for ARM Templates uploaded. |
| ARM Template Parameters | Tracks the input and output parameters of the ARM templates. |
| Database Version History | Tracks the database script versions. | 
| Deployment Attributes | Tracks the required deployment parameters that are used to deploy application in a subscription. | 
| Email Template | Tracks the emails templates html. | 
| Events | Tracks the list of events available. | 
| Known Users | Tracks the users that have access to login to the admin portal. | 
| Metered Audit Logs | Tracks all the metered data posted to Billing API on a subscription. | 
| Offers | Stores the SaaS offers information. | 
| Offer Attributes | Tracks the list of all Custom fields that to be filled by customer after purchasing a subscription. | 
| Plan Attribute Mapping | Tracks the list of Custom fields mapped per plan. | 
| Plan Events Mapping | Tracks the events and email ids mapped per plan. | 
| Roles | Tracks available roles in the system. | 
| Subscription Attribute Values | Tracks the data filled by customer after purchasing the subscription. | 
| Subscription Key Vault | Tracks the key vault details, where the Deployment parameters are stored. | 
| Subscription Template Parameters | Tracks the deployment input and output values of the subscription. | 
| Value Types | Tracks all the data types available to create offer parameters. | 
| Web Job Subscription Status | Tracks the activity done by the web job. | 
| Plan Attribute Output | Does not hold any data, used to map the result of the stored procedure spGetOfferParameters. | 
| Plan Events Output | Does not hold any data, used to map the result of the stored procedure spGetPlanEvents. | 
| Subscription Key Value Output | Does not hold any data, used to map the result of the stored procedure spGetSubscriptionKeyValue. | 
| Subscription Parameters Output | Does not hold any data, used to map the result of the stored procedure spGetSubscriptionParameters. | 
| Subscription TemplateParameters Output | Does not hold any data, used to map the result of the stored procedure spGetSubscriptionTemplateParameters. |  


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
 


