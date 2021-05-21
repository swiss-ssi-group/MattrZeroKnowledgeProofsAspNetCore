using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using VaccineCredentialsIssuer.Data;
using VaccineCredentialsIssuer.MattrOpenApiClient;
using VaccineCredentialsIssuer.Services;
using Newtonsoft.Json;
using System.Linq;
using System.Threading.Tasks;

namespace VaccineCredentialsIssuer
{
    public class VaccineCredentialsIssuerCredentialsService
    {
        private readonly VaccinationDataMattrContext _VaccineCredentialsIssuerMattrContext;
        private readonly MattrConfiguration _mattrConfiguration;

        public VaccineCredentialsIssuerCredentialsService(VaccinationDataMattrContext VaccineCredentialsIssuerMattrContext,
            IOptions<MattrConfiguration> mattrConfiguration)
        {
            _VaccineCredentialsIssuerMattrContext = VaccineCredentialsIssuerMattrContext;
            _mattrConfiguration = mattrConfiguration.Value;
        }

        public async Task<(string Callback, string DidId)> GetLastVaccineCredentialIssuer()
        {
            var vaccinationDataCredentials = await _VaccineCredentialsIssuerMattrContext
                .VaccinationDataCredentials
                .OrderBy(u => u.Id)
                .LastOrDefaultAsync();

            if (vaccinationDataCredentials != null)
            {
                var callback = $"https://{_mattrConfiguration.TenantSubdomain}/ext/oidc/v1/issuers/{vaccinationDataCredentials.OidcIssuerId}/federated/callback";
                var oidcCredentialIssuer = JsonConvert.DeserializeObject<V1_CreateOidcIssuerResponse>(vaccinationDataCredentials.OidcIssuer);
                return (callback, oidcCredentialIssuer.Credential.IssuerDid);
            }

            return (string.Empty, string.Empty);
        }

        public async Task<string> GetLastVaccinationDataCredentialIssuerUrl()
        {
            var vaccinationData = await _VaccineCredentialsIssuerMattrContext
                .VaccinationDataCredentials
                .OrderBy(u => u.Id)
                .LastOrDefaultAsync();

            if (vaccinationData != null)
            {
                var url = $"openid://discovery?issuer=https://{_mattrConfiguration.TenantSubdomain}/ext/oidc/v1/issuers/{vaccinationData.OidcIssuerId}";
                return url;
            }

            return string.Empty;
        }

        public async Task<string> GetVaccinationDataCredentialIssuerUrl(string name)
        {
            var vaccinationData = await _VaccineCredentialsIssuerMattrContext
                .VaccinationDataCredentials
                .FirstOrDefaultAsync(dl => dl.Name == name);

            if (vaccinationData != null)
            {
                var url = $"openid://discovery?issuer=https://{_mattrConfiguration.TenantSubdomain}/ext/oidc/v1/issuers/{vaccinationData.OidcIssuerId}";
                return url;
            }

            return string.Empty;
        }

        public async Task CreateVaccinationData(VaccinationDataCredentials vaccinationDataCredentials)
        {
            _VaccineCredentialsIssuerMattrContext.VaccinationDataCredentials.Add(vaccinationDataCredentials);
            await _VaccineCredentialsIssuerMattrContext.SaveChangesAsync();
        }
    }
}
