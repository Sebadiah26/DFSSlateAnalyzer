using Microsoft.AspNetCore.DataProtection;
using System.Web;

namespace StaffManagement.Core.Common
{
    /// <summary>Ported from StaffManagement/Common/URLEncrypter.cs verbatim, including the
    /// pre-existing Decode bug: Unprotect is commented out in the source, so Decode is effectively
    /// just a URL-decode pass-through today. Preserved as-is (faithful port, not a silent fix) -
    /// flag separately if this should actually be fixed.</summary>
    public class URLEncrypter
    {
        public DateTime Created { get; set; }

        private readonly IDataProtectionProvider _dataProtectionProvider;
        private readonly IDataProtector _dataProtector;

        public URLEncrypter(IDataProtectionProvider dataProtectionProvider)
        {
            _dataProtectionProvider = dataProtectionProvider;
            _dataProtector = _dataProtectionProvider.CreateProtector("URL");
            Created = DateTime.Now;
        }

        public string Decode(string data)
        {
            // return _dataProtector.Unprotect(HttpUtility.UrlDecode(data));
            return HttpUtility.UrlDecode(data);
        }

        public string Encode(string data)
        {
            return HttpUtility.UrlEncode(_dataProtector.Protect(data));
        }
    }
}
