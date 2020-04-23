
CREATE TABLE SubscriptionStatusLogInWebJob
(
 ID Int Identity(1,1)
,SubscriptionId UniqueIdentifier
,SubscriptionStatus Varchar(225)
,DeploymentStatus Varchar(225)
,Description Varchar(max)
,InsertDate DateTime
)

GO
CREATE TABLE [dbo].[SubscriptionTemplateParameters]
(
Id Int identity(1,1)
,OfferName Varchar(225)
,OfferGUId UniqueIdentifier
,PlanGUID UniqueIdentifier
,PlanId   Varchar(225)
,ARMTemplateID UniqueIdentifier 
,Parameter Varchar(225)
,ParameterDataType Varchar(225)
,Value Varchar(225)
,ParameterType Varchar(225)
,EventId Int
,EventsName Varchar(225)
,AMPSubscriptionId UniqueIdentifier
,SubscriptionStatus Varchar(225)
,SubscriptionName Varchar(225)
,CreateDate Datetime
,UserId Int
)

GO

ALTER Procedure spGetSubscriptionTemplateParameters
(  
@SubscriptionId Uniqueidentifier,  
@PlanId Uniqueidentifier  
)  
AS  
BEGIN  
   
Declare @OfferId Uniqueidentifier   
Set @OfferId=(Select OfferId from Plans where PlanGuId =@PlanId )  
SELECT    
Cast( ROW_NUMBER() OVER ( ORDER BY ART.ID) as Int)RowId  
,isnull(STP.ID,0) ID  
,ofr.OfferName
,ofr.OfferGUId 
,pln.PlanGUID
,pln.PlanId
,art.ARMTemplateID
,art.Parameter
,art.ParameterDataType
,art.Value
,art.ParameterType
,PE.EventId
,EV.EventsName
,Sub.AMPSubscriptionId
,Sub.SubscriptionStatus
,sub.Name AS SubscriptionName
from 
Offers ofr
inner join Plans pln on ofr.OfferGUId=pln.OfferID
inner join PlanEventsMapping PE on pln.PlanGUID=pe.PlanId
inner join ARMTemplateParameters ART on PE.ARMTemplateId=ART.ARMTemplateId 
inner Join Subscriptions Sub on pln.PlanId=Sub.AMPPlanId
LEFT join SubscriptionTemplateParameters STP on
STP.OfferGUId=ofr.OfferGUId and STP.PlanGUId= pln.PlanGUID
inner join [Events] Ev on EV.EventsId=PE.EventId
 Where Sub.AMPSubscriptionId =@SubscriptionId




END


GO


CREATE TABLE [dbo].[SubscriptionTemplateParametersOutPut]
(
RowID int  NOT NULL
,Id Int 
,OfferName Varchar(225)
,OfferGUId UniqueIdentifier
,PlanGUID UniqueIdentifier
,PlanId   Varchar(225)
,ARMTemplateID UniqueIdentifier 
,Parameter Varchar(225)
,ParameterDataType Varchar(225)
,Value Varchar(225)
,ParameterType Varchar(225)
,EventId Int
,EventsName Varchar(225)
,AMPSubscriptionId UniqueIdentifier
,SubscriptionStatus Varchar(225)
,SubscriptionName Varchar(225)
)
