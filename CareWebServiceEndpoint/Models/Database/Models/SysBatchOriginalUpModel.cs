using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CareWebServiceEndpoint.Models.Database.Models
{
    [Table("SysBatchOriginalUp")]
    public class SysBatchOriginalUpModel
    {
        [Key]
        public string? BatchNo { get; set; }
        public string? ErrMsg { get; set; }
        public string? Status { get; set; }
        public string? RefNO { get; set; }
    }
}
