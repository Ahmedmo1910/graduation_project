using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Optivio.Domin.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optivio.Persistence.Data.Configurations.ProductConfig
{
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            
            builder.ToTable("Products");

            
            builder.HasKey(p => p.Id);

            
            builder.Property(p => p.Name)
                   .IsRequired()
                   .HasMaxLength(200);

            builder.Property(p => p.Description)
                   .HasMaxLength(1000);

            builder.Property(p => p.Color)
                   .HasMaxLength(50);

            builder.Property(p => p.Currency)
                   .IsRequired()
                   .HasMaxLength(10);

            builder.Property(p => p.Price)
                   .HasPrecision(18, 2)
                   .IsRequired();

            builder.Property(p => p.StockQuantity)
                   .IsRequired();

            builder.Property(p => p.ThumbnailUrl)
                   .HasMaxLength(500);

            builder.Property(p => p.MediaUrl)
                   .HasMaxLength(500);

            builder.Property(p => p.IsActive)
                   .IsRequired();

            
            builder.Property(p => p.Gender)
                   .HasConversion<int>()
                   .IsRequired();

            builder.Property(p => p.LensType)
                   .HasConversion<int>()
                   .IsRequired();

            

            // Product -> ProductCategory
            builder.HasOne(p => p.ProductCategories)
                   .WithMany(c => c.Products)
                   .HasForeignKey(p => p.CategoryId)
                   .OnDelete(DeleteBehavior.Restrict);

            // Product -> ProductPrand
            builder.HasOne(p => p.ProductBrands)
                   .WithMany(b => b.Products)
                   .HasForeignKey(p => p.BrandId)
                   .OnDelete(DeleteBehavior.Restrict);

            // Product -> Reviews
            builder.HasMany(p => p.Reviews)
                   .WithOne(r => r.Product)
                   .HasForeignKey(r => r.ProductId)
                   .OnDelete(DeleteBehavior.Restrict);

            // Product -> OrderItems
            builder.HasMany(p => p.OrderItems)
                   .WithOne(oi => oi.Product)
                   .HasForeignKey(oi => oi.ProductId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
