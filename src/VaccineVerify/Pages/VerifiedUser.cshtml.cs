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
        public DriverLicenseClaimsDto VerifiedDriverLicenseClaims { get; private set; }

        public async Task OnGetAsync(string challengeId)
        {
            // user query param to get challenge id and display data
            if (challengeId != null)
            {
                var verifiedDriverLicenseUser = await _VaccineVerifyDbService.GetVerifiedUser(challengeId);
                VerifiedDriverLicenseClaims = new DriverLicenseClaimsDto
                {
                    DateOfBirth = verifiedDriverLicenseUser.DateOfBirth,
                    Name = verifiedDriverLicenseUser.Name,
                    LicenseType = verifiedDriverLicenseUser.LicenseType,
                    FirstName = verifiedDriverLicenseUser.FirstName,
                    LicenseIssuedAt = verifiedDriverLicenseUser.LicenseIssuedAt
                };
            }
        }
    }

    public class DriverLicenseClaimsDto
    {
        public string Name { get; set; }
        public string FirstName { get; set; }
        public string LicenseType { get; set; }
        public string DateOfBirth { get; set; }
        public string LicenseIssuedAt { get; set; }
    }
}
