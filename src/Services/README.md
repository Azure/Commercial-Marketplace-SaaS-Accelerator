# SaaS SDK Services

## Introduction

The project offers the service layer that is leveraged by the **Customer portal** and **Publisher portal**

## Source Code

The project is located in the **SaaS.SDK.Services** folder. The project is composed of the following sections:

| Section Name | Description |
| --- | --- |  
| Contracts| Interfaces and contracts for the services
| Helpers | Email helper |
| Dependencies | Microsoft.AspNetCore.Mvc.ViewFeatures, Microsoft.Azure.Management.ResourceManager, Microsoft.Extensions.Logging.Console, Microsoft.IdentityModel.Clients.ActiveDirectory and Microsoft.Rest.ClientRuntime.Azure.Authentication|
| Models |  POCO classes - instances participate as parameters / return types from the services
| Services | Implementation of the interfaces |
| Status Handlers | Status handlers to support notifications including: AbstractSubscription, ISubscription, Notification, PendingActivation, PendingFulfillment, Unsubscribe |
| Utilities| Utility classes |

Redeployment:
wget https://dotnet.microsoft.com/download/dotnet/scripts/v1/dotnet-install.sh; `
chmod +x dotnet-install.sh; `
./dotnet-install.sh -version 6.0.417; `
$ENV:PATH="$HOME/.dotnet:$ENV:PATH"; `
dotnet tool install --global dotnet-ef --version 6.0.1; `
git clone https://github.com/Zakhele-TechWannabe/Commercial-Marketplace-SaaS-Accelerator.git --depth 1; `
cd ./Commercial-Marketplace-SaaS-Accelerator/deployment; `
.\Upgrade.ps1 `
 -WebAppNamePrefix "botsa-saas" `
 -ResourceGroupForDeployment "botsa-saas-app" `
