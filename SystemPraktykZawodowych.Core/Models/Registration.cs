using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SystemPraktykZawodowych.Core.Models
{
    public class Registration
    {
        public int registration_id { get; set; }
        public int student_id { get; set; }
        public int company_id { get; set; }
        public DateTime registration_date { get; set; }
        public byte agreement_generated { get; set; }
        public DateTime agreement_generated_date { get; set; }
    }
}
