using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SystemPraktykZawodowych.Core.Models
{
    public class Company
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string SupervisorName { get; set; }
        public string Address { get; set; }
        public int MaxInternships { get; set; }
        public int CurrentInternships { get; set; }
    }
}
