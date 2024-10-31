using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CareWebServiceEndpoint.Models
{
    [Table("UPData")]
    public class UPDataModel
    {
        [Key]
        public int Id { get; set; }
        public string? BatchNo { get; set; }
        public string? ErrMsg { get; set; }
        public string? Status { get; set; }
        public string? RefNO { get; set; }
    }
}
