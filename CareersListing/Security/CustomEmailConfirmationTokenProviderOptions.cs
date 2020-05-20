using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CareersListing.Security
{
    // this infrastucture support customised token life span
    // The setting is done in startup ConfigurationServices method 
    // it's an extension to addIdentity
    public class CustomEmailConfirmationTokenProviderOptions : DataProtectionTokenProviderOptions {}
}
