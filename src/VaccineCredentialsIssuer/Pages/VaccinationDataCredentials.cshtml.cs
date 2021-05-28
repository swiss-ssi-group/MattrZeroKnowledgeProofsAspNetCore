using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using System.Linq;
using System.Threading.Tasks;
using VaccineCredentialsIssuer.Data;
using VaccineCredentialsIssuer.Services;

namespace VaccineCredentialsIssuer.Pages
{
    public class VaccinationDataCredentialsModel : PageModel
    {
        private readonly VaccineCredentialsIssuerCredentialsService _vaccineCredentialsIssuerCredentialsService;
        private readonly MattrConfiguration _mattrConfiguration;

        public string VaccinationDataMessage { get; set; } = "Loading credentials";
        public bool HasVaccinationData { get; set; } = false;
        public VaccinationData VaccinationData { get; set; }
        public string CredentialOfferUrl { get; set; }
        public VaccinationDataCredentialsModel(
            VaccineCredentialsIssuerCredentialsService vaccineCredentialsIssuerCredentialsService,
            IOptions<MattrConfiguration> mattrConfiguration)
        {
            _vaccineCredentialsIssuerCredentialsService = vaccineCredentialsIssuerCredentialsService;
            _mattrConfiguration = mattrConfiguration.Value;
        }
        public async Task OnGetAsync()
        {

            var identityHasVaccinationDataClaims = true;

            var familyNameClaim = User.Claims.FirstOrDefault(t => t.Type == $"https://{_mattrConfiguration.TenantSubdomain}/family_name");
            var givenNameClaim = User.Claims.FirstOrDefault(t => t.Type == $"https://{_mattrConfiguration.TenantSubdomain}/given_name");
            var dateOfBirthClaim = User.Claims.FirstOrDefault(t => t.Type == $"https://{_mattrConfiguration.TenantSubdomain}/date_of_birth");
            var medicinalProductCodeClaim = User.Claims.FirstOrDefault(t => t.Type == $"https://{_mattrConfiguration.TenantSubdomain}/medicinal_product_code");
            var numberOfDosesClaim = User.Claims.FirstOrDefault(t => t.Type == $"https://{_mattrConfiguration.TenantSubdomain}/number_of_doses");
            var totalNumberOfDosesClaim = User.Claims.FirstOrDefault(t => t.Type == $"https://{_mattrConfiguration.TenantSubdomain}/total_number_of_doses");
            var vaccinationDateClaim = User.Claims.FirstOrDefault(t => t.Type == $"https://{_mattrConfiguration.TenantSubdomain}/vaccination_date");
            var countryOfVaccinationClaim = User.Claims.FirstOrDefault(t => t.Type == $"https://{_mattrConfiguration.TenantSubdomain}/country_of_vaccination");

            if (familyNameClaim == null
                || givenNameClaim == null
                || dateOfBirthClaim == null
                || medicinalProductCodeClaim == null
                || numberOfDosesClaim == null
                || totalNumberOfDosesClaim == null
                || vaccinationDateClaim == null
                || countryOfVaccinationClaim == null)
            {
                identityHasVaccinationDataClaims = false;
            }

            if (identityHasVaccinationDataClaims)
            {
                VaccinationData = new VaccinationData
                {
                    FamilyName = familyNameClaim.Value,
                    GivenName = givenNameClaim.Value,
                    DateOfBirth = dateOfBirthClaim.Value,
                    MedicinalProductCode = medicinalProductCodeClaim.Value,
                    NumberOfDoses = numberOfDosesClaim.Value,
                    TotalNumberOfDoses = totalNumberOfDosesClaim.Value,
                    VaccinationDate = vaccinationDateClaim.Value,
                    CountryOfVaccination = countryOfVaccinationClaim.Value
                };
                // get per name
                //var offerUrl = await _vaccineCredentialsIssuerCredentialsService.GetLastVaccinationDataCredentialIssuerUrl("ndlseven");

                // get the last one
                var offerUrl = await _vaccineCredentialsIssuerCredentialsService.GetLastVaccinationDataCredentialIssuerUrl();

                VaccinationDataMessage = "Add your vaccination data credentials to your wallet";
                CredentialOfferUrl = offerUrl;
                HasVaccinationData = true;
            }
            else
            {
                VaccinationDataMessage = "You have no valid vaccination data";
            }
        }
    }
}
