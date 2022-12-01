using System;
using Marketplace.SaaS.Accelerator.DataAccess.Migrations.Custom;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Marketplace.SaaS.Accelerator.DataAccess.Migrations
{
    public partial class Baseline_v5 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SchedulerFrequency",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Frequency = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SchedulerFrequency", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MeteredPlanSchedulerManagement",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SchedulerName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    SubscriptionId = table.Column<int>(type: "int", nullable: false),
                    PlanId = table.Column<int>(type: "int", nullable: false),
                    DimensionId = table.Column<int>(type: "int", nullable: false),
                    FrequencyId = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<double>(type: "float", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NextRunTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MeteredPlanSchedulerManagement", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MeteredPlanSchedulerManagement_MeteredDimensions_DimensionId",
                        column: x => x.DimensionId,
                        principalTable: "MeteredDimensions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MeteredPlanSchedulerManagement_Plans_PlanId",
                        column: x => x.PlanId,
                        principalTable: "Plans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MeteredPlanSchedulerManagement_SchedulerFrequency_FrequencyId",
                        column: x => x.FrequencyId,
                        principalTable: "SchedulerFrequency",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MeteredPlanSchedulerManagement_Subscriptions_SubscriptionId",
                        column: x => x.SubscriptionId,
                        principalTable: "Subscriptions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MeteredPlanSchedulerManagement_DimensionId",
                table: "MeteredPlanSchedulerManagement",
                column: "DimensionId");

            migrationBuilder.CreateIndex(
                name: "IX_MeteredPlanSchedulerManagement_FrequencyId",
                table: "MeteredPlanSchedulerManagement",
                column: "FrequencyId");

            migrationBuilder.CreateIndex(
                name: "IX_MeteredPlanSchedulerManagement_PlanId",
                table: "MeteredPlanSchedulerManagement",
                column: "PlanId");

            migrationBuilder.CreateIndex(
                name: "IX_MeteredPlanSchedulerManagement_SubscriptionId",
                table: "MeteredPlanSchedulerManagement",
                column: "SubscriptionId");

            migrationBuilder.BaselineV5_SeedViews();
            migrationBuilder.BaselineV5_SeedData();
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MeteredPlanSchedulerManagement");

            migrationBuilder.DropTable(
                name: "SchedulerFrequency");

            migrationBuilder.BaselineV5_DeSeedAll();
        }
    }
}
