# Publisher portal

## Introduction

This is a sample web application that demonstrates common interactions between the publisher's SaaS platform and the marketplace APIs.

## Description

The web application demonstrates how publishers can perform the following actions:

- Generate usage meter events
- Submit usage meter events against subscriptions
- Assign / revoke licenses to / from subscriptions
- Persist and transmit usage meter events

## Source Code

The Project is located in the **AdminSite** folder. The project is composed of the following sections:

| Section Name | Description |
| --- | --- |  
| Dependencies | Microsoft AspNet Core OpenID Authentication, EntityFramework and Logging extensions, ClientSite |
| Controllers | ASP.Net Core MVC controllers that are responsible to provide data  / views and handle the data posted back by the user |  
| Models | POCO classes for transferring data between the client and the endpoints |
| Utilities | Constants and classes with utility methods |
| Views | User interface components |

## Admin Page Overview

The web application allows to manage subscriptions, plans and offers, all from the same Admin Page. Once deployed, the application consists of the following sections:

| Page Name | Description |
| --- | --- |  
| Home | Shows the homepage displayed on login. It contains quick links to navigate directly to the other pages like Subscriptions, Plans and Offers. |
| Subscriptions | Shows all the subscriptions in various stages for an offer. The publisher can view user details such as Username, User Email and Subscription Name. As and when subscriptions come in, the publisher can change the subscription status once the offer is ready, by clicking Activate under Action. Since the publisher can change subscription statuses, they can also view the Activity log for each of these subscriptions. This will show the log with log time as to when the subscription status changed, including old subscription status value as well as what the status changed to.|  
| Plans |  Shows all the existing plans including private plans. It shows the Plan name, Description and the Offer the plan is tied to. It should be noted that plans for all offers by the Publisher will be displayed here. Publishers may also choose to edit certain details regarding plan activation. Changes made can be submitted by clicking Save events. |
| Offers | Shows all current active offers. It shows the currently active Offer Names, Description and Actions that can be performed. Like with plans, offers can be edited. Publishers can add extra parameters or remove currently saved parameters by clicking Add New Row or Remove respectively. Once all these changes are made, changes should be saved. Newly added parameters for an offer will show under their respective plans.|

