namespace PLNKTN.BusinessLogic
{
    interface IEmailHelper
    {
        void AddEmailMessageLine(string userId, string itemCompleted, string itemCompletedId);
        void SendEmail(string controllerName);
        int MessageLinesCount();
    }
}
