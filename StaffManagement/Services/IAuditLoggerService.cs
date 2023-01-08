namespace StaffManagement.Services
{
    public interface IAuditLoggerService
    {
        bool Log(int actiontypeid, string referencetable, string referenceid, string comment);


    }
}
