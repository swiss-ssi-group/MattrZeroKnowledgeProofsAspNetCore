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
            var vaccineTemplate = await _vaccineVerifyVerifyMattrContext
                .VaccinationDataPresentationTemplates
                .OrderBy(u => u.Id)
                .LastOrDefaultAsync();

            if (vaccineTemplate != null)
            {
                var templateId = vaccineTemplate.TemplateId;
                return (vaccineTemplate.DidId, vaccineTemplate.TemplateId);
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
                ClaimsId = item.Claims.Id,
                //CountryOfVaccination = item.Claims.CountryOfVaccination,
                DateOfBirth = item.Claims.DateOfBirth,
                FamilyName = item.Claims.FamilyName,
                GivenName = item.Claims.GivenName,
                MedicinalProductCode = item.Claims.MedicinalProductCode,
                //NumberOfDoses = item.Claims.NumberOfDoses,
                //TotalNumberOfDoses = item.Claims.TotalNumberOfDoses,
                //VaccinationDate = item.Claims.VaccinationDate,

                ChallengeId = item.ChallengeId,
                Holder = item.Holder,
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
