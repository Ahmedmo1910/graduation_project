using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Optivio.Domin.Models;
using Optivio.ServiceAbstraction.IServices;
using Optivio.Shared.DTOs.AI;
using Optivio.Shared.DTOs.Product;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;

namespace Optivio.Services.Services
{
    public class AiService : IAiService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly IProductRepository _productRepository;

        public AiService(
            HttpClient httpClient,
            IConfiguration configuration,
            IProductRepository productRepository)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _productRepository = productRepository;
        }

        public async Task<FaceAnalysisResultDto> AnalyzeAndRecommendAsync(
            Stream imageStream,
            string fileName,
            int pageSize = 10)
        {
            var faceShape = await CallAiServiceAsync(imageStream, fileName);

            if (faceShape == null)
                return new FaceAnalysisResultDto
                {
                    FaceShape = "No face detected in the image.",
                    TotalResults = 0,
                    RecommendedProducts = Enumerable.Empty<ProductDto>()
                };

            var products = await _productRepository.GetByFaceShapeAsync(faceShape, pageSize);

            if (!products.Any())
                products = await _productRepository.GetTopRatedAsync(pageSize);

            return new FaceAnalysisResultDto
            {
                FaceShape = faceShape,
                TotalResults = products.Count(),
                RecommendedProducts = products.Select(MapToDto)
            };
        }

        private async Task<string?> CallAiServiceAsync(Stream imageStream, string fileName)
        {
            var aiServiceUrl = _configuration["AiService:Url"];

            if (string.IsNullOrEmpty(aiServiceUrl))
                throw new Exception("AI Service is not configured.");

            using var content = new MultipartFormDataContent();
            content.Add(new StreamContent(imageStream), "image", fileName);

            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(60));

            HttpResponseMessage response;

            try
            {
                response = await _httpClient.PostAsync(
                    $"{aiServiceUrl}/predict",
                    content,
                    cts.Token
                );
            }
            catch (TaskCanceledException)
            {
                throw new Exception("AI Service timed out. Please try again.");
            }
            catch (HttpRequestException)
            {
                throw new Exception("AI Service is unavailable. Please try again later.");
            }

            var json = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<JsonElement>(json);

            if (result.TryGetProperty("error", out _))
                return null;

            if (!response.IsSuccessStatusCode)
                throw new Exception("AI Service returned an unexpected error.");

            if (!result.TryGetProperty("face_shape", out var faceShapeProp))
                throw new Exception("AI Service returned an unexpected response.");

            var faceShape = faceShapeProp.GetString();

            if (string.IsNullOrEmpty(faceShape))
                throw new Exception("AI Service returned an empty face shape.");

            return faceShape;
        }

        private static ProductDto MapToDto(Product p) => new()
        {
            Id = p.Id,
            Name = p.Name,
            Description = p.Description,
            Color = p.Color,
            Gender = p.Gender.ToString(),
            Size = p.Size,
            LensType = p.LensType.ToString(),
            Price = p.Price,
            Currency = p.Currency,
            StockQuantity = p.StockQuantity,
            IsActive = p.IsActive,
            ThumbnailUrl = p.ThumbnailUrl,
            MediaUrl = p.MediaUrl,
            BrandName = p.ProductBrands?.Name ?? "",
            CategoryName = p.ProductCategories?.Name ?? "",
            AverageRating = p.Reviews.Any() ? p.Reviews.Average(r => r.Rating) : 0,
            TwoDImageUrl = p.ThumbnailUrl != null
        ? $"https://backendgraduationproject1.runasp.net/api/files/images2d/{Path.GetFileName(p.ThumbnailUrl)}"
        : null
        };

        public async Task<byte[]> TryOnAsync(IFormFile userImage, string glassesUrl)
        {
            var aiServiceUrl = _configuration["AiService:Url"];

            if (string.IsNullOrEmpty(aiServiceUrl))
                throw new Exception("AI Service is not configured.");

            using var content = new MultipartFormDataContent();

            using var stream = userImage.OpenReadStream();
            content.Add(new StreamContent(stream), "face_image", userImage.FileName);
            content.Add(new StringContent(glassesUrl), "glasses_url");

            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(60));

            HttpResponseMessage response;

            try
            {
                response = await _httpClient.PostAsync(
                    $"{"http://51.21.135.52:5000"}/try-on",
                    content,
                    cts.Token
                );
            }
            catch (TaskCanceledException)
            {
                throw new Exception("AI Service timed out. Please try again.");
            }
            catch (HttpRequestException)
            {
                throw new Exception("AI Service is unavailable. Please try again later.");
            }

            if (!response.IsSuccessStatusCode)
                throw new Exception("AI Service returned an unexpected error.");

            return await response.Content.ReadAsByteArrayAsync();
        }
    }
}