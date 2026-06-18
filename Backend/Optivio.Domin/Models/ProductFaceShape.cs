using Optivio.Domin.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optivio.Domin.Models
{
    public class ProductFaceShape
    {
        public int ProductId { get; set; }
        public Product Product { get; set; } = null!;

        public FaceShape FaceShape { get; set; }
    }
}
