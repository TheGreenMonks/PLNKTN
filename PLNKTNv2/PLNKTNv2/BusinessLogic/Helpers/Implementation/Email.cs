using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Mail;

namespace PLNKTNv2.BusinessLogic.Helpers.Implementation
{
    public class Email : IMessenger
    {
        private readonly string pwFile = "C:\\gmpw.txt";
        private ICollection<string> messageLines = new List<string>();

        public void AddLine(string userId, string itemCompleted, string itemCompletedId)
        {
            messageLines.Add("At the timestamp associated with this email User '" + userId +
                                    "' has completed a '" + itemCompleted + "' with the ID of '" +
                                    itemCompletedId + "'.\n");
        }

        public int LineCount()
        {
            return messageLines.Count;
        }

        public void Send(string controllerName)
        {
            string strEmailMessageLines = GenerateEmailMessage(messageLines);
            //IDictionary<string, string> emailCreds = GetEmailCredentials(pwFile);
            //MailMessage message = GenerateEmail(strEmailMessageLines, controllerName, emailCreds["fromEmail"], emailCreds["toEmail"]);
            MailMessage message = GenerateEmail(strEmailMessageLines, controllerName, "a@b.com", "a@c.com");

            /*var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                Credentials = new NetworkCredential(message.From.Address, emailCreds["pw"]),
                Timeout = 20000
            };*/

            //smtp.Send(message);

            Debug.WriteLine(message.Body);
            //Debug.WriteLine("Location: RewardsController in 'sendEmail()' method.");
            //Debug.WriteLine("Cause: Could not send email, possibly due to bad password file, bad access or bad email set up.");
        }
        private MailMessage GenerateEmail(string strMessages, string controllerName, string fromEmail, string toEmail)
        {
            // Code to set up email and send it.
            var fromAddress = new MailAddress(fromEmail, "PLNKTN Web App");
            var toAddress = new MailAddress(toEmail, "Developers");
            string subject = controllerName + " Completion Calculation Execution";
            string body = "The " + controllerName + " Completion Calculation method was executed at " +
                DateTime.UtcNow.ToLongDateString() + " " + DateTime.Now.ToLongTimeString() + "\n\n" +
                "The following " + controllerName + "(s) have been completed:\n\n" + strMessages +

                "\nBest regards,\n\n\n" + "The PLNKTN Web App";

            using (var message = new MailMessage(fromAddress, toAddress)
            {
                Subject = subject,
                Body = body
            })

                return message;
        }

        private string GenerateEmailMessage(ICollection<string> messageLines)
        {
            string strMessages = "";

            if (messageLines.Count > 0)
            {
                foreach (var msg in messageLines)
                {
                    strMessages += msg;
                }
            }
            else
            {
                strMessages = "None this time...\n";
            }

            return strMessages;
        }
        private IDictionary<string, string> GetEmailCredentials(string pwFile)
        {
            IDictionary<string, string> emailCreds = new Dictionary<string, string>();

            if (File.Exists(pwFile))
            {
                using (StreamReader stream = System.IO.File.OpenText(pwFile))
                {
                    emailCreds.Add("fromEmail", stream.ReadLine());
                    emailCreds.Add("toEmail", stream.ReadLine());
                    emailCreds.Add("pw", stream.ReadLine());
                }
            }

            return emailCreds;
        }
    }
}