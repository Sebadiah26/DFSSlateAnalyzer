namespace StaffManagement.Core.Common
{
    public interface IEmail
    {
        void Send(EmailMessage emailMessage);
        string GetEmailBodyFromListItem<T>(T listitem, string title) where T : class;
    }
}
