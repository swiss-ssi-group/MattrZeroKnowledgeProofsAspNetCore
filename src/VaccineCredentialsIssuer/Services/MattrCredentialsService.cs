using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using VaccineCredentialsIssuer.Data;
using VaccineCredentialsIssuer.MattrOpenApiClient;
using VaccineCredentialsIssuer.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace VaccineCredentialsIssuer
{
    public class MattrCredentialsService
    {
        private readonly IConfiguration _configuration;
        private readonly VaccineCredentialsIssuerCredentialsService _vaccineCredentialsIssuerCredentialsService;
        private readonly IHttpClientFactory _clientFactory;
        private readonly MattrTokenApiService _mattrTokenApiService;
        private readonly MattrConfiguration _mattrConfiguration;

        public MattrCredentialsService(IConfiguration configuration,
            VaccineCredentialsIssuerCredentialsService vaccineCredentialsIssuerCredentialsService,
            IHttpClientFactory clientFactory,
            IOptions<MattrConfiguration> mattrConfiguration,
            MattrTokenApiService mattrTokenApiService)
        {
            _configuration = configuration;
            _vaccineCredentialsIssuerCredentialsService = vaccineCredentialsIssuerCredentialsService;
            _clientFactory = clientFactory;
            _mattrTokenApiService = mattrTokenApiService;
            _mattrConfiguration = mattrConfiguration.Value;
        }

        public async Task<string> CreateCredentialsAndCallback(string name)
        {
            // create a new one
            var vaccinationDataCredentials = await CreateMattrDidAndCredentialIssuer();
            vaccinationDataCredentials.Name = name;
            await _vaccineCredentialsIssuerCredentialsService.CreateVaccinationData(vaccinationDataCredentials);

            var callback = $"https://{_mattrConfiguration.TenantSubdomain}/ext/oidc/v1/issuers/{vaccinationDataCredentials.OidcIssuerId}/federated/callback";
            return callback;
        }

        private async Task<VaccinationDataCredentials> CreateMattrDidAndCredentialIssuer()
        {
            HttpClient client = _clientFactory.CreateClient();
            var accessToken = await _mattrTokenApiService.GetApiToken(client, "mattrAccessToken");
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", accessToken);
            client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json");

            var did = await CreateMattrDid(client);
            var oidcIssuer = await CreateMattrCredentialIssuer(client, did);

            return new VaccinationDataCredentials
            {
                Name = "not_named",
                Did = JsonConvert.SerializeObject(did),
                OidcIssuer = JsonConvert.SerializeObject(oidcIssuer),
                OidcIssuerId = oidcIssuer.Id
            };
        }

        private async Task<V1_CreateOidcIssuerResponse> CreateMattrCredentialIssuer(HttpClient client, V1_CreateDidResponse did)
        {
            // create vc, post to credentials api
            // https://learn.mattr.global/tutorials/issue/oidc-bridge/setup-issuer

            var createCredentialsUrl = $"https://{_mattrConfiguration.TenantSubdomain}/ext/oidc/v1/issuers";

            var payload = new MattrOpenApiClient.V1_CreateOidcIssuerRequest
            {
                Credential = new Credential
                {
                    IssuerDid = did.Did,
                    Name = "VaccinationCertificate7",
                    Context = new List<Uri> {
                        new Uri( "https://schema.org"),
                        new Uri( "https://www.w3.org/2018/credentials/v1")
                    },
                    Type = new List<string> { "VerifiableCredential" }
                },
                ClaimMappings = new List<ClaimMappings>
                {
                    new ClaimMappings{ JsonLdTerm="family_name", OidcClaim=$"https://{_mattrConfiguration.TenantSubdomain}/family_name"},
                    new ClaimMappings{ JsonLdTerm="given_name", OidcClaim=$"https://{_mattrConfiguration.TenantSubdomain}/given_name"},
                    new ClaimMappings{ JsonLdTerm="date_of_birth", OidcClaim=$"https://{_mattrConfiguration.TenantSubdomain}/date_of_birth"},
                    new ClaimMappings{ JsonLdTerm="medicinal_product_code", OidcClaim=$"https://{_mattrConfiguration.TenantSubdomain}/medicinal_product_code"},
                    new ClaimMappings{ JsonLdTerm="number_of_doses", OidcClaim=$"https://{_mattrConfiguration.TenantSubdomain}/number_of_doses"},
                    new ClaimMappings{ JsonLdTerm="total_number_of_doses", OidcClaim=$"https://{_mattrConfiguration.TenantSubdomain}/total_number_of_doses"},
                    new ClaimMappings{ JsonLdTerm="vaccination_date", OidcClaim=$"https://{_mattrConfiguration.TenantSubdomain}/vaccination_date"},
                    new ClaimMappings{ JsonLdTerm="country_of_vaccination", OidcClaim=$"https://{_mattrConfiguration.TenantSubdomain}/country_of_vaccination"}
                },
                FederatedProvider = new FederatedProvider
                {
                    ClientId = _configuration["Auth0Wallet:ClientId"],
                    ClientSecret = _configuration["Auth0Wallet:ClientSecret"],
                    Url = new Uri($"https://{_configuration["Auth0Wallet:Domain"]}"),
                    Scope = new List<string> { "openid", "profile", "email" }
                }
            };

            var payloadJson = JsonConvert.SerializeObject(payload);

            var uri = new Uri(createCredentialsUrl);

            using (var content = new StringContentWithoutCharset(payloadJson, "application/json"))
            {
                var createOidcIssuerResponse = await client.PostAsync(uri, content);

                if (createOidcIssuerResponse.StatusCode == System.Net.HttpStatusCode.Created)
                {
                    var v1CreateOidcIssuerResponse = JsonConvert.DeserializeObject<V1_CreateOidcIssuerResponse>(
                            await createOidcIssuerResponse.Content.ReadAsStringAsync());

                    return v1CreateOidcIssuerResponse;
                }

                var error = await createOidcIssuerResponse.Content.ReadAsStringAsync();
            }

            throw new Exception("whoops something went wrong");
        }

        private async Task<V1_CreateDidResponse> CreateMattrDid(HttpClient client)
        {
            // create did , post to dids 
            // https://learn.mattr.global/api-ref/#operation/createDid
            // https://learn.mattr.global/tutorials/dids/use-did/

            var createDidUrl = $"https://{_mattrConfiguration.TenantSubdomain}/core/v1/dids";

            var payload = new MattrOpenApiClient.V1_CreateDidDocument
            {
                Method = MattrOpenApiClient.V1_CreateDidDocumentMethod.Key,
                Options = new MattrOptions()
            };
            var payloadJson = JsonConvert.SerializeObject(payload);
            var uri = new Uri(createDidUrl);

            using (var content = new StringContentWithoutCharset(payloadJson, "application/json"))
            {
                var createDidResponse = await client.PostAsync(uri, content);

                if (createDidResponse.StatusCode == System.Net.HttpStatusCode.Created)
                {
                    var v1CreateDidResponse = JsonConvert.DeserializeObject<V1_CreateDidResponse>(
                            await createDidResponse.Content.ReadAsStringAsync());

                    return v1CreateDidResponse;
                }

                var error = await createDidResponse.Content.ReadAsStringAsync();
            }

            return null;
        }
    }
    public class MattrOptions
    {
        /// <summary>
        /// The supported key types for the DIDs are ed25519 and bls12381g2. 
        /// If the keyType is omitted, the default key type that will be used is ed25519.
        /// 
        /// If the keyType in options is set to bls12381g2 a DID will be created with 
        /// a BLS key type which supports BBS+ signatures for issuing ZKP-enabled credentials.
        /// </summary>
        public string keyType { get; set; } = "bls12381g2";
    }
}