
-- After teh commented section we have the Latest tables of sandwich model

/* Sandwich model script OLD 
GO
IF EXISTS(SELECT 1 FROM SYS.TABLES WHERE NAME ='Plans')
	BEGIN
		ALTER TABLE Plans ADD OfferID INT
		ALTER TABLE Plans ADD DeployToCustomerSubscription BIT
	END

GO

CREATE TABLE Offers
(
Id Int identity(1,1) primary key
,OfferId Varchar(225)
,OfferName Varchar(225)
,CreateDate Datetime
,UserId INT
)

GO

CREATE TABLE OfferAttributes
(
ID  INT Identity(1,1) Primary key
,ParameterId Varchar(225)
,DisplayName Varchar(225)
,Description Varchar(225)
,ValueTypeId int
,FromList Bit
,ValuesList Varchar(max)
,Max Int
,Min Int
,OfferID int
,Type Varchar(225)
,DisplaySequence Int
,Isactive bit
,CreateDate Datetime
,UserId INT
)
GO

--ADD FK TO OFFERS.ID
CREATE TABLE SubscriptionAttributeValues
(
ID Int Identity(1,1)  Primary key
,PlanAttributeId Int
,Value Varchar(225)
,SubscriptionId uniqueidentifier
,PlanID Int
,OfferID Int 
,CreateDate  Datetime
,UserId INT
)
GO

GO

CREATE TABLE ValueTypes 
(
ValueTypeId Int identity(1,1) Primary key
,ValueType  Varchar(225)
,CreateDate DATETIME
)

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
CREATE TABLE ARMTemplates 
(
ARMTempalteID Int Identity(1,1) Primary key
,ARMTempalteName Varchar(225)
,TemplateLocation  Varchar(225)
,Isactive bit
,CreateDate  Datetime
,UserId  INT
)

GO

CREATE TABLE PlanAttributeMapping
(
PlanAttributeId Int identity(1,1) Primary key
,PlanId Int
,OfferAttributeID Int
,IsEnabled Int
,CreateDate  Datetime
,UserId  INT
)


GO

CREATE TABLE PlanEventsMapping
(
Id Int Identity(1,1) Primary key
,PlanId Int
,ARMTemplateId Int
,EventId Int
,Isactive Bit
,SuccessStateEmails Varchar(225)
,FailureStateEmails Varchar(225)
,CreateDate  Datetime
,UserId  Int
)




*/

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
	[OfferId] [uniqueidentifier] not NULL,
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
	[OfferGUId] [uniqueidentifier] NULL,
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
/****** Object:  Table [dbo].[PlanEventsMapping]    Script Date: 04-03-2020 12.32.32 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
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
		ALTER TABLE Plans ADD PlanGUID UNIQUEIDENTIFIER NOT NULL
		ALTER TABLE Plans ADD OfferID UNIQUEIDENTIFIER NOT NULL
		ALTER TABLE Plans ADD DeployToCustomerSubscription BIT
	END


GO
--Update Plans set OfferID=(select OfferGUId from Offers)
UPDATE PLANS SET PlanGUID = NEWID() where PlanGUID is null;
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

--truncate table PlanAttributeOutput
--Insert into PlanAttributeOutput
--Exec spGetOfferParameters 'B8F4D276-15EB-4EB6-89D4-E600FF1098EF'

go
/* 
Exec spGetOfferParameters 'B8F4D276-15EB-4EB6-89D4-E600FF1098EF'
*/
ALTER Procedure spGetOfferParameters
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
ALTER Procedure spGetPlanEvents
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

--Insert into PlanEventsOutPut
--Exec spGetPlanEvents 'B8F4D276-15EB-4EB6-89D4-E600FF1098EF'

--Insert into Events
--Select 'Activate' , 1, Getdate() UNION ALL
--Select 'Unsubscribe' , 1, Getdate()

----update Events set EventsName='Unsubscribe' where EventsId=2

GO

alter table [PlanAttributeMapping] alter column PlanId uniqueidentifier not null
alter table PlanEventsMapping alter column PlanId uniqueidentifier not null
alter table [OfferAttributes] alter column OfferId uniqueidentifier not null
alter table Offers alter column OfferGUId uniqueidentifier not null
--alter table Plans alter column OfferID  uniqueidentifier not null
--alter table Plans alter column PlanGUID  uniqueidentifier not null
 alter table [OfferAttributes] alter column  [OfferId] [uniqueidentifier] not NULL
 alter table [OfferAttributes] alter column  [FromList] [bit]  Not NULL
 alter table [OfferAttributes] alter column  [Isactive] [bit] Not NULL
 alter table  [PlanAttributeMapping] alter column  [IsEnabled] bit  not NULL
 alter table  [PlanAttributeMapping] alter column  [OfferAttributeID] int  not NULL
 alter table   PlanEventsMapping  alter column  [PlanId] [uniqueidentifier] not NULL
alter table   PlanEventsMapping  alter column 	[ARMTemplateId] [uniqueidentifier] not NULL
alter table   PlanEventsMapping	 alter column [EventId] [int] not NULL
alter table   PlanEventsMapping	 alter column  [Isactive] [bit] Not NULL

alter table  SubscriptionAttributeValues alter column	[PlanAttributeId] [int] NOT NULL
alter table  SubscriptionAttributeValues alter column	[SubscriptionId] [uniqueidentifier] NOT NULL
alter table  SubscriptionAttributeValues alter column	[PlanID] [uniqueidentifier] NOT  NULL
alter table  SubscriptionAttributeValues alter column	[OfferID] [uniqueidentifier]  NOT NULL

 GO
