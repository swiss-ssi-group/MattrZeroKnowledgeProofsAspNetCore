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
            var driverLicenseCredentials = await _VaccineCredentialsIssuerMattrContext
                .VaccinationDataCredentials
                .OrderBy(u => u.Id)
                .LastOrDefaultAsync();

            if (driverLicenseCredentials != null)
            {
                var callback = $"https://{_mattrConfiguration.TenantSubdomain}/ext/oidc/v1/issuers/{driverLicenseCredentials.OidcIssuerId}/federated/callback";
                var oidcCredentialIssuer = JsonConvert.DeserializeObject<V1_CreateOidcIssuerResponse>(driverLicenseCredentials.OidcIssuer);
                return (callback, oidcCredentialIssuer.Credential.IssuerDid);
            }

            return (string.Empty, string.Empty);
        }

        public async Task<string> GetLastDriverLicenseCredentialIssuerUrl()
        {
            var driverLicense = await _VaccineCredentialsIssuerMattrContext
                .VaccinationDataCredentials
                .OrderBy(u => u.Id)
                .LastOrDefaultAsync();

            if (driverLicense != null)
            {
                var url = $"openid://discovery?issuer=https://{_mattrConfiguration.TenantSubdomain}/ext/oidc/v1/issuers/{driverLicense.OidcIssuerId}";
                return url;
            }

            return string.Empty;
        }

        public async Task<string> GetDriverLicenseCredentialIssuerUrl(string name)
        {
            var driverLicense = await _VaccineCredentialsIssuerMattrContext
                .VaccinationDataCredentials
                .FirstOrDefaultAsync(dl => dl.Name == name);

            if (driverLicense != null)
            {
                var url = $"openid://discovery?issuer=https://{_mattrConfiguration.TenantSubdomain}/ext/oidc/v1/issuers/{driverLicense.OidcIssuerId}";
                return url;
            }

            return string.Empty;
        }

        public async Task CreateDriverLicense(VaccinationDataCredentials driverLicense)
        {
            _VaccineCredentialsIssuerMattrContext.VaccinationDataCredentials.Add(driverLicense);
            await _VaccineCredentialsIssuerMattrContext.SaveChangesAsync();
        }
    }
}
