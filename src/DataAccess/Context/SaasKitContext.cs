using Marketplace.SaaS.Accelerator.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace Marketplace.SaaS.Accelerator.DataAccess.Context;

public partial class SaasKitContext : DbContext
{
    public SaasKitContext()
    {
    }

    public SaasKitContext(DbContextOptions<SaasKitContext> options)
        : base(options)
    {
    }

    public virtual DbSet<ApplicationConfiguration> ApplicationConfiguration { get; set; }
    public virtual DbSet<ApplicationLog> ApplicationLog { get; set; }
    public virtual DbSet<DatabaseVersionHistory> DatabaseVersionHistory { get; set; }
    public virtual DbSet<EmailTemplate> EmailTemplate { get; set; }
    public virtual DbSet<Events> Events { get; set; }
    public virtual DbSet<KnownUsers> KnownUsers { get; set; }
    public virtual DbSet<MeteredAuditLogs> MeteredAuditLogs { get; set; }
    public virtual DbSet<MeteredDimensions> MeteredDimensions { get; set; }
    public virtual DbSet<OfferAttributes> OfferAttributes { get; set; }
    public virtual DbSet<Offers> Offers { get; set; }
    public virtual DbSet<PlanAttributeMapping> PlanAttributeMapping { get; set; }
    public virtual DbSet<PlanAttributeOutput> PlanAttributeOutput { get; set; }
    public virtual DbSet<PlanEventsMapping> PlanEventsMapping { get; set; }
    public virtual DbSet<PlanEventsOutPut> PlanEventsOutPut { get; set; }
    public virtual DbSet<Plans> Plans { get; set; }
    public virtual DbSet<Roles> Roles { get; set; }
    public virtual DbSet<SubscriptionAttributeValues> SubscriptionAttributeValues { get; set; }
    public virtual DbSet<SubscriptionAuditLogs> SubscriptionAuditLogs { get; set; }
    public virtual DbSet<SubscriptionEmailOutput> SubscriptionEmailOutput { get; set; }
    public virtual DbSet<SubscriptionParametersOutput> SubscriptionParametersOutput { get; set; }
    public virtual DbSet<Subscriptions> Subscriptions { get; set; }
    public virtual DbSet<Users> Users { get; set; }
    public virtual DbSet<ValueTypes> ValueTypes { get; set; }
    public virtual DbSet<WebJobSubscriptionStatus> WebJobSubscriptionStatus { get; set; }

    public virtual DbSet<SchedulerFrequency> SchedulerFrequency { get; set; }
    public virtual DbSet<MeteredPlanSchedulerManagement> MeteredPlanSchedulerManagement { get; set; }
    public virtual DbSet<SchedulerManagerView> SchedulerManagerView { get; set; }


    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {

        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ApplicationConfiguration>(entity =>
        {
            entity.Property(e => e.Id).HasColumnName("ID");

            entity.Property(e => e.Description).HasMaxLength(255);

            entity.Property(e => e.Name).HasMaxLength(50);
        });

        modelBuilder.Entity<ApplicationLog>(entity =>
        {
            entity.Property(e => e.ActionTime).HasColumnType("datetime");

            entity.Property(e => e.LogDetail)
                .HasMaxLength(4000)
                .IsUnicode(false);
        });

        modelBuilder.Entity<DatabaseVersionHistory>(entity =>
        {
            entity.HasNoKey();

            entity.Property(e => e.ChangeLog).IsRequired();

            entity.Property(e => e.CreateBy).HasMaxLength(100);

            entity.Property(e => e.CreateDate).HasColumnType("datetime");

            entity.Property(e => e.Id)
                .HasColumnName("ID")
                .ValueGeneratedOnAdd();

            entity.Property(e => e.VersionNumber).HasColumnType("decimal(6, 2)");
        });

        modelBuilder.Entity<EmailTemplate>(entity =>
        {
            entity.Property(e => e.Id).HasColumnName("ID");

            entity.Property(e => e.Bcc)
                .HasColumnName("BCC")
                .HasMaxLength(1000)
                .IsUnicode(false);

            entity.Property(e => e.Cc)
                .HasColumnName("CC")
                .HasMaxLength(1000)
                .IsUnicode(false);

            entity.Property(e => e.Description)
                .HasMaxLength(1000)
                .IsUnicode(false);

            entity.Property(e => e.InsertDate).HasColumnType("datetime");

            entity.Property(e => e.Status)
                .HasMaxLength(1000)
                .IsUnicode(false);

            entity.Property(e => e.Subject)
                .HasMaxLength(1000)
                .IsUnicode(false);

            entity.Property(e => e.TemplateBody).IsUnicode(false);

            entity.Property(e => e.ToRecipients)
                .HasMaxLength(1000)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Events>(entity =>
        {
            entity.Property(e => e.CreateDate).HasColumnType("datetime");

            entity.Property(e => e.EventsName)
                .HasMaxLength(225)
                .IsUnicode(false);
        });

        modelBuilder.Entity<KnownUsers>(entity =>
        {
            entity.Property(e => e.UserEmail)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.Role)
                .WithMany(p => p.KnownUsers)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__KnownUser__RoleI__619B8048");

            entity.Property(i => i.Id)
                .ValueGeneratedOnAdd();
        });

        modelBuilder.Entity<MeteredAuditLogs>(entity =>
        {
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");

            entity.Property(e => e.RequestJson)
                .HasMaxLength(500)
                .IsUnicode(false);

            entity.Property(e => e.ResponseJson)
                .HasMaxLength(500)
                .IsUnicode(false);

            entity.Property(e => e.RunBy)
                .HasMaxLength(255)
                .IsUnicode(false);

            entity.Property(e => e.StatusCode)
                .HasMaxLength(100)
                .IsUnicode(false);

            entity.Property(e => e.SubscriptionUsageDate).HasColumnType("datetime");

            entity.HasOne(d => d.Subscription)
                .WithMany(p => p.MeteredAuditLogs)
                .HasForeignKey(d => d.SubscriptionId)
                .HasConstraintName("FK__MeteredAu__Subsc__628FA481");
        });

        modelBuilder.Entity<MeteredDimensions>(entity =>
        {
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");

            entity.Property(e => e.Description)
                .HasMaxLength(250)
                .IsUnicode(false);

            entity.Property(e => e.Dimension)
                .HasMaxLength(150)
                .IsUnicode(false);

            entity.HasOne(d => d.Plan)
                .WithMany(p => p.MeteredDimensions)
                .HasForeignKey(d => d.PlanId)
                .HasConstraintName("FK__MeteredDi__PlanI__6383C8BA");

            entity.HasMany(e => e.MeteredPlanSchedulerManagements)
                .WithOne(e => e.MeteredDimensions)
                .HasForeignKey(e => e.DimensionId);
        });

        modelBuilder.Entity<OfferAttributes>(entity =>
        {
            entity.Property(e => e.Id).HasColumnName("ID");

            entity.Property(e => e.CreateDate).HasColumnType("datetime");

            entity.Property(e => e.Description)
                .HasMaxLength(225)
                .IsUnicode(false);

            entity.Property(e => e.DisplayName)
                .HasMaxLength(225)
                .IsUnicode(false);

            entity.Property(e => e.ParameterId)
                .HasMaxLength(225)
                .IsUnicode(false);

            entity.Property(e => e.Type)
                .HasMaxLength(225)
                .IsUnicode(false);

            entity.Property(e => e.ValuesList).IsUnicode(false);
        });

        modelBuilder.Entity<Offers>(entity =>
        {
            entity.Property(e => e.CreateDate).HasColumnType("datetime");

            entity.Property(e => e.OfferGuid).HasColumnName("OfferGUId");

            entity.Property(e => e.OfferId)
                .HasMaxLength(225)
                .IsUnicode(false);

            entity.Property(e => e.OfferName)
                .HasMaxLength(225)
                .IsUnicode(false);
        });

        modelBuilder.Entity<PlanAttributeMapping>(entity =>
        {
            entity.HasKey(e => e.PlanAttributeId)
                .HasName("PK__PlanAttr__8B476A98C058FAF2");

            entity.Property(e => e.CreateDate).HasColumnType("datetime");

            entity.Property(e => e.OfferAttributeId).HasColumnName("OfferAttributeID");
        });

        modelBuilder.Entity<PlanAttributeOutput>(entity =>
        {
            entity.HasKey(e => e.RowNumber)
                .HasName("PK__PlanAttr__AAAC09D888FE3E40");

            entity.Property(e => e.RowNumber).ValueGeneratedNever();

            entity.Property(e => e.DisplayName)
                .IsRequired()
                .HasMaxLength(225)
                .IsUnicode(false);

            entity.Property(e => e.Type)
                .HasMaxLength(225)
                .IsUnicode(false);
        });

        modelBuilder.Entity<PlanEventsMapping>(entity =>
        {
            entity.Property(e => e.CreateDate).HasColumnType("datetime");

            entity.Property(e => e.FailureStateEmails)
                .HasMaxLength(225)
                .IsUnicode(false);

            entity.Property(e => e.SuccessStateEmails)
                .HasMaxLength(225)
                .IsUnicode(false);
        });

        modelBuilder.Entity<PlanEventsOutPut>(entity =>
        {
            entity.HasKey(e => e.RowNumber)
                .HasName("PK__PlanEven__AAAC09D8C9229532");

            entity.Property(e => e.RowNumber).ValueGeneratedNever();

            entity.Property(e => e.EventsName)
                .IsRequired()
                .HasMaxLength(225)
                .IsUnicode(false);

            entity.Property(e => e.FailureStateEmails).IsUnicode(false);

            entity.Property(e => e.Id).HasColumnName("ID");

            entity.Property(e => e.SuccessStateEmails).IsUnicode(false);
        });

        modelBuilder.Entity<Plans>(entity =>
        {
            entity.Property(e => e.Description)
                .HasMaxLength(500)
                .IsUnicode(false);

            entity.Property(e => e.DisplayName)
                .HasMaxLength(100)
                .IsUnicode(false);

            entity.Property(e => e.OfferId).HasColumnName("OfferID");

            entity.Property(e => e.PlanGuid).HasColumnName("PlanGUID");

            entity.Property(e => e.PlanId)
                .HasMaxLength(100)
                .IsUnicode(false);

            entity.HasMany(e => e.MeteredPlanSchedulerManagements)
                .WithOne(e => e.Plan)
                .HasForeignKey(e => e.PlanId);
        });

        modelBuilder.Entity<Roles>(entity =>
        {
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<SubscriptionAttributeValues>(entity =>
        {
            entity.Property(e => e.Id).HasColumnName("ID");

            entity.Property(e => e.CreateDate).HasColumnType("datetime");

            entity.Property(e => e.OfferId).HasColumnName("OfferID");

            entity.Property(e => e.PlanId).HasColumnName("PlanID");

            entity.Property(e => e.Value)
                .HasMaxLength(225)
                .IsUnicode(false);
        });

        modelBuilder.Entity<SubscriptionAuditLogs>(entity =>
        {
            entity.Property(e => e.Attribute)
                .HasMaxLength(20)
                .IsUnicode(false);

            entity.Property(e => e.CreateDate).HasColumnType("datetime");

            entity.Property(e => e.NewValue).IsUnicode(false);

            entity.Property(e => e.OldValue)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.Property(e => e.SubscriptionId).HasColumnName("SubscriptionID");

            entity.HasOne(d => d.Subscription)
                .WithMany(p => p.SubscriptionAuditLogs)
                .HasForeignKey(d => d.SubscriptionId)
                .HasConstraintName("FK__Subscript__Subsc__6477ECF3");
        });

        modelBuilder.Entity<SubscriptionEmailOutput>(entity =>
        {
            entity.HasNoKey();

            entity.Property(e => e.Id).ValueGeneratedOnAdd();

            entity.Property(e => e.Name)
                .HasMaxLength(225)
                .IsUnicode(false);

            entity.Property(e => e.Value).IsUnicode(false);
        });

        modelBuilder.Entity<SubscriptionParametersOutput>(entity =>
        {
            entity.HasKey(e => e.RowNumber)
                .HasName("PK__Subscrip__AAAC09D8BA727059");

            entity.Property(e => e.RowNumber).ValueGeneratedNever();

            entity.Property(e => e.CreateDate).HasColumnType("datetime");

            entity.Property(e => e.DisplayName)
                .IsRequired()
                .HasMaxLength(225)
                .IsUnicode(false);

            entity.Property(e => e.Htmltype)
                .IsRequired()
                .HasColumnName("HTMLType")
                .HasMaxLength(225)
                .IsUnicode(false);

            entity.Property(e => e.OfferAttributeId).HasColumnName("OfferAttributeID");

            entity.Property(e => e.Type)
                .IsRequired()
                .HasMaxLength(225)
                .IsUnicode(false);

            entity.Property(e => e.Value)
                .IsRequired()
                .IsUnicode(false);

            entity.Property(e => e.ValueType)
                .IsRequired()
                .HasMaxLength(225)
                .IsUnicode(false);

            entity.Property(e => e.ValuesList)
                .IsRequired()
                .HasMaxLength(225)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Subscriptions>(entity =>
        {
            entity.Property(e => e.AmpplanId)
                .HasColumnName("AMPPlanId")
                .HasMaxLength(100)
                .IsUnicode(false);

            entity.Property(e => e.Ampquantity).HasColumnName("AMPQuantity");

            entity.Property(e => e.AmpsubscriptionId)
                .HasColumnName("AMPSubscriptionId")
                .HasDefaultValueSql("(newid())");

            entity.Property(e => e.CreateDate).HasColumnType("datetime");

            entity.Property(e => e.ModifyDate).HasColumnType("datetime");

            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .IsUnicode(false);

            entity.Property(e => e.PurchaserEmail)
                .HasMaxLength(225)
                .IsUnicode(false);

            entity.Property(e => e.SubscriptionStatus)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.User)
                .WithMany(p => p.Subscriptions)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__Subscript__UserI__656C112C");

            entity.HasMany(e => e.MeteredPlanSchedulerManagements)
                .WithOne(e => e.Subscriptions)
                .HasForeignKey(e => e.SubscriptionId);
        });

        modelBuilder.Entity<Users>(entity =>
        {
            entity.HasKey(e => e.UserId);

            entity.Property(e => e.CreatedDate).HasColumnType("datetime");

            entity.Property(e => e.EmailAddress)
                .HasMaxLength(100)
                .IsUnicode(false);

            entity.Property(e => e.FullName)
                .HasMaxLength(200)
                .IsUnicode(false);
        });

        modelBuilder.Entity<ValueTypes>(entity =>
        {
            entity.HasKey(e => e.ValueTypeId)
                .HasName("PK__ValueTyp__A51E9C5AEA096123");

            entity.Property(e => e.CreateDate).HasColumnType("datetime");

            entity.Property(e => e.Htmltype)
                .HasColumnName("HTMLType")
                .HasMaxLength(225)
                .IsUnicode(false);

            entity.Property(e => e.ValueType)
                .HasMaxLength(225)
                .IsUnicode(false);
        });

        modelBuilder.Entity<WebJobSubscriptionStatus>(entity =>
        {
            entity.Property(e => e.Id).HasColumnName("ID");

            entity.Property(e => e.Description).IsUnicode(false);

            entity.Property(e => e.InsertDate).HasColumnType("datetime");

            entity.Property(e => e.SubscriptionStatus)
                .HasMaxLength(225)
                .IsUnicode(false);
        });

        modelBuilder.Entity<MeteredPlanSchedulerManagement>(entity =>
        {
            entity.Property(e => e.SchedulerName)
                .HasMaxLength(50)
                .IsRequired();

            entity.Property(e => e.SubscriptionId)
                .IsRequired();

            entity.Property(e => e.PlanId)
                .IsRequired();

            entity.Property(e => e.DimensionId)
                .IsRequired();

            entity.Property(e => e.Quantity)
                .IsRequired();

            entity.Property(e => e.FrequencyId)
                .IsRequired();

            entity.Property(e => e.StartDate)
                .IsRequired();
        });

        modelBuilder.Entity<SchedulerFrequency>(entity =>
        {
            entity.Property(e => e.Frequency)
                .IsUnicode(false)
                .IsRequired()
                .HasMaxLength(50);

            entity.HasMany(e => e.MeteredPlanSchedulerManagements)
                .WithOne(e => e.SchedulerFrequency)
                .HasForeignKey(e => e.FrequencyId);
        });

        modelBuilder.Entity<SchedulerManagerView>(entity =>
        {
            entity.HasNoKey();
            entity.ToView("SchedulerManagerView");
            entity.Property(e => e.PlanId).IsUnicode(false);
            entity.Property(e => e.Dimension).IsUnicode(false);
            entity.Property(e => e.Frequency).IsUnicode(false);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}