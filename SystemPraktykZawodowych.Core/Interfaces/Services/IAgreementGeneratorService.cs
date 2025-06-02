using SystemPraktykZawodowych.Core.Models;

namespace SystemPraktykZawodowych.Core.Interfaces.Services;

public interface IAgreementGeneratorService
{
    public Task<byte[]> GenerateAgreement(Registration registration);

}