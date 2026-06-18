using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Optivio.Domin.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optivio.Persistence.Data.Configurations.OrderConfiguration
{
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.ToTable("Orders");

            builder.HasKey(o => o.Id);

            builder.Property(o => o.OrderNumber)
                   .IsRequired()
                   .HasMaxLength(50);

            builder.Property(o => o.Status)
                   .HasConversion<int>()
                   .IsRequired();

            builder.Property(o => o.Currency)
                   .HasMaxLength(10)
                   .IsRequired();

            builder.Property(o => o.TotalsPrice)
                   .HasPrecision(18, 2);

            builder.Property(o => o.Discount)
                   .HasPrecision(18, 2);

            builder.Property(o => o.ShippingCost)
                   .HasPrecision(18, 2);

            builder.HasMany(o => o.OrderItems)
                   .WithOne(oi => oi.Order)
                   .HasForeignKey(oi => oi.OrderId)
                   .OnDelete(DeleteBehavior.Cascade);

            
        }
    }
}
