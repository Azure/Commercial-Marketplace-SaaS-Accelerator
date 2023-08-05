using Marketplace.SaaS.Accelerator.DataAccess.Migrations.Custom;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Marketplace.SaaS.Accelerator.DataAccess.Migrations
{
    public partial class Baseline_v8 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.BaselineV8_SeedData();
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.BaselineV8_DeSeedData();
        }
    }
}
