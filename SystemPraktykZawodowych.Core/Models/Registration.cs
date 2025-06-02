using System;

namespace SystemPraktykZawodowych.Core.Models
{
    public class Registration
    {
        public int RegistrationId { get; set; }
        public int StudentId { get; set; }
        public int CompanyId { get; set; }
        public DateTime RegistrationDate { get; set; } = DateTime.UtcNow; // Default to current UTC time
        public byte AgreementGenerated { get; set; } = 0; // 0 - not generated, 1 - generated
        public DateTime? AgreementGeneratedDate { get; set; } = null; // Nullable to allow for no date if not generated
    }
}
