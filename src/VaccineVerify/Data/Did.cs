using System.ComponentModel.DataAnnotations;

namespace VaccineVerify.Data
{
    public class Did
    {
        public int Id { get; set; }
        public string DidId { get; set; }
        public string DidTypeId { get; set; }
        public string DidData { get; set; }
        public string Name { get; set; }
    }
}
