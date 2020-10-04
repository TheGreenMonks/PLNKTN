using Amazon.Lambda.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace PLNKTNv2.BusinessLogic.Helpers.Implementation
{
    public class Email : IMessenger
    {
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

            string body = "" + controllerName + "were executed at " +
                DateTime.UtcNow.ToLongDateString() + " " + DateTime.Now.ToLongTimeString() + "\n\n" +
                "" + controllerName + " have been completed:\n\n" + strEmailMessageLines +

                "\nBest regards,\n\n\n" + "The PLNKTN Web App";

            Debug.WriteLine(body);
            LambdaLogger.Log(body);
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
    }
}