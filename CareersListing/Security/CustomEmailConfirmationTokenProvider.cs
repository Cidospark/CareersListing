using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CareersListing.Security
{
    // this infrastucture support customised token life span
    // it depends on <DataProtectionTokenProviderOptions> class
    public class CustomEmailConfirmationTokenProvider<TUser> : DataProtectorTokenProvider<TUser> where TUser : class
    {
        public CustomEmailConfirmationTokenProvider(IDataProtectionProvider dataProtectionProvider, 
                                                    IOptions<DataProtectionTokenProviderOptions> options) 
                                                    : base(dataProtectionProvider, options)  {}
    }
}
