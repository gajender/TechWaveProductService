using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ProductCatalog.Domain.Models;
using ProductCatalog.Domain.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProductCatalog.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly ILogger<CategoriesController> _logger;
        private readonly ICategoryService _categoryService;
        public CategoriesController(ICategoryService categoryService, ILogger<CategoriesController> logger)
        {
            _categoryService = categoryService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IEnumerable<Category>> Get()
        {
            return await _categoryService.Get();
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var category = await _categoryService.Get(id);
            if (category != null)
                return Ok(category);

            _logger.LogWarning("No Category item found for {id}", id);
            return NotFound($"No Category item found for {id}");
        }

        [HttpPost]
        public async Task<IActionResult> Post(Category category)
        {
            var savedCategory = await _categoryService.Save(category);
            if (savedCategory.Item1 != null)
                return Ok(savedCategory.Item1);

            _logger.LogError("Save Failed {category}", category);
            return BadRequest(savedCategory.Item2);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(Guid Id, Category category)
        {
            category.Id = Id;
            var updatedCategory = await _categoryService.Update(category);
            if (updatedCategory.Item1 != null)
                return Ok(updatedCategory.Item1);

            return BadRequest(updatedCategory.Item2);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid Id)
        {
            var deleteOperation = await _categoryService.Delete(Id);

            if (deleteOperation.Item1)
                return Ok();

            return BadRequest(deleteOperation.Item2);
        }
    }
}
