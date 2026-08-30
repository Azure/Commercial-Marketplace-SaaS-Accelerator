using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Query.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marketplace.SaaS.Accelerator.DataAccess.Migrations.Custom
{
    internal static class BaselineV821_Seed
    {
        public static void BaselineV821_SeedData(this MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@$"
                    IF NOT EXISTS (SELECT * FROM [dbo].[ApplicationConfiguration] WHERE [Name] = 'ValidateWebhookOperation')
                    BEGIN
                        INSERT [dbo].[ApplicationConfiguration] ( [Name], [Value], [Description]) VALUES ( N'ValidateWebhookOperation', N'true', N'Validates webhook notifications against the marketplace Get Operation API before mutating local subscription state.')
                    END
                GO");
        }

        public static void BaselineV821_DeSeedData(this MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@$"

                IF EXISTS (SELECT * FROM [dbo].[ApplicationConfiguration] WHERE [Name] = 'ValidateWebhookOperation')
                BEGIN
                    DELETE FROM [dbo].[ApplicationConfiguration]  WHERE [Name] = 'ValidateWebhookOperation'
                END
                GO");
        }
    }
}
