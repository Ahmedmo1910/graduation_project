using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optivio.Domin.Models
{
    public class ProductCategory : BaseEntity<int>
    {
        [Required]
        [MaxLength(150)]
        public string Name { get; set; } = null!;

        #region RelationShips
        public ICollection<Product> Products { get; set; } = new List<Product>();
        #endregion
    }
}
