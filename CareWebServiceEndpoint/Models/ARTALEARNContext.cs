using Microsoft.EntityFrameworkCore;

namespace CareWebServiceEndpoint.Models
{
    public class ARTALEARNContext : DbContext
    {
        public ARTALEARNContext(DbContextOptions<ARTALEARNContext> options) : base(options) { }

        public DbSet<UPDataModel> CatalogUPData { get; set; }
    }
}
