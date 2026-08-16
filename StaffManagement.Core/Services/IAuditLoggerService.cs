namespace StaffManagement.Core.Services
{
    public interface IAuditLoggerService
    {
        bool Log(int actiontypeid, string referencetable, string referenceid, string comment);
    }
}
