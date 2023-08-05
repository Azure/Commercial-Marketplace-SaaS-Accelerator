using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Query.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marketplace.SaaS.Accelerator.DataAccess.Migrations.Custom
{
    internal static class BaselineV8_Seed
    {
        public static void BaselineV8_SeedData(this MigrationBuilder migrationBuilder)
        {
            var seedDate = DateTime.Now;
            migrationBuilder.Sql(@$"
                    INSERT [dbo].[ApplicationConfiguration] ( [Name], [Value], [Description]) VALUES ( N'EnablesSuccessfulSchedulerEmail', N'False', N'This will enable sending email for successful metered usage.')
                    INSERT [dbo].[ApplicationConfiguration] ( [Name], [Value], [Description]) VALUES ( N'EnablesFailureSchedulerEmail', N'False', N'This will enable sending email for failure metered usage.')
                    INSERT [dbo].[ApplicationConfiguration] ( [Name], [Value], [Description]) VALUES ( N'EnablesMissingSchedulerEmail', N'False', N'This will enable sending email for missing metered usage.')
                    INSERT [dbo].[ApplicationConfiguration] ( [Name], [Value], [Description]) VALUES ( N'SchedulerEmailTo', N'', N'Scheduler email receiver(s) ')
                    INSERT [dbo].[EmailTemplate] ([Status],[Description],[InsertDate],[TemplateBody],[Subject],[isActive]) VALUES (N'Accepted',N'Accepted',GetDate(),N'<html> <head> <meta http-equiv=""Content-Type"" content=""text/html; charset=UTF-8""/> </head> <body leftmargin=""0"" marginwidth=""0"" topmargin=""0"" marginheight=""0"" offset=""0""> <center> <table align=""center"" border=""0"" cellpadding=""0"" cellspacing=""0"" height=""100%"" width=""100%"" id=""bodyTable""> <tr><td align=""center"" valign=""top"" id=""bodyCell""><!-- BEGIN TEMPLATE // --><table border=""0"" cellpadding=""0"" cellspacing=""0"" id=""templateContainer""><tr><td align=""center"" valign=""top""><!-- BEGIN BODY // --><table border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%"" id=""templateBody""><tr><td valign=""top"" class=""bodyContent""><h2>Subscription ${{SubscriptionName}}</h2><br><p>The Scheduled Task ${{SchedulerTaskName}} was fired <b>Successfully</b></p><p>The following section is the deatil results.</p><hr/><table border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%"" id=""templateBody"">${{ResponseJson}} </table></td></tr></table></td></tr></table><!-- // END BODY --></td></tr> </table> <!-- // END TEMPLATE --> </center> </body> </html>',N'Scheduled SaaS Metered Usage Submitted Successfully!',N'True')
                    INSERT [dbo].[EmailTemplate] ([Status],[Description],[InsertDate],[TemplateBody],[Subject],[isActive]) VALUES (N'Failure',N'Failure',GetDate(),N'<html><head><meta http-equiv=""Content-Type"" content=""text/html; charset=UTF-8""/></head><body leftmargin=""0"" marginwidth=""0"" topmargin=""0"" marginheight=""0"" offset=""0""><center><table align=""center"" border=""0"" cellpadding=""0"" cellspacing=""0"" height=""100%"" width=""100%"" id=""bodyTable""><tr><td align=""center"" valign=""top"" id=""bodyCell""><!-- BEGIN TEMPLATE // --><table border=""0"" cellpadding=""0"" cellspacing=""0"" id=""templateContainer""> 	<tr><td align=""center"" valign=""top""><!-- BEGIN BODY // -->	<table border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%"" id=""templateBody""><tr>	<td valign=""top"" class=""bodyContent""><h2 >Subscription ${{SubscriptionName}}</h2><br><p>The Scheduled Task ${{SchedulerTaskName}} was fired<b> but Failed to Submit Data</b></p><br>Please try again or contact technical support to troubleshoot the issue.<p>The following section is the deatil results.</p><hr/><table border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%"" id=""templateBody"">${{ResponseJson}}</table></td></tr></table></td>	</tr></table><!-- // END BODY --></td></tr></table><!-- // END TEMPLATE --></center></body> </html>',N'Scheduled SaaS Metered Usage Failure!',N'True')
                    INSERT [dbo].[EmailTemplate] ([Status],[Description],[InsertDate],[TemplateBody],[Subject],[isActive]) VALUES (N'Missing',N'Missing',GetDate(),N'<html><head><meta http-equiv=""Content-Type"" content=""text/html; charset=UTF-8""/></head><body leftmargin=""0"" marginwidth=""0"" topmargin=""0"" marginheight=""0"" offset=""0""><center><table align=""center"" border=""0"" cellpadding=""0"" cellspacing=""0"" height=""100%"" width=""100%"" id=""bodyTable""><tr><td align=""center"" valign=""top"" id=""bodyCell""><!-- BEGIN TEMPLATE // --><table border=""0"" cellpadding=""0"" cellspacing=""0"" id=""templateContainer""> 	<tr><td align=""center"" valign=""top""><!-- BEGIN BODY // -->	<table border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%"" id=""templateBody""><tr>	<td valign=""top"" class=""bodyContent""><h2 >Subscription ${{SubscriptionName}}</h2><br><p>The Scheduled Task ${{SchedulerTaskName}} was skipped by scheduler engine!</b></p><br>Please try again or contact technical support to troubleshoot the issue.<p>The following section is the deatil results.</p><hr/><table border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%"" id=""templateBody"">${{ResponseJson}}</table></td></tr></table></td>	</tr></table><!-- // END BODY --></td></tr></table><!-- // END TEMPLATE --></center></body> </html>',N'Scheduled SaaS Metered Task was Skipped!',N'True')
                GO");
        }

        public static void BaselineV7_DeSeedData(this MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@$"
                Delete from [dbo].[ApplicationConfiguration] where [Name]= 'EnablesSuccessfulSchedulerEmail'
                Delete from [dbo].[ApplicationConfiguration] where [Name]= 'EnablesFailureSchedulerEmail'
                Delete From [dbo].[ApplicationConfiguration] where [Name]= 'EnablesMissingSchedulerEmail'
                Delete From [dbo].[ApplicationConfiguration] where [Name]= 'SchedulerEmailTo'
                Delete From [dbo].[EmailTemplate] Where [Subject] = 'Scheduled SaaS Metered Usage Submitted Successfully!'
                Delete From [dbo].[EmailTemplate] Where [Subject] = 'Scheduled SaaS Metered Usage Failure!'
                Delete From [dbo].[EmailTemplate] Where [Subject] = 'Scheduled SaaS Metered Task was Skipped!'
                GO");
        }
    }
}