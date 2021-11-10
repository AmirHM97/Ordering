using Cloudware.Microservice.Ordering.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cloudware.Microservice.Ordering.Infrastructure.EntityConfiguration
{
    public class OrderEntityConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.HasKey(ci => ci.Id);
           // builder.HasOne(h => h.Address).WithMany().HasForeignKey(k => k.AddressId);
            builder.HasMany(h => h.OrderItems).WithOne(ci=>ci.Order).HasForeignKey(k => k.OrderId);
        }
    }
}
