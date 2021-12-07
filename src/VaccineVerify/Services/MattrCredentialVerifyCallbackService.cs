using VaccineVerify.MattrOpenApiClient;
using VaccineVerify.Services;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using VaccineVerify.Data;
using Microsoft.Extensions.Options;
using System.Text.Encodings.Web;
using System.Security.Cryptography;

namespace VaccineVerify
{
    /// <summary>
    /// https://learn.mattr.global/tutorials/verify/using-callback/callback-e-to-e
    /// </summary>
    public class MattrCredentialVerifyCallbackService
    {
        private static string MATTR_CALLBACK_VERIFY_PATH = "api/Verification/VerificationDataCallback";

        /// <summary>
        /// TODO calculate this
        /// </summary>
        private static double MATTR_EPOCH_EXPIRES_TIME_VERIFIY = 1638836401000;

        private readonly IHttpClientFactory _clientFactory;
        private readonly MattrTokenApiService _mattrTokenApiService;
        private readonly VaccineVerifyDbService _vaccineVerifyDbService;
        private readonly MattrConfiguration _mattrConfiguration;
        private readonly MattrCreateDidService _mattrCreateDidService;

        public MattrCredentialVerifyCallbackService(IHttpClientFactory clientFactory,
            IOptions<MattrConfiguration> mattrConfiguration,
            MattrTokenApiService mattrTokenApiService,
            VaccineVerifyDbService vaccineVerifyDbService,
            MattrCreateDidService mattrCreateDidService)
        {
            _clientFactory = clientFactory;
            _mattrTokenApiService = mattrTokenApiService;
            _vaccineVerifyDbService = vaccineVerifyDbService;
            _mattrConfiguration = mattrConfiguration.Value;
            _mattrCreateDidService = mattrCreateDidService;
        }


        /// <summary>
        /// https://learn.mattr.global/tutorials/verify/using-callback/callback-e-to-e
        /// </summary>
        /// <param name="callbackBaseUrl"></param>
        /// <returns></returns>
        public async Task<(string WalletUrl, string ChallengeId)> CreateVerifyCallback(string callbackBaseUrl)
        {
            callbackBaseUrl = callbackBaseUrl.Trim();
            if (!callbackBaseUrl.EndsWith('/'))
            {
                callbackBaseUrl = $"{callbackBaseUrl}/";
            }

            var callbackUrlFull = $"{callbackBaseUrl}{MATTR_CALLBACK_VERIFY_PATH}";
            var challenge = GetEncodedRandomString();

            HttpClient client = _clientFactory.CreateClient();
            var accessToken = await _mattrTokenApiService.GetApiToken(client, "mattrAccessToken");

            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", accessToken);
            client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json");

            var template = await _vaccineVerifyDbService.GetLastVaccinationDataPresentationTemplate();

            var didToVerify = await _mattrCreateDidService.GetDidOrCreate("did_for_verify");
            // Request DID from ledger
            V1_GetDidResponse did = await RequestDID(didToVerify.Did, client);

            // Invoke the Presentation Request
            var invokePresentationResponse = await InvokePresentationRequest(
                client,
                didToVerify.Did,
                template.TemplateId,
                challenge,
                callbackUrlFull);

            // Sign and Encode the Presentation Request body
            var signAndEncodePresentationRequestBodyResponse = await SignAndEncodePresentationRequestBody(
                client, did, invokePresentationResponse);

            // fix strange DTO
            var jws = signAndEncodePresentationRequestBodyResponse.Replace("\"", "");

            // save to db // TODO add this back once working
            var vaccinationDataPresentationVerify = new VaccinationDataPresentationVerify
            {
                DidId = template.DidId,
                TemplateId = template.TemplateId,
                CallbackUrl = callbackUrlFull,
                Challenge = challenge,
                InvokePresentationResponse = JsonConvert.SerializeObject(invokePresentationResponse),
                Did = JsonConvert.SerializeObject(did),
                SignAndEncodePresentationRequestBody = jws
            };
            await _vaccineVerifyDbService.CreateVaccinationDataPresentationVerify(vaccinationDataPresentationVerify);

            var walletUrl = $"https://{_mattrConfiguration.TenantSubdomain}/?request={jws}";

            return (walletUrl, challenge);
        }

        private async Task<VerifyRequestResponse> InvokePresentationRequest(
            HttpClient client,
            string didId,
            string templateId,
            string challenge,
            string callbackUrl)
        {
            var createDidUrl = $"https://{_mattrConfiguration.TenantSubdomain}/v1/presentations/requests";

            var payload = new MattrOpenApiClient.V1_CreatePresentationRequestRequest
            {
                Did = didId,
                TemplateId = templateId,
                Challenge = challenge,
                CallbackUrl = new Uri(callbackUrl),
                ExpiresTime = MATTR_EPOCH_EXPIRES_TIME_VERIFIY // Epoch time
            };
            var payloadJson = JsonConvert.SerializeObject(payload);
            var uri = new Uri(createDidUrl);

            using (var content = new StringContentWithoutCharset(payloadJson, "application/json"))
            {
                var response = await client.PostAsync(uri, content);

                if (response.StatusCode == System.Net.HttpStatusCode.Created)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var v1CreatePresentationRequestResponse = JsonConvert.DeserializeObject<VerifyRequestResponse>(
                            responseContent);

                    return v1CreatePresentationRequestResponse;
                }

                var error = await response.Content.ReadAsStringAsync();
            }

            return null;
        }

        private async Task<V1_GetDidResponse> RequestDID(string didId, HttpClient client)
        {
            var requestUrl = $"https://{_mattrConfiguration.TenantSubdomain}/core/v1/dids/{didId}";
            var uri = new Uri(requestUrl);

            var didResponse = await client.GetAsync(uri);

            if (didResponse.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var v1CreateDidResponse = JsonConvert.DeserializeObject<V1_GetDidResponse>(
                        await didResponse.Content.ReadAsStringAsync());

                return v1CreateDidResponse;
            }

            var error = await didResponse.Content.ReadAsStringAsync();
            return null;
        }

        private async Task<string> SignAndEncodePresentationRequestBody(
            HttpClient client,
            V1_GetDidResponse did,
            VerifyRequestResponse v1CreatePresentationRequestResponse)
        {
            var createDidUrl = $"https://{_mattrConfiguration.TenantSubdomain}/v1/messaging/sign";

            object didUrlArray;
            did.DidDocument.AdditionalProperties.TryGetValue("authentication", out didUrlArray);
            var didUrl = didUrlArray.ToString().Split("\"")[1];
            var payload = new MattrOpenApiClient.SignMessageRequest
            {
                DidUrl = didUrl,
                Payload = v1CreatePresentationRequestResponse.Request
            };
            var payloadJson = JsonConvert.SerializeObject(payload);
            var uri = new Uri(createDidUrl);

            using (var content = new StringContentWithoutCharset(payloadJson, "application/json"))
            {
                var response = await client.PostAsync(uri, content);

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    return result;
                }

                var error = await response.Content.ReadAsStringAsync();
            }

            return null;
        }


        private string GetEncodedRandomString()
        {
            var base64 = Convert.ToBase64String(GenerateRandomBytes(30));
            return HtmlEncoder.Default.Encode(base64);
        }

        private byte[] GenerateRandomBytes(int length)
        {
            var item = RandomNumberGenerator.Create();
            var byteArray = new byte[length];
            item.GetBytes(byteArray);
            return byteArray;
        }
    }
}
