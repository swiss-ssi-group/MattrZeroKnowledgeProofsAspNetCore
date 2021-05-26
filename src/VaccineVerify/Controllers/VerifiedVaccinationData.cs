using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace VaccineVerify.Controllers
{
    ///{
    ///    "presentationType": "QueryByFrame",
    ///    "challengeId": "RhOtpTa8vNh1EId6sJ7AVD3prerMMDSkfWZrUPzt",
    ///    "claims": {
    ///        "id": "did:key:z6MkmGHPWdKjLqiTydLHvRRdHPNDdUDKDudjiF87RNFjM2fb",
    ///        "http://schema.org/country_of_vaccination": "CH",
    ///        "http://schema.org/date_of_birth": "1953-07-21",
    ///        "http://schema.org/family_name": "Bob",
    ///        "http://schema.org/given_name": "Lammy",
    ///        "http://schema.org/medicinal_product_code": "Pfizer/BioNTech Comirnaty EU/1/20/1528",
    ///        "http://schema.org/number_of_doses": "2",
    ///        "http://schema.org/total_number_of_doses": "2",
    ///        "http://schema.org/vaccination_date": "2021-05-12"
    ///    },
    ///    "verified": true,
    ///    "holder": "did:key:z6MkmGHPWdKjLqiTydLHvRRdHPNDdUDKDudjiF87RNFjM2fb"
    ///}
    public class VerifiedVaccinationData
    {
        [JsonPropertyName("presentationType")]
        public string PresentationType { get; set; }
        [Key]
        [JsonPropertyName("challengeId")]
        public string ChallengeId { get; set; }

        [JsonPropertyName("claims")]
        public VerifiedVaccinationDataClaims Claims { get; set; } = new VerifiedVaccinationDataClaims();

        [JsonPropertyName("verified")]
        public bool Verified { get; set; }
        [JsonPropertyName("holder")]
        public string Holder { get; set; }
    }
}
