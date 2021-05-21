using Microsoft.EntityFrameworkCore;
using VaccineVerify.Data;
using System.Linq;
using System.Threading.Tasks;
using VaccineVerify.Controllers;

namespace VaccineVerify
{
    public class VaccineVerifyDbService
    {
        private readonly VaccineVerifyVerifyMattrContext _VaccineVerifyVerifyMattrContext;

        public VaccineVerifyDbService(VaccineVerifyVerifyMattrContext VaccineVerifyVerifyMattrContext)
        {
            _VaccineVerifyVerifyMattrContext = VaccineVerifyVerifyMattrContext;
        }

        public async Task<(string DidId, string TemplateId)> GetLastDriverLicensePrsentationTemplate()
        {
            var driverLicenseTemplate = await _VaccineVerifyVerifyMattrContext
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

        public async Task CreateDriverLicensePresentationTemplate(VaccinationDataPresentationTemplate vaccinationDataPresentationTemplate)
        {
            _VaccineVerifyVerifyMattrContext.VaccinationDataPresentationTemplates.Add(vaccinationDataPresentationTemplate);
            await _VaccineVerifyVerifyMattrContext.SaveChangesAsync();
        }

        public async Task<bool> ChallengeExists(string challengeId)
        {
            return await _VaccineVerifyVerifyMattrContext
                .VaccinationDataPresentationVerifications
                .AnyAsync(d => d.Challenge == challengeId);
        }

        public async Task CreateDrivingLicensePresentationVerify(VaccinationDataPresentationVerify vaccinationDataPresentationVerify)
        {
            _VaccineVerifyVerifyMattrContext.VaccinationDataPresentationVerifications.Add(vaccinationDataPresentationVerify);
            await _VaccineVerifyVerifyMattrContext.SaveChangesAsync();
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

            _VaccineVerifyVerifyMattrContext.VerifiedVaccinationsData.Add(data);
            await _VaccineVerifyVerifyMattrContext.SaveChangesAsync();
        }

        public async Task<VerifiedVaccinationsData> GetVerifiedUser(string challengeId)
        {
            return await _VaccineVerifyVerifyMattrContext
                .VerifiedVaccinationsData
                .FirstOrDefaultAsync(v => v.ChallengeId == challengeId);
        }
    }
}
