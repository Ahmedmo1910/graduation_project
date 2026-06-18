using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optivio.Shared.DTOs.Review
{
    public class CreateReviewDto
    {
        public int Rating { get; set; }
        public string Title { get; set; } = null!;
        public string Body { get; set; } = null!;
        public int ProductId { get; set; }
        public int OrderId { get; set; }
    }
}
