using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;

namespace VaccineVerify.Pages
{
    public class VerifiedUserModel : PageModel
    {
        private readonly VaccineVerifyDbService _VaccineVerifyDbService;

        public VerifiedUserModel(VaccineVerifyDbService VaccineVerifyDbService)
        {
            _VaccineVerifyDbService = VaccineVerifyDbService;
        }

        public string ChallengeId { get; set; }
        public VaccineVerifiedClaimsDto VerifiedVaccinationDataClaims { get; private set; }

        public async Task OnGetAsync(string challengeId)
        {
            // user query param to get challenge id and display data
            if (challengeId != null)
            {
                var verifiedDriverLicenseUser = await _VaccineVerifyDbService.GetVerifiedUser(challengeId);
                VerifiedVaccinationDataClaims = new VaccineVerifiedClaimsDto
                {
                    DateOfBirth = verifiedDriverLicenseUser.DateOfBirth,
                    MedicinalProductCode = verifiedDriverLicenseUser.MedicinalProductCode,
                    FamilyName = verifiedDriverLicenseUser.FamilyName,
                    GivenName = verifiedDriverLicenseUser.GivenName,
                    VaccinationDate = verifiedDriverLicenseUser.VaccinationDate,
                    CountryOfVaccination = verifiedDriverLicenseUser.CountryOfVaccination
                };
            }
        }
    }

    public class VaccineVerifiedClaimsDto
    {
        public string MedicinalProductCode { get; set; }
        public string GivenName { get; set; }
        public string FamilyName { get; set; }
        public string DateOfBirth { get; set; }
        public string VaccinationDate { get; set; }
        public string CountryOfVaccination { get; set; }
    }
}
