using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CareersListing.Utilities
{
    public class ValidEmailDomainAttribute : ValidationAttribute
    {
        private readonly string allowedDomain;

        public ValidEmailDomainAttribute(string allowedDomain)
        {
            this.allowedDomain = allowedDomain;
        }

        // the method to override here is isValid
        public override bool IsValid(object value)
        {
            // take the value param, convert it to string and split it on the @ sign
            // change the splited string @ index 1 to uppercase and 
            // compare it with the allowedDomain attribute
            // return true or false
            string[] strings = value.ToString().Split("@");
            return strings[1].ToUpper() == allowedDomain.ToUpper();
        }
    }
}
