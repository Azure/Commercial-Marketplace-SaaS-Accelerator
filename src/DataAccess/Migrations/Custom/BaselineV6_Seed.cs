using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marketplace.SaaS.Accelerator.DataAccess.Migrations.Custom
{
    internal static class BaselineV6_Seed
    {
        public static void BaselineV6_SeedData(this MigrationBuilder migrationBuilder)
        {
            var seedDate = DateTime.Now;
            migrationBuilder.Sql(@$"
IF NOT EXISTS (SELECT * FROM [dbo].[SchedulerFrequency] WHERE [Frequency] = 'OneTime')
BEGIN
    INSERT INTO [dbo].[SchedulerFrequency] (Frequency) VALUES ('OneTime')
END
GO");
            migrationBuilder.Sql(@$"
INSERT [dbo].[ApplicationConfiguration] ( [Name], [Value], [Description]) VALUES ( N'EnableHourlyMeterSchedules', N'False', N'This will enable to run Hourly meter scheduled items')
INSERT [dbo].[ApplicationConfiguration] ( [Name], [Value], [Description]) VALUES ( N'EnableDailyMeterSchedules', N'False', N'This will enable to run Daily meter scheduled items')
INSERT [dbo].[ApplicationConfiguration] ( [Name], [Value], [Description]) VALUES ( N'EnableWeeklyMeterSchedules', N'False', N'This will enable to run Weekly meter scheduled items')
INSERT [dbo].[ApplicationConfiguration] ( [Name], [Value], [Description]) VALUES ( N'EnableMonthlyMeterSchedules', N'False', N'This will enable to run Monthly meter scheduled items')
INSERT [dbo].[ApplicationConfiguration] ( [Name], [Value], [Description]) VALUES ( N'EnableYearlyMeterSchedules', N'False', N'This will enable to run Yearly meter scheduled items')
INSERT [dbo].[ApplicationConfiguration] ( [Name], [Value], [Description]) VALUES ( N'EnableOneTimeMeterSchedules', N'False', N'This will enable to run OneTime meter scheduled items')
GO");
        }
    }
}