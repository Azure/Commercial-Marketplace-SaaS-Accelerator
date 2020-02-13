using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Marketplace.SaasKit.Client.DataAccess.Entities;

namespace Microsoft.Marketplace.SaasKit.Client.DataAccess.Context
{
    public partial class SaasKitContext : DbContext
    {
        public SaasKitContext()
        {
        }

        public SaasKitContext(DbContextOptions<SaasKitContext> options)
            : base(options)
        {
        }

        public virtual DbSet<ApplicationLog> ApplicationLog { get; set; }
        public virtual DbSet<MeteredAuditLogs> MeteredAuditLogs { get; set; }
        public virtual DbSet<MeteredDimensions> MeteredDimensions { get; set; }
        public virtual DbSet<Plans> Plans { get; set; }
        public virtual DbSet<SubscriptionAuditLogs> SubscriptionAuditLogs { get; set; }
        public virtual DbSet<SubscriptionLicenses> SubscriptionLicenses { get; set; }
        public virtual DbSet<Subscriptions> Subscriptions { get; set; }
        public virtual DbSet<Users> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ApplicationLog>(entity =>
            {
                entity.Property(e => e.ActionTime).HasColumnType("datetime");

                entity.Property(e => e.LogDetail)
                    .HasMaxLength(4000)
                    .IsUnicode(false);
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

                entity.Property(e => e.StatusCode)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.SubscriptionUsageDate).HasColumnType("datetime");

                entity.HasOne(d => d.Subscription)
                    .WithMany(p => p.MeteredAuditLogs)
                    .HasForeignKey(d => d.SubscriptionId)
                    .HasConstraintName("FK__MeteredAu__Subsc__02FC7413");
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
                    .HasConstraintName("FK__MeteredDi__PlanI__05D8E0BE");
            });

            modelBuilder.Entity<Plans>(entity =>
            {
                entity.Property(e => e.Description)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.DisplayName)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.PlanId)
                    .HasMaxLength(100)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<SubscriptionAuditLogs>(entity =>
            {
                entity.Property(e => e.Attribute)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.CreateDate).HasColumnType("datetime");

                entity.Property(e => e.NewValue)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.OldValue)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.SubscriptionId).HasColumnName("SubscriptionID");

                entity.HasOne(d => d.Subscription)
                    .WithMany(p => p.SubscriptionAuditLogs)
                    .HasForeignKey(d => d.SubscriptionId)
                    .HasConstraintName("FK__Subscript__Subsc__6383C8BA");
            });

            modelBuilder.Entity<SubscriptionLicenses>(entity =>
            {
                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.LicenseKey)
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.SubscriptionId).HasColumnName("SubscriptionID");

                entity.HasOne(d => d.Subscription)
                    .WithMany(p => p.SubscriptionLicenses)
                    .HasForeignKey(d => d.SubscriptionId)
                    .HasConstraintName("FK__Subscript__Subsc__0A9D95DB");
            });

            modelBuilder.Entity<Subscriptions>(entity =>
            {
                entity.HasComment("Subscription Status as \"Subscribed\",\"Unsubscribed\", \"PendingFulfillmentStart\"");

                entity.Property(e => e.AmpplanId)
                    .HasColumnName("AMPPlanId")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.AmpsubscriptionId)
                    .HasColumnName("AMPSubscriptionId")
                    .HasDefaultValueSql("(newid())");

                entity.Property(e => e.CreateDate).HasColumnType("datetime");

                entity.Property(e => e.ModifyDate).HasColumnType("datetime");

                entity.Property(e => e.Name)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.SubscriptionStatus)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Subscriptions)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK__Subscript__UserI__656C112C");
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

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
