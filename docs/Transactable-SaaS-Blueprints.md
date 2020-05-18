# Microsoft Commercial Marketplace Transactable SaaS Offer Blueprints

## Introduction

This SDK's goal is to provide guidance and the components needed for Publishers (ISVs and StartUps) to create transactable Software as a-Service (SaaS) offers in the Azure and AppSource marketplaces.  

The SDK provides the components required for the implementations of the marketplace APIs (fulfillment v2 and metering service), and additional components that showcase how to build a customer portal, logging, and administration of the customer's subscriptions. These are the core projects in the SDK:  

- **Transactable SaaS Client Library** that implements the fulfillment v2 and metering service APIs and the webhook that handles messages from the Marketplace's E-commerce engine.
- **Customer portal** that showcases how to register, provision, and activate the customer subscription. Implemented using ASP.Net Core 3.1, it uses the SaaS Client library and Data Access Library to to invoke and persists interactions with the fulfillment APIs. In addition, it provides interfaces for a customer to manage their subscriptions and plans.
- **Publisher portal** that showcases how to generate metering based transactions, persistence of those transactions and transmission of these transactions to the metering service API.
- **Client Data Access library** that demonstrates how to persist the Plans, Subscriptions, and transactions with the fulfillment and metering service APIs.
