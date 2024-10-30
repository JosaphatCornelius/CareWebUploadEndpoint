using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CareWebServiceEndpoint.Models
{
    [Table("SysBatchUp")]
    public class SysBatchUpModel
    {
        [Key]
        public string? BatchNo { get; set; }
        public string? ErrMsg { get; set; }
        public string? Status { get; set; }
    }
}
