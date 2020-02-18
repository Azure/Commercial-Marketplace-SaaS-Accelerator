# Microsoft Commercial Marketplace Transactable SaaS Offer Blueprints


## Introduction

This SDK's goal is to provide guidance and the components needed for Publishers (ISVs and StartUps) to create transactable Software as a-Service (SaaS) offers in the Azure and AppSource marketplaces.  

The SDK provides the components required for the implementations of the billing (fulfillment v2 and metered) APIs, and additional components that showcase how to build a customer provisioning interface, logging, and administration of the customer's subscriptions. These are the core projects in the SDK:  

- **Transactable SaaS Client Library** that implements the fulfillment v2 and metered APIs and the Web-hook that handles messages from the Marketplace's E-commerce engine.
- **Customer provisioning sample web application** that showcases how to register, provision, and activate the customer subscription. Implemented using ASP.Net Core 3.1, it uses the SaaS Client library and Data Access Library to to invoke and persists interactions with the fulfillment APIs. In addition, it provides interfaces for a customer to manage their subscriptions and plans. 
- **Publisher sample web application** that showcases how to generate metered based transactions, persistence of those transactions and transmission of these transactions to the metered billing API. 
- **Client Data Access library** that demonstrates how to persist the Plans, Subscriptions, and transactions with the fulfillment and Metered APIs.






