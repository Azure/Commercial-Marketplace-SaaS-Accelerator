using System;
using Marketplace.SaaS.Accelerator.DataAccess.Migrations.Custom;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Marketplace.SaaS.Accelerator.DataAccess.Migrations
{
    public partial class Baseline_v2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ApplicationConfiguration",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationConfiguration", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "ApplicationLog",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ActionTime = table.Column<DateTime>(type: "datetime", nullable: true),
                    LogDetail = table.Column<string>(type: "varchar(4000)", unicode: false, maxLength: 4000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationLog", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DatabaseVersionHistory",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VersionNumber = table.Column<decimal>(type: "decimal(6,2)", nullable: false),
                    ChangeLog = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    CreateBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                });

            migrationBuilder.CreateTable(
                name: "EmailTemplate",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Status = table.Column<string>(type: "varchar(1000)", unicode: false, maxLength: 1000, nullable: true),
                    Description = table.Column<string>(type: "varchar(1000)", unicode: false, maxLength: 1000, nullable: true),
                    InsertDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    TemplateBody = table.Column<string>(type: "varchar(max)", unicode: false, nullable: true),
                    Subject = table.Column<string>(type: "varchar(1000)", unicode: false, maxLength: 1000, nullable: true),
                    ToRecipients = table.Column<string>(type: "varchar(1000)", unicode: false, maxLength: 1000, nullable: true),
                    CC = table.Column<string>(type: "varchar(1000)", unicode: false, maxLength: 1000, nullable: true),
                    BCC = table.Column<string>(type: "varchar(1000)", unicode: false, maxLength: 1000, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmailTemplate", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Events",
                columns: table => new
                {
                    EventsId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EventsName = table.Column<string>(type: "varchar(225)", unicode: false, maxLength: 225, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: true),
                    CreateDate = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Events", x => x.EventsId);
                });

            migrationBuilder.CreateTable(
                name: "OfferAttributes",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ParameterId = table.Column<string>(type: "varchar(225)", unicode: false, maxLength: 225, nullable: true),
                    DisplayName = table.Column<string>(type: "varchar(225)", unicode: false, maxLength: 225, nullable: true),
                    Description = table.Column<string>(type: "varchar(225)", unicode: false, maxLength: 225, nullable: true),
                    ValueTypeId = table.Column<int>(type: "int", nullable: true),
                    FromList = table.Column<bool>(type: "bit", nullable: false),
                    ValuesList = table.Column<string>(type: "varchar(max)", unicode: false, nullable: true),
                    Max = table.Column<int>(type: "int", nullable: true),
                    Min = table.Column<int>(type: "int", nullable: true),
                    Type = table.Column<string>(type: "varchar(225)", unicode: false, maxLength: 225, nullable: true),
                    DisplaySequence = table.Column<int>(type: "int", nullable: true),
                    Isactive = table.Column<bool>(type: "bit", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    UserId = table.Column<int>(type: "int", nullable: true),
                    OfferId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsDelete = table.Column<bool>(type: "bit", nullable: true),
                    IsRequired = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OfferAttributes", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Offers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OfferId = table.Column<string>(type: "varchar(225)", unicode: false, maxLength: 225, nullable: true),
                    OfferName = table.Column<string>(type: "varchar(225)", unicode: false, maxLength: 225, nullable: true),
                    CreateDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    UserId = table.Column<int>(type: "int", nullable: true),
                    OfferGUId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Offers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PlanAttributeMapping",
                columns: table => new
                {
                    PlanAttributeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PlanId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OfferAttributeID = table.Column<int>(type: "int", nullable: false),
                    IsEnabled = table.Column<bool>(type: "bit", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    UserId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__PlanAttr__8B476A98C058FAF2", x => x.PlanAttributeId);
                });

            migrationBuilder.CreateTable(
                name: "PlanAttributeOutput",
                columns: table => new
                {
                    RowNumber = table.Column<int>(type: "int", nullable: false),
                    PlanAttributeId = table.Column<int>(type: "int", nullable: false),
                    PlanId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OfferAttributeId = table.Column<int>(type: "int", nullable: false),
                    DisplayName = table.Column<string>(type: "varchar(225)", unicode: false, maxLength: 225, nullable: false),
                    IsEnabled = table.Column<bool>(type: "bit", nullable: false),
                    Type = table.Column<string>(type: "varchar(225)", unicode: false, maxLength: 225, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__PlanAttr__AAAC09D888FE3E40", x => x.RowNumber);
                });

            migrationBuilder.CreateTable(
                name: "PlanEventsMapping",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PlanId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EventId = table.Column<int>(type: "int", nullable: false),
                    Isactive = table.Column<bool>(type: "bit", nullable: false),
                    SuccessStateEmails = table.Column<string>(type: "varchar(225)", unicode: false, maxLength: 225, nullable: true),
                    FailureStateEmails = table.Column<string>(type: "varchar(225)", unicode: false, maxLength: 225, nullable: true),
                    CreateDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    UserId = table.Column<int>(type: "int", nullable: true),
                    CopyToCustomer = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlanEventsMapping", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PlanEventsOutPut",
                columns: table => new
                {
                    RowNumber = table.Column<int>(type: "int", nullable: false),
                    ID = table.Column<int>(type: "int", nullable: false),
                    PlanId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Isactive = table.Column<bool>(type: "bit", nullable: false),
                    SuccessStateEmails = table.Column<string>(type: "varchar(max)", unicode: false, nullable: true),
                    FailureStateEmails = table.Column<string>(type: "varchar(max)", unicode: false, nullable: true),
                    EventId = table.Column<int>(type: "int", nullable: false),
                    EventsName = table.Column<string>(type: "varchar(225)", unicode: false, maxLength: 225, nullable: false),
                    CopyToCustomer = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__PlanEven__AAAC09D8C9229532", x => x.RowNumber);
                });

            migrationBuilder.CreateTable(
                name: "Plans",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PlanId = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: true),
                    Description = table.Column<string>(type: "varchar(500)", unicode: false, maxLength: 500, nullable: true),
                    DisplayName = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: true),
                    IsmeteringSupported = table.Column<bool>(type: "bit", nullable: true),
                    IsPerUser = table.Column<bool>(type: "bit", nullable: true),
                    PlanGUID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OfferID = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Plans", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SubscriptionAttributeValues",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PlanAttributeId = table.Column<int>(type: "int", nullable: false),
                    Value = table.Column<string>(type: "varchar(225)", unicode: false, maxLength: 225, nullable: true),
                    SubscriptionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    UserId = table.Column<int>(type: "int", nullable: true),
                    PlanID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OfferID = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubscriptionAttributeValues", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "SubscriptionEmailOutput",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "varchar(225)", unicode: false, maxLength: 225, nullable: true),
                    Value = table.Column<string>(type: "varchar(max)", unicode: false, nullable: true)
                },
                constraints: table =>
                {
                });

            migrationBuilder.CreateTable(
                name: "SubscriptionParametersOutput",
                columns: table => new
                {
                    RowNumber = table.Column<int>(type: "int", nullable: false),
                    Id = table.Column<int>(type: "int", nullable: false),
                    PlanAttributeId = table.Column<int>(type: "int", nullable: false),
                    OfferAttributeID = table.Column<int>(type: "int", nullable: false),
                    DisplayName = table.Column<string>(type: "varchar(225)", unicode: false, maxLength: 225, nullable: false),
                    Type = table.Column<string>(type: "varchar(225)", unicode: false, maxLength: 225, nullable: false),
                    ValueType = table.Column<string>(type: "varchar(225)", unicode: false, maxLength: 225, nullable: false),
                    DisplaySequence = table.Column<int>(type: "int", nullable: false),
                    IsEnabled = table.Column<bool>(type: "bit", nullable: false),
                    IsRequired = table.Column<bool>(type: "bit", nullable: true),
                    Value = table.Column<string>(type: "varchar(max)", unicode: false, nullable: false),
                    SubscriptionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OfferId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PlanId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: true),
                    CreateDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    FromList = table.Column<bool>(type: "bit", nullable: false),
                    ValuesList = table.Column<string>(type: "varchar(225)", unicode: false, maxLength: 225, nullable: false),
                    Max = table.Column<int>(type: "int", nullable: false),
                    Min = table.Column<int>(type: "int", nullable: false),
                    HTMLType = table.Column<string>(type: "varchar(225)", unicode: false, maxLength: 225, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Subscrip__AAAC09D8BA727059", x => x.RowNumber);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmailAddress = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    FullName = table.Column<string>(type: "varchar(200)", unicode: false, maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.UserId);
                });

            migrationBuilder.CreateTable(
                name: "ValueTypes",
                columns: table => new
                {
                    ValueTypeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ValueType = table.Column<string>(type: "varchar(225)", unicode: false, maxLength: 225, nullable: true),
                    CreateDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    HTMLType = table.Column<string>(type: "varchar(225)", unicode: false, maxLength: 225, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__ValueTyp__A51E9C5AEA096123", x => x.ValueTypeId);
                });

            migrationBuilder.CreateTable(
                name: "WebJobSubscriptionStatus",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SubscriptionId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    SubscriptionStatus = table.Column<string>(type: "varchar(225)", unicode: false, maxLength: 225, nullable: true),
                    Description = table.Column<string>(type: "varchar(max)", unicode: false, nullable: true),
                    InsertDate = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WebJobSubscriptionStatus", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "MeteredDimensions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Dimension = table.Column<string>(type: "varchar(150)", unicode: false, maxLength: 150, nullable: true),
                    PlanId = table.Column<int>(type: "int", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    Description = table.Column<string>(type: "varchar(250)", unicode: false, maxLength: 250, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MeteredDimensions", x => x.Id);
                    table.ForeignKey(
                        name: "FK__MeteredDi__PlanI__6383C8BA",
                        column: x => x.PlanId,
                        principalTable: "Plans",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "KnownUsers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserEmail = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    RoleId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KnownUsers", x => x.Id);
                    table.ForeignKey(
                        name: "FK__KnownUser__RoleI__619B8048",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Subscriptions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AMPSubscriptionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    SubscriptionStatus = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    AMPPlanId = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: true),
                    CreateBy = table.Column<int>(type: "int", nullable: true),
                    CreateDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    ModifyDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    UserId = table.Column<int>(type: "int", nullable: true),
                    Name = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: true),
                    AMPQuantity = table.Column<int>(type: "int", nullable: false),
                    PurchaserEmail = table.Column<string>(type: "varchar(225)", unicode: false, maxLength: 225, nullable: true),
                    PurchaserTenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Subscriptions", x => x.Id);
                    table.ForeignKey(
                        name: "FK__Subscript__UserI__656C112C",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId");
                });

            migrationBuilder.CreateTable(
                name: "MeteredAuditLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SubscriptionId = table.Column<int>(type: "int", nullable: true),
                    RequestJson = table.Column<string>(type: "varchar(500)", unicode: false, maxLength: 500, nullable: true),
                    ResponseJson = table.Column<string>(type: "varchar(500)", unicode: false, maxLength: 500, nullable: true),
                    StatusCode = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    SubscriptionUsageDate = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MeteredAuditLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK__MeteredAu__Subsc__628FA481",
                        column: x => x.SubscriptionId,
                        principalTable: "Subscriptions",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "SubscriptionAuditLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SubscriptionID = table.Column<int>(type: "int", nullable: true),
                    Attribute = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: true),
                    OldValue = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    NewValue = table.Column<string>(type: "varchar(max)", unicode: false, nullable: true),
                    CreateDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    CreateBy = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubscriptionAuditLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK__Subscript__Subsc__6477ECF3",
                        column: x => x.SubscriptionID,
                        principalTable: "Subscriptions",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_KnownUsers_RoleId",
                table: "KnownUsers",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_MeteredAuditLogs_SubscriptionId",
                table: "MeteredAuditLogs",
                column: "SubscriptionId");

            migrationBuilder.CreateIndex(
                name: "IX_MeteredDimensions_PlanId",
                table: "MeteredDimensions",
                column: "PlanId");

            migrationBuilder.CreateIndex(
                name: "IX_SubscriptionAuditLogs_SubscriptionID",
                table: "SubscriptionAuditLogs",
                column: "SubscriptionID");

            migrationBuilder.CreateIndex(
                name: "IX_Subscriptions_UserId",
                table: "Subscriptions",
                column: "UserId");


            migrationBuilder.BaselineV2_SeedStoredProcedures();
            migrationBuilder.BaselineV2_SeedData();
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApplicationConfiguration");

            migrationBuilder.DropTable(
                name: "ApplicationLog");

            migrationBuilder.DropTable(
                name: "DatabaseVersionHistory");

            migrationBuilder.DropTable(
                name: "EmailTemplate");

            migrationBuilder.DropTable(
                name: "Events");

            migrationBuilder.DropTable(
                name: "KnownUsers");

            migrationBuilder.DropTable(
                name: "MeteredAuditLogs");

            migrationBuilder.DropTable(
                name: "MeteredDimensions");

            migrationBuilder.DropTable(
                name: "OfferAttributes");

            migrationBuilder.DropTable(
                name: "Offers");

            migrationBuilder.DropTable(
                name: "PlanAttributeMapping");

            migrationBuilder.DropTable(
                name: "PlanAttributeOutput");

            migrationBuilder.DropTable(
                name: "PlanEventsMapping");

            migrationBuilder.DropTable(
                name: "PlanEventsOutPut");

            migrationBuilder.DropTable(
                name: "SubscriptionAttributeValues");

            migrationBuilder.DropTable(
                name: "SubscriptionAuditLogs");

            migrationBuilder.DropTable(
                name: "SubscriptionEmailOutput");

            migrationBuilder.DropTable(
                name: "SubscriptionParametersOutput");

            migrationBuilder.DropTable(
                name: "ValueTypes");

            migrationBuilder.DropTable(
                name: "WebJobSubscriptionStatus");

            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.DropTable(
                name: "Plans");

            migrationBuilder.DropTable(
                name: "Subscriptions");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.BaselineV2_DeSeedAll();
        }
    }
}
