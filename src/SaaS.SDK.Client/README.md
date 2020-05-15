# Transactable SaaS Client Library

## Introduction

This library implements the **[Fulfillment v2](https://docs.microsoft.com/en-us/azure/marketplace/partner-center-portal/pc-saas-fulfillment-api-v2)** and **[Metering service](https://docs.microsoft.com/en-us/azure/marketplace/partner-center-portal/marketplace-metering-service-apis)** APIs and the **[Webhook](https://docs.microsoft.com/en-us/azure/marketplace/partner-center-portal/pc-saas-fulfillment-api-v2#implementing-a-webhook-on-the-saas-service)** that handles messages from the Marketplace's E-commerce engine.

### Description

This is the **core** library for the transactable SaaS SDK. It provides the implementation of the APIs

### Source Code

The Project is located in the **SaaS.SDK.Client** folder. The project is composed of the following sections:

| Section Name | Description |
| --- | --- |  
| Attributes | Custom attribute used to annotate models |
| Configurations | Model to hold API configuration |
| Contracts | Interfaces that define the contract to be implemented |
| Exceptions | Custom Exceptions |
| Helpers | Classes with utility methods |
| Models | Models to hold the request / response to / from marketplace APIs |
| Network | Comprises rest client implementation|
| Services | Implementation for the contracts defined in **Contracts** folder |
| WebHook | Contains the model for the payload received by the **Webhook**, a processor that decodes the action and delegates further processing to a handler ( implemented by client ) |

## Implementation Best Practices

- Implement IWebhookHandler to process the data received via Webhook and pass that to the WebhookProcessor.
- Catch custom exceptions thrown by the implementations of Fulfillment API and metering service API clients and consume the additional information related to the error.
