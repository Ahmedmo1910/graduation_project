using Microsoft.AspNetCore.Http;
using Optivio.Shared.DTOs.AI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optivio.ServiceAbstraction.IServices
{
    public interface IAiService
    {
        Task<FaceAnalysisResultDto> AnalyzeAndRecommendAsync(
            Stream imageStream,
            string fileName,
            int pageSize = 10);

        Task<byte[]> TryOnAsync(IFormFile userImage, string glassesUrl);
    }
}
