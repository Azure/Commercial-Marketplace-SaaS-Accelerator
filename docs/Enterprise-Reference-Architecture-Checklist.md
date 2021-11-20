# Architect Examples Deployment Checklist:
For ARM Template steps are in the following order
- Create Resource Group 
- Create Custom NSG if it is needed 
- Create Network and defined the subnets ranges and assign default or custom NSG
    - Hight recommended to defined specific subnet where all private links will use   
- Create LogAnalytics
- Create Application Insights
- Create Private DNS Zone for all private link to use during the deployment configuration
- Create Key Vault
- Enable Key Vault VNET Integration
- Enable Key Vault Private link
- Enable Key Vault diagnostic and metric Data to Log Analytics

- Create Storage Account 
- Create Blob/Table ..etc
- Enable Storage Account VNET Integration
- Enable Storage Account Private link
- Enable Storage Account diagnostic and metric Data to Log Analytics
- Store Storage Account connection string in KeyVault

- Create Azure SQL Server 
- Create Azure SQL database
- Enable Azure SQL Server VNET Integration
- Enable Azure SQL Server Private link
- Enable Azure SQL Server and Database audit, advace threading ..etc
- Enable Azure SQL Server and Database diagnostic and metric Data to Log Analytics
- Store Azure SQL database connection string in KeyVault
- Upload the SQL Schema file


- Create App Service Plan 
- Enable App Service Plan diagnostic and metric Data to Log Analytics

- Create Web App and reference App ServicePlan 
- Link Web App Cofiguration to KeyVault
- Add VNET Inegration Configuration please refer to [docs](https://docs.microsoft.com/en-us/azure/app-service/overview-vnet-integration)
- Enable Web App VNET Integration
- Enable Web App Private link
- Enable Web App diagnostic and metric Data to Log Analytics
- Enable Web App Application Insights

- Create Application Gateway
- Configure Front-End IP pool
- Configure Backend and reference to Web App
- Configure HTTP Route
- Configure Listener
- Enable Application Gateway VNET Integration
- Enable Application Gateway Private link
- Enable Application Gateway Application Insight

** These step for mutli region deployment only**. 
- Deploy Azure Traffic Manager 
- Enable Geo Replication for Storage Account 
- Enable Geo Replication for SQL Server






