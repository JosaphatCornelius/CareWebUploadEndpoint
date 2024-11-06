using Microsoft.EntityFrameworkCore;

namespace CareWebServiceEndpoint.Models
{
    public class SEAWEBContext : DbContext
    {
        public SEAWEBContext(DbContextOptions<SEAWEBContext> options) : base(options) { }
        public DbSet<SysBatchOriginalUpModel> CatalogSysBatchOriginalUP { get; set; }
        public DbSet<ICOVERModel> CatalogICOVER { get; set; }
    }
}
