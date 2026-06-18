using Optivio.Shared.DTOs.Product;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optivio.Shared.DTOs.AI
{
    public class FaceAnalysisResultDto
    {
        public string FaceShape { get; set; } = null!;
        public int TotalResults { get; set; }
        public IEnumerable<ProductDto> RecommendedProducts { get; set; } = new List<ProductDto>();
    }
}
