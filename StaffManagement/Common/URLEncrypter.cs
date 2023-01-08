using Microsoft.AspNetCore.DataProtection;
using System;
using System.Web;

namespace StaffManagement.Common
{
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
            // return HttpUtility.UrlEncode(data);
        }


    }

}
