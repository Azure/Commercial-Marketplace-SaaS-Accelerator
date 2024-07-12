IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20221118045814_Baseline_v2'
)
BEGIN
    CREATE TABLE [ApplicationConfiguration] (
        [ID] int NOT NULL IDENTITY,
        [Name] nvarchar(50) NULL,
        [Value] nvarchar(max) NULL,
        [Description] nvarchar(255) NULL,
        CONSTRAINT [PK_ApplicationConfiguration] PRIMARY KEY ([ID])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20221118045814_Baseline_v2'
)
BEGIN
    CREATE TABLE [ApplicationLog] (
        [Id] int NOT NULL IDENTITY,
        [ActionTime] datetime NULL,
        [LogDetail] varchar(4000) NULL,
        CONSTRAINT [PK_ApplicationLog] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20221118045814_Baseline_v2'
)
BEGIN
    CREATE TABLE [DatabaseVersionHistory] (
        [ID] int NOT NULL IDENTITY,
        [VersionNumber] decimal(6,2) NOT NULL,
        [ChangeLog] nvarchar(max) NOT NULL,
        [CreateDate] datetime NOT NULL,
        [CreateBy] nvarchar(100) NULL
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20221118045814_Baseline_v2'
)
BEGIN
    CREATE TABLE [EmailTemplate] (
        [ID] int NOT NULL IDENTITY,
        [Status] varchar(1000) NULL,
        [Description] varchar(1000) NULL,
        [InsertDate] datetime NULL,
        [TemplateBody] varchar(max) NULL,
        [Subject] varchar(1000) NULL,
        [ToRecipients] varchar(1000) NULL,
        [CC] varchar(1000) NULL,
        [BCC] varchar(1000) NULL,
        [IsActive] bit NOT NULL,
        CONSTRAINT [PK_EmailTemplate] PRIMARY KEY ([ID])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20221118045814_Baseline_v2'
)
BEGIN
    CREATE TABLE [Events] (
        [EventsId] int NOT NULL IDENTITY,
        [EventsName] varchar(225) NULL,
        [IsActive] bit NULL,
        [CreateDate] datetime NULL,
        CONSTRAINT [PK_Events] PRIMARY KEY ([EventsId])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20221118045814_Baseline_v2'
)
BEGIN
    CREATE TABLE [OfferAttributes] (
        [ID] int NOT NULL IDENTITY,
        [ParameterId] varchar(225) NULL,
        [DisplayName] varchar(225) NULL,
        [Description] varchar(225) NULL,
        [ValueTypeId] int NULL,
        [FromList] bit NOT NULL,
        [ValuesList] varchar(max) NULL,
        [Max] int NULL,
        [Min] int NULL,
        [Type] varchar(225) NULL,
        [DisplaySequence] int NULL,
        [Isactive] bit NOT NULL,
        [CreateDate] datetime NULL,
        [UserId] int NULL,
        [OfferId] uniqueidentifier NOT NULL,
        [IsDelete] bit NULL,
        [IsRequired] bit NULL,
        CONSTRAINT [PK_OfferAttributes] PRIMARY KEY ([ID])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20221118045814_Baseline_v2'
)
BEGIN
    CREATE TABLE [Offers] (
        [Id] int NOT NULL IDENTITY,
        [OfferId] varchar(225) NULL,
        [OfferName] varchar(225) NULL,
        [CreateDate] datetime NULL,
        [UserId] int NULL,
        [OfferGUId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_Offers] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20221118045814_Baseline_v2'
)
BEGIN
    CREATE TABLE [PlanAttributeMapping] (
        [PlanAttributeId] int NOT NULL IDENTITY,
        [PlanId] uniqueidentifier NOT NULL,
        [OfferAttributeID] int NOT NULL,
        [IsEnabled] bit NOT NULL,
        [CreateDate] datetime NULL,
        [UserId] int NULL,
        CONSTRAINT [PK__PlanAttr__8B476A98C058FAF2] PRIMARY KEY ([PlanAttributeId])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20221118045814_Baseline_v2'
)
BEGIN
    CREATE TABLE [PlanAttributeOutput] (
        [RowNumber] int NOT NULL,
        [PlanAttributeId] int NOT NULL,
        [PlanId] uniqueidentifier NOT NULL,
        [OfferAttributeId] int NOT NULL,
        [DisplayName] varchar(225) NOT NULL,
        [IsEnabled] bit NOT NULL,
        [Type] varchar(225) NULL,
        CONSTRAINT [PK__PlanAttr__AAAC09D888FE3E40] PRIMARY KEY ([RowNumber])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20221118045814_Baseline_v2'
)
BEGIN
    CREATE TABLE [PlanEventsMapping] (
        [Id] int NOT NULL IDENTITY,
        [PlanId] uniqueidentifier NOT NULL,
        [EventId] int NOT NULL,
        [Isactive] bit NOT NULL,
        [SuccessStateEmails] varchar(225) NULL,
        [FailureStateEmails] varchar(225) NULL,
        [CreateDate] datetime NULL,
        [UserId] int NULL,
        [CopyToCustomer] bit NULL,
        CONSTRAINT [PK_PlanEventsMapping] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20221118045814_Baseline_v2'
)
BEGIN
    CREATE TABLE [PlanEventsOutPut] (
        [RowNumber] int NOT NULL,
        [ID] int NOT NULL,
        [PlanId] uniqueidentifier NOT NULL,
        [Isactive] bit NOT NULL,
        [SuccessStateEmails] varchar(max) NULL,
        [FailureStateEmails] varchar(max) NULL,
        [EventId] int NOT NULL,
        [EventsName] varchar(225) NOT NULL,
        [CopyToCustomer] bit NULL,
        CONSTRAINT [PK__PlanEven__AAAC09D8C9229532] PRIMARY KEY ([RowNumber])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20221118045814_Baseline_v2'
)
BEGIN
    CREATE TABLE [Plans] (
        [Id] int NOT NULL IDENTITY,
        [PlanId] varchar(100) NULL,
        [Description] varchar(500) NULL,
        [DisplayName] varchar(100) NULL,
        [IsmeteringSupported] bit NULL,
        [IsPerUser] bit NULL,
        [PlanGUID] uniqueidentifier NOT NULL,
        [OfferID] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_Plans] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20221118045814_Baseline_v2'
)
BEGIN
    CREATE TABLE [Roles] (
        [Id] int NOT NULL IDENTITY,
        [Name] varchar(50) NULL,
        CONSTRAINT [PK_Roles] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20221118045814_Baseline_v2'
)
BEGIN
    CREATE TABLE [SubscriptionAttributeValues] (
        [ID] int NOT NULL IDENTITY,
        [PlanAttributeId] int NOT NULL,
        [Value] varchar(225) NULL,
        [SubscriptionId] uniqueidentifier NOT NULL,
        [CreateDate] datetime NULL,
        [UserId] int NULL,
        [PlanID] uniqueidentifier NOT NULL,
        [OfferID] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_SubscriptionAttributeValues] PRIMARY KEY ([ID])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20221118045814_Baseline_v2'
)
BEGIN
    CREATE TABLE [SubscriptionEmailOutput] (
        [Id] int NOT NULL IDENTITY,
        [Name] varchar(225) NULL,
        [Value] varchar(max) NULL
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20221118045814_Baseline_v2'
)
BEGIN
    CREATE TABLE [SubscriptionParametersOutput] (
        [RowNumber] int NOT NULL,
        [Id] int NOT NULL,
        [PlanAttributeId] int NOT NULL,
        [OfferAttributeID] int NOT NULL,
        [DisplayName] varchar(225) NOT NULL,
        [Type] varchar(225) NOT NULL,
        [ValueType] varchar(225) NOT NULL,
        [DisplaySequence] int NOT NULL,
        [IsEnabled] bit NOT NULL,
        [IsRequired] bit NULL,
        [Value] varchar(max) NOT NULL,
        [SubscriptionId] uniqueidentifier NOT NULL,
        [OfferId] uniqueidentifier NOT NULL,
        [PlanId] uniqueidentifier NOT NULL,
        [UserId] int NULL,
        [CreateDate] datetime NULL,
        [FromList] bit NOT NULL,
        [ValuesList] varchar(225) NOT NULL,
        [Max] int NOT NULL,
        [Min] int NOT NULL,
        [HTMLType] varchar(225) NOT NULL,
        CONSTRAINT [PK__Subscrip__AAAC09D8BA727059] PRIMARY KEY ([RowNumber])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20221118045814_Baseline_v2'
)
BEGIN
    CREATE TABLE [Users] (
        [UserId] int NOT NULL IDENTITY,
        [EmailAddress] varchar(100) NULL,
        [CreatedDate] datetime NULL,
        [FullName] varchar(200) NULL,
        CONSTRAINT [PK_Users] PRIMARY KEY ([UserId])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20221118045814_Baseline_v2'
)
BEGIN
    CREATE TABLE [ValueTypes] (
        [ValueTypeId] int NOT NULL IDENTITY,
        [ValueType] varchar(225) NULL,
        [CreateDate] datetime NULL,
        [HTMLType] varchar(225) NULL,
        CONSTRAINT [PK__ValueTyp__A51E9C5AEA096123] PRIMARY KEY ([ValueTypeId])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20221118045814_Baseline_v2'
)
BEGIN
    CREATE TABLE [WebJobSubscriptionStatus] (
        [ID] int NOT NULL IDENTITY,
        [SubscriptionId] uniqueidentifier NULL,
        [SubscriptionStatus] varchar(225) NULL,
        [Description] varchar(max) NULL,
        [InsertDate] datetime NULL,
        CONSTRAINT [PK_WebJobSubscriptionStatus] PRIMARY KEY ([ID])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20221118045814_Baseline_v2'
)
BEGIN
    CREATE TABLE [MeteredDimensions] (
        [Id] int NOT NULL IDENTITY,
        [Dimension] varchar(150) NULL,
        [PlanId] int NULL,
        [CreatedDate] datetime NULL,
        [Description] varchar(250) NULL,
        CONSTRAINT [PK_MeteredDimensions] PRIMARY KEY ([Id]),
        CONSTRAINT [FK__MeteredDi__PlanI__6383C8BA] FOREIGN KEY ([PlanId]) REFERENCES [Plans] ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20221118045814_Baseline_v2'
)
BEGIN
    CREATE TABLE [KnownUsers] (
        [Id] int NOT NULL IDENTITY,
        [UserEmail] varchar(50) NULL,
        [RoleId] int NOT NULL,
        CONSTRAINT [PK_KnownUsers] PRIMARY KEY ([Id]),
        CONSTRAINT [FK__KnownUser__RoleI__619B8048] FOREIGN KEY ([RoleId]) REFERENCES [Roles] ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20221118045814_Baseline_v2'
)
BEGIN
    CREATE TABLE [Subscriptions] (
        [Id] int NOT NULL IDENTITY,
        [AMPSubscriptionId] uniqueidentifier NOT NULL DEFAULT ((newid())),
        [SubscriptionStatus] varchar(50) NULL,
        [AMPPlanId] varchar(100) NULL,
        [IsActive] bit NULL,
        [CreateBy] int NULL,
        [CreateDate] datetime NULL,
        [ModifyDate] datetime NULL,
        [UserId] int NULL,
        [Name] varchar(100) NULL,
        [AMPQuantity] int NOT NULL,
        [PurchaserEmail] varchar(225) NULL,
        [PurchaserTenantId] uniqueidentifier NULL,
        CONSTRAINT [PK_Subscriptions] PRIMARY KEY ([Id]),
        CONSTRAINT [FK__Subscript__UserI__656C112C] FOREIGN KEY ([UserId]) REFERENCES [Users] ([UserId])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20221118045814_Baseline_v2'
)
BEGIN
    CREATE TABLE [MeteredAuditLogs] (
        [Id] int NOT NULL IDENTITY,
        [SubscriptionId] int NULL,
        [RequestJson] varchar(500) NULL,
        [ResponseJson] varchar(500) NULL,
        [StatusCode] varchar(100) NULL,
        [CreatedDate] datetime NULL,
        [CreatedBy] int NOT NULL,
        [SubscriptionUsageDate] datetime NULL,
        CONSTRAINT [PK_MeteredAuditLogs] PRIMARY KEY ([Id]),
        CONSTRAINT [FK__MeteredAu__Subsc__628FA481] FOREIGN KEY ([SubscriptionId]) REFERENCES [Subscriptions] ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20221118045814_Baseline_v2'
)
BEGIN
    CREATE TABLE [SubscriptionAuditLogs] (
        [Id] int NOT NULL IDENTITY,
        [SubscriptionID] int NULL,
        [Attribute] varchar(20) NULL,
        [OldValue] varchar(50) NULL,
        [NewValue] varchar(max) NULL,
        [CreateDate] datetime NULL,
        [CreateBy] int NULL,
        CONSTRAINT [PK_SubscriptionAuditLogs] PRIMARY KEY ([Id]),
        CONSTRAINT [FK__Subscript__Subsc__6477ECF3] FOREIGN KEY ([SubscriptionID]) REFERENCES [Subscriptions] ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20221118045814_Baseline_v2'
)
BEGIN
    CREATE INDEX [IX_KnownUsers_RoleId] ON [KnownUsers] ([RoleId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20221118045814_Baseline_v2'
)
BEGIN
    CREATE INDEX [IX_MeteredAuditLogs_SubscriptionId] ON [MeteredAuditLogs] ([SubscriptionId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20221118045814_Baseline_v2'
)
BEGIN
    CREATE INDEX [IX_MeteredDimensions_PlanId] ON [MeteredDimensions] ([PlanId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20221118045814_Baseline_v2'
)
BEGIN
    CREATE INDEX [IX_SubscriptionAuditLogs_SubscriptionID] ON [SubscriptionAuditLogs] ([SubscriptionID]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20221118045814_Baseline_v2'
)
BEGIN
    CREATE INDEX [IX_Subscriptions_UserId] ON [Subscriptions] ([UserId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20221118045814_Baseline_v2'
)
BEGIN

    /*   
    Exec spGetSubscriptionParameters '53ff9e28-a55b-65e1-ff75-709aec6420fd','a35d4259-f3c9-429b-a871-21c4593fa4bf'  
    */
    EXEC(N'
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
    ,ISNULL(OA.DisplayName,'''')DisplayName  
    ,ISNULL(OA.Type,'''')Type  
    ,ISNULL(VT.ValueType,'''') ValueType  
    ,ISnull(OA.DisplaySequence,0)DisplaySequence  
    ,isnull(PA.IsEnabled,0) IsEnabled  
    ,isnull(OA.IsRequired,0) IsRequired  
    ,ISNULL(Value,'''')Value  
    ,ISNULL(SubscriptionId,@SubscriptionId) SubscriptionId  
    ,ISNULL(SAV.OfferID,OA.OfferId) OfferID  
    ,SAV.UserId  
    ,SAV.CreateDate  
    ,ISNULL(oA.FromList,0) FromList  
    ,ISNULL(OA.ValuesList,'''') ValuesList  
    ,ISNULL(OA.Max,0) Max  
    ,ISNULL(OA.Min,0) Min
    ,ISNULL(VT.HTMLType,'''') HTMLType  
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
    ')
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20221118045814_Baseline_v2'
)
BEGIN

    /*   
    Exec spGetPlanEvents 'B8F4D276-15EB-4EB6-89D4-E600FF1098EF'  
    */
    EXEC(N'
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
    ,ISNULL(OEM.SuccessStateEmails,'''')SuccessStateEmails  
    ,ISNULL(OEM.FailureStateEmails,'''')FailureStateEmails  
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
    ')
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20221118045814_Baseline_v2'
)
BEGIN

    /*   
    Exec spGetOfferParameters 'B8F4D276-15EB-4EB6-89D4-E600FF1098EF'  
    */  
    EXEC(N'
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
    ')
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20221118045814_Baseline_v2'
)
BEGIN

    EXEC(N'
        Create Procedure [dbo].[spGetFormattedEmailBody]  
        (  
        @subscriptionId varchar(225),  
        @processStatus varchar(225)   
        )  
      
        /*  
        EXEC spGetFormattedEmailBody ''86C334EC-5973-D337-7B2B-0D676513B0F9'',''success''
        */  
      
        AS  
        BEGIN  
        declare @html varchar(max)   
      
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
      
        Declare @applicationName Varchar(225) =(select [value] from [ApplicationConfiguration] where [Name]=''ApplicationName'')  
        Declare @welcomeText varchar(MAX)=''''  
      
      
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
      
        select ''Customer Email Address'',@customerEmailAddress   UNION ALL  
        select ''Customer Name'',@customerName       UNION ALL  
        select ''SaaS Subscription Id'',@subscriptionId     UNION ALL  
        select ''SaaS Subscription Name'',@subscriptionName    UNION ALL  
        select ''SaaS Subscription Status'',@subscriptionStatus   UNION ALL  
        select ''Plan'',@planName           UNION ALL  
        select ''Purchaser Email Address'',@customerEmailAddress   UNION ALL  
        select ''Purchaser Tenant'',@purchaserTenant      UNION ALL     
        -- Parameters  
        select   
        ISNULL(OA.DisplayName,'''') DisplayName, ISNULL(Value,'''')Value      
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
        Declare @subscriptionContent VARCHAR(MAX)=''''  
      
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
             
            set @subscriptionContent = @subscriptionContent + ''<tr><td><b>''+ @htmlLabel+''</b></td> <td>'' + @htmlValue + ''</td> </tr>''  
      
                FETCH NEXT FROM cursor_html INTO   
                    @htmlLabel,   
            @htmlValue;  
            END;  
      
        CLOSE cursor_html;  
      
        DEALLOCATE cursor_html;  
      
        -- Cursor End  
      
        -- Welcome text  
      
        IF (@processStatus =''failure'')  
         BEGIN  
          set @welcomeText= ''Your request for the subscription has been failed.''  
          set @html = (SELECT TemplateBody FROM EmailTemplate WHERE Status = ''Failed'')
         END  
      
        IF (@processStatus =''success'')   
         BEGIN  
         IF (@subscriptionStatus= ''PendingActivation'')  
            BEGIN  
          set @welcomeText= ''A request for purchase with the following details is awaiting your action for activation.''  
           END  
         IF (@subscriptionStatus= ''Subscribed'')  
            BEGIN  
          set @welcomeText= ''Your request for the purchase has been approved.''  
           END  
         IF (@subscriptionStatus= ''Unsubscribed'')  
            BEGIN  
          set @welcomeText= ''A subscription with the following details was deleted from Azure.''  
           END   
             set @html = (SELECT TemplateBody FROM EmailTemplate WHERE Status = @subscriptionStatus)
        END  
     
         select  @html=REPLACE(@html,''${subscriptiondetails}'',@subscriptionContent)  
          ,@html=REPLACE(@html,''${welcometext}'',@welcomeText)  
          ,@html=REPLACE(@html,''${ApplicationName}'',@applicationName)  
       
      
         select 1 AS ID,''Email'' AS [Name], @html as [Value]  
      
      
        /* test values  
         --select @subscriptionContent  
         -- select * from #Temp  
      
        SELECT   
          @subscriptionId    as ''subscriptionId''  
        , @processStatus    as ''processStatus''  
        , @planId       as ''planId''  
        , @offerId       as ''offerId''  
        , @offerGUId      as ''offerGUId''  
        , @subscriptionStatus    as ''subscriptionStatus''  
        , @subscriptionName    as ''subscriptionName''  
        , @planName      as ''planName''  
        , @offerName      as ''offerName''  
        , @customerName      as ''customerName''  
        , @customerEmailAddress   as ''customerEmailAddress''  
        , @purchaserEmial      as ''purchaserEmial''  
        , @purchaserTenant     as ''purchaserTenant''  
        , @UserId       as ''UserId''  
        , @applicationName    as ''applicationName''  
        , @welcomeText     as ''welcomeText''  
        , @tablehtml     as ''tablehtml''  
        */  
      
        End
    ')
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20221118045814_Baseline_v2'
)
BEGIN

    INSERT INTO ValueTypes
        (ValueType,CreateDate,HTMLType)
    VALUES 
        ('Int','7/12/2024 3:22:00 PM','int'),
        ('String','7/12/2024 3:22:00 PM','string'),
        ('Date','7/12/2024 3:22:00 PM','date')

END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20221118045814_Baseline_v2'
)
BEGIN
    INSERT INTO Roles (name) VALUES ('PublisherAdmin')
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20221118045814_Baseline_v2'
)
BEGIN

    INSERT INTO Events
    	(EventsName,IsActive,CreateDate)
    VALUES
        ('Activate',1,'7/12/2024 3:22:00 PM'),
    	('Unsubscribe',1,'7/12/2024 3:22:00 PM'),
    	('Pending Activation',1,'7/12/2024 3:22:00 PM')

END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20221118045814_Baseline_v2'
)
BEGIN

    INSERT INTO ApplicationConfiguration
    	([Name],[Value],[Description])
    VALUES
        ('SMTPFromEmail','','SMTP Email'),
    	('SMTPPassword','','SMTP Password'),
    	('SMTPHost','','SMTP Host'),
    	('SMTPPort','','SMTP Port'),
    	('SMTPUserName','','SMTP User Name'),
    	('SMTPSslEnabled','','SMTP Ssl Enabled'),
    	('ApplicationName','Contoso','Application Name'),
    	('IsEmailEnabledForSubscriptionActivation','true','Active Email Enabled'),
    	('IsEmailEnabledForUnsubscription','true','Unsubscribe Email Enabled'),
    	('IsAutomaticProvisioningSupported','false','Skip Activation - Automatic Provisioning Supported'),
    	('IsEmailEnabledForPendingActivation','false','Email Enabled For Pending Activation')

END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20221118045814_Baseline_v2'
)
BEGIN

    IF NOT EXISTS (SELECT * FROM ApplicationConfiguration WHERE Name = 'AcceptSubscriptionUpdates')
    BEGIN
        INSERT INTO ApplicationConfiguration (Name,Value,Description)
        VALUES ('AcceptSubscriptionUpdates','false','Accepts subscriptions plan or quantity updates')
    END
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20221118045814_Baseline_v2'
)
BEGIN
    IF NOT EXISTS (SELECT * FROM ApplicationConfiguration WHERE Name = 'LogoFile')
    BEGIN
        INSERT INTO ApplicationConfiguration (Name,Value,Description)
        VALUES ('LogoFile','','Logo File')
    END
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20221118045814_Baseline_v2'
)
BEGIN
    IF NOT EXISTS (SELECT * FROM ApplicationConfiguration WHERE Name = 'FaviconFile')
    BEGIN
        INSERT INTO ApplicationConfiguration (Name,Value,Description)
        VALUES ('FaviconFile','','Favicon File')
    END
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20221118045814_Baseline_v2'
)
BEGIN

    INSERT INTO EmailTemplate
    	([Status],[Description],[InsertDate],[TemplateBody],[Subject],[IsActive])
    VALUES
        ('Failed','Failed','7/12/2024 3:22:00 PM', '
    <!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">  
    <html xmlns="http://www.w3.org/1999/xhtml">
       <head>
          <!--<note>Use this Template for Pending Activation email </note>-->      
          <meta http-equiv="Content-Type" content="text/html; charset=UTF-8" />
          <!--<title>Cloud Market Place - Your registration is successful</title>-->      
          <style type="text/css">          /* /\/\/\/\/\/\/\/\/ CLIENT-SPECIFIC STYLES /\/\/\/\/\/\/\/\/ */            #outlook a {              padding: 0;          }          /* Force Outlook to provide a view in browser message */            .ReadMsgBody {              width: 100%;          }            .ExternalClass {              width: 100%;          }              /* Force Hotmail to display emails at full width */                .ExternalClass, .ExternalClass p, .ExternalClass span, .ExternalClass font, .ExternalClass td, .ExternalClass div {                  line-height: 100%;              }          /* Force Hotmail to display normal line spacing */            body, table, td, p, a, li, blockquote {              -webkit-text-size-adjust: 100%;              -ms-text-size-adjust: 100%;          }          /* Prevent WebKit and Windows mobile changing default text sizes */            table, td {              mso-table-lspace: 0pt;              mso-table-rspace: 0pt;          }          /* Remove spacing between tables in Outlook 2007 and up */            img {              -ms-interpolation-mode: bicubic;          }          /* Allow smoother rendering of resized image in Internet Explorer */ /* /\/\/\/\/\/\/\/\/ RESET STYLES /\/\/\/\/\/\/\/\/ */            body {              margin: 0;              padding: 0;          }            img {              border: 0;              height: auto;              line-height: 100%;              outline: none;              text-decoration: none;          }            table {              border-collapse: collapse !important;          }            body, #bodyTable, #bodyCell {              height: 100% !important;              margin: 0;              padding: 0;              width: 100% !important;          }          /* /\/\/\/\/\/\/\/\/ TEMPLATE STYLES /\/\/\/\/\/\/\/\/ */ /* ========== Page Styles ========== */            #bodyCell {              padding: 20px;          }            #templateContainer {              width: 600px;          }          /*** @tab Page* @section background style* @tip Set the background color and top border for your email. You may want to choose colors that match your companys branding.* @theme page*/            body, #bodyTable { /*@editable*/              background-color: #FFFFFF;          }          /*** @tab Page* @section background style* @tip Set the background color and top border for your email. You may want to choose colors that match your companys branding.* @theme page*/            #bodyCell { /*@editable*/              border-top: 4px solid #FFFFFF;          }          /*** @tab Page* @section email border* @tip Set the border for your email.*/            #templateContainer { /*@editable*/              border: 1px solid #BBBBBB;          }          /*** @tab Page* @section heading 1* @tip Set the styling for all first-level headings in your emails. These should be the largest of your headings.* @style heading 1*/            h1 { /*@editable*/              color: #202020 !important;              display: block; /*@editable*/              font-family: "Segoe UI", Tahoma,Sans-serif; /*@editable*/              font-size: 26px; /*@editable*/              font-style: normal; /*@editable*/              font-weight: bold; /*@editable*/              line-height: 100%; /*@editable*/              letter-spacing: normal;              margin-top: 0;              margin-right: 0;              margin-bottom: 10px;              margin-left: 0; /*@editable*/              text-align: left;          }          /*** @tab Page* @section heading 2* @tip Set the styling for all second-level headings in your emails.* @style heading 2*/            h2 { /*@editable*/              color: #404040 !important;              display: block; /*@editable*/              font-family: "Segoe UI", Tahoma,Sans-serif; /*@editable*/              font-size: 20px; /*@editable*/              font-style: normal; /*@editable*/              font-weight: bold; /*@editable*/              line-height: 100%; /*@editable*/              letter-spacing: normal;              margin-top: 0;              margin-right: 0;              margin-bottom: 10px;              margin-left: 0; /*@editable*/              text-align: left;          }          /*** @tab Page* @section heading 3* @tip Set the styling for all third-level headings in your emails.* @style heading 3*/            h3 { /*@editable*/              color: #606060 !important;              display: block; /*@editable*/              font-family: "Segoe UI", Tahoma,Sans-serif; /*@editable*/              font-size: 16px; /*@editable*/              font-style: italic; /*@editable*/              font-weight: normal; /*@editable*/              line-height: 100%; /*@editable*/              letter-spacing: normal;              margin-top: 0;              margin-right: 0;              margin-bottom: 10px;              margin-left: 0; /*@editable*/              text-align: left;          }          /*** @tab Page* @section heading 4* @tip Set the styling for all fourth-level headings in your emails. These should be the smallest of your headings.* @style heading 4*/            h4 { /*@editable*/              color: #000000 !important;              display: block; /*@editable*/              font-family: "Segoe UI", Tahoma,Sans-serif; /*@editable*/              font-size: 14px; /*@editable*/              font-style: italic; /*@editable*/              font-weight: normal; /*@editable*/              line-height: 100%; /*@editable*/              letter-spacing: normal;              margin-top: 0;              margin-right: 0;              margin-bottom: 10px;              margin-left: 0; /*@editable*/              text-align: left;          }          /* ========== Header Styles ========== */ /*** @tab Header* @section preheader style* @tip Set the background color and bottom border for your emails preheader area.* @theme header*/            #templatePreheader { /*@editable*/              background-color: #FFFFFF; /*@editable*/              border-bottom: 1px solid #CCCCCC;          }          /*** @tab Header* @section preheader text* @tip Set the styling for your emails preheader text. Choose a size and color that is easy to read.*/            .preheaderContent { /*@editable*/              color: #000000; /*@editable*/              font-family: wf_segoe-ui_normal, "Segoe UI", "Segoe WP", Tahoma, Arial, sans-serif; /*@editable*/              font-size: 10px; /*@editable*/              line-height: 125%; /*@editable*/              text-align: left;          }              /*** @tab Header* @section preheader link* @tip Set the styling for your emails preheader links. Choose a color that helps them stand out from your text.*/                .preheaderContent a:link, .preheaderContent a:visited, /* Yahoo! Mail Override */ .preheaderContent a .yshortcuts /* Yahoo! Mail Override */ { /*@editable*/                  color: #606060; /*@editable*/                  font-weight: normal; /*@editable*/                  text-decoration: underline;              }          /*** @tab Header* @section header style* @tip Set the background color and borders for your emails header area.* @theme header*/            #templateHeader { /*@editable*/              background-color: #FFFFFF; /*@editable*/              border-top: 1px solid #FFFFFF; /*@editable*/              border-bottom: 1px solid #CCCCCC;          }          /*** @tab Header* @section header text* @tip Set the styling for your emails header text. Choose a size and color that is easy to read.*/            .headerContent { /*@editable*/              color: #505050; /*@editable*/              font-family: "Segoe UI", Tahoma,Sans-serif; /*@editable*/              font-size: 20px; /*@editable*/              font-weight: bold; /*@editable*/              line-height: 100%; /*@editable*/              padding-top: 0; /*@editable*/              padding-right: 0; /*@editable*/              padding-bottom: 0; /*@editable*/              padding-left: 0; /*@editable*/              text-align: left; /*@editable*/              vertical-align: middle;          }              /*** @tab Header* @section header link* @tip Set the styling for your emails header links. Choose a color that helps them stand out from your text.*/                .headerContent a:link, .headerContent a:visited, /* Yahoo! Mail Override */ .headerContent a .yshortcuts /* Yahoo! Mail Override */ { /*@editable*/                  color: #EB4102; /*@editable*/                  font-weight: normal; /*@editable*/                  text-decoration: underline;              }            #headerImage {              height: auto;              max-width: 600px;          }          /* ========== Body Styles ========== */ /*** @tab Body* @section body style* @tip Set the background color and borders for your emails body area.*/            #templateBody { /*@editable*/              background-color: #FFFFFF; /*@editable*/              border-top: 1px solid #FFFFFF; /*@editable*/              border-bottom: 1px solid #CCCCCC;          }          /*** @tab Body* @section body text* @tip Set the styling for your emails main content text. Choose a size and color that is easy to read.* @theme main*/            .bodyContent { /*@editable*/              color: #505050; /*@editable*/              font-family: "Segoe UI", Tahoma,Sans-serif; /*@editable*/              font-size: 16px; /*@editable*/              line-height: 150%;              padding-top: 20px;              padding-right: 20px;              padding-bottom: 20px;              padding-left: 20px; /*@editable*/              text-align: left;          }              /*** @tab Body* @section body link* @tip Set the styling for your emails main content links. Choose a color that helps them stand out from your text.*/                .bodyContent a:link, .bodyContent a:visited, /* Yahoo! Mail Override */ .bodyContent a .yshortcuts /* Yahoo! Mail Override */ { /*@editable*/                  color: #EB4102; /*@editable*/                  font-weight: normal; /*@editable*/                  text-decoration: underline;              }                .bodyContent img {                  display: inline;                  height: auto;                  max-width: 560px;              }          /* ========== Column Styles ========== */            .templateColumnContainer {              display: inline;              width: 260px;          }          /*** @tab Columns* @section column style* @tip Set the background color and borders for your emails column area.*/            #templateColumns { /*@editable*/              background-color: #FFFFFF; /*@editable*/              border-top: 1px solid #FFFFFF; /*@editable*/              border-bottom: 1px solid #CCCCCC;          }          /*** @tab Columns* @section left column text* @tip Set the styling for your emails left column content text. Choose a size and color that is easy to read.*/            .leftColumnContent { /*@editable*/              color: #505050; /*@editable*/              font-family: "Segoe UI", Tahoma,Sans-serif; /*@editable*/              font-size: 14px; /*@editable*/              line-height: 150%;              padding-top: 0;              padding-right: 0;              padding-bottom: 20px;              padding-left: 0; /*@editable*/              text-align: left;          }              /*** @tab Columns* @section left column link* @tip Set the styling for your emails left column content links. Choose a color that helps them stand out from your text.*/                .leftColumnContent a:link, .leftColumnContent a:visited, /* Yahoo! Mail Override */ .leftColumnContent a .yshortcuts /* Yahoo! Mail Override */ { /*@editable*/                  color: #EB4102; /*@editable*/                  font-weight: normal; /*@editable*/                  text-decoration: underline;              }          /*** @tab Columns* @section right column text* @tip Set the styling for your emails right column content text. Choose a size and color that is easy to read.*/            .rightColumnContent { /*@editable*/              color: #505050; /*@editable*/              font-family: "Segoe UI", Tahoma,Sans-serif; /*@editable*/              font-size: 14px; /*@editable*/              line-height: 150%;              padding-top: 0;              padding-right: 0;              padding-bottom: 20px;              padding-left: 0; /*@editable*/              text-align: left;          }              /*** @tab Columns* @section right column link* @tip Set the styling for your emails right column content links. Choose a color that helps them stand out from your text.*/                .rightColumnContent a:link, .rightColumnContent a:visited, /* Yahoo! Mail Override */ .rightColumnContent a .yshortcuts /* Yahoo! Mail Override */ { /*@editable*/                  color: #EB4102; /*@editable*/                  font-weight: normal; /*@editable*/                  text-decoration: underline;              }                .leftColumnContent img, .rightColumnContent img {                  display: inline;                  height: auto;                  max-width: 260px;              }          /* ========== Footer Styles ========== */ /*** @tab Footer* @section footer style* @tip Set the background color and borders for your emails footer area.* @theme footer*/            #templateFooter { /*@editable*/              background-color: #FFFFFF; /*@editable*/              border-top: 1px solid #FFFFFF;          }          /*** @tab Footer* @section footer text* @tip Set the styling for your emails footer text. Choose a size and color that is easy to read.* @theme footer*/            .footerContent { /*@editable*/              color: #808080; /*@editable*/              font-family: "Segoe UI", Tahoma,Sans-serif; /*@editable*/              font-size: 10px; /*@editable*/              line-height: 150%;              padding-top: 20px;              padding-right: 20px;              padding-bottom: 20px;              padding-left: 20px; /*@editable*/              text-align: left;          }              /*** @tab Footer* @section footer link* @tip Set the styling for your emails footer links. Choose a color that helps them stand out from your text.*/                .footerContent a:link, .footerContent a:visited, /* Yahoo! Mail Override */ .footerContent a .yshortcuts, .footerContent a span /* Yahoo! Mail Override */ { /*@editable*/                  color: #606060; /*@editable*/                  font-weight: normal; /*@editable*/                  text-decoration: underline;              }          /* /\/\/\/\/\/\/\/\/ MOBILE STYLES /\/\/\/\/\/\/\/\/ */            @media only screen and (max-width: 480px) { /* /\/\/\/\/\/\/ CLIENT-SPECIFIC MOBILE STYLES /\/\/\/\/\/\/ */                body, table, td, p, a, li, blockquote {                  -webkit-text-size-adjust: none !important;              }              /* Prevent Webkit platforms from changing default text sizes */                body {                  width: 100% !important;                  min-width: 100% !important;              }              /* Prevent iOS Mail from adding padding to the body */ /* /\/\/\/\/\/\/ MOBILE RESET STYLES /\/\/\/\/\/\/ */                #bodyCell {                  padding: 10px !important;              }              /* /\/\/\/\/\/\/ MOBILE TEMPLATE STYLES /\/\/\/\/\/\/ */ /* ======== Page Styles ======== */ /*** @tab Mobile Styles* @section template width* @tip Make the template fluid for portrait or landscape view adaptability. If a fluid layout doesnt work for you, set the width to 300px instead.*/                #templateContainer {                  max-width: 600px !important; /*@editable*/                  width: 100% !important;              }              /*** @tab Mobile Styles* @section heading 1* @tip Make the first-level headings larger in size for better readability on small screens.*/                h1 { /*@editable*/                  font-size: 24px !important; /*@editable*/                  line-height: 100% !important;              }              /*** @tab Mobile Styles* @section heading 2* @tip Make the second-level headings larger in size for better readability on small screens.*/                h2 { /*@editable*/                  font-size: 20px !important; /*@editable*/                  line-height: 100% !important;              }              /*** @tab Mobile Styles* @section heading 3* @tip Make the third-level headings larger in size for better readability on small screens.*/                h3 { /*@editable*/                  font-size: 18px !important; /*@editable*/                  line-height: 100% !important;              }              /*** @tab Mobile Styles* @section heading 4* @tip Make the fourth-level headings larger in size for better readability on small screens.*/                h4 { /*@editable*/                  font-size: 16px !important; /*@editable*/                  line-height: 100% !important;              }              /* ======== Header Styles ======== */                #templatePreheader {                  display: none !important;              }              /* Hide the template preheader to save space */ /*** @tab Mobile Styles* @section header image* @tip Make the main header image fluid for portrait or landscape view adaptability, and set the images original width as the max-width. If a fluid setting doesnt work, set the image width to half its original size instead.*/                #headerImage {                  height: auto !important; /*@editable*/                  max-width: 600px !important; /*@editable*/                  width: 100% !important;              }              /*** @tab Mobile Styles* @section header text* @tip Make the header content text larger in size for better readability on small screens. We recommend a font size of at least 16px.*/                .headerContent { /*@editable*/                  font-size: 20px !important; /*@editable*/                  line-height: 125% !important;              }              /* ======== Body Styles ======== */ /*** @tab Mobile Styles* @section body image* @tip Make the main body image fluid for portrait or landscape view adaptability, and set the images original width as the max-width. If a fluid setting doesnt work, set the image width to half its original size instead.*/                #bodyImage {                  height: auto !important; /*@editable*/                  max-width: 560px !important; /*@editable*/                  width: 100% !important;              }              /*** @tab Mobile Styles* @section body text* @tip Make the body content text larger in size for better readability on small screens. We recommend a font size of at least 16px.*/                .bodyContent { /*@editable*/                  font-size: 18px !important; /*@editable*/                  line-height: 125% !important;              }              /* ======== Column Styles ======== */                .templateColumnContainer {                  display: block !important;                  width: 100% !important;              }              /*** @tab Mobile Styles* @section column image* @tip Make the column image fluid for portrait or landscape view adaptability, and set the images original width as the max-width. If a fluid setting doesnt work, set the image width to half its original size instead.*/                .columnImage {                  height: auto !important; /*@editable*/                  max-width: 260px !important; /*@editable*/                  width: 100% !important;              }              /*** @tab Mobile Styles* @section left column text* @tip Make the left column content text larger in size for better readability on small screens. We recommend a font size of at least 16px.*/                .leftColumnContent { /*@editable*/                  font-size: 16px !important; /*@editable*/                  line-height: 125% !important;              }              /*** @tab Mobile Styles* @section right column text* @tip Make the right column content text larger in size for better readability on small screens. We recommend a font size of at least 16px.*/                .rightColumnContent { /*@editable*/                  font-size: 16px !important; /*@editable*/                  line-height: 125% !important;              }              /* ======== Footer Styles ======== */ /*** @tab Mobile Styles* @section footer text* @tip Make the body content text larger in size for better readability on small screens.*/                .footerContent { /*@editable*/                  font-size: 14px !important; /*@editable*/                  line-height: 115% !important;              }                    .footerContent a {                      display: block !important;                  }              /* Place footer social and utility links on their own lines, for easier access */          }      </style>
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
                                     <td valign="top" class="headerContent">                                              <img src="https://raw.githubusercontent.com/Azure/Commercial-Marketplace-SaaS-Accelerator/main/src/CustomerSite/wwwroot/contoso-sales.png" style="max-width: 300px; display: block; margin-left: auto; margin-right: auto; padding-top:10px;padding-bottom:10px;" id="headerImage" />                                          </td>
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
                                                                            <p>${welcometext}</p>
                                                                              <table border="0" cellpadding="0" cellspacing="0" width="100%" id="templateBody">
                                                                              ${subscriptiondetails}   
                                                                                                                  </table>
                                        <p style=" margin-left: auto; margin-right: auto; text-align:right;">                                                  <a href="https://saaskitdemoapp.azurewebsites.net/">                                                      <button style="background-color:#2168A6;line-height:30px;color:white"><b>View Details</b></button>                                                  </a>                                              </p>
                                        <!--     CTA button -->                                              <!--<table style="background: #0078D7;" cellspacing="0" cellpadding="0" align="left">                                                  <tbody>                                                      <tr>                                                          <td style="padding-left: 15px; font-size: 18px; line-height: 20px; font-family:"Segoe UI Light"; color: #ffffff;">                                                              <a style="text-decoration: none; font-size: 18px; line-height: 20px; font-family:"Segoe UI Light"; color: #ffffff;" href="${LinkToPortal}/#/login">${LoginButtonTextInEmail}</a>                                                          </td>                                                          <td style="line-height: 1px; font-size: 1px; padding: 10px;">                                                              <a>                                                                  <img src="https://info.microsoft.com/rs/157-GQE-382/images/Program-CTAButton-whiteltr.png" border="0" alt="" height="20" />                                                              </a>                                                          </td>                                                      </tr>                                                  </tbody>                                              </table>-->                                              <!--     end CTA button -->                                          
                                     </td>
                                  </tr>
                         <tr>
                            <td align="center" valign="top">
                               <!-- BEGIN PREHEADER // -->                                  
                               <table border="0" cellpadding="0" cellspacing="0" width="100%" id="templatePreheader">
                                  <tr>
                                     <td valign="top" class="preheaderContent" style="padding-top: 10px; padding-right: 20px; padding-bottom: 10px; padding-left: 20px;">                                              You are receiving this message because of an interaction with                                              <a href="https://saaskitdemoapp.azurewebsites.net/">Contoso</a>.                                              Please contact us at <a href="https://saaskitdemoapp.azurewebsites.net/">Contoso</a>                                              in case you think you have received this message in error or need help.                                          </td>
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
             <!-- // END BODY -->          </td>          </tr>          </table>           <!-- // END TEMPLATE -->      
          </center>
       </body>
    </html>','Failed',1),
    	('PendingActivation','Pending Activation','7/12/2024 3:22:00 PM', '
    <!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">  
    <html xmlns="http://www.w3.org/1999/xhtml">
       <head>
          <!--<note>Use this Template for Pending Activation email </note>-->      
          <meta http-equiv="Content-Type" content="text/html; charset=UTF-8" />
          <!--<title>Cloud Market Place - Your registration is successful</title>-->      
          <style type="text/css">          /* /\/\/\/\/\/\/\/\/ CLIENT-SPECIFIC STYLES /\/\/\/\/\/\/\/\/ */            #outlook a {              padding: 0;          }          /* Force Outlook to provide a view in browser message */            .ReadMsgBody {              width: 100%;          }            .ExternalClass {              width: 100%;          }              /* Force Hotmail to display emails at full width */                .ExternalClass, .ExternalClass p, .ExternalClass span, .ExternalClass font, .ExternalClass td, .ExternalClass div {                  line-height: 100%;              }          /* Force Hotmail to display normal line spacing */            body, table, td, p, a, li, blockquote {              -webkit-text-size-adjust: 100%;              -ms-text-size-adjust: 100%;          }          /* Prevent WebKit and Windows mobile changing default text sizes */            table, td {              mso-table-lspace: 0pt;              mso-table-rspace: 0pt;          }          /* Remove spacing between tables in Outlook 2007 and up */            img {              -ms-interpolation-mode: bicubic;          }          /* Allow smoother rendering of resized image in Internet Explorer */ /* /\/\/\/\/\/\/\/\/ RESET STYLES /\/\/\/\/\/\/\/\/ */            body {              margin: 0;              padding: 0;          }            img {              border: 0;              height: auto;              line-height: 100%;              outline: none;              text-decoration: none;          }            table {              border-collapse: collapse !important;          }            body, #bodyTable, #bodyCell {              height: 100% !important;              margin: 0;              padding: 0;              width: 100% !important;          }          /* /\/\/\/\/\/\/\/\/ TEMPLATE STYLES /\/\/\/\/\/\/\/\/ */ /* ========== Page Styles ========== */            #bodyCell {              padding: 20px;          }            #templateContainer {              width: 600px;          }          /*** @tab Page* @section background style* @tip Set the background color and top border for your email. You may want to choose colors that match your companys branding.* @theme page*/            body, #bodyTable { /*@editable*/              background-color: #FFFFFF;          }          /*** @tab Page* @section background style* @tip Set the background color and top border for your email. You may want to choose colors that match your companys branding.* @theme page*/            #bodyCell { /*@editable*/              border-top: 4px solid #FFFFFF;          }          /*** @tab Page* @section email border* @tip Set the border for your email.*/            #templateContainer { /*@editable*/              border: 1px solid #BBBBBB;          }          /*** @tab Page* @section heading 1* @tip Set the styling for all first-level headings in your emails. These should be the largest of your headings.* @style heading 1*/            h1 { /*@editable*/              color: #202020 !important;              display: block; /*@editable*/              font-family: "Segoe UI", Tahoma,Sans-serif; /*@editable*/              font-size: 26px; /*@editable*/              font-style: normal; /*@editable*/              font-weight: bold; /*@editable*/              line-height: 100%; /*@editable*/              letter-spacing: normal;              margin-top: 0;              margin-right: 0;              margin-bottom: 10px;              margin-left: 0; /*@editable*/              text-align: left;          }          /*** @tab Page* @section heading 2* @tip Set the styling for all second-level headings in your emails.* @style heading 2*/            h2 { /*@editable*/              color: #404040 !important;              display: block; /*@editable*/              font-family: "Segoe UI", Tahoma,Sans-serif; /*@editable*/              font-size: 20px; /*@editable*/              font-style: normal; /*@editable*/              font-weight: bold; /*@editable*/              line-height: 100%; /*@editable*/              letter-spacing: normal;              margin-top: 0;              margin-right: 0;              margin-bottom: 10px;              margin-left: 0; /*@editable*/              text-align: left;          }          /*** @tab Page* @section heading 3* @tip Set the styling for all third-level headings in your emails.* @style heading 3*/            h3 { /*@editable*/              color: #606060 !important;              display: block; /*@editable*/              font-family: "Segoe UI", Tahoma,Sans-serif; /*@editable*/              font-size: 16px; /*@editable*/              font-style: italic; /*@editable*/              font-weight: normal; /*@editable*/              line-height: 100%; /*@editable*/              letter-spacing: normal;              margin-top: 0;              margin-right: 0;              margin-bottom: 10px;              margin-left: 0; /*@editable*/              text-align: left;          }          /*** @tab Page* @section heading 4* @tip Set the styling for all fourth-level headings in your emails. These should be the smallest of your headings.* @style heading 4*/            h4 { /*@editable*/              color: #000000 !important;              display: block; /*@editable*/              font-family: "Segoe UI", Tahoma,Sans-serif; /*@editable*/              font-size: 14px; /*@editable*/              font-style: italic; /*@editable*/              font-weight: normal; /*@editable*/              line-height: 100%; /*@editable*/              letter-spacing: normal;              margin-top: 0;              margin-right: 0;              margin-bottom: 10px;              margin-left: 0; /*@editable*/              text-align: left;          }          /* ========== Header Styles ========== */ /*** @tab Header* @section preheader style* @tip Set the background color and bottom border for your emails preheader area.* @theme header*/            #templatePreheader { /*@editable*/              background-color: #FFFFFF; /*@editable*/              border-bottom: 1px solid #CCCCCC;          }          /*** @tab Header* @section preheader text* @tip Set the styling for your emails preheader text. Choose a size and color that is easy to read.*/            .preheaderContent { /*@editable*/              color: #000000; /*@editable*/              font-family: wf_segoe-ui_normal, "Segoe UI", "Segoe WP", Tahoma, Arial, sans-serif; /*@editable*/              font-size: 10px; /*@editable*/              line-height: 125%; /*@editable*/              text-align: left;          }              /*** @tab Header* @section preheader link* @tip Set the styling for your emails preheader links. Choose a color that helps them stand out from your text.*/                .preheaderContent a:link, .preheaderContent a:visited, /* Yahoo! Mail Override */ .preheaderContent a .yshortcuts /* Yahoo! Mail Override */ { /*@editable*/                  color: #606060; /*@editable*/                  font-weight: normal; /*@editable*/                  text-decoration: underline;              }          /*** @tab Header* @section header style* @tip Set the background color and borders for your emails header area.* @theme header*/            #templateHeader { /*@editable*/              background-color: #FFFFFF; /*@editable*/              border-top: 1px solid #FFFFFF; /*@editable*/              border-bottom: 1px solid #CCCCCC;          }          /*** @tab Header* @section header text* @tip Set the styling for your emails header text. Choose a size and color that is easy to read.*/            .headerContent { /*@editable*/              color: #505050; /*@editable*/              font-family: "Segoe UI", Tahoma,Sans-serif; /*@editable*/              font-size: 20px; /*@editable*/              font-weight: bold; /*@editable*/              line-height: 100%; /*@editable*/              padding-top: 0; /*@editable*/              padding-right: 0; /*@editable*/              padding-bottom: 0; /*@editable*/              padding-left: 0; /*@editable*/              text-align: left; /*@editable*/              vertical-align: middle;          }              /*** @tab Header* @section header link* @tip Set the styling for your emails header links. Choose a color that helps them stand out from your text.*/                .headerContent a:link, .headerContent a:visited, /* Yahoo! Mail Override */ .headerContent a .yshortcuts /* Yahoo! Mail Override */ { /*@editable*/                  color: #EB4102; /*@editable*/                  font-weight: normal; /*@editable*/                  text-decoration: underline;              }            #headerImage {              height: auto;              max-width: 600px;          }          /* ========== Body Styles ========== */ /*** @tab Body* @section body style* @tip Set the background color and borders for your emails body area.*/            #templateBody { /*@editable*/              background-color: #FFFFFF; /*@editable*/              border-top: 1px solid #FFFFFF; /*@editable*/              border-bottom: 1px solid #CCCCCC;          }          /*** @tab Body* @section body text* @tip Set the styling for your emails main content text. Choose a size and color that is easy to read.* @theme main*/            .bodyContent { /*@editable*/              color: #505050; /*@editable*/              font-family: "Segoe UI", Tahoma,Sans-serif; /*@editable*/              font-size: 16px; /*@editable*/              line-height: 150%;              padding-top: 20px;              padding-right: 20px;              padding-bottom: 20px;              padding-left: 20px; /*@editable*/              text-align: left;          }              /*** @tab Body* @section body link* @tip Set the styling for your emails main content links. Choose a color that helps them stand out from your text.*/                .bodyContent a:link, .bodyContent a:visited, /* Yahoo! Mail Override */ .bodyContent a .yshortcuts /* Yahoo! Mail Override */ { /*@editable*/                  color: #EB4102; /*@editable*/                  font-weight: normal; /*@editable*/                  text-decoration: underline;              }                .bodyContent img {                  display: inline;                  height: auto;                  max-width: 560px;              }          /* ========== Column Styles ========== */            .templateColumnContainer {              display: inline;              width: 260px;          }          /*** @tab Columns* @section column style* @tip Set the background color and borders for your emails column area.*/            #templateColumns { /*@editable*/              background-color: #FFFFFF; /*@editable*/              border-top: 1px solid #FFFFFF; /*@editable*/              border-bottom: 1px solid #CCCCCC;          }          /*** @tab Columns* @section left column text* @tip Set the styling for your emails left column content text. Choose a size and color that is easy to read.*/            .leftColumnContent { /*@editable*/              color: #505050; /*@editable*/              font-family: "Segoe UI", Tahoma,Sans-serif; /*@editable*/              font-size: 14px; /*@editable*/              line-height: 150%;              padding-top: 0;              padding-right: 0;              padding-bottom: 20px;              padding-left: 0; /*@editable*/              text-align: left;          }              /*** @tab Columns* @section left column link* @tip Set the styling for your emails left column content links. Choose a color that helps them stand out from your text.*/                .leftColumnContent a:link, .leftColumnContent a:visited, /* Yahoo! Mail Override */ .leftColumnContent a .yshortcuts /* Yahoo! Mail Override */ { /*@editable*/                  color: #EB4102; /*@editable*/                  font-weight: normal; /*@editable*/                  text-decoration: underline;              }          /*** @tab Columns* @section right column text* @tip Set the styling for your emails right column content text. Choose a size and color that is easy to read.*/            .rightColumnContent { /*@editable*/              color: #505050; /*@editable*/              font-family: "Segoe UI", Tahoma,Sans-serif; /*@editable*/              font-size: 14px; /*@editable*/              line-height: 150%;              padding-top: 0;              padding-right: 0;              padding-bottom: 20px;              padding-left: 0; /*@editable*/              text-align: left;          }              /*** @tab Columns* @section right column link* @tip Set the styling for your emails right column content links. Choose a color that helps them stand out from your text.*/                .rightColumnContent a:link, .rightColumnContent a:visited, /* Yahoo! Mail Override */ .rightColumnContent a .yshortcuts /* Yahoo! Mail Override */ { /*@editable*/                  color: #EB4102; /*@editable*/                  font-weight: normal; /*@editable*/                  text-decoration: underline;              }                .leftColumnContent img, .rightColumnContent img {                  display: inline;                  height: auto;                  max-width: 260px;              }          /* ========== Footer Styles ========== */ /*** @tab Footer* @section footer style* @tip Set the background color and borders for your emails footer area.* @theme footer*/            #templateFooter { /*@editable*/              background-color: #FFFFFF; /*@editable*/              border-top: 1px solid #FFFFFF;          }          /*** @tab Footer* @section footer text* @tip Set the styling for your emails footer text. Choose a size and color that is easy to read.* @theme footer*/            .footerContent { /*@editable*/              color: #808080; /*@editable*/              font-family: "Segoe UI", Tahoma,Sans-serif; /*@editable*/              font-size: 10px; /*@editable*/              line-height: 150%;              padding-top: 20px;              padding-right: 20px;              padding-bottom: 20px;              padding-left: 20px; /*@editable*/              text-align: left;          }              /*** @tab Footer* @section footer link* @tip Set the styling for your emails footer links. Choose a color that helps them stand out from your text.*/                .footerContent a:link, .footerContent a:visited, /* Yahoo! Mail Override */ .footerContent a .yshortcuts, .footerContent a span /* Yahoo! Mail Override */ { /*@editable*/                  color: #606060; /*@editable*/                  font-weight: normal; /*@editable*/                  text-decoration: underline;              }          /* /\/\/\/\/\/\/\/\/ MOBILE STYLES /\/\/\/\/\/\/\/\/ */            @media only screen and (max-width: 480px) { /* /\/\/\/\/\/\/ CLIENT-SPECIFIC MOBILE STYLES /\/\/\/\/\/\/ */                body, table, td, p, a, li, blockquote {                  -webkit-text-size-adjust: none !important;              }              /* Prevent Webkit platforms from changing default text sizes */                body {                  width: 100% !important;                  min-width: 100% !important;              }              /* Prevent iOS Mail from adding padding to the body */ /* /\/\/\/\/\/\/ MOBILE RESET STYLES /\/\/\/\/\/\/ */                #bodyCell {                  padding: 10px !important;              }              /* /\/\/\/\/\/\/ MOBILE TEMPLATE STYLES /\/\/\/\/\/\/ */ /* ======== Page Styles ======== */ /*** @tab Mobile Styles* @section template width* @tip Make the template fluid for portrait or landscape view adaptability. If a fluid layout doesnt work for you, set the width to 300px instead.*/                #templateContainer {                  max-width: 600px !important; /*@editable*/                  width: 100% !important;              }              /*** @tab Mobile Styles* @section heading 1* @tip Make the first-level headings larger in size for better readability on small screens.*/                h1 { /*@editable*/                  font-size: 24px !important; /*@editable*/                  line-height: 100% !important;              }              /*** @tab Mobile Styles* @section heading 2* @tip Make the second-level headings larger in size for better readability on small screens.*/                h2 { /*@editable*/                  font-size: 20px !important; /*@editable*/                  line-height: 100% !important;              }              /*** @tab Mobile Styles* @section heading 3* @tip Make the third-level headings larger in size for better readability on small screens.*/                h3 { /*@editable*/                  font-size: 18px !important; /*@editable*/                  line-height: 100% !important;              }              /*** @tab Mobile Styles* @section heading 4* @tip Make the fourth-level headings larger in size for better readability on small screens.*/                h4 { /*@editable*/                  font-size: 16px !important; /*@editable*/                  line-height: 100% !important;              }              /* ======== Header Styles ======== */                #templatePreheader {                  display: none !important;              }              /* Hide the template preheader to save space */ /*** @tab Mobile Styles* @section header image* @tip Make the main header image fluid for portrait or landscape view adaptability, and set the images original width as the max-width. If a fluid setting doesnt work, set the image width to half its original size instead.*/                #headerImage {                  height: auto !important; /*@editable*/                  max-width: 600px !important; /*@editable*/                  width: 100% !important;              }              /*** @tab Mobile Styles* @section header text* @tip Make the header content text larger in size for better readability on small screens. We recommend a font size of at least 16px.*/                .headerContent { /*@editable*/                  font-size: 20px !important; /*@editable*/                  line-height: 125% !important;              }              /* ======== Body Styles ======== */ /*** @tab Mobile Styles* @section body image* @tip Make the main body image fluid for portrait or landscape view adaptability, and set the images original width as the max-width. If a fluid setting doesnt work, set the image width to half its original size instead.*/                #bodyImage {                  height: auto !important; /*@editable*/                  max-width: 560px !important; /*@editable*/                  width: 100% !important;              }              /*** @tab Mobile Styles* @section body text* @tip Make the body content text larger in size for better readability on small screens. We recommend a font size of at least 16px.*/                .bodyContent { /*@editable*/                  font-size: 18px !important; /*@editable*/                  line-height: 125% !important;              }              /* ======== Column Styles ======== */                .templateColumnContainer {                  display: block !important;                  width: 100% !important;              }              /*** @tab Mobile Styles* @section column image* @tip Make the column image fluid for portrait or landscape view adaptability, and set the images original width as the max-width. If a fluid setting doesnt work, set the image width to half its original size instead.*/                .columnImage {                  height: auto !important; /*@editable*/                  max-width: 260px !important; /*@editable*/                  width: 100% !important;              }              /*** @tab Mobile Styles* @section left column text* @tip Make the left column content text larger in size for better readability on small screens. We recommend a font size of at least 16px.*/                .leftColumnContent { /*@editable*/                  font-size: 16px !important; /*@editable*/                  line-height: 125% !important;              }              /*** @tab Mobile Styles* @section right column text* @tip Make the right column content text larger in size for better readability on small screens. We recommend a font size of at least 16px.*/                .rightColumnContent { /*@editable*/                  font-size: 16px !important; /*@editable*/                  line-height: 125% !important;              }              /* ======== Footer Styles ======== */ /*** @tab Mobile Styles* @section footer text* @tip Make the body content text larger in size for better readability on small screens.*/                .footerContent { /*@editable*/                  font-size: 14px !important; /*@editable*/                  line-height: 115% !important;              }                    .footerContent a {                      display: block !important;                  }              /* Place footer social and utility links on their own lines, for easier access */          }      </style>
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
                                     <td valign="top" class="headerContent">                                              <img src="https://raw.githubusercontent.com/Azure/Commercial-Marketplace-SaaS-Accelerator/main/src/CustomerSite/wwwroot/contoso-sales.png" style="max-width: 300px; display: block; margin-left: auto; margin-right: auto; padding-top:10px;padding-bottom:10px;" id="headerImage" />                                          </td>
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
                                                                            <p>${welcometext}</p>
                                                                              <table border="0" cellpadding="0" cellspacing="0" width="100%" id="templateBody">
                                                                              ${subscriptiondetails}   
                                                                                                                  </table>
                                        <p style=" margin-left: auto; margin-right: auto; text-align:right;">                                                  <a href="https://saaskitdemoapp.azurewebsites.net/">                                                      <button style="background-color:#2168A6;line-height:30px;color:white"><b>View Details</b></button>                                                  </a>                                              </p>
                                        <!--     CTA button -->                                              <!--<table style="background: #0078D7;" cellspacing="0" cellpadding="0" align="left">                                                  <tbody>                                                      <tr>                                                          <td style="padding-left: 15px; font-size: 18px; line-height: 20px; font-family:"Segoe UI Light"; color: #ffffff;">                                                              <a style="text-decoration: none; font-size: 18px; line-height: 20px; font-family:"Segoe UI Light"; color: #ffffff;" href="${LinkToPortal}/#/login">${LoginButtonTextInEmail}</a>                                                          </td>                                                          <td style="line-height: 1px; font-size: 1px; padding: 10px;">                                                              <a>                                                                  <img src="https://info.microsoft.com/rs/157-GQE-382/images/Program-CTAButton-whiteltr.png" border="0" alt="" height="20" />                                                              </a>                                                          </td>                                                      </tr>                                                  </tbody>                                              </table>-->                                              <!--     end CTA button -->                                          
                                     </td>
                                  </tr>
                         <tr>
                            <td align="center" valign="top">
                               <!-- BEGIN PREHEADER // -->                                  
                               <table border="0" cellpadding="0" cellspacing="0" width="100%" id="templatePreheader">
                                  <tr>
                                     <td valign="top" class="preheaderContent" style="padding-top: 10px; padding-right: 20px; padding-bottom: 10px; padding-left: 20px;">                                              You are receiving this message because of an interaction with                                              <a href="https://saaskitdemoapp.azurewebsites.net/">Contoso</a>.                                              Please contact us at <a href="https://saaskitdemoapp.azurewebsites.net/">Contoso</a>                                              in case you think you have received this message in error or need help.                                          </td>
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
             <!-- // END BODY -->          </td>          </tr>          </table>           <!-- // END TEMPLATE -->      
          </center>
       </body>
    </html>
    ','Pending Activation',1),
    	('Subscribed','Subscribed','7/12/2024 3:22:00 PM', '
    <!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">  
    <html xmlns="http://www.w3.org/1999/xhtml">
       <head>
          <!--<note>Use this Template for Pending Activation email </note>-->      
          <meta http-equiv="Content-Type" content="text/html; charset=UTF-8" />
          <!--<title>Cloud Market Place - Your registration is successful</title>-->      
          <style type="text/css">          /* /\/\/\/\/\/\/\/\/ CLIENT-SPECIFIC STYLES /\/\/\/\/\/\/\/\/ */            #outlook a {              padding: 0;          }          /* Force Outlook to provide a view in browser message */            .ReadMsgBody {              width: 100%;          }            .ExternalClass {              width: 100%;          }              /* Force Hotmail to display emails at full width */                .ExternalClass, .ExternalClass p, .ExternalClass span, .ExternalClass font, .ExternalClass td, .ExternalClass div {                  line-height: 100%;              }          /* Force Hotmail to display normal line spacing */            body, table, td, p, a, li, blockquote {              -webkit-text-size-adjust: 100%;              -ms-text-size-adjust: 100%;          }          /* Prevent WebKit and Windows mobile changing default text sizes */            table, td {              mso-table-lspace: 0pt;              mso-table-rspace: 0pt;          }          /* Remove spacing between tables in Outlook 2007 and up */            img {              -ms-interpolation-mode: bicubic;          }          /* Allow smoother rendering of resized image in Internet Explorer */ /* /\/\/\/\/\/\/\/\/ RESET STYLES /\/\/\/\/\/\/\/\/ */            body {              margin: 0;              padding: 0;          }            img {              border: 0;              height: auto;              line-height: 100%;              outline: none;              text-decoration: none;          }            table {              border-collapse: collapse !important;          }            body, #bodyTable, #bodyCell {              height: 100% !important;              margin: 0;              padding: 0;              width: 100% !important;          }          /* /\/\/\/\/\/\/\/\/ TEMPLATE STYLES /\/\/\/\/\/\/\/\/ */ /* ========== Page Styles ========== */            #bodyCell {              padding: 20px;          }            #templateContainer {              width: 600px;          }          /*** @tab Page* @section background style* @tip Set the background color and top border for your email. You may want to choose colors that match your companys branding.* @theme page*/            body, #bodyTable { /*@editable*/              background-color: #FFFFFF;          }          /*** @tab Page* @section background style* @tip Set the background color and top border for your email. You may want to choose colors that match your companys branding.* @theme page*/            #bodyCell { /*@editable*/              border-top: 4px solid #FFFFFF;          }          /*** @tab Page* @section email border* @tip Set the border for your email.*/            #templateContainer { /*@editable*/              border: 1px solid #BBBBBB;          }          /*** @tab Page* @section heading 1* @tip Set the styling for all first-level headings in your emails. These should be the largest of your headings.* @style heading 1*/            h1 { /*@editable*/              color: #202020 !important;              display: block; /*@editable*/              font-family: "Segoe UI", Tahoma,Sans-serif; /*@editable*/              font-size: 26px; /*@editable*/              font-style: normal; /*@editable*/              font-weight: bold; /*@editable*/              line-height: 100%; /*@editable*/              letter-spacing: normal;              margin-top: 0;              margin-right: 0;              margin-bottom: 10px;              margin-left: 0; /*@editable*/              text-align: left;          }          /*** @tab Page* @section heading 2* @tip Set the styling for all second-level headings in your emails.* @style heading 2*/            h2 { /*@editable*/              color: #404040 !important;              display: block; /*@editable*/              font-family: "Segoe UI", Tahoma,Sans-serif; /*@editable*/              font-size: 20px; /*@editable*/              font-style: normal; /*@editable*/              font-weight: bold; /*@editable*/              line-height: 100%; /*@editable*/              letter-spacing: normal;              margin-top: 0;              margin-right: 0;              margin-bottom: 10px;              margin-left: 0; /*@editable*/              text-align: left;          }          /*** @tab Page* @section heading 3* @tip Set the styling for all third-level headings in your emails.* @style heading 3*/            h3 { /*@editable*/              color: #606060 !important;              display: block; /*@editable*/              font-family: "Segoe UI", Tahoma,Sans-serif; /*@editable*/              font-size: 16px; /*@editable*/              font-style: italic; /*@editable*/              font-weight: normal; /*@editable*/              line-height: 100%; /*@editable*/              letter-spacing: normal;              margin-top: 0;              margin-right: 0;              margin-bottom: 10px;              margin-left: 0; /*@editable*/              text-align: left;          }          /*** @tab Page* @section heading 4* @tip Set the styling for all fourth-level headings in your emails. These should be the smallest of your headings.* @style heading 4*/            h4 { /*@editable*/              color: #000000 !important;              display: block; /*@editable*/              font-family: "Segoe UI", Tahoma,Sans-serif; /*@editable*/              font-size: 14px; /*@editable*/              font-style: italic; /*@editable*/              font-weight: normal; /*@editable*/              line-height: 100%; /*@editable*/              letter-spacing: normal;              margin-top: 0;              margin-right: 0;              margin-bottom: 10px;              margin-left: 0; /*@editable*/              text-align: left;          }          /* ========== Header Styles ========== */ /*** @tab Header* @section preheader style* @tip Set the background color and bottom border for your emails preheader area.* @theme header*/            #templatePreheader { /*@editable*/              background-color: #FFFFFF; /*@editable*/              border-bottom: 1px solid #CCCCCC;          }          /*** @tab Header* @section preheader text* @tip Set the styling for your emails preheader text. Choose a size and color that is easy to read.*/            .preheaderContent { /*@editable*/              color: #000000; /*@editable*/              font-family: wf_segoe-ui_normal, "Segoe UI", "Segoe WP", Tahoma, Arial, sans-serif; /*@editable*/              font-size: 10px; /*@editable*/              line-height: 125%; /*@editable*/              text-align: left;          }              /*** @tab Header* @section preheader link* @tip Set the styling for your emails preheader links. Choose a color that helps them stand out from your text.*/                .preheaderContent a:link, .preheaderContent a:visited, /* Yahoo! Mail Override */ .preheaderContent a .yshortcuts /* Yahoo! Mail Override */ { /*@editable*/                  color: #606060; /*@editable*/                  font-weight: normal; /*@editable*/                  text-decoration: underline;              }          /*** @tab Header* @section header style* @tip Set the background color and borders for your emails header area.* @theme header*/            #templateHeader { /*@editable*/              background-color: #FFFFFF; /*@editable*/              border-top: 1px solid #FFFFFF; /*@editable*/              border-bottom: 1px solid #CCCCCC;          }          /*** @tab Header* @section header text* @tip Set the styling for your emails header text. Choose a size and color that is easy to read.*/            .headerContent { /*@editable*/              color: #505050; /*@editable*/              font-family: "Segoe UI", Tahoma,Sans-serif; /*@editable*/              font-size: 20px; /*@editable*/              font-weight: bold; /*@editable*/              line-height: 100%; /*@editable*/              padding-top: 0; /*@editable*/              padding-right: 0; /*@editable*/              padding-bottom: 0; /*@editable*/              padding-left: 0; /*@editable*/              text-align: left; /*@editable*/              vertical-align: middle;          }              /*** @tab Header* @section header link* @tip Set the styling for your emails header links. Choose a color that helps them stand out from your text.*/                .headerContent a:link, .headerContent a:visited, /* Yahoo! Mail Override */ .headerContent a .yshortcuts /* Yahoo! Mail Override */ { /*@editable*/                  color: #EB4102; /*@editable*/                  font-weight: normal; /*@editable*/                  text-decoration: underline;              }            #headerImage {              height: auto;              max-width: 600px;          }          /* ========== Body Styles ========== */ /*** @tab Body* @section body style* @tip Set the background color and borders for your emails body area.*/            #templateBody { /*@editable*/              background-color: #FFFFFF; /*@editable*/              border-top: 1px solid #FFFFFF; /*@editable*/              border-bottom: 1px solid #CCCCCC;          }          /*** @tab Body* @section body text* @tip Set the styling for your emails main content text. Choose a size and color that is easy to read.* @theme main*/            .bodyContent { /*@editable*/              color: #505050; /*@editable*/              font-family: "Segoe UI", Tahoma,Sans-serif; /*@editable*/              font-size: 16px; /*@editable*/              line-height: 150%;              padding-top: 20px;              padding-right: 20px;              padding-bottom: 20px;              padding-left: 20px; /*@editable*/              text-align: left;          }              /*** @tab Body* @section body link* @tip Set the styling for your emails main content links. Choose a color that helps them stand out from your text.*/                .bodyContent a:link, .bodyContent a:visited, /* Yahoo! Mail Override */ .bodyContent a .yshortcuts /* Yahoo! Mail Override */ { /*@editable*/                  color: #EB4102; /*@editable*/                  font-weight: normal; /*@editable*/                  text-decoration: underline;              }                .bodyContent img {                  display: inline;                  height: auto;                  max-width: 560px;              }          /* ========== Column Styles ========== */            .templateColumnContainer {              display: inline;              width: 260px;          }          /*** @tab Columns* @section column style* @tip Set the background color and borders for your emails column area.*/            #templateColumns { /*@editable*/              background-color: #FFFFFF; /*@editable*/              border-top: 1px solid #FFFFFF; /*@editable*/              border-bottom: 1px solid #CCCCCC;          }          /*** @tab Columns* @section left column text* @tip Set the styling for your emails left column content text. Choose a size and color that is easy to read.*/            .leftColumnContent { /*@editable*/              color: #505050; /*@editable*/              font-family: "Segoe UI", Tahoma,Sans-serif; /*@editable*/              font-size: 14px; /*@editable*/              line-height: 150%;              padding-top: 0;              padding-right: 0;              padding-bottom: 20px;              padding-left: 0; /*@editable*/              text-align: left;          }              /*** @tab Columns* @section left column link* @tip Set the styling for your emails left column content links. Choose a color that helps them stand out from your text.*/                .leftColumnContent a:link, .leftColumnContent a:visited, /* Yahoo! Mail Override */ .leftColumnContent a .yshortcuts /* Yahoo! Mail Override */ { /*@editable*/                  color: #EB4102; /*@editable*/                  font-weight: normal; /*@editable*/                  text-decoration: underline;              }          /*** @tab Columns* @section right column text* @tip Set the styling for your emails right column content text. Choose a size and color that is easy to read.*/            .rightColumnContent { /*@editable*/              color: #505050; /*@editable*/              font-family: "Segoe UI", Tahoma,Sans-serif; /*@editable*/              font-size: 14px; /*@editable*/              line-height: 150%;              padding-top: 0;              padding-right: 0;              padding-bottom: 20px;              padding-left: 0; /*@editable*/              text-align: left;          }              /*** @tab Columns* @section right column link* @tip Set the styling for your emails right column content links. Choose a color that helps them stand out from your text.*/                .rightColumnContent a:link, .rightColumnContent a:visited, /* Yahoo! Mail Override */ .rightColumnContent a .yshortcuts /* Yahoo! Mail Override */ { /*@editable*/                  color: #EB4102; /*@editable*/                  font-weight: normal; /*@editable*/                  text-decoration: underline;              }                .leftColumnContent img, .rightColumnContent img {                  display: inline;                  height: auto;                  max-width: 260px;              }          /* ========== Footer Styles ========== */ /*** @tab Footer* @section footer style* @tip Set the background color and borders for your emails footer area.* @theme footer*/            #templateFooter { /*@editable*/              background-color: #FFFFFF; /*@editable*/              border-top: 1px solid #FFFFFF;          }          /*** @tab Footer* @section footer text* @tip Set the styling for your emails footer text. Choose a size and color that is easy to read.* @theme footer*/            .footerContent { /*@editable*/              color: #808080; /*@editable*/              font-family: "Segoe UI", Tahoma,Sans-serif; /*@editable*/              font-size: 10px; /*@editable*/              line-height: 150%;              padding-top: 20px;              padding-right: 20px;              padding-bottom: 20px;              padding-left: 20px; /*@editable*/              text-align: left;          }              /*** @tab Footer* @section footer link* @tip Set the styling for your emails footer links. Choose a color that helps them stand out from your text.*/                .footerContent a:link, .footerContent a:visited, /* Yahoo! Mail Override */ .footerContent a .yshortcuts, .footerContent a span /* Yahoo! Mail Override */ { /*@editable*/                  color: #606060; /*@editable*/                  font-weight: normal; /*@editable*/                  text-decoration: underline;              }          /* /\/\/\/\/\/\/\/\/ MOBILE STYLES /\/\/\/\/\/\/\/\/ */            @media only screen and (max-width: 480px) { /* /\/\/\/\/\/\/ CLIENT-SPECIFIC MOBILE STYLES /\/\/\/\/\/\/ */                body, table, td, p, a, li, blockquote {                  -webkit-text-size-adjust: none !important;              }              /* Prevent Webkit platforms from changing default text sizes */                body {                  width: 100% !important;                  min-width: 100% !important;              }              /* Prevent iOS Mail from adding padding to the body */ /* /\/\/\/\/\/\/ MOBILE RESET STYLES /\/\/\/\/\/\/ */                #bodyCell {                  padding: 10px !important;              }              /* /\/\/\/\/\/\/ MOBILE TEMPLATE STYLES /\/\/\/\/\/\/ */ /* ======== Page Styles ======== */ /*** @tab Mobile Styles* @section template width* @tip Make the template fluid for portrait or landscape view adaptability. If a fluid layout doesnt work for you, set the width to 300px instead.*/                #templateContainer {                  max-width: 600px !important; /*@editable*/                  width: 100% !important;              }              /*** @tab Mobile Styles* @section heading 1* @tip Make the first-level headings larger in size for better readability on small screens.*/                h1 { /*@editable*/                  font-size: 24px !important; /*@editable*/                  line-height: 100% !important;              }              /*** @tab Mobile Styles* @section heading 2* @tip Make the second-level headings larger in size for better readability on small screens.*/                h2 { /*@editable*/                  font-size: 20px !important; /*@editable*/                  line-height: 100% !important;              }              /*** @tab Mobile Styles* @section heading 3* @tip Make the third-level headings larger in size for better readability on small screens.*/                h3 { /*@editable*/                  font-size: 18px !important; /*@editable*/                  line-height: 100% !important;              }              /*** @tab Mobile Styles* @section heading 4* @tip Make the fourth-level headings larger in size for better readability on small screens.*/                h4 { /*@editable*/                  font-size: 16px !important; /*@editable*/                  line-height: 100% !important;              }              /* ======== Header Styles ======== */                #templatePreheader {                  display: none !important;              }              /* Hide the template preheader to save space */ /*** @tab Mobile Styles* @section header image* @tip Make the main header image fluid for portrait or landscape view adaptability, and set the images original width as the max-width. If a fluid setting doesnt work, set the image width to half its original size instead.*/                #headerImage {                  height: auto !important; /*@editable*/                  max-width: 600px !important; /*@editable*/                  width: 100% !important;              }              /*** @tab Mobile Styles* @section header text* @tip Make the header content text larger in size for better readability on small screens. We recommend a font size of at least 16px.*/                .headerContent { /*@editable*/                  font-size: 20px !important; /*@editable*/                  line-height: 125% !important;              }              /* ======== Body Styles ======== */ /*** @tab Mobile Styles* @section body image* @tip Make the main body image fluid for portrait or landscape view adaptability, and set the images original width as the max-width. If a fluid setting doesnt work, set the image width to half its original size instead.*/                #bodyImage {                  height: auto !important; /*@editable*/                  max-width: 560px !important; /*@editable*/                  width: 100% !important;              }              /*** @tab Mobile Styles* @section body text* @tip Make the body content text larger in size for better readability on small screens. We recommend a font size of at least 16px.*/                .bodyContent { /*@editable*/                  font-size: 18px !important; /*@editable*/                  line-height: 125% !important;              }              /* ======== Column Styles ======== */                .templateColumnContainer {                  display: block !important;                  width: 100% !important;              }              /*** @tab Mobile Styles* @section column image* @tip Make the column image fluid for portrait or landscape view adaptability, and set the images original width as the max-width. If a fluid setting doesnt work, set the image width to half its original size instead.*/                .columnImage {                  height: auto !important; /*@editable*/                  max-width: 260px !important; /*@editable*/                  width: 100% !important;              }              /*** @tab Mobile Styles* @section left column text* @tip Make the left column content text larger in size for better readability on small screens. We recommend a font size of at least 16px.*/                .leftColumnContent { /*@editable*/                  font-size: 16px !important; /*@editable*/                  line-height: 125% !important;              }              /*** @tab Mobile Styles* @section right column text* @tip Make the right column content text larger in size for better readability on small screens. We recommend a font size of at least 16px.*/                .rightColumnContent { /*@editable*/                  font-size: 16px !important; /*@editable*/                  line-height: 125% !important;              }              /* ======== Footer Styles ======== */ /*** @tab Mobile Styles* @section footer text* @tip Make the body content text larger in size for better readability on small screens.*/                .footerContent { /*@editable*/                  font-size: 14px !important; /*@editable*/                  line-height: 115% !important;              }                    .footerContent a {                      display: block !important;                  }              /* Place footer social and utility links on their own lines, for easier access */          }      </style>
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
                                     <td valign="top" class="headerContent">                                              <img src="https://raw.githubusercontent.com/Azure/Commercial-Marketplace-SaaS-Accelerator/main/src/CustomerSite/wwwroot/contoso-sales.png" style="max-width: 300px; display: block; margin-left: auto; margin-right: auto; padding-top:10px;padding-bottom:10px;" id="headerImage" />                                          </td>
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
                                                                            <p>${welcometext}</p>
                                                                              <table border="0" cellpadding="0" cellspacing="0" width="100%" id="templateBody">
                                                                              ${subscriptiondetails}   
                                                                                                                  </table>
                                        <p style=" margin-left: auto; margin-right: auto; text-align:right;">                                                  <a href="https://saaskitdemoapp.azurewebsites.net/">                                                      <button style="background-color:#2168A6;line-height:30px;color:white"><b>View Details</b></button>                                                  </a>                                              </p>
                                        <!--     CTA button -->                                              <!--<table style="background: #0078D7;" cellspacing="0" cellpadding="0" align="left">                                                  <tbody>                                                      <tr>                                                          <td style="padding-left: 15px; font-size: 18px; line-height: 20px; font-family:"Segoe UI Light"; color: #ffffff;">                                                              <a style="text-decoration: none; font-size: 18px; line-height: 20px; font-family:"Segoe UI Light"; color: #ffffff;" href="${LinkToPortal}/#/login">${LoginButtonTextInEmail}</a>                                                          </td>                                                          <td style="line-height: 1px; font-size: 1px; padding: 10px;">                                                              <a>                                                                  <img src="https://info.microsoft.com/rs/157-GQE-382/images/Program-CTAButton-whiteltr.png" border="0" alt="" height="20" />                                                              </a>                                                          </td>                                                      </tr>                                                  </tbody>                                              </table>-->                                              <!--     end CTA button -->                                          
                                     </td>
                                  </tr>
                         <tr>
                            <td align="center" valign="top">
                               <!-- BEGIN PREHEADER // -->                                  
                               <table border="0" cellpadding="0" cellspacing="0" width="100%" id="templatePreheader">
                                  <tr>
                                     <td valign="top" class="preheaderContent" style="padding-top: 10px; padding-right: 20px; padding-bottom: 10px; padding-left: 20px;">                                              You are receiving this message because of an interaction with                                              <a href="https://saaskitdemoapp.azurewebsites.net/">Contoso</a>.                                              Please contact us at <a href="https://saaskitdemoapp.azurewebsites.net/">Contoso</a>                                              in case you think you have received this message in error or need help.                                          </td>
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
             <!-- // END BODY -->          </td>          </tr>          </table>           <!-- // END TEMPLATE -->      
          </center>
       </body>
    </html>
    ','Subscribed',1),
    	('Unsubscribed','Unsubscribed','7/12/2024 3:22:00 PM', '
    <!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">  
    <html xmlns="http://www.w3.org/1999/xhtml">
       <head>
          <!--<note>Use this Template for Pending Activation email </note>-->      
          <meta http-equiv="Content-Type" content="text/html; charset=UTF-8" />
          <!--<title>Cloud Market Place - Your registration is successful</title>-->      
          <style type="text/css">          /* /\/\/\/\/\/\/\/\/ CLIENT-SPECIFIC STYLES /\/\/\/\/\/\/\/\/ */            #outlook a {              padding: 0;          }          /* Force Outlook to provide a view in browser message */            .ReadMsgBody {              width: 100%;          }            .ExternalClass {              width: 100%;          }              /* Force Hotmail to display emails at full width */                .ExternalClass, .ExternalClass p, .ExternalClass span, .ExternalClass font, .ExternalClass td, .ExternalClass div {                  line-height: 100%;              }          /* Force Hotmail to display normal line spacing */            body, table, td, p, a, li, blockquote {              -webkit-text-size-adjust: 100%;              -ms-text-size-adjust: 100%;          }          /* Prevent WebKit and Windows mobile changing default text sizes */            table, td {              mso-table-lspace: 0pt;              mso-table-rspace: 0pt;          }          /* Remove spacing between tables in Outlook 2007 and up */            img {              -ms-interpolation-mode: bicubic;          }          /* Allow smoother rendering of resized image in Internet Explorer */ /* /\/\/\/\/\/\/\/\/ RESET STYLES /\/\/\/\/\/\/\/\/ */            body {              margin: 0;              padding: 0;          }            img {              border: 0;              height: auto;              line-height: 100%;              outline: none;              text-decoration: none;          }            table {              border-collapse: collapse !important;          }            body, #bodyTable, #bodyCell {              height: 100% !important;              margin: 0;              padding: 0;              width: 100% !important;          }          /* /\/\/\/\/\/\/\/\/ TEMPLATE STYLES /\/\/\/\/\/\/\/\/ */ /* ========== Page Styles ========== */            #bodyCell {              padding: 20px;          }            #templateContainer {              width: 600px;          }          /*** @tab Page* @section background style* @tip Set the background color and top border for your email. You may want to choose colors that match your companys branding.* @theme page*/            body, #bodyTable { /*@editable*/              background-color: #FFFFFF;          }          /*** @tab Page* @section background style* @tip Set the background color and top border for your email. You may want to choose colors that match your companys branding.* @theme page*/            #bodyCell { /*@editable*/              border-top: 4px solid #FFFFFF;          }          /*** @tab Page* @section email border* @tip Set the border for your email.*/            #templateContainer { /*@editable*/              border: 1px solid #BBBBBB;          }          /*** @tab Page* @section heading 1* @tip Set the styling for all first-level headings in your emails. These should be the largest of your headings.* @style heading 1*/            h1 { /*@editable*/              color: #202020 !important;              display: block; /*@editable*/              font-family: "Segoe UI", Tahoma,Sans-serif; /*@editable*/              font-size: 26px; /*@editable*/              font-style: normal; /*@editable*/              font-weight: bold; /*@editable*/              line-height: 100%; /*@editable*/              letter-spacing: normal;              margin-top: 0;              margin-right: 0;              margin-bottom: 10px;              margin-left: 0; /*@editable*/              text-align: left;          }          /*** @tab Page* @section heading 2* @tip Set the styling for all second-level headings in your emails.* @style heading 2*/            h2 { /*@editable*/              color: #404040 !important;              display: block; /*@editable*/              font-family: "Segoe UI", Tahoma,Sans-serif; /*@editable*/              font-size: 20px; /*@editable*/              font-style: normal; /*@editable*/              font-weight: bold; /*@editable*/              line-height: 100%; /*@editable*/              letter-spacing: normal;              margin-top: 0;              margin-right: 0;              margin-bottom: 10px;              margin-left: 0; /*@editable*/              text-align: left;          }          /*** @tab Page* @section heading 3* @tip Set the styling for all third-level headings in your emails.* @style heading 3*/            h3 { /*@editable*/              color: #606060 !important;              display: block; /*@editable*/              font-family: "Segoe UI", Tahoma,Sans-serif; /*@editable*/              font-size: 16px; /*@editable*/              font-style: italic; /*@editable*/              font-weight: normal; /*@editable*/              line-height: 100%; /*@editable*/              letter-spacing: normal;              margin-top: 0;              margin-right: 0;              margin-bottom: 10px;              margin-left: 0; /*@editable*/              text-align: left;          }          /*** @tab Page* @section heading 4* @tip Set the styling for all fourth-level headings in your emails. These should be the smallest of your headings.* @style heading 4*/            h4 { /*@editable*/              color: #000000 !important;              display: block; /*@editable*/              font-family: "Segoe UI", Tahoma,Sans-serif; /*@editable*/              font-size: 14px; /*@editable*/              font-style: italic; /*@editable*/              font-weight: normal; /*@editable*/              line-height: 100%; /*@editable*/              letter-spacing: normal;              margin-top: 0;              margin-right: 0;              margin-bottom: 10px;              margin-left: 0; /*@editable*/              text-align: left;          }          /* ========== Header Styles ========== */ /*** @tab Header* @section preheader style* @tip Set the background color and bottom border for your emails preheader area.* @theme header*/            #templatePreheader { /*@editable*/              background-color: #FFFFFF; /*@editable*/              border-bottom: 1px solid #CCCCCC;          }          /*** @tab Header* @section preheader text* @tip Set the styling for your emails preheader text. Choose a size and color that is easy to read.*/            .preheaderContent { /*@editable*/              color: #000000; /*@editable*/              font-family: wf_segoe-ui_normal, "Segoe UI", "Segoe WP", Tahoma, Arial, sans-serif; /*@editable*/              font-size: 10px; /*@editable*/              line-height: 125%; /*@editable*/              text-align: left;          }              /*** @tab Header* @section preheader link* @tip Set the styling for your emails preheader links. Choose a color that helps them stand out from your text.*/                .preheaderContent a:link, .preheaderContent a:visited, /* Yahoo! Mail Override */ .preheaderContent a .yshortcuts /* Yahoo! Mail Override */ { /*@editable*/                  color: #606060; /*@editable*/                  font-weight: normal; /*@editable*/                  text-decoration: underline;              }          /*** @tab Header* @section header style* @tip Set the background color and borders for your emails header area.* @theme header*/            #templateHeader { /*@editable*/              background-color: #FFFFFF; /*@editable*/              border-top: 1px solid #FFFFFF; /*@editable*/              border-bottom: 1px solid #CCCCCC;          }          /*** @tab Header* @section header text* @tip Set the styling for your emails header text. Choose a size and color that is easy to read.*/            .headerContent { /*@editable*/              color: #505050; /*@editable*/              font-family: "Segoe UI", Tahoma,Sans-serif; /*@editable*/              font-size: 20px; /*@editable*/              font-weight: bold; /*@editable*/              line-height: 100%; /*@editable*/              padding-top: 0; /*@editable*/              padding-right: 0; /*@editable*/              padding-bottom: 0; /*@editable*/              padding-left: 0; /*@editable*/              text-align: left; /*@editable*/              vertical-align: middle;          }              /*** @tab Header* @section header link* @tip Set the styling for your emails header links. Choose a color that helps them stand out from your text.*/                .headerContent a:link, .headerContent a:visited, /* Yahoo! Mail Override */ .headerContent a .yshortcuts /* Yahoo! Mail Override */ { /*@editable*/                  color: #EB4102; /*@editable*/                  font-weight: normal; /*@editable*/                  text-decoration: underline;              }            #headerImage {              height: auto;              max-width: 600px;          }          /* ========== Body Styles ========== */ /*** @tab Body* @section body style* @tip Set the background color and borders for your emails body area.*/            #templateBody { /*@editable*/              background-color: #FFFFFF; /*@editable*/              border-top: 1px solid #FFFFFF; /*@editable*/              border-bottom: 1px solid #CCCCCC;          }          /*** @tab Body* @section body text* @tip Set the styling for your emails main content text. Choose a size and color that is easy to read.* @theme main*/            .bodyContent { /*@editable*/              color: #505050; /*@editable*/              font-family: "Segoe UI", Tahoma,Sans-serif; /*@editable*/              font-size: 16px; /*@editable*/              line-height: 150%;              padding-top: 20px;              padding-right: 20px;              padding-bottom: 20px;              padding-left: 20px; /*@editable*/              text-align: left;          }              /*** @tab Body* @section body link* @tip Set the styling for your emails main content links. Choose a color that helps them stand out from your text.*/                .bodyContent a:link, .bodyContent a:visited, /* Yahoo! Mail Override */ .bodyContent a .yshortcuts /* Yahoo! Mail Override */ { /*@editable*/                  color: #EB4102; /*@editable*/                  font-weight: normal; /*@editable*/                  text-decoration: underline;              }                .bodyContent img {                  display: inline;                  height: auto;                  max-width: 560px;              }          /* ========== Column Styles ========== */            .templateColumnContainer {              display: inline;              width: 260px;          }          /*** @tab Columns* @section column style* @tip Set the background color and borders for your emails column area.*/            #templateColumns { /*@editable*/              background-color: #FFFFFF; /*@editable*/              border-top: 1px solid #FFFFFF; /*@editable*/              border-bottom: 1px solid #CCCCCC;          }          /*** @tab Columns* @section left column text* @tip Set the styling for your emails left column content text. Choose a size and color that is easy to read.*/            .leftColumnContent { /*@editable*/              color: #505050; /*@editable*/              font-family: "Segoe UI", Tahoma,Sans-serif; /*@editable*/              font-size: 14px; /*@editable*/              line-height: 150%;              padding-top: 0;              padding-right: 0;              padding-bottom: 20px;              padding-left: 0; /*@editable*/              text-align: left;          }              /*** @tab Columns* @section left column link* @tip Set the styling for your emails left column content links. Choose a color that helps them stand out from your text.*/                .leftColumnContent a:link, .leftColumnContent a:visited, /* Yahoo! Mail Override */ .leftColumnContent a .yshortcuts /* Yahoo! Mail Override */ { /*@editable*/                  color: #EB4102; /*@editable*/                  font-weight: normal; /*@editable*/                  text-decoration: underline;              }          /*** @tab Columns* @section right column text* @tip Set the styling for your emails right column content text. Choose a size and color that is easy to read.*/            .rightColumnContent { /*@editable*/              color: #505050; /*@editable*/              font-family: "Segoe UI", Tahoma,Sans-serif; /*@editable*/              font-size: 14px; /*@editable*/              line-height: 150%;              padding-top: 0;              padding-right: 0;              padding-bottom: 20px;              padding-left: 0; /*@editable*/              text-align: left;          }              /*** @tab Columns* @section right column link* @tip Set the styling for your emails right column content links. Choose a color that helps them stand out from your text.*/                .rightColumnContent a:link, .rightColumnContent a:visited, /* Yahoo! Mail Override */ .rightColumnContent a .yshortcuts /* Yahoo! Mail Override */ { /*@editable*/                  color: #EB4102; /*@editable*/                  font-weight: normal; /*@editable*/                  text-decoration: underline;              }                .leftColumnContent img, .rightColumnContent img {                  display: inline;                  height: auto;                  max-width: 260px;              }          /* ========== Footer Styles ========== */ /*** @tab Footer* @section footer style* @tip Set the background color and borders for your emails footer area.* @theme footer*/            #templateFooter { /*@editable*/              background-color: #FFFFFF; /*@editable*/              border-top: 1px solid #FFFFFF;          }          /*** @tab Footer* @section footer text* @tip Set the styling for your emails footer text. Choose a size and color that is easy to read.* @theme footer*/            .footerContent { /*@editable*/              color: #808080; /*@editable*/              font-family: "Segoe UI", Tahoma,Sans-serif; /*@editable*/              font-size: 10px; /*@editable*/              line-height: 150%;              padding-top: 20px;              padding-right: 20px;              padding-bottom: 20px;              padding-left: 20px; /*@editable*/              text-align: left;          }              /*** @tab Footer* @section footer link* @tip Set the styling for your emails footer links. Choose a color that helps them stand out from your text.*/                .footerContent a:link, .footerContent a:visited, /* Yahoo! Mail Override */ .footerContent a .yshortcuts, .footerContent a span /* Yahoo! Mail Override */ { /*@editable*/                  color: #606060; /*@editable*/                  font-weight: normal; /*@editable*/                  text-decoration: underline;              }          /* /\/\/\/\/\/\/\/\/ MOBILE STYLES /\/\/\/\/\/\/\/\/ */            @media only screen and (max-width: 480px) { /* /\/\/\/\/\/\/ CLIENT-SPECIFIC MOBILE STYLES /\/\/\/\/\/\/ */                body, table, td, p, a, li, blockquote {                  -webkit-text-size-adjust: none !important;              }              /* Prevent Webkit platforms from changing default text sizes */                body {                  width: 100% !important;                  min-width: 100% !important;              }              /* Prevent iOS Mail from adding padding to the body */ /* /\/\/\/\/\/\/ MOBILE RESET STYLES /\/\/\/\/\/\/ */                #bodyCell {                  padding: 10px !important;              }              /* /\/\/\/\/\/\/ MOBILE TEMPLATE STYLES /\/\/\/\/\/\/ */ /* ======== Page Styles ======== */ /*** @tab Mobile Styles* @section template width* @tip Make the template fluid for portrait or landscape view adaptability. If a fluid layout doesnt work for you, set the width to 300px instead.*/                #templateContainer {                  max-width: 600px !important; /*@editable*/                  width: 100% !important;              }              /*** @tab Mobile Styles* @section heading 1* @tip Make the first-level headings larger in size for better readability on small screens.*/                h1 { /*@editable*/                  font-size: 24px !important; /*@editable*/                  line-height: 100% !important;              }              /*** @tab Mobile Styles* @section heading 2* @tip Make the second-level headings larger in size for better readability on small screens.*/                h2 { /*@editable*/                  font-size: 20px !important; /*@editable*/                  line-height: 100% !important;              }              /*** @tab Mobile Styles* @section heading 3* @tip Make the third-level headings larger in size for better readability on small screens.*/                h3 { /*@editable*/                  font-size: 18px !important; /*@editable*/                  line-height: 100% !important;              }              /*** @tab Mobile Styles* @section heading 4* @tip Make the fourth-level headings larger in size for better readability on small screens.*/                h4 { /*@editable*/                  font-size: 16px !important; /*@editable*/                  line-height: 100% !important;              }              /* ======== Header Styles ======== */                #templatePreheader {                  display: none !important;              }              /* Hide the template preheader to save space */ /*** @tab Mobile Styles* @section header image* @tip Make the main header image fluid for portrait or landscape view adaptability, and set the images original width as the max-width. If a fluid setting doesnt work, set the image width to half its original size instead.*/                #headerImage {                  height: auto !important; /*@editable*/                  max-width: 600px !important; /*@editable*/                  width: 100% !important;              }              /*** @tab Mobile Styles* @section header text* @tip Make the header content text larger in size for better readability on small screens. We recommend a font size of at least 16px.*/                .headerContent { /*@editable*/                  font-size: 20px !important; /*@editable*/                  line-height: 125% !important;              }              /* ======== Body Styles ======== */ /*** @tab Mobile Styles* @section body image* @tip Make the main body image fluid for portrait or landscape view adaptability, and set the images original width as the max-width. If a fluid setting doesnt work, set the image width to half its original size instead.*/                #bodyImage {                  height: auto !important; /*@editable*/                  max-width: 560px !important; /*@editable*/                  width: 100% !important;              }              /*** @tab Mobile Styles* @section body text* @tip Make the body content text larger in size for better readability on small screens. We recommend a font size of at least 16px.*/                .bodyContent { /*@editable*/                  font-size: 18px !important; /*@editable*/                  line-height: 125% !important;              }              /* ======== Column Styles ======== */                .templateColumnContainer {                  display: block !important;                  width: 100% !important;              }              /*** @tab Mobile Styles* @section column image* @tip Make the column image fluid for portrait or landscape view adaptability, and set the images original width as the max-width. If a fluid setting doesnt work, set the image width to half its original size instead.*/                .columnImage {                  height: auto !important; /*@editable*/                  max-width: 260px !important; /*@editable*/                  width: 100% !important;              }              /*** @tab Mobile Styles* @section left column text* @tip Make the left column content text larger in size for better readability on small screens. We recommend a font size of at least 16px.*/                .leftColumnContent { /*@editable*/                  font-size: 16px !important; /*@editable*/                  line-height: 125% !important;              }              /*** @tab Mobile Styles* @section right column text* @tip Make the right column content text larger in size for better readability on small screens. We recommend a font size of at least 16px.*/                .rightColumnContent { /*@editable*/                  font-size: 16px !important; /*@editable*/                  line-height: 125% !important;              }              /* ======== Footer Styles ======== */ /*** @tab Mobile Styles* @section footer text* @tip Make the body content text larger in size for better readability on small screens.*/                .footerContent { /*@editable*/                  font-size: 14px !important; /*@editable*/                  line-height: 115% !important;              }                    .footerContent a {                      display: block !important;                  }              /* Place footer social and utility links on their own lines, for easier access */          }      </style>
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
                                     <td valign="top" class="headerContent">                                              <img src="https://raw.githubusercontent.com/Azure/Commercial-Marketplace-SaaS-Accelerator/main/src/CustomerSite/wwwroot/contoso-sales.png" style="max-width: 300px; display: block; margin-left: auto; margin-right: auto; padding-top:10px;padding-bottom:10px;" id="headerImage" />                                          </td>
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
                                                                            <p>${welcometext}</p>
                                                                              <table border="0" cellpadding="0" cellspacing="0" width="100%" id="templateBody">
                                                                              ${subscriptiondetails}   
                                                                                                                  </table>
                                        <p style=" margin-left: auto; margin-right: auto; text-align:right;">                                                  <a href="https://saaskitdemoapp.azurewebsites.net/">                                                      <button style="background-color:#2168A6;line-height:30px;color:white"><b>View Details</b></button>                                                  </a>                                              </p>
                                        <!--     CTA button -->                                              <!--<table style="background: #0078D7;" cellspacing="0" cellpadding="0" align="left">                                                  <tbody>                                                      <tr>                                                          <td style="padding-left: 15px; font-size: 18px; line-height: 20px; font-family:"Segoe UI Light"; color: #ffffff;">                                                              <a style="text-decoration: none; font-size: 18px; line-height: 20px; font-family:"Segoe UI Light"; color: #ffffff;" href="${LinkToPortal}/#/login">${LoginButtonTextInEmail}</a>                                                          </td>                                                          <td style="line-height: 1px; font-size: 1px; padding: 10px;">                                                              <a>                                                                  <img src="https://info.microsoft.com/rs/157-GQE-382/images/Program-CTAButton-whiteltr.png" border="0" alt="" height="20" />                                                              </a>                                                          </td>                                                      </tr>                                                  </tbody>                                              </table>-->                                              <!--     end CTA button -->                                          
                                     </td>
                                  </tr>
                         <tr>
                            <td align="center" valign="top">
                               <!-- BEGIN PREHEADER // -->                                  
                               <table border="0" cellpadding="0" cellspacing="0" width="100%" id="templatePreheader">
                                  <tr>
                                     <td valign="top" class="preheaderContent" style="padding-top: 10px; padding-right: 20px; padding-bottom: 10px; padding-left: 20px;">                                              You are receiving this message because of an interaction with                                              <a href="https://saaskitdemoapp.azurewebsites.net/">Contoso</a>.                                              Please contact us at <a href="https://saaskitdemoapp.azurewebsites.net/">Contoso</a>                                              in case you think you have received this message in error or need help.                                          </td>
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
             <!-- // END BODY -->          </td>          </tr>          </table>           <!-- // END TEMPLATE -->      
          </center>
       </body>
    </html>
    ','Unsubscribed',1)

END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20221118045814_Baseline_v2'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20221118045814_Baseline_v2', N'8.0.6');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20221118203340_Baseline_v5'
)
BEGIN
    CREATE TABLE [SchedulerFrequency] (
        [Id] int NOT NULL IDENTITY,
        [Frequency] varchar(50) NOT NULL,
        CONSTRAINT [PK_SchedulerFrequency] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20221118203340_Baseline_v5'
)
BEGIN
    CREATE TABLE [MeteredPlanSchedulerManagement] (
        [Id] int NOT NULL IDENTITY,
        [SchedulerName] nvarchar(50) NOT NULL,
        [SubscriptionId] int NOT NULL,
        [PlanId] int NOT NULL,
        [DimensionId] int NOT NULL,
        [FrequencyId] int NOT NULL,
        [Quantity] float NOT NULL,
        [StartDate] datetime2 NOT NULL,
        [NextRunTime] datetime2 NULL,
        CONSTRAINT [PK_MeteredPlanSchedulerManagement] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_MeteredPlanSchedulerManagement_MeteredDimensions_DimensionId] FOREIGN KEY ([DimensionId]) REFERENCES [MeteredDimensions] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_MeteredPlanSchedulerManagement_Plans_PlanId] FOREIGN KEY ([PlanId]) REFERENCES [Plans] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_MeteredPlanSchedulerManagement_SchedulerFrequency_FrequencyId] FOREIGN KEY ([FrequencyId]) REFERENCES [SchedulerFrequency] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_MeteredPlanSchedulerManagement_Subscriptions_SubscriptionId] FOREIGN KEY ([SubscriptionId]) REFERENCES [Subscriptions] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20221118203340_Baseline_v5'
)
BEGIN
    CREATE INDEX [IX_MeteredPlanSchedulerManagement_DimensionId] ON [MeteredPlanSchedulerManagement] ([DimensionId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20221118203340_Baseline_v5'
)
BEGIN
    CREATE INDEX [IX_MeteredPlanSchedulerManagement_FrequencyId] ON [MeteredPlanSchedulerManagement] ([FrequencyId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20221118203340_Baseline_v5'
)
BEGIN
    CREATE INDEX [IX_MeteredPlanSchedulerManagement_PlanId] ON [MeteredPlanSchedulerManagement] ([PlanId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20221118203340_Baseline_v5'
)
BEGIN
    CREATE INDEX [IX_MeteredPlanSchedulerManagement_SubscriptionId] ON [MeteredPlanSchedulerManagement] ([SubscriptionId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20221118203340_Baseline_v5'
)
BEGIN

    EXEC(N'
    CREATE VIEW [dbo].[SchedulerManagerView]
    	AS SELECT 
    	m.Id,
    	m.SchedulerName,
    	s.AMPSubscriptionId,
    	s.Name as SubscriptionName,
    	s.PurchaserEmail,
    	p.PlanId,
    	d.Dimension,
    	f.Frequency,
    	m.Quantity,
    	m.StartDate,
    	m.NextRunTime
    	FROM MeteredPlanSchedulerManagement m
    	inner join SchedulerFrequency f	on m.FrequencyId=f.Id
    	inner join Subscriptions s on m.SubscriptionId=s.Id
    	inner join Plans p on m.PlanId=p.Id
    	inner join MeteredDimensions d on m.DimensionId=d.Id
    ')
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20221118203340_Baseline_v5'
)
BEGIN

    INSERT INTO [dbo].[SchedulerFrequency] (Frequency) VALUES ('Hourly')
    INSERT INTO [dbo].[SchedulerFrequency] (Frequency) VALUES ('Daily')
    INSERT INTO [dbo].[SchedulerFrequency] (Frequency) VALUES ('Weekly')
    INSERT INTO [dbo].[SchedulerFrequency] (Frequency) VALUES ('Monthly')
    INSERT INTO [dbo].[SchedulerFrequency] (Frequency) VALUES ('Yearly')

END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20221118203340_Baseline_v5'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20221118203340_Baseline_v5', N'8.0.6');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20221118211554_Baseline_v6'
)
BEGIN
    ALTER TABLE [MeteredAuditLogs] ADD [RunBy] varchar(255) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20221118211554_Baseline_v6'
)
BEGIN

    IF NOT EXISTS (SELECT * FROM [dbo].[SchedulerFrequency] WHERE [Frequency] = 'OneTime')
    BEGIN
        INSERT INTO [dbo].[SchedulerFrequency] (Frequency) VALUES ('OneTime')
    END;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20221118211554_Baseline_v6'
)
BEGIN

    INSERT [dbo].[ApplicationConfiguration] ( [Name], [Value], [Description]) VALUES ( N'EnableHourlyMeterSchedules', N'False', N'This will enable to run Hourly meter scheduled items')
    INSERT [dbo].[ApplicationConfiguration] ( [Name], [Value], [Description]) VALUES ( N'EnableDailyMeterSchedules', N'False', N'This will enable to run Daily meter scheduled items')
    INSERT [dbo].[ApplicationConfiguration] ( [Name], [Value], [Description]) VALUES ( N'EnableWeeklyMeterSchedules', N'False', N'This will enable to run Weekly meter scheduled items')
    INSERT [dbo].[ApplicationConfiguration] ( [Name], [Value], [Description]) VALUES ( N'EnableMonthlyMeterSchedules', N'False', N'This will enable to run Monthly meter scheduled items')
    INSERT [dbo].[ApplicationConfiguration] ( [Name], [Value], [Description]) VALUES ( N'EnableYearlyMeterSchedules', N'False', N'This will enable to run Yearly meter scheduled items')
    INSERT [dbo].[ApplicationConfiguration] ( [Name], [Value], [Description]) VALUES ( N'EnableOneTimeMeterSchedules', N'False', N'This will enable to run OneTime meter scheduled items')
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20221118211554_Baseline_v6'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20221118211554_Baseline_v6', N'8.0.6');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20230726232155_Baseline_v7'
)
BEGIN

                        IF NOT EXISTS (SELECT * FROM [dbo].[ApplicationConfiguration] WHERE [Name] = 'WebNotificationUrl')
                        BEGIN
                            INSERT [dbo].[ApplicationConfiguration] ( [Name], [Value], [Description]) VALUES ( N'WebNotificationUrl', N'', N'Setting this URL will enable pushing LandingPage/Webhook events to this external URL')
                        END

                        IF NOT EXISTS (SELECT * FROM [dbo].[ApplicationConfiguration] WHERE [Name] = 'EnablesSuccessfulSchedulerEmail')
                        BEGIN
                            INSERT [dbo].[ApplicationConfiguration] ( [Name], [Value], [Description]) VALUES ( N'EnablesSuccessfulSchedulerEmail', N'False', N'This will enable sending email for successful metered usage.')
                        END

                        IF NOT EXISTS (SELECT * FROM [dbo].[ApplicationConfiguration] WHERE [Name] = 'EnablesFailureSchedulerEmail')
                        BEGIN
                            INSERT [dbo].[ApplicationConfiguration] ( [Name], [Value], [Description]) VALUES ( N'EnablesFailureSchedulerEmail', N'False', N'This will enable sending email for failure metered usage.')
                        END

                        IF NOT EXISTS (SELECT * FROM [dbo].[ApplicationConfiguration] WHERE [Name] = 'EnablesMissingSchedulerEmail')
                        BEGIN
                            INSERT [dbo].[ApplicationConfiguration] ( [Name], [Value], [Description]) VALUES ( N'EnablesMissingSchedulerEmail', N'False', N'This will enable sending email for missing metered usage.')
                        END

                        IF NOT EXISTS (SELECT * FROM [dbo].[ApplicationConfiguration] WHERE [Name] = 'SchedulerEmailTo')
                        BEGIN
                            INSERT [dbo].[ApplicationConfiguration] ( [Name], [Value], [Description]) VALUES ( N'SchedulerEmailTo', N'', N'Scheduler email receiver(s) ')
                        END

                        IF NOT EXISTS (SELECT * FROM [dbo].[EmailTemplate] WHERE [Status] = 'Accepted')
                        BEGIN
                            INSERT [dbo].[EmailTemplate] ([Status],[Description],[InsertDate],[TemplateBody],[Subject],[isActive]) VALUES (N'Accepted',N'Accepted',GetDate(),N'<html> <head> <meta http-equiv="Content-Type" content="text/html; charset=UTF-8"/> </head> <body leftmargin="0" marginwidth="0" topmargin="0" marginheight="0" offset="0"> <center> <table align="center" border="0" cellpadding="0" cellspacing="0" height="100%" width="100%" id="bodyTable"> <tr><td align="center" valign="top" id="bodyCell"><!-- BEGIN TEMPLATE // --><table border="0" cellpadding="0" cellspacing="0" id="templateContainer"><tr><td align="center" valign="top"><!-- BEGIN BODY // --><table border="0" cellpadding="0" cellspacing="0" width="100%" id="templateBody"><tr><td valign="top" class="bodyContent"><h2>Subscription ****SubscriptionName****</h2><br><p>The Scheduled Task ****SchedulerTaskName**** was fired <b>Successfully</b></p><p>The following section is the deatil results.</p><hr/><table border="0" cellpadding="0" cellspacing="0" width="100%" id="templateBody">****ResponseJson**** </table></td></tr></table></td></tr></table><!-- // END BODY --></td></tr> </table> <!-- // END TEMPLATE --> </center> </body> </html>',N'Scheduled SaaS Metered Usage Submitted Successfully!',N'True')
                        END

                        IF NOT EXISTS (SELECT * FROM [dbo].[EmailTemplate] WHERE [Status] = 'Failure')
                        BEGIN
                            INSERT [dbo].[EmailTemplate] ([Status],[Description],[InsertDate],[TemplateBody],[Subject],[isActive]) VALUES (N'Failure',N'Failure',GetDate(),N'<html><head><meta http-equiv="Content-Type" content="text/html; charset=UTF-8"/></head><body leftmargin="0" marginwidth="0" topmargin="0" marginheight="0" offset="0"><center><table align="center" border="0" cellpadding="0" cellspacing="0" height="100%" width="100%" id="bodyTable"><tr><td align="center" valign="top" id="bodyCell"><!-- BEGIN TEMPLATE // --><table border="0" cellpadding="0" cellspacing="0" id="templateContainer"> 	<tr><td align="center" valign="top"><!-- BEGIN BODY // -->	<table border="0" cellpadding="0" cellspacing="0" width="100%" id="templateBody"><tr>	<td valign="top" class="bodyContent"><h2 >Subscription ****SubscriptionName****</h2><br><p>The Scheduled Task ****SchedulerTaskName**** was fired<b> but Failed to Submit Data</b></p><br>Please try again or contact technical support to troubleshoot the issue.<p>The following section is the deatil results.</p><hr/><table border="0" cellpadding="0" cellspacing="0" width="100%" id="templateBody">****ResponseJson****</table></td></tr></table></td>	</tr></table><!-- // END BODY --></td></tr></table><!-- // END TEMPLATE --></center></body> </html>',N'Scheduled SaaS Metered Usage Failure!',N'True')
                        END

                        IF NOT EXISTS (SELECT * FROM [dbo].[EmailTemplate] WHERE [Status] = 'Missing')
                        BEGIN
                            INSERT [dbo].[EmailTemplate] ([Status],[Description],[InsertDate],[TemplateBody],[Subject],[isActive]) VALUES (N'Missing',N'Missing',GetDate(),N'<html><head><meta http-equiv="Content-Type" content="text/html; charset=UTF-8"/></head><body leftmargin="0" marginwidth="0" topmargin="0" marginheight="0" offset="0"><center><table align="center" border="0" cellpadding="0" cellspacing="0" height="100%" width="100%" id="bodyTable"><tr><td align="center" valign="top" id="bodyCell"><!-- BEGIN TEMPLATE // --><table border="0" cellpadding="0" cellspacing="0" id="templateContainer"> 	<tr><td align="center" valign="top"><!-- BEGIN BODY // -->	<table border="0" cellpadding="0" cellspacing="0" width="100%" id="templateBody"><tr>	<td valign="top" class="bodyContent"><h2 >Subscription ****SubscriptionName****</h2><br><p>The Scheduled Task ****SchedulerTaskName**** was skipped by scheduler engine!</b></p><br>Please try again or contact technical support to troubleshoot the issue.<p>The following section is the deatil results.</p><hr/><table border="0" cellpadding="0" cellspacing="0" width="100%" id="templateBody">****ResponseJson****</table></td></tr></table></td>	</tr></table><!-- // END BODY --></td></tr></table><!-- // END TEMPLATE --></center></body> </html>',N'Scheduled SaaS Metered Task was Skipped!',N'True')
                        END

END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20230726232155_Baseline_v7'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20230726232155_Baseline_v7', N'8.0.6');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20230912052848_SubscriptionDetails_v740'
)
BEGIN
    ALTER TABLE [Subscriptions] ADD [AmpOfferId] nvarchar(max) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20230912052848_SubscriptionDetails_v740'
)
BEGIN
    ALTER TABLE [Subscriptions] ADD [EndDate] datetime2 NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20230912052848_SubscriptionDetails_v740'
)
BEGIN
    ALTER TABLE [Subscriptions] ADD [StartDate] datetime2 NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20230912052848_SubscriptionDetails_v740'
)
BEGIN
    ALTER TABLE [Subscriptions] ADD [Term] nvarchar(max) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20230912052848_SubscriptionDetails_v740'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20230912052848_SubscriptionDetails_v740', N'8.0.6');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20231115232155_Baseline_v741'
)
BEGIN

                        IF NOT EXISTS (SELECT * FROM [dbo].[ApplicationConfiguration] WHERE [Name] = 'IsMeteredBillingEnabled')
                        BEGIN
                            INSERT [dbo].[ApplicationConfiguration] ( [Name], [Value], [Description]) VALUES ( N'IsMeteredBillingEnabled', N'true', N'Enable Metered Billing Feature')
                        END
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20231115232155_Baseline_v741'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20231115232155_Baseline_v741', N'8.0.6');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240312055030_baseline751'
)
BEGIN

                        IF NOT EXISTS (SELECT * FROM [dbo].[ApplicationConfiguration] WHERE [Name] = 'ValidateWebhookJwtToken')
                        BEGIN
                            INSERT [dbo].[ApplicationConfiguration] ( [Name], [Value], [Description]) VALUES ( N'ValidateWebhookJwtToken', N'true', N'Validates JWT token when webhook event is recieved.')
                        END
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240312055030_baseline751'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20240312055030_baseline751', N'8.0.6');
END;
GO

COMMIT;
GO

