namespace StaffManagement.Blazor.Services
{
    /// <summary>
    /// Scoped one-shot message store, replacing Session["SecurityMessage"] + TempData["SecurityMessage"]
    /// (see HomeController.Index reading/clearing it in the source). Used by PermissionGate to tell
    /// the page it bounced the user back to why they ended up there.
    /// </summary>
    public class FlashMessageService
    {
        public string? Message { get; private set; }

        public event Action? OnChange;

        public void SetMessage(string message)
        {
            Message = message;
            OnChange?.Invoke();
        }

        public void Clear()
        {
            Message = null;
        }
    }
}
