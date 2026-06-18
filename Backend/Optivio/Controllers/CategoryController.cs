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
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CategoryController(ICategoryRepository categoryRepository, IUnitOfWork unitOfWork)
        {
            _categoryRepository = categoryRepository;
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var categories = await _categoryRepository.GetAllAsync();
            var result = categories.Select(c => new CategoryDto { Id = c.Id, Name = c.Name });
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            if (category == null)
                return NotFound(new { message = "Category not found" });
            return Ok(new CategoryDto { Id = category.Id, Name = category.Name });
        }
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(CategoryDto dto)
        {
            try
            {
                var category = new ProductCategory { Name = dto.Name };
                await _categoryRepository.AddAsync(category);
                await _unitOfWork.SaveChangesAsync();
                return Ok(new CategoryDto { Id = category.Id, Name = category.Name });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, CategoryDto dto)
        {
            try
            {
                var category = await _categoryRepository.GetByIdAsync(id);
                if (category == null)
                    return NotFound(new { message = "Category not found" });

                category.Name = dto.Name;
                await _categoryRepository.UpdateAsync(category);
                await _unitOfWork.SaveChangesAsync();
                return Ok(new CategoryDto { Id = category.Id, Name = category.Name });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}