using System;
using System.ComponentModel.DataAnnotations;

namespace VaccineCredentialsIssuer.Data
{
    public class VaccinationDataCredentials
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public Guid OidcIssuerId { get; set; }
        public string OidcIssuer { get; set; }
        public string Did { get; set; }
    }
}
