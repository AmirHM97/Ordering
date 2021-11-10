
using Cloudware.Microservice.Ordering.Infrastructure;
using Cloudware.Utilities.Common.Setting;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Cloudware.Microservice.Product.Infrastructure
{
    public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<OrderingWriteDbContext>
    {
        public OrderingWriteDbContext CreateDbContext(string[] args)
        {

            // IConfigurationRoot configuration = new ConfigurationBuilder()
            //  .SetBasePath(Directory.GetCurrentDirectory())
            //  .AddJsonFile("appsettings.json")
            //  .Build();
            // var connection = configuration.GetSection("ConnectionStrings:MsSql");
            var optionsBuilder = new DbContextOptionsBuilder<OrderingWriteDbContext>();
            optionsBuilder.UseSqlServer();
            return new OrderingWriteDbContext(optionsBuilder.Options);
        }
    }
}

