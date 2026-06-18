using Optivio.Domin.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optivio.Domin.Models
{
    public class Product : BaseEntity<int>
    {
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string Color { get; set; }= null!;
        public Gender Gender { get; set; }
        public string Size { get; set; } = null!;
        public LensType LensType { get; set; }

        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public bool IsActive { get; set; }
        public string Currency { get; set; } = null!;

        public string ThumbnailUrl { get; set; } = null!;
        public string MediaUrl { get; set; } = null!;


        #region RelationShips

         #region One-To-Many RelationShips
         public int BrandId { get; set; }
         public ProductBrand ProductBrands { get; set; } = null!;

         public int CategoryId { get; set; }
         public ProductCategory ProductCategories { get; set; } = null!;
        #endregion

        //public Wishlist Wishlist { get; set; } = null!;

        public ICollection<WishlistItem> WishlistItems { get; set; } = new List<WishlistItem>();

        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
        public ICollection<Review> Reviews { get; set; } = new List<Review>();

        public ICollection<ProductFaceShape> SuitableFaceShapes { get; set; } = new List<ProductFaceShape>();

        #endregion

    }
}
