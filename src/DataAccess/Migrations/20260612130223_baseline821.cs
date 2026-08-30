using Marketplace.SaaS.Accelerator.DataAccess.Migrations.Custom;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Marketplace.SaaS.Accelerator.DataAccess.Migrations
{
    public partial class baseline821 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.BaselineV821_SeedData();
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.BaselineV821_DeSeedData();
        }
    }
}
