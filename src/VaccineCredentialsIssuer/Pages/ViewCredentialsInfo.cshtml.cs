using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace VaccineCredentialsIssuer.Pages
{
    public class ViewLastCredentialsInfoModel : PageModel
    {
        private readonly VaccineCredentialsIssuerCredentialsService _vaccineCredentialsIssuerCredentialsService;

        public string LatestVaccinationDid { get; set; }
        public string LatestVaccinationDataCallback { get; set; }

        public string CredentialOfferUrl { get; set; }
        public ViewLastCredentialsInfoModel(VaccineCredentialsIssuerCredentialsService vaccineCredentialsIssuerCredentialsService)
        {
            _vaccineCredentialsIssuerCredentialsService = vaccineCredentialsIssuerCredentialsService;
        }
        public async Task OnGetAsync()
        {
            var credentialIssuer = await _vaccineCredentialsIssuerCredentialsService.GetLastVaccineCredentialIssuer();
            LatestVaccinationDataCallback = credentialIssuer.Callback;
            LatestVaccinationDid = credentialIssuer.DidId;
        }
    }
}
