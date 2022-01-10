# Azure SQL Managed Database Performance and Monitoring
To monitor the performance of a database in Azure SQL Database and Azure SQL Managed Instance, start by monitoring the CPU and IO resources used by publisher workload relative to the level of database performance publisher chosed in selecting a particular service tier and performance level. 

To accomplish this, Azure SQL Database and Azure SQL Managed Instance emit resource metrics that can be viewed in the Azure portal.

The following metrics enable punlisher to see if a database is reaching 100% of
 - processor
 - memory
 -  IO resources
 
  High DTU or processor percentage, as well as high IO percentage, indicates that your workload might need more CPU or IO resources. It might also indicate queries that need to be optimized.
 
### 1- Add Metrics
 From Main page for Azure SQL database click Metrics Then click on metrics dropdown list and select "CPU percentage"
 ![enable SQL](images/monitoring/sql/sql-monitor-1.png)

Repeat the steps for memory and IO Resources
![enable SQL](images/monitoring/sql/sql-monitor-1-1.png)

### 2- Add Alert
From the Metrics dashboard click **New Alert Rule**
![enable SQL](images/monitoring/sql/sql-monitor-2.png)

Azure Metrics service will open Alert page where publisher add Action groups
![enable SQL](images/monitoring/sql/sql-monitor-3.png)

Select Email Action Group
![enable SQL](images/monitoring/sql/sql-monitor-4.png)

Fill Alert rule details
![enable SQL](images/monitoring/sql/sql-monitor-5.png)

Click create alert rule.

For more about the **Action Groups**]** please refer to [Microsoft Docs](https://docs.microsoft.com/en-us/azure/azure-monitor/alerts/action-groups).

  
### Summary
This section shows How-To 
- Create SQL performance and monitoring metrics
- Create Alert to notify group or person.

### Other Monitor Recommendation
- [Web App Monitoring and Alert instruction](./WebApp-Monitoring.md).
- [App Registration Credentials Monitoring and Alert instruction](./App-Reg-Monitoring.md).