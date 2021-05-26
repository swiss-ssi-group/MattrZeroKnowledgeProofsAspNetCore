using System.ComponentModel.DataAnnotations;

namespace VaccineVerify.Data
{
    public class VerifiedVaccinationsData
    {
        [Key]
        public string ChallengeId { get; set; }
        public string PresentationType { get; set; }
        public string ClaimsId { get; set; }
        public string CountryOfVaccination { get; set; }
        public string DateOfBirth { get; set; }
        public string FamilyName { get; set; }
        public string GivenName { get; set; }
        public string MedicinalProductCode { get; set; }
        public string NumberOfDoses { get; set; }
        public string TotalNumberOfDoses { get; set; }
        public string VaccinationDate { get; set; }
        public bool Verified { get; set; }
        public string Holder { get; set; }
    }
}
