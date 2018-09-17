using Microsoft.Extensions.PlatformAbstractions;
using PeterKottas.DotNetCore.WindowsService.Base;
using PeterKottas.DotNetCore.WindowsService.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;


namespace TFSLabelTagNotifcation
{
    public class TFSLabelCheck : MicroService, IMicroService
    {
        private IMicroServiceController controller;

        public TFSLabelCheck()
        {
            controller = null;
        }

        public TFSLabelCheck(IMicroServiceController controller)
        {
            this.controller = controller;
        }

        private string fileName = Path.Combine(PlatformServices.Default.Application.ApplicationBasePath, "log.txt");
        public void Start()
        {
            StartBase();
            // 1,000 milliseconds = 1 second
            // 300 seconds = 5 minutes
            // 300,000 milliseconds = 5 minutes
            Timers.Start("Poller", 300000, () =>
            {
                File.AppendAllText(fileName, string.Format("Polling at {0}\n", DateTime.Now.ToString("o")));

                TFSLabelsDataProvider dtData = new TFSLabelsDataProvider();
                IEnumerable<RawLabelsModel> dtResults = dtData.GetTFSLabels();

                using (IEnumerator<RawLabelsModel> sequenceEnum = dtResults.GetEnumerator())
                {
                    while (sequenceEnum.MoveNext())
                    {
                        File.AppendAllText(fileName, "*********************************************\n");
                        RawLabelsModel item = sequenceEnum.Current;
                        File.AppendAllText(fileName, string.Format("Starting to process LabelId: {0}\n", item.LabelId));

                        FinalLabelsModel lblRecord = dtData.GetLabelRecord(item.LabelId);
                        File.AppendAllText(fileName, string.Format("Found record for LabelId: {0}\n", item.LabelId));

                        String htmlBody = FormatLabelTable(lblRecord);
                        bool isSent = SendEmail(htmlBody, "Label Creation Notification");
                        if (isSent)
                        {
                            File.AppendAllText(fileName, string.Format("Email sent for LabelId: {0}\n", item.LabelId));
                            dtData.MarkAsSent(item.LabelId);
                            File.AppendAllText(fileName, string.Format("Record marked SENT for LabelId: {0}\n", item.LabelId));
                        } else
                        {
                            File.AppendAllText(fileName, string.Format("Email failed for LabelId: {0}\n", item.LabelId));
                        }

                        File.AppendAllText(fileName, "*********************************************\n");
                    }
                }
            });
            Console.WriteLine("I started");
            File.AppendAllText(fileName, "Started\n");
        }

        public void Stop()
        {
            StopBase();
            File.AppendAllText(fileName, "Stopped\n");
            Console.WriteLine("I stopped");
        }

        public String FormatLabelTable(FinalLabelsModel Record)
        {
            StringBuilder strbTable = new StringBuilder();

            // user-specified time zone
            TimeZoneInfo tziCentral = TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time");
            DateTime dtCentral = TimeZoneInfo.ConvertTimeFromUtc(Record.LastModified, tziCentral);

            strbTable.Append("<H1>TFS Label</H1>");
            strbTable.Append("<table style=\"background - color: #ffffff; filter: alpha(opacity=40); opacity: 0.95;border:1px black solid;\">");
            strbTable.Append("<tr>");
            strbTable.AppendFormat(" <th align=\"right\" bgcolor=\"#93CCEA\">Label ID</th> <td>{0}</td>", Record.LabelId);
            strbTable.Append("</tr>");
            strbTable.Append("<tr>");
            strbTable.AppendFormat(" <th align=\"right\" bgcolor=\"#93CCEA\">Label Name</th> <td>{0}</td>", Record.LabelName);
            strbTable.Append("</tr>");
            strbTable.Append("<tr>");
            strbTable.AppendFormat(" <th align=\"right\" bgcolor=\"#93CCEA\">Comment</th> <td>{0}</td>", Record.Comment);
            strbTable.Append("</tr>");
            strbTable.Append("<tr>");
            strbTable.AppendFormat(" <th align=\"right\" bgcolor=\"#93CCEA\">Project Name</th> <td>{0}</td>", Record.ProjectName);
            strbTable.Append("</tr>");
            strbTable.Append("<tr>");
            strbTable.AppendFormat(" <th align=\"right\" bgcolor=\"#93CCEA\">Path</th> <td>{0}</td>", Record.Path);
            strbTable.Append("</tr>");
            strbTable.Append("<tr>");
            strbTable.AppendFormat(" <th align=\"right\" bgcolor=\"#93CCEA\">Last Modified</th> <td>{0}</td>", dtCentral);
            strbTable.Append("</tr>");
            strbTable.Append("<tr>");
            strbTable.AppendFormat(" <th align=\"right\" bgcolor=\"#93CCEA\">User</th> <td>{0}</td>", Record.User);
            strbTable.Append("</tr>");
            strbTable.Append("</table>");

            return strbTable.ToString();
        }
        
        private bool SendEmail(String Body, String Subject)
        {
            bool IsSent = false;
            try
            {
                System.Net.Mail.SmtpClient client = new System.Net.Mail.SmtpClient("smtp.server.com");
                client.UseDefaultCredentials = true;

                // Set client.UseDefaultCredentials = false above before setting your SMTP credentials next
                //client.Credentials = new System.Net.NetworkCredential("username", "password");

                System.Net.Mail.MailMessage mailMessage = new System.Net.Mail.MailMessage();
                mailMessage.From = new System.Net.Mail.MailAddress("donotreply@mycompany.com", "Some Display Name");
                mailMessage.To.Add("Richard.Nunez@mycompany.com");
                mailMessage.Subject = Subject;
                mailMessage.IsBodyHtml = true;
                mailMessage.Body = Body;
                client.Send(mailMessage);
                IsSent = true;
            } catch
            {
                IsSent = false;
            }
            
            return IsSent;
        }
    }
}
