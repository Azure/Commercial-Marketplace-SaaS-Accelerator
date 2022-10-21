# Unit Tests Project

## Introduction

Unit test project that validates and tests the marketplace API calls generated from the Services client library.

## Description

The Unit tests project will continue to evolve to include a larger set of tests and would include mocks to validate the API calls done by the **[Fulfillment v2](https://docs.microsoft.com/en-us/azure/marketplace/partner-center-portal/pc-saas-fulfillment-api-v2)** and **[Marketplace Metering service](https://docs.microsoft.com/en-us/azure/marketplace/partner-center-portal/marketplace-metering-service-apis)** APIs

## Source Code

The Project is located in the **Services.UnitTest** folder. The project is composed of the following sections:

| Section Name | Description |
| --- | --- |  
| Dependencies | SaaS.SDK.Client, MSTest Framework MSTest TestAdapter, and Microsoft Configuration Extensions |
| FullfillmentAPItest.cs | Contains the Unit tests for the **[Fulfillment v2](https://docs.microsoft.com/en-us/azure/marketplace/partner-center-portal/pc-saas-fulfillment-api-v2)** API  |
| MeteredAPITest.cs | Contains the Unit tests for the **[Marketplace Metering service](https://docs.microsoft.com/en-us/azure/marketplace/partner-center-portal/marketplace-metering-service-apis)** API |
