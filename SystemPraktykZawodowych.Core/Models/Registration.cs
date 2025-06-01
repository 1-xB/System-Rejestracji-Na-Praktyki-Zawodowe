using System;

namespace SystemPraktykZawodowych.Core.Models
{
    public class Registration
    {
        public int RegistrationId { get; set; }
        public int StudentId { get; set; }
        public int CompanyId { get; set; }
        public DateTime RegistrationDate { get; set; }
        public byte AgreementGenerated { get; set; }
        public DateTime AgreementGeneratedDate { get; set; }
    }
}
