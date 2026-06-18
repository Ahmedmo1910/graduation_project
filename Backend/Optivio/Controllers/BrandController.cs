using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Optivio.Domin.Models;
using Optivio.ServiceAbstraction.IServices;
using Optivio.ServiceAbstraction.IUnitOfWork;
using Optivio.Shared.DTOs.Product;

namespace Optivio.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BrandController : ControllerBase
    {
        private readonly IBrandRepository _brandRepository;
        private readonly IUnitOfWork _unitOfWork;

        public BrandController(IBrandRepository brandRepository, IUnitOfWork unitOfWork)
        {
            _brandRepository = brandRepository;
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var brands = await _brandRepository.GetAllAsync();
            var result = brands.Select(b => new BrandDto { Id = b.Id, Name = b.Name });
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var brand = await _brandRepository.GetByIdAsync(id);
            if (brand == null)
                return NotFound(new { message = "Brand not found" });
            return Ok(new BrandDto { Id = brand.Id, Name = brand.Name });
        }
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(BrandDto dto)
        {
            try
            {
                var brand = new ProductBrand { Name = dto.Name };
                await _brandRepository.AddAsync(brand);
                await _unitOfWork.SaveChangesAsync();
                return Ok(new BrandDto { Id = brand.Id, Name = brand.Name });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, BrandDto dto)
        {
            try
            {
                var brand = await _brandRepository.GetByIdAsync(id);
                if (brand == null)
                    return NotFound(new { message = "Brand not found" });

                brand.Name = dto.Name;
                await _brandRepository.UpdateAsync(brand);
                await _unitOfWork.SaveChangesAsync();
                return Ok(new BrandDto { Id = brand.Id, Name = brand.Name });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}