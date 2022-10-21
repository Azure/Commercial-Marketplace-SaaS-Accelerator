# Architect Examples Deployment Checklist:
For ARM Template steps are in the following order
1. Create Resource Group 
2. **If Needed** Create Custom Network Security Group **NSG** if default NSG is not sufficent for your deployment. 
3. Create Network and defined the subnets ranges and assign default or custom NSG
    - Hight recommended to defined specific subnet where all private links will use   
4. Create LogAnalytics
5. Create Application Insights
6. Create Private DNS Zone for all private link to use during the deployment configuration
7. Create Key Vault
8. Enable Key Vault VNET Integration
9. Enable Key Vault Private link
10. Enable Key Vault diagnostic and metric Data to Log Analytics

17. Create Azure SQL Server 
18. Create Azure SQL database
19. Enable Azure SQL Server VNET Integration
20. Enable Azure SQL Server Private link
21. Enable Azure SQL Server and Database audit, advance threading ..etc
22. Enable Azure SQL Server and Database diagnostic and metric Data to Log Analytics
23. Store Azure SQL database connection string in KeyVault
24. Upload the SQL Schema file


25. Create SaaS Portal App Service Plan 
26. Enable SaaS Portal App Service Plan diagnostic and metric Data to Log Analytics

27. Create SaaS Portal Web App and reference SaaS Portal App ServicePlan 
28. Link SaaS Portal Web App Configuration to KeyVault
29. Add VNET Inegration Configuration please refer to [docs](https://docs.microsoft.com/en-us/azure/app-service/overview-vnet-integration)
30. Enable SaaS Portal Web App VNET Integration
31. Enable SaaS Portal Web App Private link
32. Enable SaaS Portal Web App diagnostic and metric Data to Log Analytics
33. Enable SaaS Portal Web App Application Insights

34. Create SaaS Admin App Service Plan 
35. Enable SaaS Admin App Service Plan diagnostic and metric Data to Log Analytics
36. Create SaaS Admin Web App and reference SaaS Admin App ServicePlan 
37. Link SaaS Admin Web App Configuration to KeyVault
38. Add VNET Integration Configuration please refer to [docs](https://docs.microsoft.com/en-us/azure/app-service/overview-vnet-integration)
39. Enable SaaS Admin Web App VNET Integration
40. Enable SaaS Admin Web App Private link
41. Enable SaaS Admin Web App diagnostic and metric Data to Log Analytics
42. Enable SaaS Admin Web App Application Insights
43. Create Application Gateway
44. Configure Front-End IP pool
45. Configure Backend and reference to Web App
46. Configure HTTP Route
47. Configure Listener
48. Enable Application Gateway VNET Integration
49. Enable Application Gateway Private link
50. Enable Application Gateway Application Insight


** These step for multi-region deployment only**. 

51. Deploy Azure Traffic Manager 
52. Enable Geo Replication for SQL Server






