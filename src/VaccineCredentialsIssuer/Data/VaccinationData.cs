namespace VaccineCredentialsIssuer.Data
{
    public class VaccinationData
    {
        public string FamilyName { get; set; }
        public string GivenName { get; set; }
        public string DateOfBirth { get; set; }
        /// <summary>
        /// https://github.com/admin-ch/CovidCertificate-Examples/blob/main/valuesets/vaccine-medicinal-product.json
        /// </summary>
        public string MedicinalProductCode { get; set; }
        public string NumberOfDoses { get; set; }
        public string TotalNumberOfDoses { get; set; }
        public string VaccinationDate { get; set; }
        public string CountryOfVaccination { get; set; }
    }
}

