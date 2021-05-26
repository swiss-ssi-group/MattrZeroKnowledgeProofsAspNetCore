using Microsoft.EntityFrameworkCore;
using VaccineVerify.Data;
using System.Linq;
using System.Threading.Tasks;
using VaccineVerify.Controllers;

namespace VaccineVerify
{
    public class VaccineVerifyDbService
    {
        private readonly VaccineVerifyVerifyMattrContext _vaccineVerifyVerifyMattrContext;

        public VaccineVerifyDbService(VaccineVerifyVerifyMattrContext vaccineVerifyVerifyMattrContext)
        {
            _vaccineVerifyVerifyMattrContext = vaccineVerifyVerifyMattrContext;
        }

        public async Task<(string DidId, string TemplateId)> GetLastVaccinationDataPresentationTemplate()
        {
            var driverLicenseTemplate = await _vaccineVerifyVerifyMattrContext
                .VaccinationDataPresentationTemplates
                .OrderBy(u => u.Id)
                .LastOrDefaultAsync();

            if (driverLicenseTemplate != null)
            {
                var templateId = driverLicenseTemplate.TemplateId;
                return (driverLicenseTemplate.DidId, driverLicenseTemplate.TemplateId);
            }

            return (string.Empty, string.Empty);
        }

        public async Task CreateVaccinationDataTemplate(VaccinationDataPresentationTemplate vaccinationDataPresentationTemplate)
        {
            _vaccineVerifyVerifyMattrContext.VaccinationDataPresentationTemplates.Add(vaccinationDataPresentationTemplate);
            await _vaccineVerifyVerifyMattrContext.SaveChangesAsync();
        }

        public async Task<bool> ChallengeExists(string challengeId)
        {
            return await _vaccineVerifyVerifyMattrContext
                .VaccinationDataPresentationVerifications
                .AnyAsync(d => d.Challenge == challengeId);
        }

        public async Task CreateVaccinationDataPresentationVerify(VaccinationDataPresentationVerify vaccinationDataPresentationVerify)
        {
            _vaccineVerifyVerifyMattrContext.VaccinationDataPresentationVerifications.Add(vaccinationDataPresentationVerify);
            await _vaccineVerifyVerifyMattrContext.SaveChangesAsync();
        }

        public async Task PersistVerification(VerifiedVaccinationData item)
        {
            var data = new VerifiedVaccinationsData
            {
                DateOfBirth = item.Claims.DateOfBirth,
                ChallengeId = item.ChallengeId,
                ClaimsId = item.Claims.Id,
                FirstName = item.Claims.FirstName,
                Holder = item.Holder,
                LicenseIssuedAt = item.Claims.LicenseIssuedAt,
                LicenseType = item.Claims.LicenseType,
                Name = item.Claims.Name,
                PresentationType = item.PresentationType,
                Verified = item.Verified
            };

            _vaccineVerifyVerifyMattrContext.VerifiedVaccinationsData.Add(data);
            await _vaccineVerifyVerifyMattrContext.SaveChangesAsync();
        }

        public async Task<VerifiedVaccinationsData> GetVerifiedUser(string challengeId)
        {
            return await _vaccineVerifyVerifyMattrContext
                .VerifiedVaccinationsData
                .FirstOrDefaultAsync(v => v.ChallengeId == challengeId);
        }

        public async Task<Did> GetDid(string name)
        {
            return await _vaccineVerifyVerifyMattrContext
                .Dids
                .FirstOrDefaultAsync(v => v.Name == name);
        }

        public async Task<Did> CreateDid(Did did)
        {
            _vaccineVerifyVerifyMattrContext.Dids.Add(did);
            await _vaccineVerifyVerifyMattrContext.SaveChangesAsync();
            return did;
        }
    }
}
