/*Basic version 1.0 Schema*/
GO
/****** Object:  Table [dbo].[ApplicationConfiguration]    Script Date: 05-15-2020 12.56.43 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ApplicationConfiguration](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](50) NULL,
	[Value] [nvarchar](max) NULL,
	[Description] [nvarchar](255) NULL,
 CONSTRAINT [PK_ApplicationConfiguration] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ApplicationLog]    Script Date: 05-15-2020 12.56.43 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ApplicationLog](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ActionTime] [datetime] NULL,
	[LogDetail] [varchar](4000) NULL,
 CONSTRAINT [PK_ApplicationLog] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DatabaseVersionHistory]    Script Date: 05-15-2020 12.56.43 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DatabaseVersionHistory](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[VersionNumber] [decimal](6, 2) NOT NULL,
	[ChangeLog] [nvarchar](max) NOT NULL,
	[CreateDate] [datetime] NOT NULL,
	[CreateBy] [nvarchar](100) NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[EmailTemplate]    Script Date: 05-15-2020 12.56.43 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[EmailTemplate](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Status] [varchar](1000) NULL,
	[Description] [varchar](1000) NULL,
	[InsertDate] [datetime] NULL,
	[TemplateBody] [varchar](max) NULL,
	[Subject] [varchar](1000) NULL,
	[ToRecipients] [varchar](1000) NULL,
	[CC] [varchar](1000) NULL,
	[BCC] [varchar](1000) NULL,
	[IsActive] [bit] NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Events]    Script Date: 05-15-2020 12.56.43 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Events](
	[EventsId] [int] IDENTITY(1,1) NOT NULL,
	[EventsName] [varchar](225) NULL,
	[IsActive] [bit] NULL,
	[CreateDate] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[EventsId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[KnownUsers]    Script Date: 05-15-2020 12.56.43 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[KnownUsers](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[UserEmail] [varchar](50) NULL,
	[RoleId] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[MeteredAuditLogs]    Script Date: 05-15-2020 12.56.43 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MeteredAuditLogs](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[SubscriptionId] [int] NULL,
	[RequestJson] [varchar](500) NULL,
	[ResponseJson] [varchar](500) NULL,
	[StatusCode] [varchar](100) NULL,
	[CreatedDate] [datetime] NULL,
	[CreatedBy] [int] NOT NULL,
	[SubscriptionUsageDate] [datetime] NULL,
 CONSTRAINT [PK_MeteredAuditLogs] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[MeteredDimensions]    Script Date: 05-15-2020 12.56.43 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MeteredDimensions](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Dimension] [varchar](150) NULL,
	[PlanId] [int] NULL,
	[CreatedDate] [datetime] NULL,
	[Description] [varchar](250) NULL,
 CONSTRAINT [PK_MeteredDimensions] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[OfferAttributes]    Script Date: 05-15-2020 12.56.43 PM ******/
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
	[FromList] [bit] NOT NULL,
	[ValuesList] [varchar](max) NULL,
	[Max] [int] NULL,
	[Min] [int] NULL,
	[Type] [varchar](225) NULL,
	[DisplaySequence] [int] NULL,
	[Isactive] [bit] NOT NULL,
	[CreateDate] [datetime] NULL,
	[UserId] [int] NULL,
	[OfferId] [uniqueidentifier] NOT NULL,
	[IsDelete] [bit] NULL,
	[IsRequired] [bit] NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Offers]    Script Date: 05-15-2020 12.56.43 PM ******/
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
	[OfferGUId] [uniqueidentifier] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[PlanAttributeMapping]    Script Date: 05-15-2020 12.56.43 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PlanAttributeMapping](
	[PlanAttributeId] [int] IDENTITY(1,1) NOT NULL,
	[PlanId] [uniqueidentifier] NOT NULL,
	[OfferAttributeID] [int] NOT NULL,
	[IsEnabled] [bit] NOT NULL,
	[CreateDate] [datetime] NULL,
	[UserId] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[PlanAttributeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[PlanAttributeOutput]    Script Date: 05-15-2020 12.56.43 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PlanAttributeOutput](
	[RowNumber] [int] NOT NULL,
	[PlanAttributeId] [int] NOT NULL,
	[PlanId] [uniqueidentifier] NOT NULL,
	[OfferAttributeId] [int] NOT NULL,
	[DisplayName] [varchar](225) NOT NULL,
	[IsEnabled] [bit] NOT NULL,
	[Type] [varchar](225) NULL,
PRIMARY KEY CLUSTERED 
(
	[RowNumber] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[PlanEventsMapping]    Script Date: 05-15-2020 12.56.43 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PlanEventsMapping](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[PlanId] [uniqueidentifier] NOT NULL,
	[EventId] [int] NOT NULL,
	[Isactive] [bit] NOT NULL,
	[SuccessStateEmails] [varchar](225) NULL,
	[FailureStateEmails] [varchar](225) NULL,
	[CreateDate] [datetime] NULL,
	[UserId] [int] NULL,
	[CopyToCustomer] [bit] NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[PlanEventsOutPut]    Script Date: 05-15-2020 12.56.43 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PlanEventsOutPut](
	[RowNumber] [int] NOT NULL,
	[ID] [int] NOT NULL,
	[PlanId] [uniqueidentifier] NOT NULL,
	[Isactive] [bit] NOT NULL,
	[SuccessStateEmails] [varchar](max) NULL,
	[FailureStateEmails] [varchar](max) NULL,
	[EventId] [int] NOT NULL,
	[EventsName] [varchar](225) NOT NULL,
	[CopyToCustomer] [bit] NULL,
PRIMARY KEY CLUSTERED 
(
	[RowNumber] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Plans]    Script Date: 05-15-2020 12.56.43 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Plans](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[PlanId] [varchar](100) NULL,
	[Description] [varchar](500) NULL,
	[DisplayName] [varchar](100) NULL,
	[IsmeteringSupported] [bit] NULL,
	[IsPerUser] [bit] NULL,
	[PlanGUID] [uniqueidentifier] NOT NULL,
	[OfferID] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_Plans] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Roles]    Script Date: 05-15-2020 12.56.43 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Roles](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](50) NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[SubscriptionAttributeValues]    Script Date: 05-15-2020 12.56.43 PM ******/
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
	[PlanID] [uniqueidentifier] NOT NULL,
	[OfferID] [uniqueidentifier] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[SubscriptionAuditLogs]    Script Date: 05-15-2020 12.56.43 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SubscriptionAuditLogs](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[SubscriptionID] [int] NULL,
	[Attribute] [varchar](20) NULL,
	[OldValue] [varchar](50) NULL,
	[NewValue] [varchar](max) NULL,
	[CreateDate] [datetime] NULL,
	[CreateBy] [int] NULL,
 CONSTRAINT [PK_SubscriptionAuditLog] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[SubscriptionParametersOutput]    Script Date: 05-15-2020 12.56.43 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SubscriptionParametersOutput](
	[RowNumber] [int] NOT NULL,
	[Id] [int] NOT NULL,
	[PlanAttributeId] [int] NOT NULL,
	[OfferAttributeID] [int] NOT NULL,
	[DisplayName] [varchar](225) NOT NULL,
	[Type] [varchar](225) NOT NULL,
	[ValueType] [varchar](225) NOT NULL,
	[DisplaySequence] [int] NOT NULL,
	[IsEnabled] [bit] NOT NULL,
	[IsRequired] [bit] NULL,
	[Value] [varchar](max) NOT NULL,
	[SubscriptionId] [uniqueidentifier] NOT NULL,
	[OfferId] [uniqueidentifier] NOT NULL,
	[PlanId] [uniqueidentifier] NOT NULL,
	[UserId] [int] NULL,
	[CreateDate] [datetime] NULL,
	[FromList] [bit] NOT NULL,
	[ValuesList] [varchar](225) NOT NULL,
	[Max] [int] NOT NULL,
	[Min] [int] NOT NULL,
	[HTMLType] [varchar](225) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[RowNumber] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Subscriptions]    Script Date: 05-15-2020 12.56.43 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Subscriptions](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[AMPSubscriptionId] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[SubscriptionStatus] [varchar](50) NULL,
	[AMPPlanId] [varchar](100) NULL,
	[IsActive] [bit] NULL,
	[CreateBy] [int] NULL,
	[CreateDate] [datetime] NULL,
	[ModifyDate] [datetime] NULL,
	[UserId] [int] NULL,
	[Name] [varchar](100) NULL,
	[AMPQuantity] [int] NOT NULL,
	[PurchaserEmail] [varchar](225) NULL,
	[PurchaserTenantId] [uniqueidentifier] NULL,
 CONSTRAINT [PK_Subscriptions] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Users]    Script Date: 05-15-2020 12.56.43 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Users](
	[UserId] [int] IDENTITY(1,1) NOT NULL,
	[EmailAddress] [varchar](100) NULL,
	[CreatedDate] [datetime] NULL,
	[FullName] [varchar](200) NULL,
 CONSTRAINT [PK_Users] PRIMARY KEY CLUSTERED 
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ValueTypes]    Script Date: 05-15-2020 12.56.43 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ValueTypes](
	[ValueTypeId] [int] IDENTITY(1,1) NOT NULL,
	[ValueType] [varchar](225) NULL,
	[CreateDate] [datetime] NULL,
	[HTMLType] [varchar](225) NULL,
PRIMARY KEY CLUSTERED 
(
	[ValueTypeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[WebJobSubscriptionStatus]    Script Date: 05-15-2020 12.56.43 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[WebJobSubscriptionStatus](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[SubscriptionId] [uniqueidentifier] NULL,
	--[ARMTemplateID] [uniqueidentifier] NULL,
	[SubscriptionStatus] [varchar](225) NULL,
	[Description] [varchar](max) NULL,
	[InsertDate] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
ALTER TABLE [dbo].[Subscriptions] ADD  CONSTRAINT [DF_Subscriptions_AMPSubscriptionId]  DEFAULT (newid()) FOR [AMPSubscriptionId]
GO
ALTER TABLE [dbo].[KnownUsers]  WITH CHECK ADD FOREIGN KEY([RoleId])
REFERENCES [dbo].[Roles] ([Id])
GO
ALTER TABLE [dbo].[MeteredAuditLogs]  WITH CHECK ADD FOREIGN KEY([SubscriptionId])
REFERENCES [dbo].[Subscriptions] ([Id])
GO
ALTER TABLE [dbo].[MeteredDimensions]  WITH CHECK ADD FOREIGN KEY([PlanId])
REFERENCES [dbo].[Plans] ([Id])
GO
ALTER TABLE [dbo].[SubscriptionAuditLogs]  WITH CHECK ADD FOREIGN KEY([SubscriptionID])
REFERENCES [dbo].[Subscriptions] ([Id])
GO
ALTER TABLE [dbo].[Subscriptions]  WITH CHECK ADD FOREIGN KEY([UserId])
REFERENCES [dbo].[Users] ([UserId])
GO
/****** Object:  StoredProcedure [dbo].[spGetOfferParameters]    Script Date: 05-15-2020 12.56.43 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
/*   
Exec spGetOfferParameters 'B8F4D276-15EB-4EB6-89D4-E600FF1098EF'  
*/  
CREATE Procedure [dbo].[spGetOfferParameters]  
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
,OA.Type
from [dbo].[OfferAttributes] OA  
left  join   
[dbo].[PlanAttributeMapping]  PA  
on OA.ID= PA.OfferAttributeID and OA.OfferId=@OfferId  
and  PA.PlanId=@PlanId  
where    
OA.Isactive=1   
  
END  

GO
/****** Object:  StoredProcedure [dbo].[spGetPlanEvents]    Script Date: 05-15-2020 12.56.44 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/*   
Exec spGetPlanEvents 'B8F4D276-15EB-4EB6-89D4-E600FF1098EF'  
*/  
CREATE Procedure [dbo].[spGetPlanEvents]  
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
,ISNULL(OEM.CopyToCustomer,0) CopyToCustomer
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

GO
/****** Object:  StoredProcedure [dbo].[spGetSubscriptionKeyValue]    Script Date: 05-15-2020 12.56.44 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE Procedure [dbo].[spGetSubscriptionKeyValue](      
@SubscriptionId UniqueIdentifier      
)      
AS      
Begin      
      
Declare @Subscription Varchar(225)       
Declare @Plan Varchar(225)       
Declare @PlanGuId UniqueIdentifier      
Declare @Offer Varchar(225)       
Declare @OfferGuId UniqueIdentifier      
      
set @Subscription = (select  top 1 Name from Subscriptions where AMPSubscriptionId=@SubscriptionId)      
set @Plan = (select  top 1 AMPPlanId from Subscriptions where  AMPSubscriptionId=@SubscriptionId)      
set @PlanGuId = (select  top 1 PlanGUID from Plans where  PlanId=@Plan)      
set @OfferGuId = (select  top 1 OfferID from Plans where  PlanId=@Plan)      
set @Offer = (select  top 1 OfferName from offers where OfferGUId= @OfferGuId)      
      
      
Create Table #Keyvalue (Id Int Identity(1,1),  [Key] Varchar(100), [Value] varchar(100))       
Insert into #Keyvalue      
Select 'Subscription' as [Key] ,Replace(@Subscription,' ','-')as [Value] Union all      
Select 'Plan' as [Key] , Replace(@Plan,' ','-') as [Value] Union all      
Select 'Offer' as [Key] ,Replace(@Offer,' ','-') as [Value]       
      
Select * from #Keyvalue      
      
END      
GO
/****** Object:  StoredProcedure [dbo].[spGetSubscriptionParameters]    Script Date: 05-15-2020 12.56.44 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
/*   
Exec spGetSubscriptionParameters '53ff9e28-a55b-65e1-ff75-709aec6420fd','a35d4259-f3c9-429b-a871-21c4593fa4bf'  
*/  
CREATE Procedure [dbo].[spGetSubscriptionParameters]  
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
,isnull(OA.IsRequired,0) IsRequired  
,ISNULL(Value,'')Value  
,ISNULL(SubscriptionId,@SubscriptionId) SubscriptionId  
,ISNULL(SAV.OfferID,OA.OfferId) OfferID  
,SAV.UserId  
,SAV.CreateDate  
,ISNULL(oA.FromList,0) FromList  
,ISNULL(OA.ValuesList,'') ValuesList  
,ISNULL(OA.Max,0) Max  
,ISNULL(OA.Min,0) Min
,ISNULL(VT.HTMLType,'') HTMLType  
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
  

GO
/****** Object:  StoredProcedure [dbo].[spGetSubscriptionTemplateParameters]    Script Date: 05-15-2020 12.56.44 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO



  /*
CREATE Procedure [dbo].[spGetSubscriptionTemplateParameters]  
(    
@SubscriptionId Uniqueidentifier,    
@PlanId Uniqueidentifier    
)    
AS    
BEGIN    
     
Declare @OfferId Uniqueidentifier     
Set @OfferId=(Select OfferId from Plans where PlanGuId =@PlanId )  
If exists(select 1 from SubscriptionTemplateParameters where AMPSubscriptionId=@SubscriptionId)
BEGIN
Select 
Id
,OfferName
,OfferGUId
,PlanGUID
,PlanId
--,ARMTemplateID
,Parameter
,ParameterDataType
,Value
,ParameterType
,EventId
,EventsName
,AMPSubscriptionId
,SubscriptionStatus
,SubscriptionName

 from SubscriptionTemplateParameters where AMPSubscriptionId=@SubscriptionId
 END
 ELSE
 BEGIN
SELECT      
Cast( ROW_NUMBER() OVER ( ORDER BY ART.ID) as Int)RowId    
,0 ID    
,ofr.OfferName  
,ofr.OfferGUId   
,pln.PlanGUID  
,pln.PlanId  
--,art.ARMTemplateID  
--,art.Parameter  
--,art.ParameterDataType  
--,art.Value  
--,art.ParameterType  
,PE.EventId  
,EV.EventsName  
,Sub.AMPSubscriptionId  
,Sub.SubscriptionStatus  
,sub.Name AS SubscriptionName  
from   
Offers ofr  
inner join Plans pln on ofr.OfferGUId=pln.OfferID  
inner join PlanEventsMapping PE on pln.PlanGUID=pe.PlanId  
--inner join ARMTemplateParameters ART on PE.ARMTemplateId=ART.ARMTemplateId   
inner Join Subscriptions Sub on pln.PlanId=Sub.AMPPlanId  
inner join [Events] Ev on EV.EventsId=PE.EventId  
 Where Sub.AMPSubscriptionId =@SubscriptionId  
 and EV.EventsName  ='Activate'
  END
   
  
END  
  */
  
GO
GO
/*Insert scripts*/

IF EXISTS(SELECT 1 FROM SYS.TABLES WHERE NAME ='ROLES')
BEGIN
	IF NOT EXISTS(SELECT 1 FROM ROLES WHERE NAME='PUBLISHERADMIN')
	BEGIN

	INSERT INTO ROLES
		SELECT 'PublisherAdmin'
	END
END
-- END
GO

GO
INSERT [dbo].[ApplicationConfiguration] ( [Name], [Value], [Description]) VALUES ( N'SMTPFromEmail', N'', N'SMTP Email')
GO
INSERT [dbo].[ApplicationConfiguration] ( [Name], [Value], [Description]) VALUES ( N'SMTPPassword', N'', N'SMTP Password')
GO
INSERT [dbo].[ApplicationConfiguration] ( [Name], [Value], [Description]) VALUES ( N'SMTPHost', N'', N'SMTP Host')
GO
INSERT [dbo].[ApplicationConfiguration] ( [Name], [Value], [Description]) VALUES ( N'SMTPPort', N'', N'SMTP Port')
GO
INSERT [dbo].[ApplicationConfiguration] ( [Name], [Value], [Description]) VALUES ( N'SMTPUserName', N'', N'SMTP User Name')
GO
INSERT [dbo].[ApplicationConfiguration] ( [Name], [Value], [Description]) VALUES ( N'SMTPSslEnabled', N'TRUE', N'SMTP Ssl Enabled')
GO
INSERT [dbo].[ApplicationConfiguration] ( [Name], [Value], [Description]) VALUES ( N'ApplicationName', N'Contoso', N'Application Name')
GO
INSERT [dbo].[ApplicationConfiguration] ( [Name], [Value], [Description]) VALUES ( N'IsEmailEnabledForSubscriptionActivation', N'True', N'Active Email Enabled')
GO
INSERT [dbo].[ApplicationConfiguration] ( [Name], [Value], [Description]) VALUES ( N'IsEmailEnabledForUnsubscription', N'True', N'Unsubscribe Email Enabled')
GO

IF NOT EXISTS (SELECT * FROM ApplicationConfiguration WHERE Name = 'IsAutomaticProvisioningSupported')
BEGIN
    INSERT INTO ApplicationConfiguration (Name,Value,Description)
    SELECT 'IsAutomaticProvisioningSupported','false','Skip Activation - Automatic Provisioning Supported'
END
GO

IF NOT EXISTS (SELECT * FROM ApplicationConfiguration WHERE Name = 'IsEmailEnabledForPendingActivation')
BEGIN
    INSERT INTO ApplicationConfiguration (Name,Value,Description)
    SELECT 'IsEmailEnabledForPendingActivation','false','Email Enabled For Pending Activation'
END
GO
INSERT INTO [DatabaseVersionHistory] 

Select 1.0, 'Master Schema',Getdate(), 'DB User'

GO


Insert into Events
Select 'Activate' , 1, Getdate() UNION ALL
Select 'Unsubscribe' , 1, Getdate() UNION ALL
Select 'Pending Activation', 1,Getdate()

GO

INSERT INTO [dbo].[ValueTypes]
Select 'Int',Getdate(), 'int' UNION ALL
select 'String', Getdate(),'string' UNION ALL
select 'Date', Getdate() ,'date'
GO


Insert into EmailTemplate (Status,Description,InsertDate,TemplateBody,Subject,ToRecipients,CC,BCC,IsActive)
SELECT 'Template','Template',GETDATE(),'<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">                     
<html xmlns="http://www.w3.org/1999/xhtml">
   <head>
      <!--<note>Use this Template for Pending Activation email </note>-->                                                                            
      <meta http-equiv="Content-Type" content="text/html; charset=UTF-8" />
      <!--<title>Cloud Market Place - Your registration is successful</title>-->                                                                              
      <style type="text/css">          /* /\/\/\/\/\/\/\/\/ CLIENT-SPECIFIC STYLES /\/\/\/\/\/\/\/\/ */ #outlook a {              padding: 0;          }          /* Force Outlook to provide a view in browser message */            .ReadMsgBody {              width: 100%;          }            .ExternalClass {              width: 100%;          }              /* Force Hotmail to display emails at full width */                .ExternalClass, .ExternalClass p, .ExternalClass span, .ExternalClass font, .ExternalClass td, .ExternalClass div {                  line-height: 100%;              }          /* Force Hotmail to display normal line spacing */            body, table, td, p, a, li, blockquote {              -webkit-text-size-adjust: 100%;              -ms-text-size-adjust: 100%;          }          /* Prevent WebKit and Windows mobile changing default text sizes */            table, td {              mso-table-lspace: 0pt;              mso-table-rspace: 0pt;          }          /* Remove spacing between tables in Outlook 2007 and up */            img {              -ms-interpolation-mode: bicubic;          }          /* Allow smoother rendering of resized image in Internet Explorer */ /* /\/\/\/\/\/\/\/\/ RESET STYLES /\/\/\/\/\/\/\/\/ */            body {              margin: 0;              padding: 0;          }            img {              border: 0;              height: auto;              line-height: 100%;              outline: none;              text-decoration: none;          }            table {              border-collapse: collapse !important;          }            body, #bodyTable, #bodyCell {              height: 100% !important;              margin: 0;              padding: 0;              width: 100% !important;          }          /* /\/\/\/\/\/\/\/\/ TEMPLATE STYLES /\/\/\/\/\/\/\/\/ */ /* ========== Page Styles ========== */            #bodyCell {              padding: 20px;          }            #templateContainer {              width: 600px;          }          /*** @tab Page* @section background style* @tip Set the background color and top border for your email. You may want to choose colors that match your companys branding.* @theme page*/            body, #bodyTable { /*@editable*/              background-color: #FFFFFF;          }          /*** @tab Page* @section background style* @tip Set the background color and top border for your email. You may want to choose colors that match your companys branding.* @theme page*/            #bodyCell { /*@editable*/              border-top: 4px solid #FFFFFF;          }          /*** @tab Page* @section email border* @tip Set the border for your email.*/            #templateContainer { /*@editable*/              border: 1px solid #BBBBBB;          }          /*** @tab Page* @section heading 1* @tip Set the styling for all first-level headings in your emails. These should be the largest of your headings.* @style heading 1*/            h1 { /*@editable*/              color: #202020 !important;              display: block; /*@editable*/              font-family: "Segoe UI", Tahoma,Sans-serif; /*@editable*/              font-size: 26px; /*@editable*/              font-style: normal; /*@editable*/              font-weight: bold; /*@editable*/              line-height: 100%; /*@editable*/              letter-spacing: normal;              margin-top: 0;              margin-right: 0;              margin-bottom: 10px;              margin-left: 0; /*@editable*/              text-align: left;          }          /*** @tab Page* @section heading 2* @tip Set the styling for all second-level headings in your emails.* @style heading 2*/            h2 { /*@editable*/              color: #404040 !important;              display: block; /*@editable*/              font-family: "Segoe UI", Tahoma,Sans-serif; /*@editable*/              font-size: 20px; /*@editable*/              font-style: normal; /*@editable*/              font-weight: bold; /*@editable*/              line-height: 100%; /*@editable*/              letter-spacing: normal;              margin-top: 0;              margin-right: 0;              margin-bottom: 10px;              margin-left: 0; /*@editable*/              text-align: left;          }          /*** @tab Page* @section heading 3* @tip Set the styling for all third-level headings in your emails.* @style heading 3*/            h3 { /*@editable*/              color: #606060 !important;              display: block; /*@editable*/              font-family: "Segoe UI", Tahoma,Sans-serif; /*@editable*/              font-size: 16px; /*@editable*/              font-style: italic; /*@editable*/              font-weight: normal; /*@editable*/              line-height: 100%; /*@editable*/              letter-spacing: normal;              margin-top: 0;              margin-right: 0;              margin-bottom: 10px;              margin-left: 0; /*@editable*/              text-align: left;          }          /*** @tab Page* @section heading 4* @tip Set the styling for all fourth-level headings in your emails. These should be the smallest of your headings.* @style heading 4*/            h4 { /*@editable*/              color: #000000 !important;              display: block; /*@editable*/              font-family: "Segoe UI", Tahoma,Sans-serif; /*@editable*/              font-size: 14px; /*@editable*/              font-style: italic; /*@editable*/              font-weight: normal; /*@editable*/              line-height: 100%; /*@editable*/              letter-spacing: normal;              margin-top: 0;              margin-right: 0;              margin-bottom: 10px;              margin-left: 0; /*@editable*/              text-align: left;          }          /* ========== Header Styles ========== */ /*** @tab Header* @section preheader style* @tip Set the background color and bottom border for your emails preheader area.* @theme header*/            #templatePreheader { /*@editable*/              background-color: #FFFFFF; /*@editable*/              border-bottom: 1px solid #CCCCCC;          }          /*** @tab Header* @section preheader text* @tip Set the styling for your emails preheader text. Choose a size and color that is easy to read.*/            .preheaderContent { /*@editable*/              color: #000000; /*@editable*/              font-family: wf_segoe-ui_normal, "Segoe UI", "Segoe WP", Tahoma, Arial, sans-serif; /*@editable*/              font-size: 10px; /*@editable*/              line-height: 125%; /*@editable*/              text-align: left;          }              /*** @tab Header* @section preheader link* @tip Set the styling for your emails preheader links. Choose a color that helps them stand out from your text.*/                .preheaderContent a:link, .preheaderContent a:visited, /* Yahoo! Mail Override */ .preheaderContent a .yshortcuts /* Yahoo! Mail Override */ { /*@editable*/                  color: #606060; /*@editable*/                  font-weight: normal; /*@editable*/                  text-decoration: underline;              }          /*** @tab Header* @section header style* @tip Set the background color and borders for your emails header area.* @theme header*/            #templateHeader { /*@editable*/              background-color: #FFFFFF; /*@editable*/              border-top: 1px solid #FFFFFF; /*@editable*/              border-bottom: 1px solid #CCCCCC;          }          /*** @tab Header* @section header text* @tip Set the styling for your emails header text. Choose a size and color that is easy to read.*/            .headerContent { /*@editable*/              color: #505050; /*@editable*/              font-family: "Segoe UI", Tahoma,Sans-serif; /*@editable*/              font-size: 20px; /*@editable*/              font-weight: bold; /*@editable*/              line-height: 100%; /*@editable*/              padding-top: 0; /*@editable*/              padding-right: 0; /*@editable*/              padding-bottom: 0; /*@editable*/              padding-left: 0; /*@editable*/              text-align: left; /*@editable*/              vertical-align: middle;          }              /*** @tab Header* @section header link* @tip Set the styling for your emails header links. Choose a color that helps them stand out from your text.*/                .headerContent a:link, .headerContent a:visited, /* Yahoo! Mail Override */ .headerContent a .yshortcuts /* Yahoo! Mail Override */ { /*@editable*/                  color: #EB4102; /*@editable*/                  font-weight: normal; /*@editable*/                  text-decoration: underline;              }            #headerImage {              height: auto;              max-width: 600px;          }          /* ========== Body Styles ========== */ /*** @tab Body* @section body style* @tip Set the background color and borders for your emails body area.*/            #templateBody { /*@editable*/              background-color: #FFFFFF; /*@editable*/              border-top: 1px solid #FFFFFF; /*@editable*/              border-bottom: 1px solid #CCCCCC;          }          /*** @tab Body* @section body text* @tip Set the styling for your emails main content text. Choose a size and color that is easy to read.* @theme main*/            .bodyContent { /*@editable*/              color: #505050; /*@editable*/              font-family: "Segoe UI", Tahoma,Sans-serif; /*@editable*/              font-size: 16px; /*@editable*/              line-height: 150%;              padding-top: 20px;              padding-right: 20px;              padding-bottom: 20px;              padding-left: 20px; /*@editable*/              text-align: left;          }              /*** @tab Body* @section body link* @tip Set the styling for your emails main content links. Choose a color that helps them stand out from your text.*/                .bodyContent a:link, .bodyContent a:visited, /* Yahoo! Mail Override */ .bodyContent a .yshortcuts /* Yahoo! Mail Override */ { /*@editable*/                  color: #EB4102; /*@editable*/                  font-weight: normal; /*@editable*/                  text-decoration: underline;              }                .bodyContent img {                  display: inline;                  height: auto;                  max-width: 560px;              }          /* ========== Column Styles ========== */            .templateColumnContainer {              display: inline;              width: 260px;          }          /*** @tab Columns* @section column style* @tip Set the background color and borders for your emails column area.*/            #templateColumns { /*@editable*/              background-color: #FFFFFF; /*@editable*/              border-top: 1px solid #FFFFFF; /*@editable*/              border-bottom: 1px solid #CCCCCC;          }          /*** @tab Columns* @section left column text* @tip Set the styling for your emails left column content text. Choose a size and color that is easy to read.*/            .leftColumnContent { /*@editable*/              color: #505050; /*@editable*/              font-family: "Segoe UI", Tahoma,Sans-serif; /*@editable*/              font-size: 14px; /*@editable*/              line-height: 150%;              padding-top: 0;              padding-right: 0;              padding-bottom: 20px;              padding-left: 0; /*@editable*/              text-align: left;          }              /*** @tab Columns* @section left column link* @tip Set the styling for your emails left column content links. Choose a color that helps them stand out from your text.*/                .leftColumnContent a:link, .leftColumnContent a:visited, /* Yahoo! Mail Override */ .leftColumnContent a .yshortcuts /* Yahoo! Mail Override */ { /*@editable*/                  color: #EB4102; /*@editable*/                  font-weight: normal; /*@editable*/                  text-decoration: underline;              }          /*** @tab Columns* @section right column text* @tip Set the styling for your emails right column content text. Choose a size and color that is easy to read.*/            .rightColumnContent { /*@editable*/              color: #505050; /*@editable*/              font-family: "Segoe UI", Tahoma,Sans-serif; /*@editable*/              font-size: 14px; /*@editable*/              line-height: 150%;              padding-top: 0;              padding-right: 0;              padding-bottom: 20px;              padding-left: 0; /*@editable*/              text-align: left;          }              /*** @tab Columns* @section right column link* @tip Set the styling for your emails right column content links. Choose a color that helps them stand out from your text.*/                .rightColumnContent a:link, .rightColumnContent a:visited, /* Yahoo! Mail Override */ .rightColumnContent a .yshortcuts /* Yahoo! Mail Override */ { /*@editable*/                  color: #EB4102; /*@editable*/                  font-weight: normal; /*@editable*/                  text-decoration: underline;              }                .leftColumnContent img, .rightColumnContent img {                  display: inline;                  height: auto;                  max-width: 260px;              }          /* ========== Footer Styles ========== */ /*** @tab Footer* @section footer style* @tip Set the background color and borders for your emails footer area.* @theme footer*/            #templateFooter { /*@editable*/              background-color: #FFFFFF; /*@editable*/              border-top: 1px solid #FFFFFF;          }          /*** @tab Footer* @section footer text* @tip Set the styling for your emails footer text. Choose a size and color that is easy to read.* @theme footer*/            .footerContent { /*@editable*/              color: #808080; /*@editable*/              font-family: "Segoe UI", Tahoma,Sans-serif; /*@editable*/              font-size: 10px; /*@editable*/              line-height: 150%;              padding-top: 20px;              padding-right: 20px;              padding-bottom: 20px;              padding-left: 20px; /*@editable*/              text-align: left;          }              /*** @tab Footer* @section footer link* @tip Set the styling for your emails footer links. Choose a color that helps them stand out from your text.*/                .footerContent a:link, .footerContent a:visited, /* Yahoo! Mail Override */ .footerContent a .yshortcuts, .footerContent a span /* Yahoo! Mail Override */ { /*@editable*/                  color: #606060; /*@editable*/                  font-weight: normal; /*@editable*/                  text-decoration: underline;              }          /* /\/\/\/\/\/\/\/\/ MOBILE STYLES /\/\/\/\/\/\/\/\/ */            @media only screen and (max-width: 480px) { /* /\/\/\/\/\/\/ CLIENT-SPECIFIC MOBILE STYLES /\/\/\/\/\/\/ */                body, table, td, p, a, li, blockquote {                  -webkit-text-size-adjust: none !important;              }              /* Prevent Webkit platforms from changing default text sizes */                body {                  width: 100% !important;                  min-width: 100% !important;              }              /* Prevent iOS Mail from adding padding to the body */ /* /\/\/\/\/\/\/ MOBILE RESET STYLES /\/\/\/\/\/\/ */                #bodyCell {                  padding: 10px !important;              }              /* /\/\/\/\/\/\/ MOBILE TEMPLATE STYLES /\/\/\/\/\/\/ */ /* ======== Page Styles ======== */ /*** @tab Mobile Styles* @section template width* @tip Make the template fluid for portrait or landscape view adaptability. If a fluid layout doesnt work for you, set the width to 300px instead.*/                #templateContainer {                  max-width: 600px !important; /*@editable*/                  width: 100% !important;              }              /*** @tab Mobile Styles* @section heading 1* @tip Make the first-level headings larger in size for better readability on small screens.*/                h1 { /*@editable*/                  font-size: 24px !important; /*@editable*/                  line-height: 100% !important;              }              /*** @tab Mobile Styles* @section heading 2* @tip Make the second-level headings larger in size for better readability on small screens.*/                h2 { /*@editable*/                  font-size: 20px !important; /*@editable*/                  line-height: 100% !important;              }              /*** @tab Mobile Styles* @section heading 3* @tip Make the third-level headings larger in size for better readability on small screens.*/                h3 { /*@editable*/                  font-size: 18px !important; /*@editable*/                  line-height: 100% !important;              }              /*** @tab Mobile Styles* @section heading 4* @tip Make the fourth-level headings larger in size for better readability on small screens.*/                h4 { /*@editable*/                  font-size: 16px !important; /*@editable*/                  line-height: 100% !important;              }              /* ======== Header Styles ======== */                #templatePreheader {                  display: none !important;              }              /* Hide the template preheader to save space */ /*** @tab Mobile Styles* @section header image* @tip Make the main header image fluid for portrait or landscape view adaptability, and set the images original width as the max-width. If a fluid setting doesnt work, set the image width to half its original size instead.*/                #headerImage {                  height: auto !important; /*@editable*/                  max-width: 600px !important; /*@editable*/                  width: 100% !important;              }              /*** @tab Mobile Styles* @section header text* @tip Make the header content text larger in size for better readability on small screens. We recommend a font size of at least 16px.*/                .headerContent { /*@editable*/                  font-size: 20px !important; /*@editable*/                  line-height: 125% !important;              }              /* ======== Body Styles ======== */ /*** @tab Mobile Styles* @section body image* @tip Make the main body image fluid for portrait or landscape view adaptability, and set the images original width as the max-width. If a fluid setting doesnt work, set the image width to half its original size instead.*/                #bodyImage {                  height: auto !important; /*@editable*/                  max-width: 560px !important; /*@editable*/                  width: 100% !important;              }              /*** @tab Mobile Styles* @section body text* @tip Make the body content text larger in size for better readability on small screens. We recommend a font size of at least 16px.*/                .bodyContent { /*@editable*/                  font-size: 18px !important; /*@editable*/                  line-height: 125% !important;              }              /* ======== Column Styles ======== */                .templateColumnContainer {                  display: block !important;                  width: 100% !important;              }              /*** @tab Mobile Styles* @section column image* @tip Make the column image fluid for portrait or landscape view adaptability, and set the images original width as the max-width. If a fluid setting doesnt work, set the image width to half its original size instead.*/                .columnImage {                  height: auto !important; /*@editable*/                  max-width: 260px !important; /*@editable*/                  width: 100% !important;              }              /*** @tab Mobile Styles* @section left column text* @tip Make the left column content text larger in size for better readability on small screens. We recommend a font size of at least 16px.*/                .leftColumnContent { /*@editable*/                  font-size: 16px !important; /*@editable*/                  line-height: 125% !important;              }              /*** @tab Mobile Styles* @section right column text* @tip Make the right column content text larger in size for better readability on small screens. We recommend a font size of at least 16px.*/                .rightColumnContent { /*@editable*/                  font-size: 16px !important; /*@editable*/                  line-height: 125% !important;              }              /* ======== Footer Styles ======== */ /*** @tab Mobile Styles* @section footer text* @tip Make the body content text larger in size for better readability on small screens.*/                .footerContent { /*@editable*/                  font-size: 14px !important; /*@editable*/                  line-height: 115% !important;              }                    .footerContent a {                      display: block !important;                  }              /* Place footer social and utility links on their own lines, for easier access */          }      </style>
   </head>
   <body leftmargin="0" marginwidth="0" topmargin="0" marginheight="0" offset="0">
      <center>
         <table align="center" border="0" cellpadding="0" cellspacing="0" height="100%" width="100%" id="bodyTable">
            <tr>
               <td align="center" valign="top" id="bodyCell">
                  <!-- BEGIN TEMPLATE // -->                                                                                                                                                                                               
                  <table border="0" cellpadding="0" cellspacing="0" id="templateContainer">
                     <tr>
                        <td align="center" valign="top">
                           <!-- BEGIN HEADER // -->                                                                                                                                                                                                                                                                                      
                           <table border="0" cellpadding="0" cellspacing="0" width="100%" id="templateHeader">
                              <tr>
                                 <td valign="top"  class="headerContent">                                              <img src="https://media-exp1.licdn.com/dms/image/C510BAQHaTIBZfLFCGg/company-logo_200_200/0?e=1592438400&v=beta&t=AHOqaRJR_Thf72VDOKQId2QKdwepIp-sIiSIDtALzhQ" style="max-width: 300px; display: block; margin-left: auto; margin-right: auto; padding-top:10px;padding-bottom:10px;" id="headerImage" />                                          </td>
                              </tr>
                           </table>
                           <!-- // END HEADER -->                                                                                                                                                                                                                                                       
                        </td>
                     </tr>
                     <tr>
                        <td align="center" valign="top">
                           <!-- BEGIN BODY // -->                                                                                                                                                                                                                                                                                      
                           <table border="0" cellpadding="0" cellspacing="0" width="100%" id="templateBody">
                              <tr>
                                 <td valign="top" class="bodyContent">
                                    <h1 style="display: block;  margin-left: auto;  margin-right: auto;  width: 75%;">Welcome to ${ApplicationName}!</h1>
                                    #if(${processStatus} == "failure")                                                                                                                                                         
                                    <p>Your request for the subscription has been failed.</p>
                                    #elseif(${SaasSubscriptionStatus} == "PendingActivation")                                                                                                                                                        
                                    <p>A request for purchase with the following details is awaiting your action for activation.                                       </p>
                                    #elseif(${SaasSubscriptionStatus} == "Subscribed")                                                                                                                                                        
                                    <p>Your request for the purchase has been approved.                                       </p>
                                    #elseif(${SaasSubscriptionStatus} == "Unsubscribed")                                                                                                                                                         
                                    <p>A subscription with the following details was deleted from Azure.</p>
                                    #end                                                                                                                                                                              
                                    <table border="0" cellpadding="0" cellspacing="0" width="100%" id="templateBody">
                                       <tr>
                                          <td><b>Customer Email Address</b></td>
                                          <td>${CustomerEmailAddress}</td>
                                       </tr>
                                       <tr>
                                          <td><b>Customer Name</b></td>
                                          <td>${CustomerName}</td>
                                       </tr>
                                       <tr>
                                          <td><b>ID</b></td>
                                          <td>${Id}</td>
                                       </tr>
                                       <tr>
                                          <td><b>Subscription Name</b></td>
                                          <td>${SubscriptionName}</td>
                                       </tr>
                                       <tr>
                                          <td><b>Status</b></td>
                                          <td>${SaasSubscriptionStatus}</td>
                                       </tr>
                                       <tr>
                                          <td><b>Purchaser Email</b></td>
                                          <td>${PurchaserEmail}</td>
                                       </tr>
                                       <tr>
                                          <td><b>Purchaser Tenant ID</b></td>
                                          <td>${PurchaserTenant}</td>
                                       </tr>
                                       #foreach( $prod in $parms )                                                                                                                                                                                                                         
                                       <tr>
                                          <td><b>$prod.DisplayName</b></td>
                                          <td>$prod.Value</td>
                                       </tr>
                                       #end                                                                                                                                                                                                                                              
                                    </table>
                                    <p style=" margin-left: auto; margin-right: auto; text-align:right;">                                                  <a href="https://saaskitdemoapp.azurewebsites.net/"><button style="background-color:#2168A6;line-height:30px;color:white"><b>View Details</b></button></a>                                              </p>
                                    <!--     CTA button -->                                              <!--<table style="background: #0078D7;" cellspacing="0" cellpadding="0" align="left">                                                  <tbody>                                                      <tr>                                                          <td style="padding-left: 15px; font-size: 18px; line-height: 20px; font-family:"Segoe UI Light"; color: #ffffff;">                                                              <a style="text-decoration: none; font-size: 18px; line-height: 20px; font-family:"Segoe UI Light"; color: #ffffff;" href="${LinkToPortal}/#/login">${LoginButtonTextInEmail}</a>                                                          </td>                                                          <td style="line-height: 1px; font-size: 1px; padding: 10px;">                                                              <a>                                                                  <img src="https://info.microsoft.com/rs/157-GQE-382/images/Program-CTAButton-whiteltr.png" border="0" alt="" height="20" />                                                              </a>                                                          </td>                                                      </tr>                                                  </tbody>                                              </table>-->                                              <!--     end CTA button -->                                                                                                                                                                                                                                                                                                                                              
                                 </td>
                              </tr>
                              <tr>
                                 <td align="center" valign="top">
                                    <!-- BEGIN PREHEADER // -->                                                                                                                                                                                                                                                                                                                                                                          
                                    <table border="0" cellpadding="0" cellspacing="0" width="100%" id="templatePreheader">
                                       <tr>
                                          <td valign="top" class="preheaderContent" style="padding-top: 10px; padding-right: 20px; padding-bottom: 10px; padding-left: 20px;">                                                          You are receiving this message because of an interaction with                                                          <a href="https://saaskitdemoapp.azurewebsites.net/">Contoso</a>.                                                          Please contact us at <a href="https://saaskitdemoapp.azurewebsites.net/">Contoso</a>                                                          in case you think you have received this message in error or need help.                                                      </td>
                                          <!-- *|IFNOT:ARCHIVE_PAGE|* -->                                                      <!-- *|END:IF|* -->                                                                                                                                                                                                                                                                                                                                                                                                         
                                       </tr>
                                    </table>
                                    <!-- // END PREHEADER -->                                                                                                                                                                                                                                                                                                                                              
                                 </td>
                              </tr>
                           </table>
                        </td>
                     </tr>
                  </table>
                  <!-- // END BODY -->                                                                                                                                                                   
               </td>
            </tr>
         </table>
         <!-- // END TEMPLATE -->                                                                              
      </center>
   </body>
</html>','','','','',1

GO

GO
Create Table SubscriptionEmailOutput
(
Id Int identity(1,1)
,Name varchar(225)
,Value varchar(MAX)
)


GO


CREATE Procedure spGenerateEmailHtml
(
@subscriptionId varchar(225),
@processStatus varchar(225) 
)
AS
BEGIN
declare @html varchar(max) =(select TemplateBody from [EmailTemplate] where status='SQL Template')

DECLARE 
  @planId varchar(225)
, @planGUId varchar(225)
, @planName varchar(225)
, @offerId varchar(225)
, @offerGUId varchar(225)
, @subscriptionStatus varchar(225)
, @subscriptionName varchar(225)
, @offerName varchar(225)
, @customerName  varchar(225)
, @customerEmailAddress  varchar(225)
, @purchaserEmial  varchar(225)
, @purchaserTenant  varchar(225)
, @UserId int

Declare @applicationName Varchar(225) =(select [value] from [ApplicationConfiguration] where [Name]='ApplicationName')
Declare @welcomeText varchar(MAX)=''
Declare @tablehtml varchar(MAX)='<table border="0" cellpadding="0" cellspacing="0" width="100%" id="templateBody">${InnerHtml} </table>'
Declare @arminputparms  varchar(MAX)=''
Declare @armoutputparms  varchar(MAX)=''

IF EXISTS (SELECT 1 FROM SUBSCRIPTIONS WHERE AMPSubscriptionId=@subscriptionId)
BEGIN
	select  @planId =AMPPLanId,
	@subscriptionStatus =subscriptionstatus,
	@subscriptionName= [Name],
	@purchaserEmial=PurchaserEmail,
	@purchaserTenant=PurchaserTenantId,
	@UserId = UserId
	FROM SUBSCRIPTIONS WHERE AMPSubscriptionId=@subscriptionId
	
	 select 
	@customerName =FullName,
	@customerEmailAddress= EmailAddress
	FROM USERS WHERE USERID=@UserId


	IF EXISTS (SELECT 1 FROM PLANS WHERE PLANID=@planId)
	BEGIN
		SELECT @offerGUId = OFFERID,
				@planName= DISPLAYNAME,
				@planGUId=PlanGUID
		 FROM PLANS WHERE  PLANID=@planId
		
		IF EXISTS (SELECT 1 FROM OFFERS WHERE OFFERGUID=@offerGUId)
		BEGIN
			Select @offerId = OFFERID ,
					@offerName=OFFERNAME 
			FROM OFFERS WHERE  OFFERGUID=@offerGUId
		END
	END
END

	


Create Table #Temp(HtmlLabel varchar(max), HtmlValue varchar(max))
Insert into #Temp

select 'Customer Email Address',@customerEmailAddress	  UNION ALL
select 'Customer Name',@customerName					  UNION ALL
select 'SaaS Subscription Id',@subscriptionId			  UNION ALL
select 'SaaS Subscription Name',@subscriptionName		  UNION ALL
select 'SaaS Subscription Status',@subscriptionStatus	  UNION ALL
select 'Plan',@planName									  UNION ALL
select 'Purchaser Email Address',@customerEmailAddress	  UNION ALL
select 'Purchaser Tenant',@purchaserTenant				  UNION ALL	  
-- Parameters
select 
ISNULL(OA.DisplayName,'') DisplayName, ISNULL(Value,'')Value    
from      [dbo].[OfferAttributes] OA    
Inner  join     
[dbo].[PlanAttributeMapping]  PA    
on OA.ID= PA.OfferAttributeID and OA.OfferId=@OfferguId    
and  PA.PlanId=@PlanguId    
INNER Join  SubscriptionAttributeValues SAV    
on SAV.PlanAttributeId= PA.PlanAttributeId    
and SAV.SubscriptionId=@SubscriptionId    
where OA.Isactive=1   and PA.IsEnabled=1 	


-- Cursor Begin
Declare @subscriptionContent VARCHAR(MAX)=''

DECLARE 
    @htmlLabel VARCHAR(MAX), 
    @htmlValue   VARCHAR(MAX)

DECLARE cursor_html CURSOR
FOR SELECT 
        htmlLabel, 
        HtmlValue
    FROM 
        #Temp

OPEN cursor_html;

FETCH NEXT FROM cursor_html INTO 
    @htmlLabel, 
    @htmlValue;

WHILE @@FETCH_STATUS = 0
    BEGIN
       
	   set @subscriptionContent = @subscriptionContent + '<tr><td><b>'+ @htmlLabel+'</b></td> <td>' + @htmlValue + '</td> </tr> </br>'

        FETCH NEXT FROM cursor_html INTO 
            @htmlLabel, 
    @htmlValue;
    END;

CLOSE cursor_html;

DEALLOCATE cursor_html;

-- Cursor End

-- Welcome text

IF (@processStatus ='failure')
	BEGIN
		set @welcomeText= 'Your request for the subscription has been failed.'
	END

IF (@processStatus ='success')	
 BEGIN
	IF (@subscriptionStatus= 'PendingActivation')
 	  BEGIN
		set @welcomeText= 'A request for purchase with the following details is awaiting your action for activation.'
	  END
	IF (@subscriptionStatus= 'Subscribed')
 	  BEGIN
		set @welcomeText= 'Your request for the purchase has been approved.'
	  END
	IF (@subscriptionStatus= 'Unsubscribed')
 	  BEGIN
		set @welcomeText= 'A subscription with the following details was deleted from Azure.'
	  END	
END


 select  @html=REPLACE(@html,'${subscriptiontable}',@subscriptionContent)
		,@html=REPLACE(@html,'${welcometext}',@welcomeText)
		,@html=REPLACE(@html,'${ApplicationName}',@applicationName)
		,@html=REPLACE(@html,'${arminputparms}',@arminputparms)
		,@html=REPLACE(@html,'${armoutputparms}',@armoutputparms)
		 
		

 select 1 AS ID,'Email' AS [Name], @html as [Value]


/* test values
 --select @subscriptionContent
 -- select * from #Temp

SELECT 
  @subscriptionId		  as 'subscriptionId'
, @processStatus		  as 'processStatus'
, @planId 				  as 'planId'
, @offerId 				  as 'offerId'
, @offerGUId 			  as 'offerGUId'
, @subscriptionStatus 	  as 'subscriptionStatus'
, @subscriptionName 	  as 'subscriptionName'
, @planName 			  as 'planName'
, @offerName 			  as 'offerName'
, @customerName  		  as 'customerName'
, @customerEmailAddress   as 'customerEmailAddress'
, @purchaserEmial  		  as 'purchaserEmial'
, @purchaserTenant  	  as 'purchaserTenant'
, @UserId 				  as 'UserId'
, @applicationName		  as 'applicationName'
, @welcomeText			  as 'welcomeText'
, @tablehtml			  as 'tablehtml'
*/

End


GO

/****** Object:  Table [dbo].[SubscriptionEmailOutput]    Script Date: 05-17-2020 6.17.50 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[SubscriptionEmailOutput](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](225) NULL,
	[Value] [varchar](max) NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO




GO


Insert into [EmailTemplate]
Select '','',GetDate(),'<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">  <html xmlns="http://www.w3.org/1999/xhtml">  <head>      <!--<note>Use this Template for Pending Activation email </note>-->      <meta http-equiv="Content-Type" content="text/html; charset=UTF-8" />        <!--<title>Cloud Market Place - Your registration is successful</title>-->      <style type="text/css">          /* /\/\/\/\/\/\/\/\/ CLIENT-SPECIFIC STYLES /\/\/\/\/\/\/\/\/ */            #outlook a {              padding: 0;          }          /* Force Outlook to provide a view in browser message */            .ReadMsgBody {              width: 100%;          }            .ExternalClass {              width: 100%;          }              /* Force Hotmail to display emails at full width */                .ExternalClass, .ExternalClass p, .ExternalClass span, .ExternalClass font, .ExternalClass td, .ExternalClass div {                  line-height: 100%;              }          /* Force Hotmail to display normal line spacing */            body, table, td, p, a, li, blockquote {              -webkit-text-size-adjust: 100%;              -ms-text-size-adjust: 100%;          }          /* Prevent WebKit and Windows mobile changing default text sizes */            table, td {              mso-table-lspace: 0pt;              mso-table-rspace: 0pt;          }          /* Remove spacing between tables in Outlook 2007 and up */            img {              -ms-interpolation-mode: bicubic;          }          /* Allow smoother rendering of resized image in Internet Explorer */ /* /\/\/\/\/\/\/\/\/ RESET STYLES /\/\/\/\/\/\/\/\/ */            body {              margin: 0;              padding: 0;          }            img {              border: 0;              height: auto;              line-height: 100%;              outline: none;              text-decoration: none;          }            table {              border-collapse: collapse !important;          }            body, #bodyTable, #bodyCell {              height: 100% !important;              margin: 0;              padding: 0;              width: 100% !important;          }          /* /\/\/\/\/\/\/\/\/ TEMPLATE STYLES /\/\/\/\/\/\/\/\/ */ /* ========== Page Styles ========== */            #bodyCell {              padding: 20px;          }            #templateContainer {              width: 600px;          }          /*** @tab Page* @section background style* @tip Set the background color and top border for your email. You may want to choose colors that match your companys branding.* @theme page*/            body, #bodyTable { /*@editable*/              background-color: #FFFFFF;          }          /*** @tab Page* @section background style* @tip Set the background color and top border for your email. You may want to choose colors that match your companys branding.* @theme page*/            #bodyCell { /*@editable*/              border-top: 4px solid #FFFFFF;          }          /*** @tab Page* @section email border* @tip Set the border for your email.*/            #templateContainer { /*@editable*/              border: 1px solid #BBBBBB;          }          /*** @tab Page* @section heading 1* @tip Set the styling for all first-level headings in your emails. These should be the largest of your headings.* @style heading 1*/            h1 { /*@editable*/              color: #202020 !important;              display: block; /*@editable*/              font-family: "Segoe UI", Tahoma,Sans-serif; /*@editable*/              font-size: 26px; /*@editable*/              font-style: normal; /*@editable*/              font-weight: bold; /*@editable*/              line-height: 100%; /*@editable*/              letter-spacing: normal;              margin-top: 0;              margin-right: 0;              margin-bottom: 10px;              margin-left: 0; /*@editable*/              text-align: left;          }          /*** @tab Page* @section heading 2* @tip Set the styling for all second-level headings in your emails.* @style heading 2*/            h2 { /*@editable*/              color: #404040 !important;              display: block; /*@editable*/              font-family: "Segoe UI", Tahoma,Sans-serif; /*@editable*/              font-size: 20px; /*@editable*/              font-style: normal; /*@editable*/              font-weight: bold; /*@editable*/              line-height: 100%; /*@editable*/              letter-spacing: normal;              margin-top: 0;              margin-right: 0;              margin-bottom: 10px;              margin-left: 0; /*@editable*/              text-align: left;          }          /*** @tab Page* @section heading 3* @tip Set the styling for all third-level headings in your emails.* @style heading 3*/            h3 { /*@editable*/              color: #606060 !important;              display: block; /*@editable*/              font-family: "Segoe UI", Tahoma,Sans-serif; /*@editable*/              font-size: 16px; /*@editable*/              font-style: italic; /*@editable*/              font-weight: normal; /*@editable*/              line-height: 100%; /*@editable*/              letter-spacing: normal;              margin-top: 0;              margin-right: 0;              margin-bottom: 10px;              margin-left: 0; /*@editable*/              text-align: left;          }          /*** @tab Page* @section heading 4* @tip Set the styling for all fourth-level headings in your emails. These should be the smallest of your headings.* @style heading 4*/            h4 { /*@editable*/              color: #000000 !important;              display: block; /*@editable*/              font-family: "Segoe UI", Tahoma,Sans-serif; /*@editable*/              font-size: 14px; /*@editable*/              font-style: italic; /*@editable*/              font-weight: normal; /*@editable*/              line-height: 100%; /*@editable*/              letter-spacing: normal;              margin-top: 0;              margin-right: 0;              margin-bottom: 10px;              margin-left: 0; /*@editable*/              text-align: left;          }          /* ========== Header Styles ========== */ /*** @tab Header* @section preheader style* @tip Set the background color and bottom border for your emails preheader area.* @theme header*/            #templatePreheader { /*@editable*/              background-color: #FFFFFF; /*@editable*/              border-bottom: 1px solid #CCCCCC;          }          /*** @tab Header* @section preheader text* @tip Set the styling for your emails preheader text. Choose a size and color that is easy to read.*/            .preheaderContent { /*@editable*/              color: #000000; /*@editable*/              font-family: wf_segoe-ui_normal, "Segoe UI", "Segoe WP", Tahoma, Arial, sans-serif; /*@editable*/              font-size: 10px; /*@editable*/              line-height: 125%; /*@editable*/              text-align: left;          }              /*** @tab Header* @section preheader link* @tip Set the styling for your emails preheader links. Choose a color that helps them stand out from your text.*/                .preheaderContent a:link, .preheaderContent a:visited, /* Yahoo! Mail Override */ .preheaderContent a .yshortcuts /* Yahoo! Mail Override */ { /*@editable*/                  color: #606060; /*@editable*/                  font-weight: normal; /*@editable*/                  text-decoration: underline;              }          /*** @tab Header* @section header style* @tip Set the background color and borders for your emails header area.* @theme header*/            #templateHeader { /*@editable*/              background-color: #FFFFFF; /*@editable*/              border-top: 1px solid #FFFFFF; /*@editable*/              border-bottom: 1px solid #CCCCCC;          }          /*** @tab Header* @section header text* @tip Set the styling for your emails header text. Choose a size and color that is easy to read.*/            .headerContent { /*@editable*/              color: #505050; /*@editable*/              font-family: "Segoe UI", Tahoma,Sans-serif; /*@editable*/              font-size: 20px; /*@editable*/              font-weight: bold; /*@editable*/              line-height: 100%; /*@editable*/              padding-top: 0; /*@editable*/              padding-right: 0; /*@editable*/              padding-bottom: 0; /*@editable*/              padding-left: 0; /*@editable*/              text-align: left; /*@editable*/              vertical-align: middle;          }              /*** @tab Header* @section header link* @tip Set the styling for your emails header links. Choose a color that helps them stand out from your text.*/                .headerContent a:link, .headerContent a:visited, /* Yahoo! Mail Override */ .headerContent a .yshortcuts /* Yahoo! Mail Override */ { /*@editable*/                  color: #EB4102; /*@editable*/                  font-weight: normal; /*@editable*/                  text-decoration: underline;              }            #headerImage {              height: auto;              max-width: 600px;          }          /* ========== Body Styles ========== */ /*** @tab Body* @section body style* @tip Set the background color and borders for your emails body area.*/            #templateBody { /*@editable*/              background-color: #FFFFFF; /*@editable*/              border-top: 1px solid #FFFFFF; /*@editable*/              border-bottom: 1px solid #CCCCCC;          }          /*** @tab Body* @section body text* @tip Set the styling for your emails main content text. Choose a size and color that is easy to read.* @theme main*/            .bodyContent { /*@editable*/              color: #505050; /*@editable*/              font-family: "Segoe UI", Tahoma,Sans-serif; /*@editable*/              font-size: 16px; /*@editable*/              line-height: 150%;              padding-top: 20px;              padding-right: 20px;              padding-bottom: 20px;              padding-left: 20px; /*@editable*/              text-align: left;          }              /*** @tab Body* @section body link* @tip Set the styling for your emails main content links. Choose a color that helps them stand out from your text.*/                .bodyContent a:link, .bodyContent a:visited, /* Yahoo! Mail Override */ .bodyContent a .yshortcuts /* Yahoo! Mail Override */ { /*@editable*/                  color: #EB4102; /*@editable*/                  font-weight: normal; /*@editable*/                  text-decoration: underline;              }                .bodyContent img {                  display: inline;                  height: auto;                  max-width: 560px;              }          /* ========== Column Styles ========== */            .templateColumnContainer {              display: inline;              width: 260px;          }          /*** @tab Columns* @section column style* @tip Set the background color and borders for your emails column area.*/            #templateColumns { /*@editable*/              background-color: #FFFFFF; /*@editable*/              border-top: 1px solid #FFFFFF; /*@editable*/              border-bottom: 1px solid #CCCCCC;          }          /*** @tab Columns* @section left column text* @tip Set the styling for your emails left column content text. Choose a size and color that is easy to read.*/            .leftColumnContent { /*@editable*/              color: #505050; /*@editable*/              font-family: "Segoe UI", Tahoma,Sans-serif; /*@editable*/              font-size: 14px; /*@editable*/              line-height: 150%;              padding-top: 0;              padding-right: 0;              padding-bottom: 20px;              padding-left: 0; /*@editable*/              text-align: left;          }              /*** @tab Columns* @section left column link* @tip Set the styling for your emails left column content links. Choose a color that helps them stand out from your text.*/                .leftColumnContent a:link, .leftColumnContent a:visited, /* Yahoo! Mail Override */ .leftColumnContent a .yshortcuts /* Yahoo! Mail Override */ { /*@editable*/                  color: #EB4102; /*@editable*/                  font-weight: normal; /*@editable*/                  text-decoration: underline;              }          /*** @tab Columns* @section right column text* @tip Set the styling for your emails right column content text. Choose a size and color that is easy to read.*/            .rightColumnContent { /*@editable*/              color: #505050; /*@editable*/              font-family: "Segoe UI", Tahoma,Sans-serif; /*@editable*/              font-size: 14px; /*@editable*/              line-height: 150%;              padding-top: 0;              padding-right: 0;              padding-bottom: 20px;              padding-left: 0; /*@editable*/              text-align: left;          }              /*** @tab Columns* @section right column link* @tip Set the styling for your emails right column content links. Choose a color that helps them stand out from your text.*/                .rightColumnContent a:link, .rightColumnContent a:visited, /* Yahoo! Mail Override */ .rightColumnContent a .yshortcuts /* Yahoo! Mail Override */ { /*@editable*/                  color: #EB4102; /*@editable*/                  font-weight: normal; /*@editable*/                  text-decoration: underline;              }                .leftColumnContent img, .rightColumnContent img {                  display: inline;                  height: auto;                  max-width: 260px;              }          /* ========== Footer Styles ========== */ /*** @tab Footer* @section footer style* @tip Set the background color and borders for your emails footer area.* @theme footer*/            #templateFooter { /*@editable*/              background-color: #FFFFFF; /*@editable*/              border-top: 1px solid #FFFFFF;          }          /*** @tab Footer* @section footer text* @tip Set the styling for your emails footer text. Choose a size and color that is easy to read.* @theme footer*/            .footerContent { /*@editable*/              color: #808080; /*@editable*/              font-family: "Segoe UI", Tahoma,Sans-serif; /*@editable*/              font-size: 10px; /*@editable*/              line-height: 150%;              padding-top: 20px;              padding-right: 20px;              padding-bottom: 20px;              padding-left: 20px; /*@editable*/              text-align: left;          }              /*** @tab Footer* @section footer link* @tip Set the styling for your emails footer links. Choose a color that helps them stand out from your text.*/                .footerContent a:link, .footerContent a:visited, /* Yahoo! Mail Override */ .footerContent a .yshortcuts, .footerContent a span /* Yahoo! Mail Override */ { /*@editable*/                  color: #606060; /*@editable*/                  font-weight: normal; /*@editable*/                  text-decoration: underline;              }          /* /\/\/\/\/\/\/\/\/ MOBILE STYLES /\/\/\/\/\/\/\/\/ */            @media only screen and (max-width: 480px) { /* /\/\/\/\/\/\/ CLIENT-SPECIFIC MOBILE STYLES /\/\/\/\/\/\/ */                body, table, td, p, a, li, blockquote {                  -webkit-text-size-adjust: none !important;              }              /* Prevent Webkit platforms from changing default text sizes */                body {                  width: 100% !important;                  min-width: 100% !important;              }              /* Prevent iOS Mail from adding padding to the body */ /* /\/\/\/\/\/\/ MOBILE RESET STYLES /\/\/\/\/\/\/ */                #bodyCell {                  padding: 10px !important;              }              /* /\/\/\/\/\/\/ MOBILE TEMPLATE STYLES /\/\/\/\/\/\/ */ /* ======== Page Styles ======== */ /*** @tab Mobile Styles* @section template width* @tip Make the template fluid for portrait or landscape view adaptability. If a fluid layout doesnt work for you, set the width to 300px instead.*/                #templateContainer {                  max-width: 600px !important; /*@editable*/                  width: 100% !important;              }              /*** @tab Mobile Styles* @section heading 1* @tip Make the first-level headings larger in size for better readability on small screens.*/                h1 { /*@editable*/                  font-size: 24px !important; /*@editable*/                  line-height: 100% !important;              }              /*** @tab Mobile Styles* @section heading 2* @tip Make the second-level headings larger in size for better readability on small screens.*/                h2 { /*@editable*/                  font-size: 20px !important; /*@editable*/                  line-height: 100% !important;              }              /*** @tab Mobile Styles* @section heading 3* @tip Make the third-level headings larger in size for better readability on small screens.*/                h3 { /*@editable*/                  font-size: 18px !important; /*@editable*/                  line-height: 100% !important;              }              /*** @tab Mobile Styles* @section heading 4* @tip Make the fourth-level headings larger in size for better readability on small screens.*/                h4 { /*@editable*/                  font-size: 16px !important; /*@editable*/                  line-height: 100% !important;              }              /* ======== Header Styles ======== */                #templatePreheader {                  display: none !important;              }              /* Hide the template preheader to save space */ /*** @tab Mobile Styles* @section header image* @tip Make the main header image fluid for portrait or landscape view adaptability, and set the images original width as the max-width. If a fluid setting doesnt work, set the image width to half its original size instead.*/                #headerImage {                  height: auto !important; /*@editable*/                  max-width: 600px !important; /*@editable*/                  width: 100% !important;              }              /*** @tab Mobile Styles* @section header text* @tip Make the header content text larger in size for better readability on small screens. We recommend a font size of at least 16px.*/                .headerContent { /*@editable*/                  font-size: 20px !important; /*@editable*/                  line-height: 125% !important;              }              /* ======== Body Styles ======== */ /*** @tab Mobile Styles* @section body image* @tip Make the main body image fluid for portrait or landscape view adaptability, and set the images original width as the max-width. If a fluid setting doesnt work, set the image width to half its original size instead.*/                #bodyImage {                  height: auto !important; /*@editable*/                  max-width: 560px !important; /*@editable*/                  width: 100% !important;              }              /*** @tab Mobile Styles* @section body text* @tip Make the body content text larger in size for better readability on small screens. We recommend a font size of at least 16px.*/                .bodyContent { /*@editable*/                  font-size: 18px !important; /*@editable*/                  line-height: 125% !important;              }              /* ======== Column Styles ======== */                .templateColumnContainer {                  display: block !important;                  width: 100% !important;              }              /*** @tab Mobile Styles* @section column image* @tip Make the column image fluid for portrait or landscape view adaptability, and set the images original width as the max-width. If a fluid setting doesnt work, set the image width to half its original size instead.*/                .columnImage {                  height: auto !important; /*@editable*/                  max-width: 260px !important; /*@editable*/                  width: 100% !important;              }              /*** @tab Mobile Styles* @section left column text* @tip Make the left column content text larger in size for better readability on small screens. We recommend a font size of at least 16px.*/                .leftColumnContent { /*@editable*/                  font-size: 16px !important; /*@editable*/                  line-height: 125% !important;              }              /*** @tab Mobile Styles* @section right column text* @tip Make the right column content text larger in size for better readability on small screens. We recommend a font size of at least 16px.*/                .rightColumnContent { /*@editable*/                  font-size: 16px !important; /*@editable*/                  line-height: 125% !important;              }              /* ======== Footer Styles ======== */ /*** @tab Mobile Styles* @section footer text* @tip Make the body content text larger in size for better readability on small screens.*/                .footerContent { /*@editable*/                  font-size: 14px !important; /*@editable*/                  line-height: 115% !important;              }                    .footerContent a {                      display: block !important;                  }              /* Place footer social and utility links on their own lines, for easier access */          }      </style>  </head>  <body leftmargin="0" marginwidth="0" topmargin="0" marginheight="0" offset="0">      <center>          <table align="center" border="0" cellpadding="0" cellspacing="0" height="100%" width="100%" id="bodyTable">              <tr>                  <td align="center" valign="top" id="bodyCell">                      <!-- BEGIN TEMPLATE // -->                      <table border="0" cellpadding="0" cellspacing="0" id="templateContainer">                          <tr>                              <td align="center" valign="top">                                  <!-- BEGIN HEADER // -->                                  <table border="0" cellpadding="0" cellspacing="0" width="100%" id="templateHeader">                                      <tr>                                          <td valign="top" class="headerContent">                                              <img src="https://media-exp1.licdn.com/dms/image/C510BAQHaTIBZfLFCGg/company-logo_200_200/0?e=1592438400&v=beta&t=AHOqaRJR_Thf72VDOKQId2QKdwepIp-sIiSIDtALzhQ" style="max-width: 300px; display: block; margin-left: auto; margin-right: auto; padding-top:10px;padding-bottom:10px;" id="headerImage" />                                          </td>                                      </tr>                                  </table>                             <!-- // END HEADER -->                              </td>                          </tr>                          <tr>                              <td align="center" valign="top">                                  <!-- BEGIN BODY // -->                                  <table border="0" cellpadding="0" cellspacing="0" width="100%" id="templateBody">                                      <tr>                                          <td valign="top" class="bodyContent">                                              <h1 style="display: block;  margin-left: auto;  margin-right: auto;  width: 75%;">Welcome to ${ApplicationName}!</h1>                                          </td>                                      </tr>                                      <tr>                                          <td valign="top" class="bodyContent">                                              <p>${welcometext}</p>                                          </td>                                      </tr>                                      <tr>                                          <td valign="top" class="bodyContent">                                              ${subscriptiontable}                                          </td>                                      </tr>                                      <tr>                                          <td valign="top" class="bodyContent">                                              <b>Input Parameters</b></br>                                              ${arminputparms}                                          </td>                                      </tr>                                      <tr>                                          <td valign="top" class="bodyContent">                                              <b>Output Parameters</b></br>                                              ${armoutputparms}                                              </br>                                          </td>                                      </tr>                                      <tr>                                          <td valign="top" class="bodyContent">                                                                                           <p style=" margin-left: auto; margin-right: auto; text-align:right;">                                                  <a href="https://saaskitdemoapp.azurewebsites.net/">                                                      <button style="background-color:#2168A6;line-height:30px;color:white"><b>View Details</b></button>                                                  </a>                                              </p>                                      <!--     CTA button -->                                              <!--<table style="background: #0078D7;" cellspacing="0" cellpadding="0" align="left">                                                  <tbody>                                                      <tr>                                                          <td style="padding-left: 15px; font-size: 18px; line-height: 20px; font-family:"Segoe UI Light"; color: #ffffff;">                                                              <a style="text-decoration: none; font-size: 18px; line-height: 20px; font-family:"Segoe UI Light"; color: #ffffff;" href="${LinkToPortal}/#/login">${LoginButtonTextInEmail}</a>                                                          </td>                                                          <td style="line-height: 1px; font-size: 1px; padding: 10px;">                                                              <a>                                                                  <img src="https://info.microsoft.com/rs/157-GQE-382/images/Program-CTAButton-whiteltr.png" border="0" alt="" height="20" />                                                              </a>                                                          </td>                                                      </tr>                                                  </tbody>                                              </table>-->                                              <!--     end CTA button -->                                          </td>                                      </tr>                                  </table>                              </td>                          </tr>                          <tr>                              <td align="center" valign="top">                                  <!-- BEGIN PREHEADER // -->                                  <table border="0" cellpadding="0" cellspacing="0" width="100%" id="templatePreheader">                                      <tr>                                          <td valign="top" class="preheaderContent" style="padding-top: 10px; padding-right: 20px; padding-bottom: 10px; padding-left: 20px;">                                              You are receiving this message because of an interaction with                                              <a href="https://saaskitdemoapp.azurewebsites.net/">Contoso</a>.                                              Please contact us at <a href="https://saaskitdemoapp.azurewebsites.net/">Contoso</a>                                              in case you think you have received this message in error or need help.                                          </td>                                            <!-- *|IFNOT:ARCHIVE_PAGE|* -->                                                      <!-- *|END:IF|* -->                                      </tr>                                  </table>                                      <!-- // END PREHEADER -->                              </td>                          </tr>                      </table>                  </td>              </tr>          </table>                    <!-- // END BODY -->          </td>          </tr>          </table>           <!-- // END TEMPLATE -->      </center>  </body>  </html>','','','','',1