
/****** Object:  Table [dbo].[ARMTemplates]    Script Date: 04-03-2020 12.32.32 PM ******/

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ARMTemplates](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[ARMTempalteID] [uniqueidentifier] NULL,
	[ARMTempalteName] [varchar](225) NULL,
	[TemplateLocation] [varchar](225) NULL,
	[Isactive] [bit] Not NULL,
	[CreateDate] [datetime] NULL,
	[UserId] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[OfferAttributes]    Script Date: 04-03-2020 12.32.32 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[OfferAttributes](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[ParameterId] [varchar](225) NULL,
	[DisplayName] [varchar](225) NULL,
	[Description] [varchar](225) NULL,
	[ValueTypeId] [int] NULL,
	[FromList] [bit]  Not NULL,
	[ValuesList] [varchar](max) NULL,
	[Max] [int] NULL,
	[Min] [int] NULL,
	[Type] [varchar](225) NULL,
	[DisplaySequence] [int] NULL,
	[Isactive] [bit] Not NULL,
	[CreateDate] [datetime] NULL,
	[UserId] [int] NULL,
	[OfferId] [uniqueidentifier] not NULL
	,IsDelete bit
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Offers]    Script Date: 04-03-2020 12.32.32 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Offers](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[OfferId] [varchar](225) NULL,
	[OfferName] [varchar](225) NULL,
	[CreateDate] [datetime] NULL,
	[UserId] [int] NULL,
	[OfferGUId] [uniqueidentifier] NOT  NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[PlanAttributeMapping]    Script Date: 04-03-2020 12.32.32 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PlanAttributeMapping](
	[PlanAttributeId] [int] IDENTITY(1,1) NOT NULL,
	[PlanId] [uniqueidentifier] not NULL,
	[OfferAttributeID] [int]  not NULL,
	[IsEnabled] bit  not NULL,
	[CreateDate] [datetime] NULL,
	[UserId] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[PlanAttributeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO


GO
CREATE TABLE Events
(
EventsId Int identity(1,1) Primary key
,EventsName  Varchar(225)
,IsActive bit
,CreateDate DATETIME
)

GO

CREATE TABLE [dbo].[PlanEventsMapping](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[PlanId] [uniqueidentifier] not NULL,
	[ARMTemplateId] [uniqueidentifier] not NULL,
	[EventId] [int] not NULL,
	[Isactive] [bit] Not NULL,
	[SuccessStateEmails] [varchar](225) NULL,
	[FailureStateEmails] [varchar](225) NULL,
	[CreateDate] [datetime] NULL,
	[UserId] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Plans]    Script Date: 04-03-2020 12.32.32 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
/****** Object:  Table [dbo].[SubscriptionAttributeValues]    Script Date: 04-03-2020 12.32.32 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SubscriptionAttributeValues](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[PlanAttributeId] [int] NOT NULL,
	[Value] [varchar](225) NULL,
	[SubscriptionId] [uniqueidentifier] NOT NULL,
	[CreateDate] [datetime] NULL,
	[UserId] [int] NULL,
	[PlanID] [uniqueidentifier] NOT  NULL,
	[OfferID] [uniqueidentifier]  NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ValueTypes]    Script Date: 04-03-2020 12.32.32 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ValueTypes](
	[ValueTypeId] [int] IDENTITY(1,1) NOT NULL,
	[ValueType] [varchar](225) NULL,
	[CreateDate] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[ValueTypeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO


GO
IF EXISTS(SELECT 1 FROM SYS.TABLES WHERE NAME ='Plans')
	BEGIN
	ALTER TABLE Plans ADD PlanGUID UNIQUEIDENTIFIER NULL
	ALTER TABLE Plans ADD OfferID UNIQUEIDENTIFIER NULL
	ALTER TABLE Plans ADD DeployToCustomerSubscription BIT
	END


GO

--Update Plans set OfferID=(select OfferGUId from Offers)
UPDATE PLANS SET PlanGUID = NEWID() where PlanGUID is null;
declare @OfferID UNIQUEIDENTIFIER = NEWID() 
UPDATE PLANS SET OfferID = @OfferID where OfferID is null;

GO

ALTER TABLE Plans alter column  PlanGUID UNIQUEIDENTIFIER NOT NULL
ALTER TABLE Plans alter column  OfferID UNIQUEIDENTIFIER NOT NULL
GO

CREATE TABLE PlanAttributeOutput
(
RowNumber Int Primary Key, 
PlanAttributeId Int NOt NULL,
PlanId Uniqueidentifier NOt NULL,
OfferAttributeId Int NOT NULL,
DisplayName Varchar(225) NOT NULL,
IsEnabled bit NOT NULL
)

go
/* 
Exec spGetOfferParameters 'B8F4D276-15EB-4EB6-89D4-E600FF1098EF'
*/
CREATE Procedure spGetOfferParameters
(
@PlanId Uniqueidentifier
)
AS
BEGIN
 
Declare @OfferId Uniqueidentifier 
Set @OfferId=(Select OfferId from Plans where PlanGuId =@PlanId )
SELECT  
  Cast( ROW_NUMBER() OVER ( ORDER BY OA.ID) as Int)RowNumber
,isnull(PA.PlanAttributeId,0) PlanAttributeId
,ISNULL(PA.PlanId,@PlanId) PlanId 
,ISNULL(PA.OfferAttributeID ,OA.ID)  OfferAttributeID

,OA.DisplayName
--,OA.DisplaySequence
,isnull(PA.IsEnabled,0) IsEnabled
from [dbo].[OfferAttributes] OA
left  join 
[dbo].[PlanAttributeMapping]  PA
on OA.ID= PA.OfferAttributeID and OA.OfferId=@OfferId
and  PA.PlanId=@PlanId
where  
OA.Isactive=1 

END





go

/* 
Exec spGetPlanEvents 'B8F4D276-15EB-4EB6-89D4-E600FF1098EF'
*/
CREATE Procedure spGetPlanEvents
(
@PlanId Uniqueidentifier
)
AS
BEGIN
 
Declare @OfferId Uniqueidentifier 
--Set @OfferId=(Select OfferId from Plans where PlanGuId =@PlanId )
--isnull(PlanAttributeId,ID),ParameterId,DisplayName,DisplaySequence,isnull(IsEnabled,0)

SELECT  
 Cast(ROW_NUMBER() OVER ( ORDER BY E.EventsId)  as Int) RowNumber
 ,ISNULL(OEM.Id,0)  Id
,ISNULL(OEM.PlanId,@PlanId) PlanId
--,OEM.ARMTemplateId
,ISNULL(OEM.Isactive,0) Isactive

,ISNULL(OEM.SuccessStateEmails,'')SuccessStateEmails
,ISNULL(OEM.FailureStateEmails,'')FailureStateEmails
,E.EventsId as EventId

,E.EventsName
from Events  E
left  join 
PlanEventsMapping  OEM
on
E.EventsId= OEM.EventId and  OEM.PlanId= @PlanId
where  
E.Isactive=1 

END
Go

CREATE TABLE PlanEventsOutPut
(
RowNumber Int Primary Key,
ID int Not Null,
PlanId Uniqueidentifier Not Null,
Isactive bit Not NUll,
SuccessStateEmails Varchar(max),
FailureStateEmails Varchar(max), 
EventId Int Not NUll,
EventsName Varchar(225) Not NUll,
)




go


CREATE TABLE SubscriptionParametersOutput
(
RowNumber  Int Primary key
,Id Int Not Null	
,PlanAttributeId	 Int Not Null	
,OfferAttributeID	 Int Not Null	
,DisplayName		 Varchar(225) Not Null	
,Type				  Varchar(225) Not Null	
,ValueType			  Varchar(225) Not Null	
,DisplaySequence	 Int Not Null	
,IsEnabled			 bit Not Null	
,Value				  Varchar(MAx) Not Null	
,SubscriptionId		 uniqueidentifier Not Null	
,OfferId		 uniqueidentifier Not Null	
,PlanId				 uniqueidentifier Not Null	
,UserId Int
,CreateDate DateTime
)

GO


/* 
Exec spGetSubscriptionParameters '53ff9e28-a55b-65e1-ff75-709aec6420fd','a35d4259-f3c9-429b-a871-21c4593fa4bf'
*/
CREATE  Procedure spGetSubscriptionParameters
(
@SubscriptionId Uniqueidentifier,
@PlanId Uniqueidentifier
)
AS
BEGIN
 
Declare @OfferId Uniqueidentifier 
Set @OfferId=(Select OfferId from Plans where PlanGuId =@PlanId )
SELECT  
  Cast( ROW_NUMBER() OVER ( ORDER BY OA.ID) as Int)RowNumber
,isnull(SAV.ID,0) ID
,isnull(SAV.PlanAttributeId,PA.PlanAttributeId) PlanAttributeId
,ISNULL(SAV.PlanId,@PlanId) PlanId 
,ISNULL(PA.OfferAttributeID ,OA.ID)  OfferAttributeID
,ISNULL(OA.DisplayName,'')DisplayName
,ISNULL(OA.Type,'')Type
,ISNULL(VT.ValueType,'') ValueType
,ISnull(OA.DisplaySequence,0)DisplaySequence
,isnull(PA.IsEnabled,0) IsEnabled
,ISNULL(Value,'')Value
,ISNULL(SubscriptionId,@SubscriptionId) SubscriptionId
,ISNULL(SAV.OfferID,OA.OfferId) OfferID
,SAV.UserId
,SAV.CreateDate
from 
[dbo].[OfferAttributes] OA
Inner  join 
[dbo].[PlanAttributeMapping]  PA
on OA.ID= PA.OfferAttributeID and OA.OfferId=@OfferId

and  PA.PlanId=@PlanId
Left Join 
SubscriptionAttributeValues SAV
on SAV.PlanAttributeId= PA.PlanAttributeId
and SAV.SubscriptionId=@SubscriptionId

inner join ValueTypes VT
ON OA.ValueTypeId=VT.ValueTypeId

where  
OA.Isactive=1 
and PA.IsEnabled=1
END

--56E5E465-1657-45C0-BE9D-C1F011D0E77D

--Insert into PlanEventsOutPut
--Exec spGetPlanEvents 'B8F4D276-15EB-4EB6-89D4-E600FF1098EF'

--Insert into Events
--Select 'Activate' , 1, Getdate() UNION ALL
--Select 'Unsubscribe' , 1, Getdate()

----update Events set EventsName='Unsubscribe' where EventsId=2

GO

 GO
 