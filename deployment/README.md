# Transactable SaaS SDK Database

## Introduction

The customer portal and the publisher portal sample web applications use SQL Server database as the data source to store marketplace subscriptions and the status, metered dimensions by plans, and activity against subscriptions.

## Install Using SQL Scripts

The project uses EntityFramework Core Code-First approach to the database design and maintainance.
When deploying a new version of the accelerator the `dotnet-ef` tool is used to generate a migration script from your current database state to the latest state. All necesary changes will be automatically applied during deployment

When installing the accelerator for the first time, a fresh db creation script is auto-generated based on the latest version of the code and automatically creates the db schema and seeds the initial data required for operation.

## Description

The following picture illustrates the entities and the relationships among them:

![Entities and relationships](../../docs/images/amp-saas-db.png)

| Entity | Description |
| --- | --- |  
| ApplicationConfiguration | Holds application level configuration like SMTP details and feature flags |
| ApplicationLog | Activity in the application is tracked via a custom logger implementation |
| DatabaseVersionHistory | Tracks the changes to the database by versions |
| EmailTemplate | Predefined set of email templates |
| Events | List of events that occur in the context of a subscription against a plan which can be configured for email notifications |
| KnownUsers | Admin users on the publisher side that can log on to the Publisher portal|
| MeteredAuditLogs | The request and response against metering service API is stored here. Helps troubleshoot issuses when posting usage data to Azure |
| MeteredDimensions  | Stores the meters related to plans |
| OfferAttributes | Global set of attributes that could appear as additional input paramteters during the purchase of a subscription |
| Offers | Marketplace related to the publisher |
| PlanAttributeMapping | Overrides for the input attributes defined at the offer level|
| PlanEventsMapping | Event configuration by plan |
| Plans | Plans associated with the Marketplace offer|
| Roles | User roles |
| SubscriptionAttributeValues | Values provided by the user on the landing page, for the additional input attributes as configured at the plan level |
| SubscriptionAuditLogs | Activity on the subscription is saved to this table |
| Subscriptions | List of SaaS subscriptions
| Users | Users ( auto-registered due to purchase of subscriptions) |
| ValueTypes | Type of attributes, for the fields that appear on the subscription landing page|
| WebJobSubscriptionStatus | Status changes on the subscription as processed by the webjob are logged here |
| SchedulerFrequency| Lookup table with Occurance frequency |
| MeteredPlanSchedulerManagement| List of all Metered Scheduled task |
| SchedulerManagerView| View of all metered scheduled task with Parents information |


### Application Configuration

Application configuration is initialized with the following keys that the publisher has to update with appropriate values:

| Key | Description|
| --- | -- |
| SMTPFromEmail | From email address for the emails |
| SMTPPassword | Password in the credential to connect to the SMTP server |
| SMTPHost | SMTP server name |
| SMTPPort | SMTP Port name |
| SMTPUserName | Username in the credential to connect to the SMTP server |
| SMTPSslEnabled | Is SMTP SSL enabled |
| ApplicationName | Name of the application |
| IsEmailEnabledForSubscriptionActivation | Flag that defines if an email has to be sent out when a subscription is activated (Default: False, Allowed values : True / False)|
| IsEmailEnabledForUnsubscription | Flag that defines if an email has to be sent out when a subscription is deleted (Default: False, Allowed values : True / False)|
| IsAutomaticProvisioningSupported | Flag that enables activation workflow. If the value is False, the option Change Plan is not available to the customer. Clicking Subscribe button on the landing page would place the subscription in PendingActivation status and doesn't activate the subscription yet. Publisher has the option to activate the subscription, change plan and unsubscribe. If the value is True, customer can activate and change plan without any intervention required from the publisher.|
| IsEmailEnabledForPendingActivation | Flag to indicate if an email should be sent out to publisher when activation workflow is enabled.|
| AcceptSubscriptionUpdates | If True, subscription updates are allowed. If False or the key doesn't exist, Subscription updates are denied.(Allowed values : True / False)|
| IsMeteredBillingEnabled |Flag to enable Metered Billing Feature. (Allowed values : True / False) | 
| EnableHourlyMeterSchedules |Flag to enable Hourly meter scheduled frequency. (Allowed values : True / False) |
| EnableDailyMeterSchedules |Flag to enable Daily meter scheduled frequency. (Allowed values : True / False) |
| EnableWeeklyMeterSchedules |Flag to enable Weekly meter scheduled frequency. (Allowed values : True / False) |
| EnableMonthlyMeterSchedules |Flag to enable Monthly meter scheduled frequency. (Allowed values : True / False) |
| EnableYearlyMeterSchedules |Flag to enable Yearly meter scheduled frequency. (Allowed values : True / False) |
| EnableOneTimeMeterSchedules |Flag to enable One Time meter scheduled frequency. (Allowed values : True / False) | 
| EnablesSuccessfulSchedulerEmail |Flag to enable sending email for successful metered usage.(Allowed values : True / False) | 
| EnablesFailureSchedulerEmail |Flag to enable sending email for failure metered usage. (Allowed values : True / False) | 
| EnablesMissingSchedulerEmail |Flag to enable sending email for missing metered usage. (Allowed values : True / False) | 
| SchedulerEmailTo |Scheduler email receiver(s) | 
| WebNotificationUrl |Setting this URL will enable pushing LandingPage/Webhook events to this external URL | 


### EmailTemplate

Email template table contains a predefined set of templates for the emails sent out when a subscription is activated / deleted.
Publisher is required to update the ToRecipients ( list of emails separted by a ;), CC and BCC on the template entries in the EmailTemplate table.
The Body of the email is HTML and can be modified using a text editor.
