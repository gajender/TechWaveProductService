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
    public class ProductsController : ControllerBase
    {
        private readonly ILogger<ProductsController> _logger;
        private readonly IProductService _productService;
        public ProductsController(IProductService productService, ILogger<ProductsController> logger)
        {
            _productService = productService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IEnumerable<Product>> Get()
        {
            return await _productService.Get();
        }

        [Route("Category/{CategoryId}")]
        [HttpGet]
        public async Task<IActionResult> Category(Guid CategoryId)
        {
            var products = await _productService.Products(CategoryId);

            if (products.Item1 != null)
                return Ok(products.Item1);

            return BadRequest(products.Item2);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var product = await _productService.Get(id);
            if (product != null)
                return Ok(product);

            _logger.LogWarning("No Product found for {id}", id);
            return NotFound($"No Product found for {id}");
        }

        [HttpPost]
        public async Task<IActionResult> Post(Product newProduct)
        {
            var savedProduct = await _productService.Save(newProduct);
            if (savedProduct.Item1 != null)
                return Ok(savedProduct.Item1);

            _logger.LogError("Save Failed {newProduct}", newProduct);
            return BadRequest(savedProduct.Item2);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(Guid id, Product product)
        {
            var updatedProduct = await _productService.Update(product);
            if (updatedProduct.Item1 != null)
                return Ok(updatedProduct.Item1);

            return BadRequest(updatedProduct.Item2);
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(Guid Id)
        {
            if (_productService.Delete(Id))
                return Ok();

            return BadRequest();
        }
    }
}
