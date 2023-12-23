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

                    IF NOT EXISTS (SELECT * FROM [dbo].[ApplicationConfiguration] WHERE [Name] = 'EnablesSuccessfulSchedulerEmail')
                    BEGIN
                        INSERT [dbo].[ApplicationConfiguration] ( [Name], [Value], [Description]) VALUES ( N'EnablesSuccessfulSchedulerEmail', N'False', N'This will enable sending email for successful metered usage.')
                    END

                    IF NOT EXISTS (SELECT * FROM [dbo].[ApplicationConfiguration] WHERE [Name] = 'EnablesFailureSchedulerEmail')
                    BEGIN
                        INSERT [dbo].[ApplicationConfiguration] ( [Name], [Value], [Description]) VALUES ( N'EnablesFailureSchedulerEmail', N'False', N'This will enable sending email for failure metered usage.')
                    END

                    IF NOT EXISTS (SELECT * FROM [dbo].[ApplicationConfiguration] WHERE [Name] = 'EnablesMissingSchedulerEmail')
                    BEGIN
                        INSERT [dbo].[ApplicationConfiguration] ( [Name], [Value], [Description]) VALUES ( N'EnablesMissingSchedulerEmail', N'False', N'This will enable sending email for missing metered usage.')
                    END

                    IF NOT EXISTS (SELECT * FROM [dbo].[ApplicationConfiguration] WHERE [Name] = 'SchedulerEmailTo')
                    BEGIN
                        INSERT [dbo].[ApplicationConfiguration] ( [Name], [Value], [Description]) VALUES ( N'SchedulerEmailTo', N'', N'Scheduler email receiver(s) ')
                    END

                    IF NOT EXISTS (SELECT * FROM [dbo].[EmailTemplate] WHERE [Status] = 'Accepted')
                    BEGIN
                        INSERT [dbo].[EmailTemplate] ([Status],[Description],[InsertDate],[TemplateBody],[Subject],[isActive]) VALUES (N'Accepted',N'Accepted',GetDate(),N'<html> <head> <meta http-equiv=""Content-Type"" content=""text/html; charset=UTF-8""/> </head> <body leftmargin=""0"" marginwidth=""0"" topmargin=""0"" marginheight=""0"" offset=""0""> <center> <table align=""center"" border=""0"" cellpadding=""0"" cellspacing=""0"" height=""100%"" width=""100%"" id=""bodyTable""> <tr><td align=""center"" valign=""top"" id=""bodyCell""><!-- BEGIN TEMPLATE // --><table border=""0"" cellpadding=""0"" cellspacing=""0"" id=""templateContainer""><tr><td align=""center"" valign=""top""><!-- BEGIN BODY // --><table border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%"" id=""templateBody""><tr><td valign=""top"" class=""bodyContent""><h2>Subscription ****SubscriptionName****</h2><br><p>The Scheduled Task ****SchedulerTaskName**** was fired <b>Successfully</b></p><p>The following section is the deatil results.</p><hr/><table border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%"" id=""templateBody"">****ResponseJson**** </table></td></tr></table></td></tr></table><!-- // END BODY --></td></tr> </table> <!-- // END TEMPLATE --> </center> </body> </html>',N'Scheduled SaaS Metered Usage Submitted Successfully!',N'True')
                    END

                    IF NOT EXISTS (SELECT * FROM [dbo].[EmailTemplate] WHERE [Status] = 'Failure')
                    BEGIN
                        INSERT [dbo].[EmailTemplate] ([Status],[Description],[InsertDate],[TemplateBody],[Subject],[isActive]) VALUES (N'Failure',N'Failure',GetDate(),N'<html><head><meta http-equiv=""Content-Type"" content=""text/html; charset=UTF-8""/></head><body leftmargin=""0"" marginwidth=""0"" topmargin=""0"" marginheight=""0"" offset=""0""><center><table align=""center"" border=""0"" cellpadding=""0"" cellspacing=""0"" height=""100%"" width=""100%"" id=""bodyTable""><tr><td align=""center"" valign=""top"" id=""bodyCell""><!-- BEGIN TEMPLATE // --><table border=""0"" cellpadding=""0"" cellspacing=""0"" id=""templateContainer""> 	<tr><td align=""center"" valign=""top""><!-- BEGIN BODY // -->	<table border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%"" id=""templateBody""><tr>	<td valign=""top"" class=""bodyContent""><h2 >Subscription ****SubscriptionName****</h2><br><p>The Scheduled Task ****SchedulerTaskName**** was fired<b> but Failed to Submit Data</b></p><br>Please try again or contact technical support to troubleshoot the issue.<p>The following section is the deatil results.</p><hr/><table border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%"" id=""templateBody"">****ResponseJson****</table></td></tr></table></td>	</tr></table><!-- // END BODY --></td></tr></table><!-- // END TEMPLATE --></center></body> </html>',N'Scheduled SaaS Metered Usage Failure!',N'True')
                    END

                    IF NOT EXISTS (SELECT * FROM [dbo].[EmailTemplate] WHERE [Status] = 'Missing')
                    BEGIN
                        INSERT [dbo].[EmailTemplate] ([Status],[Description],[InsertDate],[TemplateBody],[Subject],[isActive]) VALUES (N'Missing',N'Missing',GetDate(),N'<html><head><meta http-equiv=""Content-Type"" content=""text/html; charset=UTF-8""/></head><body leftmargin=""0"" marginwidth=""0"" topmargin=""0"" marginheight=""0"" offset=""0""><center><table align=""center"" border=""0"" cellpadding=""0"" cellspacing=""0"" height=""100%"" width=""100%"" id=""bodyTable""><tr><td align=""center"" valign=""top"" id=""bodyCell""><!-- BEGIN TEMPLATE // --><table border=""0"" cellpadding=""0"" cellspacing=""0"" id=""templateContainer""> 	<tr><td align=""center"" valign=""top""><!-- BEGIN BODY // -->	<table border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%"" id=""templateBody""><tr>	<td valign=""top"" class=""bodyContent""><h2 >Subscription ****SubscriptionName****</h2><br><p>The Scheduled Task ****SchedulerTaskName**** was skipped by scheduler engine!</b></p><br>Please try again or contact technical support to troubleshoot the issue.<p>The following section is the deatil results.</p><hr/><table border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%"" id=""templateBody"">****ResponseJson****</table></td></tr></table></td>	</tr></table><!-- // END BODY --></td></tr></table><!-- // END TEMPLATE --></center></body> </html>',N'Scheduled SaaS Metered Task was Skipped!',N'True')
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

                    IF  EXISTS (SELECT * FROM [dbo].[ApplicationConfiguration] WHERE [Name] = 'EnablesSuccessfulSchedulerEmail')
                    BEGIN
                        DELETE FROM [dbo].[ApplicationConfiguration]  WHERE [Name] = 'EnablesSuccessfulSchedulerEmail'
                    END

                    IF  EXISTS (SELECT * FROM [dbo].[ApplicationConfiguration] WHERE [Name] = 'EnablesFailureSchedulerEmail')
                    BEGIN
                        DELETE FROM [dbo].[ApplicationConfiguration]  WHERE [Name] = 'EnablesFailureSchedulerEmail'
                    END

                    IF  EXISTS (SELECT * FROM [dbo].[ApplicationConfiguration] WHERE [Name] = 'EnablesMissingSchedulerEmail')
                    BEGIN
                        DELETE FROM [dbo].[ApplicationConfiguration] WHERE [Name] = 'EnablesMissingSchedulerEmail'
                    END

                    IF  EXISTS (SELECT * FROM [dbo].[ApplicationConfiguration] WHERE [Name] = 'SchedulerEmailTo')
                    BEGIN
                        DELETE FROM [dbo].[ApplicationConfiguration] WHERE [Name] = 'SchedulerEmailTo'
                    END

                    IF  EXISTS (SELECT * FROM [dbo].[EmailTemplate] WHERE [Status] = 'Accepted')
                    BEGIN
                        DELETE FROM [dbo].[EmailTemplate]  WHERE [Status] = 'Accepted'
                    END

                    IF  EXISTS (SELECT * FROM [dbo].[EmailTemplate] WHERE [Status] = 'Failure')
                    BEGIN
                        DELETE FROM [dbo].[EmailTemplate] WHERE [Status] = 'Failure'
                    END

                    IF  EXISTS (SELECT * FROM [dbo].[EmailTemplate] WHERE [Status] = 'Missing')
                    BEGIN
                        DELETE FROM [dbo].[EmailTemplate] WHERE [Status] = 'Missing'
                    END

                GO");
        }
    }
}