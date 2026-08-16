using System.Net.Mail;
using System.Text;

namespace StaffManagement.Core.Common
{
    /// <summary>Ported from StaffManagement/Common/Email.cs verbatim - uses System.Net.Mail.SmtpClient
    /// (BCL), not MailKit, despite the legacy project referencing the MailKit package (its MailKit
    /// usings were all commented out and unused).</summary>
    public class Email : IEmail
    {
        private readonly IEmailSettings _emailSettings;

        public Email(IEmailSettings emailSettings)
        {
            _emailSettings = emailSettings;
        }

        public void Send(EmailMessage emailMessage)
        {
            string header = GetHeader();
            string footer = GetFooter();

            MailMessage mailMessage = new MailMessage();
            mailMessage.From = new MailAddress(emailMessage.Sender);
            mailMessage.To.Add(emailMessage.Receiver);
            mailMessage.Subject = emailMessage.Subject;
            mailMessage.IsBodyHtml = true;
            mailMessage.Body = header + emailMessage.Content + footer;

            using (SmtpClient smtpClient = new SmtpClient())
            {
                smtpClient.Host = _emailSettings.SmtpServer;
                smtpClient.Timeout = 200000;
                smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtpClient.Port = _emailSettings.Port;
                smtpClient.EnableSsl = false;
                smtpClient.UseDefaultCredentials = false;

                smtpClient.Credentials = new System.Net.NetworkCredential(_emailSettings.UserName, _emailSettings.Password);
                smtpClient.Send(mailMessage);
            }
        }

        public string GetEmailBodyFromListItem<T>(T listitem, string title) where T : class
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("<tr><th>");
            sb.Append(title);
            sb.Append("</th></tr><tr><td><table width='600' border='0' align='center' cellpadding='0' cellspacing='0'><tr><td valign='top' style='padding-top:30px; padding-bottom:100px;'>");
            sb.Append("<table width='100%' border='1' cellpadding='1' cellspacing='1' bgcolor='#FFFFFF'>");
            sb.Append("<tr>");

            foreach (var property in typeof(T).GetProperties())
            {
                var propertyname = property.Name;

                sb.Append("<th>");
                if (!(property.GetValue(listitem)?.ToString() ?? string.Empty).StartsWith("http"))
                {
                    sb.Append(propertyname);
                }
                sb.Append("</th>");
            }

            sb.Append("<tr>");

            foreach (var property in typeof(T).GetProperties())
            {
                var propertyValue = property.GetValue(listitem);
                var propertyValueString = propertyValue?.ToString() ?? string.Empty;

                sb.Append("<td style='text-align:center'>");

                if (propertyValueString.StartsWith("http"))
                {
                    sb.Append("<a href=" + propertyValue + ">" + property.Name + "</a>");
                }
                else
                {
                    sb.Append(propertyValueString);
                }
                sb.Append("</td>");
            }

            sb.Append("</tr>");

            return sb.ToString();
        }

        public string GetHeader()
        {
            StringBuilder header = new StringBuilder();
            header.Append("<!DOCTYPE HTML PUBLIC '-//W3C//DTD HTML 4.01//EN' 'http://www.w3.org/TR/html4/strict.dtd'>");
            header.Append("<html><head><meta http-equiv='Content-Type' content='text/html; charset=utf-8'><title></title>");
            header.Append("<style type='text/css'>");
            header.Append("#outlook a {padding:0;}");
            header.Append("body{width:100% !important; -webkit-text-size-adjust:100%; -ms-text-size-adjust:100%; margin:0; padding:0;} /* force default font sizes */");
            header.Append(" .ExternalClass {width:100%;} .ExternalClass, .ExternalClass p, .ExternalClass span, .ExternalClass font, .ExternalClass td, .ExternalClass div {line-height: 100%;} /* Hotmail */");
            header.Append("a:active, a:visited, a[href^='tel'], a[href^='sms'] { text-decoration:none; color:#000001 !important; pointer-events: auto; cursor: default;}");
            header.Append("table td {border-collapse:collapse;}");
            header.Append("</style>");
            header.Append("</head>");
            header.Append("<body leftmargin='0' topmargin='0' marginwidth='0' marginheight='0' style='margin: 0px; padding: 0px; background-color: #FFFFFF;' bgcolor='#FFFFFF'>");
            header.Append("<table bgcolor='#f2eded' width='100%' border='0' align='center' cellpadding='0' cellspacing='0'>");

            return header.ToString();
        }

        public string GetFooter()
        {
            StringBuilder footer = new StringBuilder();
            footer.Append("</table>");
            footer.Append("</td></tr></table></td></tr></table></body></html>");
            return footer.ToString();
        }
    }
}
