# Security best practices

  - [Diagnostics and Logging](#diagnostics-and-logging)
  - [Azure Managed Identity](#azure-managed-identity)
  - [Encryption at REST](#encryption-at-rest)
  - [Private Link Integration](#private-link-integration)
  - [Azure Policies](#azure-policies)


## Diagnostics and Logging

You can add or enable [Azure Application Insights](https://docs.microsoft.com/en-us/azure/azure-monitor/app/app-insights-overview) for logging and diagnostics. The Application Insights is a service that helps developers monitor performance, detect issues, and diagnose crashes in their web applications. It is a part of the Azure Monitor, which is a platform service that provides a comprehensive solution for collecting, analyzing, and acting on telemetry from your cloud environments.

To enable Application Insights, follow the instructions here - [Enable Application Insights](https://docs.microsoft.com/en-us/azure/azure-monitor/app/azure-web-apps?tabs=net#enable-application-insights).


## Azure Managed Identity

 The [Azure Managed Identity](https://docs.microsoft.com/en-us/azure/active-directory/managed-identities-azure-resources/overview) to authenticate with the Azure services. Managed identities are a feature of Azure Active Directory. Each managed identity is an identity in the Azure AD tenant that is tied to the lifecycle of an Azure service instance. The lifecycle of the identity is managed by Azure. When the Azure service is deleted, Azure automatically deletes the identity.

   SaaS Accelerator uses the Managed Identity to authenticate with the following Azure services:
   1. Communication between App Service and KeyVault.
   2. To implement Managed Identity, for connection between App Services and Azure SQL Database, follow the instructions here - [Use a managed identity to connect to an Azure SQL database](https://docs.microsoft.com/en-us/azure/app-service/app-service-web-tutorial-connect-msi).


## Encryption at REST 
    
(Key Vault and SQL Server)

The SaaS Accelerator uses the [Azure Key Vault](https://docs.microsoft.com/en-us/azure/key-vault/general/overview) to store and manage the secrets, keys, and certificates. Azure Key Vault helps safeguard cryptographic keys and secrets used by cloud applications and services. By using Key Vault, you can encrypt keys and secrets (such as authentication keys, storage account keys, data encryption keys, .PFX files, and passwords) using keys protected by hardware security modules (HSMs).

The SaaS Accelerator uses the [Azure SQL Database](https://docs.microsoft.com/en-us/azure/azure-sql/database/) to store the data. Azure SQL Database is a fully managed relational database with built-in intelligence that supports self-driving features such as performance tuning and threat detection. It is a general-purpose relational database that supports structures such as relational data, JSON, spatial, and XML.

You can enable encryption at rest for Azure SQL Database by following the instructions here - [Transparent Data Encryption with Azure SQL Database](https://learn.microsoft.com/en-us/azure/azure-sql/database/transparent-data-encryption-tde-overview?view=azuresql&tabs=azure-portal).

Encryption for Azure SQL can be enabled by Customer managed Keys or Service Managed Keys. For more information on how to enable encryption for Azure SQL Database, refer to the following documentation - [Encryption for Azure SQL Database](https://docs.microsoft.com/en-us/azure/azure-sql/database/transparent-data-encryption-tde-overview?view=azuresql&tabs=azure-portal).


## Private Link Integration

The [Azure Private Link](https://docs.microsoft.com/en-us/azure/private-link/private-link-overview), enables you to access Azure PaaS services (for example, Azure Storage, Azure Cosmos DB, and SQL Database) over a private endpoint in your virtual network. Traffic between your virtual network and the service traverses over the Microsoft backbone network, eliminating exposure from the public Internet.

To Enable Private Link Integration for Azure Services, follow the instructions here - [Azure Private Link](https://docs.microsoft.com/en-us/azure/private-link/private-link-overview).
        

## Azure Policies

The [Azure Policies](https://docs.microsoft.com/en-us/azure/governance/policy/overview) are used to enforce rules and effects for your resources in the Azure environment. Azure Policy is a service in Azure that you use to create, assign, and manage policies. These policies enforce different rules and effects over your resources, so those resources stay compliant with your corporate standards and service level agreements.Make sure to enable Azure policies to enforce the security best practices in your Azure environment.

