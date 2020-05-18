# Customer experience

* [Overview](#overview)
* [Purchase SaaS offer](#purchase-saas-offer)
* [Landing Page](#landing-page)
* [Subscriptions](#subscriptions)
* [Activate](#activate)
* [Change plan](#change-plan)
* [Unsubscribe](#unsubscribe)
* [Change Quantity](#change-quantity)
* [View activity log](#view-activity-log)
* [Troubleshooting issues](#troubleshooting-issues)

## Overview

The Customer portal provides the landing page for the customers who purchase the transactable SaaS offer.

## Purchase SaaS offer

SaaS offers that is published in Azure Market Place can be extracted and managed from the portal.

Log on to [Azure](https://portal.azure.com)

1. Click **All Services** menu option on the left
![AllServices](./images/All-Services.png)

2. Search for resources of type **Software as a Service**.
3. The page enlists all the SaaS offers that were previously purchased.
![SaaS Subscriptions](./images/CloudSaasOfferList.png)

4. Click **Add** to proceed to purchase a new SaaS offer.
If you don't have prior subscriptions against SaaS offers, the list would be blank and you would get an option to **Create Software as a Service** button to help you proceed with the purchase.
![Create SaaS Subscription](./images/Create-SaaS-resource.png)

5. Clicking **Add** ( or **Create Software as a Service**) leads you to a page that lists down SaaS offers available for purchase.

6. Search for **Cloud SaaS** and locate our SaaS offer in the listing
![SaaS Offer](./images/Search-Results-SaaS.png)

7. Click on the tile to view the details of the offer
![SaaS Offer detail](./images/SaaS-Offer-Detail.png)
8. **Select a software plan** and click **Create**

9. Fill out the form and click **Subscribe**
![SaaS Offer](./images/Subscribe-to-Plan.png)
10. A new resource gets created and appears in the listing
![SaaS Subscriptions](./images/CloudSaasOfferList.png)
11. Click the text under **Name** to view the details of the resource
12. Click **Configure Account** option in the header bar. You will now be redirected to the SaaS offer landing page offered by the **Customer portal** in a new tab / window

13. The landing page presents the details of the offer that was purchased with an option to **Activate** the subscription.

> In a real scenario, the landing page would collect additional details relevant for provisioning the target SaaS application and any additional custom information from customer if required .

## Landing Page

1. Customer lands on the page served from the Customer portal that presents the details of the subscription related to the purchase. The landing page might present additional input fields in the form based on the configuration set by the Publisher for the plan.

2. Click **Activate** to either place a request for the publisher to act on ( applicable if activation workflow is turned on or activate the subscription, otherwise)

## Subscriptions

1. All the subscriptions purchased by the customer will be availabe under the subscriptions screen.
2. The status of each subscription will be available in the list.

3. From this screen the actions on the subscriptions like **Change Plan**, **Change Quantity**, **Activate** and **Unsubscribe** can be done depending on the status. These actions can be performed by the customer if the activation workflow is turned off by setting the **IsAutomaticProvisioningSupported** to **True**.

![SaaS Subscriptions](./images/customer-subscriptions.png)

## Activate

The below diagram illustrates the flow of information between Azure and the Customer portal.
![Information flow between Azure and Provisioning application](https://docs.microsoft.com/en-us/azure/marketplace/partner-center-portal/media/saas-post-provisioning-api-v2-calls.png)

* On the landing page, review the details presented and click **Activate**
![SaaS Subscriptions](./images/activate-subscription.png)

> The Customer portal calls the following SaaS SDK API methods in the background

```csharp
// Determine the details of the offer using the marketplace token that is available in the URL during the redirect from Azure to the landing page.
Task<ResolvedSubscriptionResult> ResolveAsync(string marketPlaceAccessToken);

// Activates the subscription to trigger the start of billing
Task<SubscriptionUpdateResult> ActivateSubscriptionAsync(Guid subscriptionId, string subscriptionPlanID);

```

* Upon successful activation of the subscription, the landing page switches to a view that enlists the subscriptions against the offer.

> You can switch to Azure and note that the **Configure Account** button is replaced by **Manage Account** button indicating that the subscription has been materialized.
> **Note** If activation workflow is enabled, by turning on the flag - **IsAutomaticProvisioningSupported** in the ApplicationConfiguration table, the application would put the subscription in PendingActivation status and the Fulfillment API to activate the subscription is not called. Publisher has the option to activate the subscription via the action menu in the subscription listing in the Publisher Portal.

## Change plan

The below diagram illustrates the flow of information between Azure and the Customer portal.
![Update subscription](https://docs.microsoft.com/en-us/azure/marketplace/partner-center-portal/media/saas-update-api-v2-calls-from-saas-service-a.png)

* Log on to **Customer portal**.
* Click **Subscriptions** from the menu on the top, in case you are not on the page that shows you the list of subscriptions.
* The table on this page enlists all the subscriptions and their status.
* Click **Change Plan** option in the dropdown menu that appears when the icon under the **Actions** column against any of the active subscriptions is clicked.
![SaaS Subscriptions](./images/customer-subscriptions.png)

* A popup appears with a list of plans that you can switch to.
* Select a desired plan and click **Change Plan**.
![SaaS Subscriptions](./images/change-plan.png)

> The Customer portal calls the following SaaS SDK API methods in the background

```csharp
// Initiate the change plan process
Task<SubscriptionUpdateResult> ChangePlanForSubscriptionAsync(Guid subscriptionId, string subscriptionPlanID);

```

>The operation is asynchronous and the call to **change plan** comes back with an operation location that should be queried for status.

```csharp
// Get the latest status of the subscription due to an operation / action.
Task<OperationResult> GetOperationStatusResultAsync(Guid subscriptionId, Guid operationId);
```

> **Note** If activation workflow is enabled, by turning on the flag - **IsAutomaticProvisioningSupported** in the ApplicationConfiguration table, the option to **Change Plan** is disabled for customers. Publisher has the option to change the plan of the subscription via the action menu in the subscription listing in the Publisher Portal.

## Unsubscribe

* Log on to **Customer portal**.
* Click **Subscriptions** from the menu on the top, in case you are not on the page that shows you the list of subscriptions.
* The table on this page enlists all the subscriptions and their status.
* Click **Unsubscribe** against an active subscription.
![SaaS Subscriptions](./images/unsubscribe.png)
* Confirm your action to trigger the deletion of the subscription.

> The Customer portal calls the following SaaS SDK API methods in the background.

```csharp
// Initiate the delete subscription process
Task<SubscriptionUpdateResult> DeleteSubscriptionAsync(Guid subscriptionId, string subscriptionPlanID);
```

> The operation is asynchronous and the call to **Unsubscribe** comes back with an operation location that should be queried for status.

```csharp
// Get the latest status of the subscription due to an operation / action.
Task<OperationResult> GetOperationStatusResultAsync(Guid subscriptionId, Guid operationId);
```

> **Note** If activation workflow is enabled, by turning on the flag - **IsAutomaticProvisioningSupported** in the ApplicationConfiguration table, the option to **Unsubscribe** is disabled for customers. Publisher has the option to delete the subscription via the action menu in the subscription listing in the Publisher Portal.

## Change Quantity

* Log on to **Customer portal**.
* Click **Subscriptions** from the menu on the top, in case you are not on the page that shows you the list of subscriptions.
* The table on this page enlists all the subscriptions and their status.
* Click **Change quantity** in the menu as shown in the below picture
![Change quantity](./images/change-quantity-menu.png)

* Provide the new quantity and click **Change Quantity** to update the quantity on the subscription

![Update quantity](./images/update-quantity-popup.png)

> Note: The update to quantity is applicable if only the subscription is against a Plan that is set to be billed per user
  
![Per user pricing](./images/per-user-plan-pricing.png)

> The Customer portal calls the following SaaS SDK API methods in the background.

## View activity log

* Log on to **Customer portal**.
* Click **Subscriptions** from the menu on the top, in case you are not on the page that shows you the list of subscriptions.
* The table on this page enlists all the subscriptions and their status.
* Click **Activity Log** to view the log of activity that happened against the subscription.
 ![SaaS Subscriptions](./images/activity-log-menu.png)
 ![SaaS Subscriptions](./images/activity-log-popup.png)
