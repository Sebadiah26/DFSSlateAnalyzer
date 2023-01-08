namespace StaffManagement.Common
{
    public interface IEmail
    {
        void Send(EmailMessage emailMessage);
        // void GetEmailBodyFromList<T>(EmailMessage message, List<T> list, string title);
        string GetEmailBodyFromListItem<T>(T listitem, string title) where T : class;
    }
}
