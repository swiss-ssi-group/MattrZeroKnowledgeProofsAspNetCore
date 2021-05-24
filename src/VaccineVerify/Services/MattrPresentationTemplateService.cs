using VaccineVerify.MattrOpenApiClient;
using VaccineVerify.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using VaccineVerify.Data;
using Microsoft.Extensions.Options;

namespace VaccineVerify
{
    public class MattrPresentationTemplateService
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly MattrTokenApiService _mattrTokenApiService;
        private readonly VaccineVerifyDbService _VaccineVerifyDbService;
        private readonly MattrConfiguration _mattrConfiguration;

        public MattrPresentationTemplateService(IHttpClientFactory clientFactory,
            IOptions<MattrConfiguration> mattrConfiguration,
            MattrTokenApiService mattrTokenApiService,
            VaccineVerifyDbService VaccineVerifyDbService)
        {
            _clientFactory = clientFactory;
            _mattrTokenApiService = mattrTokenApiService;
            _VaccineVerifyDbService = VaccineVerifyDbService;
            _mattrConfiguration = mattrConfiguration.Value;
        }

        public async Task<string> CreatePresentationTemplateId(string didId)
        {
            // create a new one
            var v1PresentationTemplateResponse = await CreateMattrPresentationTemplate(didId);

            // save to db
            var drivingLicensePresentationTemplate = new VaccinationDataPresentationTemplate
            {
                DidId = didId,
                TemplateId = v1PresentationTemplateResponse.Id,
                MattrPresentationTemplateReponse = JsonConvert.SerializeObject(v1PresentationTemplateResponse)
            };
            await _VaccineVerifyDbService.CreateVaccinationDataTemplate(drivingLicensePresentationTemplate);

            return v1PresentationTemplateResponse.Id;
        }

        private async Task<V1_PresentationTemplateResponse> CreateMattrPresentationTemplate(string didId)
        {
            HttpClient client = _clientFactory.CreateClient();
            var accessToken = await _mattrTokenApiService.GetApiToken(client, "mattrAccessToken");

            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", accessToken);
            client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json");

            var v1PresentationTemplateResponse = await CreateMattrPresentationTemplate(client, didId);
            return v1PresentationTemplateResponse;
        }

        private async Task<V1_PresentationTemplateResponse> CreateMattrPresentationTemplate(
            HttpClient client, string didId)
        {
            // create presentation, post to presentations templates api
            // https://learn.mattr.global/tutorials/verify/presentation-request-template
            // https://learn.mattr.global/tutorials/verify/presentation-request-template#create-a-privacy-preserving-presentation-request-template-for-zkp-enabled-credentials

            var createPresentationsTemplatesUrl = $"https://{_mattrConfiguration.TenantSubdomain}/v1/presentations/templates";

            var additionalPropertiesCredentialSubject = new Dictionary<string, object>();
            additionalPropertiesCredentialSubject.Add("credentialSubject", new VaccanationDataCredentialSubject
            { 
                Explicit = true
            });

            var additionalPropertiesCredentialQuery = new Dictionary<string, object>();
            additionalPropertiesCredentialQuery.Add("required", true);

            var additionalPropertiesQuery = new Dictionary<string, object>();
            additionalPropertiesQuery.Add("type", "QueryByFrame");
            additionalPropertiesQuery.Add("credentialQuery", new List<CredentialQuery2> {
                new CredentialQuery2
                {
                    Reason = "Please provide your vaccination data",
                    TrustedIssuer = new List<TrustedIssuer>{
                        new TrustedIssuer
                        {
                            Required = true,
                            Issuer = didId // DID use to create the oidc
                        }
                    },
                    Frame = new Frame
                    {
                        Context = new List<object>{
                            "https://www.w3.org/2018/credentials/v1",
                            "https://w3c-ccg.github.io/ldp-bbs2020/context/v1",
                            "https://schema.org",
                        },
                        Type = "VerifiableCredential",
                        AdditionalProperties = additionalPropertiesCredentialSubject

                    },
                    AdditionalProperties = additionalPropertiesCredentialQuery
                }
            });

            var payload = new MattrOpenApiClient.V1_CreatePresentationTemplate
            {
                Domain = _mattrConfiguration.TenantSubdomain,
                Name = "zkp-certificate-presentation-7",
                Query = new List<Query>
                {
                    new Query
                    {
                        AdditionalProperties = additionalPropertiesQuery
                    }
                }
            };

            var payloadJson = JsonConvert.SerializeObject(payload);

            var uri = new Uri(createPresentationsTemplatesUrl);

            using (var content = new StringContentWithoutCharset(payloadJson, "application/json"))
            {
                var presentationTemplateResponse = await client.PostAsync(uri, content);

                if (presentationTemplateResponse.StatusCode == System.Net.HttpStatusCode.Created)
                {

                    var v1PresentationTemplateResponse = JsonConvert
                            .DeserializeObject<MattrOpenApiClient.V1_PresentationTemplateResponse>(
                            await presentationTemplateResponse.Content.ReadAsStringAsync());

                    return v1PresentationTemplateResponse;
                }

                var error = await presentationTemplateResponse.Content.ReadAsStringAsync();

            }

            throw new Exception("whoops something went wrong");
        }
    }

    public class VaccanationDataCredentialSubject
    {
        [Newtonsoft.Json.JsonProperty("@explicit", Required = Newtonsoft.Json.Required.Always)]
        public bool Explicit { get; set; }

        [Newtonsoft.Json.JsonProperty("family_name", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required]
        public object FamilyName { get; set; } = new object();

        [Newtonsoft.Json.JsonProperty("given_name", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required]
        public object GivenName { get; set; } = new object();

        [Newtonsoft.Json.JsonProperty("date_of_birth", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required]
        public object DateOfBirth { get; set; } = new object();

        [Newtonsoft.Json.JsonProperty("medicinal_product_code", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required]
        public object MedicinalProductCode { get; set; } = new object();

        [Newtonsoft.Json.JsonProperty("number_of_doses", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required]
        public object NumberOfDoses { get; set; } = new object();

        [Newtonsoft.Json.JsonProperty("total_number_of_doses", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required]
        public object TotalNumberOfDoses { get; set; } = new object();

        [Newtonsoft.Json.JsonProperty("vaccination_date", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required]
        public object VaccinationDate { get; set; } = new object();

        [Newtonsoft.Json.JsonProperty("country_of_vaccination", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required]
        public object CountryOfVaccination { get; set; } = new object();
    }
}
