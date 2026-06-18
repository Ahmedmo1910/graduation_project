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
    public class ProductFaceShapeConfiguration : IEntityTypeConfiguration<ProductFaceShape>
    {
        public void Configure(EntityTypeBuilder<ProductFaceShape> builder)
        {
            builder.HasKey(x => new { x.ProductId, x.FaceShape });

            builder.Property(x => x.FaceShape)
                .IsRequired();

            builder.HasOne(x => x.Product)
                .WithMany(p => p.SuitableFaceShapes)
                .HasForeignKey(x => x.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.ToTable("ProductFaceShapes");
        }
    }
}
