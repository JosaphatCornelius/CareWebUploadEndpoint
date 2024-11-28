using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CareWebServiceEndpoint.Models.Database.Models
{
    [Table("Profile")]
    public class ProfileModel
    {
        [Key]
        public string? ID { get; set; }
        public string? RefID { get; set; }
        public string? Name { get; set; }
        public string? Address_1 { get; set; }
    }
}
