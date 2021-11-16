# Enterpise Reference Architect Example
The following diagram is a **sample** refernce archtiect for SaaS offer at production level.  [The Azure Well-Architected Framework](https://docs.microsoft.com/en-us/azure/architecture/framework/) is a set of guiding tenets that can be used to improve the quality of a workload. The framework consists of five pillars of architectural excellence:
- Reliability
- Security
- Cost Optimization
- Operational Excellence
- Performance Efficiency

## Architecture
![SaaS Enterprise](./images/saas-enterpise-scale.png)

This example uses the following Azure services. Details of the deployment 
architecture are located in the deployment architecture section.
- Azure Active Directory
- Azure Key Vault
- Azure SQL Database
- Application Gateway
  - Web Application Firewall
  - Firewall mode: prevention
  - Rule set: OWASP 3.0
  - Listener: port 443
- Azure virtual network
- Network security groups
- Azure DNS
- Azure Storage
- Azure Monitor
- App Service Plan
- Azure Load Balancer
- Azure Web App

**App Service plan**: An App Service plan provides the managed virtual machines (VMs) that host your app. All apps associated with a plan run on the same VM instances.

**App Service app**: Azure App Service is a fully managed platform for creating and deploying cloud applications.

**Deployment slots**: A deployment slot lets you stage a deployment and then swap it with the production deployment. That way, you avoid deploying directly into production. See the Manageability section for specific recommendations.

**IP address**: The App Service app has a public IP address and a domain name. The domain name is a subdomain of azurewebsites.net, such as contoso.azurewebsites.net.

**Azure DNS**: Azure DNS is a hosting service for DNS domains, providing name resolution using Microsoft Azure infrastructure. By hosting your domains in Azure, you can manage your DNS records using the same credentials, APIs, tools, and billing as your other Azure services. To use a custom domain name (such as contoso.com) create DNS records that map the custom domain name to the IP address. For more information, see Configure a custom domain name in Azure App Service.

**Azure SQL Database**: SQL Database is a relational database-as-a-service in the cloud. SQL Database shares its code base with the Microsoft SQL Server database engine.

**Azure Load Balancer**: Azure Load Balancer allows customers to scale their applications and create high availability for services. Load Balancer supports inbound as well as outbound scenarios, and provides low latency, high throughput, and scales up to millions of flows for all TCP and UDP applications.

## Virtual Network
The architecture defines a private virtual network with default address space.

**Network security groups**: Network security groups (NSGs) contain access control lists that allow or deny traffic within a virtual network. NSGs can be used to secure traffic at a subnet or individual VM level. The following NSGs exist:

- 1 NSG for Application Gateway
- 1 NSG for App Service
- 1 NSG for Azure SQL Database
- 1 NSG for Azure Storage Account
- 1 NSG for Azure KeyVault

Each of the NSGs have specific ports and protocols open so that the solution can work securely and correctly. In addition, the following configurations are enabled for each NSG:

- Diagnostic logs and events are enabled and stored in a storage account
- Azure Monitor logs is connected to the NSG's diagnostics

**Subnets**: Each subnet is associated with its corresponding NSG.

**Private Link**:Azure Private Link enables you to access Azure PaaS Services (for example, Azure Storage and SQL Database) and Azure hosted customer-owned/partner services over a Private Endpoint in your virtual network. Traffic between your virtual network and the service traverses over the Microsoft backbone network, eliminating exposure from the public Internet.

## Identity management
The following technologies provide identity management capabilities in the Azure environment:

- **Azure Active Directory** is Microsoft's multi-tenant cloud-based directory and identity management service. All users for this solution are created in AAD, including users accessing the Azure SQL Database.
- **Authentication** to the application is performed using AAD. For more information, see Integrating applications with Azure Active Directory. Additionally, the database column encryption uses Azure Active Directory to authenticate the application to Azure SQL Database. For more information, see how to protect sensitive data in Azure SQL Database.
- **Azure role-based access control** enables precisely focused access management for Azure. Subscription access is limited to the subscription administrator, and access to resources can be limited based on user role.
- **Azure Active Directory** Privileged Identity Management enables customers to minimize the number of users who have access to certain information. Administrators can use AAD Privileged Identity Management to discover, restrict, and monitor privileged identities and their access to resources. This functionality can also be used to enforce on-demand, just-in-time administrative access when needed.
- **Azure Active Directory** Identity Protection detects potential vulnerabilities affecting an organization’s identities, configures automated responses to detected suspicious actions related to an organization’s identities, and investigates suspicious incidents to take appropriate action to resolve them.

## Security
**Secrets Management** The solution uses Azure Key Vault for the management of keys and secrets. Azure Key Vault helps safeguard cryptographic keys and secrets used by cloud applications and services. The following Azure Key Vault capabilities help customers protect data and access to such data:

- Advanced access policies are configured on a need basis.
- Key Vault access policies are defined with minimum required permissions to keys and secrets.
- All keys and secrets in Key Vault have expiration dates.
- All keys in Key Vault are protected by specialized hardware security modules (HSMs). The key type is an HSM Protected 2048-bit RSA Key.
- All users and identities are granted minimum required permissions using role-based access control.
- Diagnostics logs for Key Vault are enabled with a retention period of at least 365 days.
- Permitted cryptographic operations for keys are restricted to the ones required.

**Application Gateway** The architecture reduces the risk of security vulnerabilities using an Application Gateway with Web Application Firewall, and the OWASP ruleset enabled. Additional capabilities include:
- End-to-End-SSL
- Enable SSL Offload
- Disable TLS v1.0 and v1.1
- Web Application Firewall
- Prevention mode with OWASP 3.0 ruleset
- Enable diagnostics logging
- Custom health probes
- Azure Security Center and Azure Advisor provide additional protection and notifications. Azure Security Center also provides a reputation system.

