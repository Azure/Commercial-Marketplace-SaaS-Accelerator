using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Query.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marketplace.SaaS.Accelerator.DataAccess.Migrations.Custom
{
    internal static class BaselineV7_Seed
    {
        public static void BaselineV7_SeedData(this MigrationBuilder migrationBuilder)
        {
            var seedDate = DateTime.Now;
            migrationBuilder.Sql(@$"
                IF NOT EXISTS (SELECT * FROM [dbo].[ApplicationConfiguration] WHERE [Name] = 'WebNotificationUrl')
                BEGIN
                    INSERT [dbo].[ApplicationConfiguration] ( [Name], [Value], [Description]) VALUES ( N'WebNotificationUrl', N'', N'Setting this URL will enable pushing LandingPage/Webhook events to this external URL')
                END
                GO");
        }

        public static void BaselineV7_DeSeedData(this MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@$"
                IF EXISTS (SELECT * FROM [dbo].[ApplicationConfiguration] WHERE [Name] = 'WebNotificationUrl')
                BEGIN
                    DELETE FROM [dbo].[ApplicationConfiguration]  WHERE [Name] = 'WebNotificationUrl'
                END
                GO");
        }
    }
}