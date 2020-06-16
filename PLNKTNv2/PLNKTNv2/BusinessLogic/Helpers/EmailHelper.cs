using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Mail;

namespace PLNKTNv2.BusinessLogic.Helpers
{
    public static class EmailHelper
    {
        internal static string EmailMessage(string userId, string itemCompleted, string itemCompletedId)
        {
            return "At the timestamp associated with this email User '" + userId +
                "' has completed a '" + itemCompleted + "' with the ID of '" + itemCompletedId + "'.\n";
        }

        internal static void SendEmail(ICollection<string> messages, string controllerName)
        {
            var pwFile = "C:\\gmpw.txt";
            string fromEmail = "";
            string toEmail = "";
            string pw = "";
            string strMessages = "";

            if (messages.Count > 0)
            {
                foreach (var msg in messages)
                {
                    strMessages += msg;
                }
            }

            // Code to get pw from local file to keep it out of the code.
            if (File.Exists(pwFile))
            {
                using (StreamReader stream = System.IO.File.OpenText(pwFile))
                {
                    fromEmail = stream.ReadLine();
                    toEmail = stream.ReadLine();
                    pw = stream.ReadLine();
                }

                // Code to set upi email and send it.
                var fromAddress = new MailAddress(fromEmail, "PLNKTN Web App");
                var toAddress = new MailAddress(toEmail, "Developers");
                string subject = controllerName + " Completion Calculation Execution";
                string body = "The " + controllerName + " Completion Calculation method was executed at " +
                    DateTime.UtcNow.ToLongDateString() + " " + DateTime.Now.ToLongTimeString() + "\n\n" +
                    "The following " + controllerName + "(s) have been completed:\n\n" + strMessages +

                    "\nBest regards,\n\n\n" + "The PLNKTN Web App";

                var smtp = new SmtpClient
                {
                    Host = "smtp.gmail.com",
                    Port = 587,
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    Credentials = new NetworkCredential(fromAddress.Address, pw),
                    Timeout = 20000
                };
                using var message = new MailMessage(fromAddress, toAddress)
                {
                    Subject = subject,
                    Body = body
                };
                smtp.Send(message);
            }
            else
            {
                Debug.WriteLine("Error: Email send error");
                Debug.WriteLine("Location: RewardsController in 'sendEmail()' method.");
                Debug.WriteLine("Cause: Could not send email, possibly due to bad password file, bad access or bad email set up.");
            }
        }
    }
}