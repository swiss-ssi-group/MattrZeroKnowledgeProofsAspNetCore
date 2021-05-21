using System.Text.Json.Serialization;

namespace VaccineVerify.Controllers
{
    /// <summary>
    /// This class totally depends on the OIDC credential issuer claims
    /// </summary>
    public class VerifiedVaccinationDataClaims
    {
        public string Id { get; set; }

        [JsonPropertyName("http://schema.org/name")]
        public string Name { get; set; }

        [JsonPropertyName("http://schema.org/firstName")]
        public string FirstName { get; set; }

        [JsonPropertyName("http://schema.org/licenseType")]
        public string LicenseType { get; set; }

        [JsonPropertyName("http://schema.org/dateOfBirth")]
        public string DateOfBirth { get; set; }

        [JsonPropertyName("http://schema.org/licenseIssuedAt")]
        public string LicenseIssuedAt { get; set; }
    }
}
