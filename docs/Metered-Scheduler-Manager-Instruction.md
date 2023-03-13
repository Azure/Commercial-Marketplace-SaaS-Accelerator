# SaaS Accelerator Metered Scheduler Manager 
Metered Scheduler Manager is a feature where Publisher can schedule **FIX Quantity** metered emiting tasks. The scheduler will monitor these tasks and trigger event based on the scheduled frequency time. Currenty the scheduler support the following time base trigger
1. Hourly
1. Daily
1. Weekly
1. Monthly
1. Yearly
1. OneTime
## Enable and Disable Metered Scheduler Manager Feature During Installation
By default, this feature is disabled during the installation. Publisher can enable this feature during the installion process by passing an optional parameter **MeteredSchedulerSupport**. Please refer to the installation steps [here](./Installation-Instructions.md)


## Enable and Disable Metered Scheduler Manager Feature Post Installation
Publisher can active the **Metered Scheduler** feature by updating Admin portal web configuration and set **SaaSApiConfiguration__SupportmeteredBilling** to **true**
![home](./images/scheduler-config.png)

Publisher can disable the feature by feature by updating Admin portal web configuration and set **SaaSApiConfiguration__SupportmeteredBilling** to **false**



## Access Metered Scheduler Manager Dashboard
 Publisher can access **Scheduler Manager Dashboard** from **Home page** or side bar menu.
![home](./images/scheduler-home.png)

To access dashboad from **Home page**, Publisher will click on **Scheduler Tile** then publisher will be redirect to dashboard summary page.

![dashboard](./images/scheduler-dashboard.png)
## Add New Metered Scheduler Manager Task
Publisher can add new scheduled task from by clicking **Add New Scheduled Metered Trigger** from **Dashboard page**

![add-task-1](./images/scheduler-add1.png)

![add-task-2](./images/scheduler-add2.png)


Publisher can schedule the task by click **Add Scheduler Usage Task** and the task will be added to the Sscheduled tasks.
Once the task is added, the new task will show up in **Dashboard page**
![add-task-2](./images/scheduler-add3.png)


## Audit Metered Scheduler Manager Task
Publisher can audit scheduled task results by accessing the **Run History**
![audit-task-1](./images/scheduler-audit1.png)

There is the example for task audit
![audit-task-2](./images/scheduler-audit2.png)

## Delete Metered Scheduler Manager Task
Publisher can delete scheduled task by click **Delete**
![delete-task-1](./images/scheduler-delete.png)

## Restriction
Currently the scheduler managers can support **only one** scheduled task per
1. Subscription
1. Plan
1. Dimension




