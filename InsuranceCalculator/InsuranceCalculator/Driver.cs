using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsuranceCalculator
{
    class Driver
    {
            public string Name {get; set;}
            public string Occupation {get; set;}
            public DateTime DateOfBirth {get; set;}
            public List<DateTime> Claims {get; set;}

            public Driver(string name, string occupation, DateTime dateofbirth, List<DateTime> claims)
            {
                Name = name;
                Occupation = occupation;
                DateOfBirth = dateofbirth;
                Claims = claims;
            }
            public Driver()
            {
                Name = "";
                Occupation = "";
                DateOfBirth = DateTime.MinValue;
                Claims = new List<DateTime>();
            }

            
    }
}
