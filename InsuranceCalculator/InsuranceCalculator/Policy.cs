using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsuranceCalculator
{
    class Policy
    {
            public DateTime PolicyStartDate;
            public List<Driver> Drivers;
            public double PolicyPrice;

            public Policy(DateTime policystartdate, List<Driver> drivers, double policyprice)
            {
                PolicyStartDate = policystartdate;
                Drivers = drivers;
                PolicyPrice = policyprice;
            }

            public Policy()
            {
                PolicyStartDate = DateTime.MinValue;
                Drivers = new List<Driver>();
                PolicyPrice = 500.0;
            }
    }
}
