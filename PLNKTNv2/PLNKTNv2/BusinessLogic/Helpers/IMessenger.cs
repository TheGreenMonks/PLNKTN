namespace PLNKTNv2.BusinessLogic.Helpers
{
    public interface IMessenger
    {
        void AddLine(string userId, string itemCompleted, string itemCompletedId);

        int LineCount();

        void Send(string controllerName);
    }
}