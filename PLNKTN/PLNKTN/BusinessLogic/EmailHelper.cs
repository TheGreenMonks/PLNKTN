using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Mail;

namespace PLNKTN.BusinessLogic
{
    public static class EmailHelper
    {
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
                using (var message = new MailMessage(fromAddress, toAddress)
                {
                    Subject = subject,
                    Body = body
                })
                {
                    smtp.Send(message);
                }
            }
            else
            {
                Debug.WriteLine("Error: Email send error");
                Debug.WriteLine("Location: RewardsController in 'sendEmail()' method.");
                Debug.WriteLine("Cause: Could not send email, possibly due to bad password file, bad access or bad email set up.");
            }
        }

        internal static void SendEmailSecureCode(string secureCode, string recipientEmail)
        {
            var pwFile = "C:\\gmpw.txt";
            string fromEmail = "";
            string toEmail = "";
            string pw = "";

            // Code to get pw from local file to keep it out of the code.
            if (File.Exists(pwFile))
            {
                using (StreamReader stream = System.IO.File.OpenText(pwFile))
                {
                    fromEmail = stream.ReadLine();
                    toEmail = stream.ReadLine();
                    pw = stream.ReadLine();
                }

                // replace static email address from file with user inputted email address
                toEmail = recipientEmail;

                // Code to set upi email and send it.
                var fromAddress = new MailAddress(fromEmail, "PLNKTN Web App");
                var toAddress = new MailAddress(toEmail, "PLNKTN App User");
                string subject = "PLNKTN Account Recovery Secure Code";
                string body = "A request to retrieve the user account associated with this email address on PLNKTN has been received.  " +
                    "Below is the security code to retrieve your account.  Please enter this into the PLNKTN app.  This code will expire in 1 hour.  \n\n" +
                    "If you did not make this request then please ignore this message.\n\n" + "Secure Code: " + secureCode +

                    "\n\nBest regards,\n\n\n" + "The PLNKTN Web App Team";

                var smtp = new SmtpClient
                {
                    Host = "smtp.gmail.com",
                    Port = 587,
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    Credentials = new NetworkCredential(fromAddress.Address, pw),
                    Timeout = 20000
                };
                using (var message = new MailMessage(fromAddress, toAddress)
                {
                    Subject = subject,
                    Body = body
                })
                {
                    smtp.Send(message);
                }
            }
            else
            {
                Debug.WriteLine("Error: Email send error");
                Debug.WriteLine("Location: AccountController in 'sendEmail()' method.");
                Debug.WriteLine("Cause: Could not send email, possibly due to bad password file, bad access or bad email set up.");
            }
        }

        internal static string EmailMessage(string userId, string itemCompleted, string itemCompletedId)
        {
            return "At the timestamp associated with this email User '" + userId +
                "' has completed a '" + itemCompleted + "' with the ID of '" + itemCompletedId + "'.\n";
        }
    }
}
