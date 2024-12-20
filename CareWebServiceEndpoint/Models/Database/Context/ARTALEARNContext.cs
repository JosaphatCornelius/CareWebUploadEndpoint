﻿using CareWebServiceEndpoint.Models.Database.Models;
using Microsoft.EntityFrameworkCore;

namespace CareWebServiceEndpoint.Models.Database.Context
{
    public class ARTALEARNContext : DbContext
    {
        public ARTALEARNContext(DbContextOptions<ARTALEARNContext> options) : base(options) { }

        public DbSet<UPDataModel> CatalogUPData { get; set; }
    }
}
