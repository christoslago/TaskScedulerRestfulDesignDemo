using Logic.Helpers;
using MailKit.Security;
using MimeKit.Text;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using Logic.Services.Interfaces;

namespace Logic.Services
{
    public class EmailsService:IEmailsService
    {
        private readonly SmtpParams SmtpParams;
        public EmailsService(SmtpParams smtpParams)
        {
            SmtpParams = smtpParams;
        }

        public bool SendTaskWithEmail(DataRepository.Models.Task taskObject,string emailTO,string TaskFrom)
        {
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(SmtpParams.From));
            email.To.Add(MailboxAddress.Parse(emailTO));
            email.Subject = "You have a new Task";
            email.Body = new TextPart(TextFormat.Html)
            {
                Text = $"<h2>New Task from {TaskFrom}</h2>" + "\n" + $"<h3>{taskObject.Name}</h3>" + "\n" + taskObject.Description
            };

            using (var smtp = new SmtpClient())
            {
                try
                {
                    smtp.Connect(SmtpParams.SmtpServer, SmtpParams.SmtpPort, SecureSocketOptions.StartTls);
                    smtp.Authenticate(SmtpParams.Username, SmtpParams.Password);
                    smtp.Send(email);
                    smtp.Disconnect(true);
                }catch(Exception e)
                {
                    return false;
                }
            };

            return true;
        }
    }
}
