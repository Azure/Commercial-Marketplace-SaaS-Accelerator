using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Query.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marketplace.SaaS.Accelerator.DataAccess.Migrations.Custom
{
    internal static class BaselineV5_Seed
    {
        public static void BaselineV5_SeedData(this MigrationBuilder migrationBuilder)
        {
            var seedDate = DateTime.Now;
            migrationBuilder.Sql(@$"
INSERT INTO [dbo].[SchedulerFrequency] (Frequency) VALUES ('Hourly')
INSERT INTO [dbo].[SchedulerFrequency] (Frequency) VALUES ('Daily')
INSERT INTO [dbo].[SchedulerFrequency] (Frequency) VALUES ('Weekly')
INSERT INTO [dbo].[SchedulerFrequency] (Frequency) VALUES ('Monthly')
INSERT INTO [dbo].[SchedulerFrequency] (Frequency) VALUES ('Yearly')
");
        }

        public static void BaselineV5_SeedViews(this MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
EXEC(N'
CREATE VIEW [dbo].[SchedulerManagerView]
	AS SELECT 
	m.Id,
	m.SchedulerName,
	s.AMPSubscriptionId,
	s.Name as SubscriptionName,
	s.PurchaserEmail,
	p.PlanId,
	d.Dimension,
	f.Frequency,
	m.Quantity,
	m.StartDate,
	m.NextRunTime
	FROM MeteredPlanSchedulerManagement m
	inner join SchedulerFrequency f	on m.FrequencyId=f.Id
	inner join Subscriptions s on m.SubscriptionId=s.Id
	inner join Plans p on m.PlanId=p.Id
	inner join MeteredDimensions d on m.DimensionId=d.Id
')");
        }

        public static void BaselineV5_DeSeedAll(this MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"DROP VIEW [dbo].[SchedulerManagerView]");
        }
    }
}