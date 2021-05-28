using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using VaccineVerify.Data;
using VaccineVerify.MattrOpenApiClient;

namespace VaccineVerify.Services
{
    public class MattrCreateDidService
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _clientFactory;
        private readonly MattrTokenApiService _mattrTokenApiService;
        private readonly MattrConfiguration _mattrConfiguration;
        private readonly VaccineVerifyDbService _vaccineVerifyDbService;


        public MattrCreateDidService(IConfiguration configuration,
            IHttpClientFactory clientFactory,
            IOptions<MattrConfiguration> mattrConfiguration,
            MattrTokenApiService mattrTokenApiService,
            VaccineVerifyDbService vaccineVerifyDbService)
        {
            _configuration = configuration;
            _clientFactory = clientFactory;
            _mattrTokenApiService = mattrTokenApiService;
            _mattrConfiguration = mattrConfiguration.Value;
            _vaccineVerifyDbService = vaccineVerifyDbService;
        }

        public async Task<V1_CreateDidResponse> GetDidOrCreate(string name)
        {
            var did = await _vaccineVerifyDbService.GetDid(name);
            if (did != null)
            {
                var payload = JsonConvert.DeserializeObject<V1_CreateDidResponse>(did.DidData);
                return payload;
            }

            var didMattr = await Create(name);

            return didMattr;
        }

        public async Task<V1_CreateDidResponse> Create(string name)
        {
            HttpClient client = _clientFactory.CreateClient();
            var accessToken = await _mattrTokenApiService.GetApiToken(client, "mattrAccessToken");
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", accessToken);
            client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json");

            var didMattr = await CreateMattrDid(client);
            var data = JsonConvert.SerializeObject(didMattr);
            var did = new Did
            {
                Name = name,
                DidData = data,
                DidTypeId = "ed25519",
                DidId = didMattr.Did
            };

            await _vaccineVerifyDbService.CreateDid(did);

            return didMattr;
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
        public string keyType { get; set; } = "ed25519";
    }
}