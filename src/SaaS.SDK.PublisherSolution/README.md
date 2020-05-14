# Publisher sample web application	

### Description

The web application demonstrates how publishers can perform the following actions:

- Generate usage meter events
- Submit usage meter events against subscriptions
- Assign / revoke licenses to / from subscriptions
- Persist and transmit usage meter events

### Source Code 

The Project is located in the **SaaS.SDK.PublisherSolution** folder. The project is composed of the following sections: 

| Section Name | Description |
| --- | --- |  
| Dependencies | Microsoft AspNet Core OpenID Authentication, EntityFramework and Logging extensions, SaaS.SDK.Client |
| Controllers | ASP.Net Core MVC controllers that are responsible to provide data  / views and handle the data posted back by the user |  
| Models | POCO classes for transferring data between the client and the endpoints | 
| Utilities | Constants and classes with utility methods | 
| Views | User interface components | 
