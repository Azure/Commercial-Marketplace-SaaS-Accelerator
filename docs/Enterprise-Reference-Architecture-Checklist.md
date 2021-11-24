# Architect Examples Deployment Checklist:
For ARM Template steps are in the following order
1. Create Resource Group 
2. Create Custom NSG if it is needed 
3. Create Network and defined the subnets ranges and assign default or custom NSG
    - Hight recommended to defined specific subnet where all private links will use   
4. Create LogAnalytics
5. Create Application Insights
6. Create Private DNS Zone for all private link to use during the deployment configuration
7. Create Key Vault
8. Enable Key Vault VNET Integration
9. Enable Key Vault Private link
10. Enable Key Vault diagnostic and metric Data to Log Analytics

11. Create Storage Account 
12. Create Blob/Table ..etc
13. Enable Storage Account VNET Integration
14. Enable Storage Account Private link
15. Enable Storage Account diagnostic and metric Data to Log Analytics
16. Store Storage Account connection string in KeyVault

17. Create Azure SQL Server 
18. Create Azure SQL database
19. Enable Azure SQL Server VNET Integration
20. Enable Azure SQL Server Private link
21. Enable Azure SQL Server and Database audit, advace threading ..etc
22. Enable Azure SQL Server and Database diagnostic and metric Data to Log Analytics
23. Store Azure SQL database connection string in KeyVault
24. Upload the SQL Schema file
25. Create App Service Plan 
26. Enable App Service Plan diagnostic and metric Data to Log Analytics

27. Create Web App and reference App ServicePlan 
28. Link Web App Cofiguration to KeyVault
29. Add VNET Inegration Configuration please refer to [docs](https://docs.microsoft.com/en-us/azure/app-service/overview-vnet-integration)
30. Enable Web App VNET Integration
31. Enable Web App Private link
32. Enable Web App diagnostic and metric Data to Log Analytics
33. Enable Web App Application Insights

34. Create Application Gateway
35. Configure Front-End IP pool
36. Configure Backend and reference to Web App
37. Configure HTTP Route
38. Configure Listener
39. Enable Application Gateway VNET Integration
40. Enable Application Gateway Private link
41. Enable Application Gateway Application Insight

** These step for mutli region deployment only**. 
42. Deploy Azure Traffic Manager 
43. Enable Geo Replication for Storage Account 
44. Enable Geo Replication for SQL Server






