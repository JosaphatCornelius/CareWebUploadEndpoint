using CareWebServiceEndpoint.Models.Database.Models;
using Microsoft.EntityFrameworkCore;

namespace CareWebServiceEndpoint.Models.Database.Context
{
    public class SEAWEBContext : DbContext
    {
        public SEAWEBContext(DbContextOptions<SEAWEBContext> options) : base(options) { }
        public DbSet<SysBatchOriginalUpModel> CatalogSysBatchOriginalUP { get; set; }
        public DbSet<ICOVERModel> CatalogICOVER { get; set; }
        public DbSet<ProfileModel> CatalogProfile { get; set; }
    }
}
