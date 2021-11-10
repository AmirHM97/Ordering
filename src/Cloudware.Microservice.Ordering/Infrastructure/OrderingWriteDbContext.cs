using Cloudware.Microservice.Ordering.Infrastructure.EntityConfiguration;
using Cloudware.Microservice.Ordering.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudware.Microservice.Ordering.Infrastructure
{
    public class OrderingWriteDbContext : DbContext, IUnitOfWork
    {
        public OrderingWriteDbContext(DbContextOptions<OrderingWriteDbContext> options):base(options)
        {

        }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
      //  public DbSet<Address> Addresses { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new OrderEntityConfiguration());
        }
    }
}
