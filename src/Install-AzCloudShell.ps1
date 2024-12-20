wget https://dotnet.microsoft.com/download/dotnet/scripts/v1/dotnet-install.sh; `
chmod +x dotnet-install.sh; `
./dotnet-install.sh -version 8.0.303; `
$ENV:PATH="$HOME/.dotnet:$ENV:PATH"; `
dotnet tool install --global dotnet-ef --version 8.0.0; `
git clone https://github.com/Azure/Commercial-Marketplace-SaaS-Accelerator.git -b 8.0.0 --depth 1; `
cd ./Commercial-Marketplace-SaaS-Accelerator/deployment; `
.\Deploy.ps1 `
 -WebAppNamePrefix "WflowSaasOffer2024" `
 -ResourceGroupForDeployment "wflow-saasoffer" `
 -PublisherAdminUsers "iowamiker@hotmail.com,mike@officemike.com" `
 -Location "Central US" 

 ✅ If the intallation completed without error complete the folllowing checklist:
   🔵 Add The following URL in PartnerCenter SaaS Technical Configuration
      ➡️ Landing Page section:       https://WflowSaasOffer2024-portal.azurewebsites.net/
      ➡️ Connection Webhook section: https://WflowSaasOffer2024-portal.azurewebsites.net/api/AzureWebhook
      ➡️ Tenant ID:                  97d1fb75-540a-436c-a547-6acd9bf286b2
      ➡️ AAD Application ID section: a42ff9ed-c271-4bcb-870b-c499211f8d31