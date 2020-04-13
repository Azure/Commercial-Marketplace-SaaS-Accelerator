--Create a database and execute the script

--/****** Object:  Table [dbo].[ApplicationLog]    Script Date: 01/15/2020 4:43:08 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[ApplicationLog](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ActionTime] [datetime] NULL,
	[LogDetail] [varchar](4000) NULL,
 CONSTRAINT [PK_ApplicationLog] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Plans]    Script Date: 01/15/2020 4:43:08 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Plans](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[PlanId] [varchar](100) NULL,
	[Description] [varchar](500) NULL,
	[DisplayName] [varchar](100) NULL,
 CONSTRAINT [PK_Plans] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[SubscriptionAuditLogs]    Script Date: 01/15/2020 4:43:08 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[SubscriptionAuditLogs](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[SubscriptionID] [int] NULL,
	[Attribute] [varchar](20) NULL,
	[OldValue] [varchar](50) NULL,
	[NewValue] [varchar](50) NULL,
	[CreateDate] [datetime] NULL,
	[CreateBy] [int] NULL,
 CONSTRAINT [PK_SubscriptionAuditLog] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Subscriptions]    Script Date: 01/15/2020 4:43:09 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Subscriptions](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[AMPSubscriptionId] [uniqueidentifier] ROWGUIDCOL  NOT NULL CONSTRAINT [DF_Subscriptions_AMPSubscriptionId]  DEFAULT (newid()),
	[SubscriptionStatus] [varchar](50) NULL,
	[AMPPlanId] [varchar](100) NULL,
	[IsActive] [bit] NULL,
	[CreateBy] [int] NULL,
	[CreateDate] [datetime] NULL,
	[ModifyDate] [datetime] NULL,
	[UserId] [int] NULL,
	[Name] [varchar](100) NULL,
 CONSTRAINT [PK_Subscriptions] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Users]    Script Date: 01/15/2020 4:43:09 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Users](
	[UserId] [int] IDENTITY(1,1) NOT NULL,
	[EmailAddress] [varchar](100) NULL,
	[CreatedDate] [datetime] NULL,
	[FullName] [varchar](200) NULL,
 CONSTRAINT [PK_Users] PRIMARY KEY CLUSTERED 
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)

GO
SET ANSI_PADDING OFF
GO
ALTER TABLE [dbo].[SubscriptionAuditLogs]  WITH CHECK ADD FOREIGN KEY([SubscriptionID])
REFERENCES [dbo].[Subscriptions] ([Id])
GO
ALTER TABLE [dbo].[Subscriptions]  WITH CHECK ADD FOREIGN KEY([UserId])
REFERENCES [dbo].[Users] ([UserId])
GO
