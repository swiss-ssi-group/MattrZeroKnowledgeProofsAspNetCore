using System.Text.Json.Serialization;

namespace VaccineVerify.Controllers
{
    /// <summary>
    /// This class totally depends on the OIDC credential issuer claims
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
    /// </summary>
    public class VerifiedVaccinationDataClaims
    {
        public string Id { get; set; }

        //[JsonPropertyName("http://schema.org/country_of_vaccination")]
        //public string CountryOfVaccination { get; set; }

        [JsonPropertyName("http://schema.org/date_of_birth")]
        public string DateOfBirth { get; set; }

        [JsonPropertyName("http://schema.org/family_name")]
        public string FamilyName { get; set; }

        [JsonPropertyName("http://schema.org/given_name")]
        public string GivenName { get; set; }

        [JsonPropertyName("http://schema.org/medicinal_product_code")]
        public string MedicinalProductCode { get; set; }

        //[JsonPropertyName("http://schema.org/number_of_doses")]
        //public string NumberOfDoses { get; set; }

        //[JsonPropertyName("http://schema.org/total_number_of_doses")]
        //public string TotalNumberOfDoses { get; set; }

        //[JsonPropertyName("http://schema.org/vaccination_date")]
        //public string VaccinationDate { get; set; }
    }
}
