using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace Marketplace.SaaS.Accelerator.DataAccess.Migrations;
public partial class AddColumnsToPlan : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<int>(
            name: "PlanLicenses",
            table: "Plans",
            nullable: false,
            defaultValue: 0);

        migrationBuilder.AddColumn<int>(
            name: "CostPerUser",
            table: "Plans",
            nullable: false,
            defaultValue: 0);

        migrationBuilder.CreateTable(
            name: "KnowledgeBaseSource",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                plans_id = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                source = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_KnowledgeBaseSource", x => x.Id);
                table.ForeignKey(
                    name: "FK_KnowledgeBaseSource_Plans_Id",
                    column: x => x.plans_id,
                    principalTable: "Plans",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateTable(
            name: "Features",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                plans_id = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                feature = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Feature", x => x.Id);
                table.ForeignKey(
                    name: "FK_Feature_Plans_Id",
                    column: x => x.plans_id,
                    principalTable: "Plans",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            });
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "PlanLicenses",
            table: "Plans");

        migrationBuilder.DropColumn(
            name: "CostPerUser",
            table: "Plans");

        migrationBuilder.DropTable(
            name: "KnowledgeBaseSource");

        migrationBuilder.DropTable(
            name: "Features");
    }
}
