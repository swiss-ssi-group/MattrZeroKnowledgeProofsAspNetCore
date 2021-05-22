using System.ComponentModel.DataAnnotations;

namespace VaccineVerify.Data
{
    public class VaccinationDataPresentationTemplate
    {
        [Key]
        public int Id { get; set; }
        public string DidId { get; set; }
        public string TemplateId { get; set; }
        public string MattrPresentationTemplateReponse { get; set; }
    }
}
