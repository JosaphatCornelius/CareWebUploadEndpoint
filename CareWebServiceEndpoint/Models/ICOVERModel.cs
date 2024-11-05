using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CareWebServiceEndpoint.Models
{
    [Table("ICOVER")]
    public class ICOVERModel
    {
        [Key]
        public int? ANO { get; set; }
        public int? INO { get; set; }
        public string? TOI { get; set; }
        public string? REMARK { get; set; }
    }
}
