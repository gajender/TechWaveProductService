using Microsoft.Extensions.Logging;
using ProductCatalog.Domain.Database;
using ProductCatalog.Domain.Extensions;
using ProductCatalog.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EF = ProductCatalog.Domain.Database.Entities;

namespace ProductCatalog.Domain.Services
{
    public interface IProductService
    {
        Task<List<Product>> Get();
        Task<Product> Get(Guid Id);
        Task<(List<Product>, List<string>)> Products(Guid CategoryId);
        Task<(Product, List<string>)> Save(Product product);
        Task<(Product, List<string>)> Update(Product product);
        bool Delete(Guid Id);
    }

    public class ProductService : IProductService
    {
        private readonly ILogger<ProductService> _logger;
        private IRepository<EF.Product> _productRepository { get; }
        private IRepository<EF.Category> _categoryRepository { get; }
        public ProductService(IRepository<EF.Product> repository, IRepository<EF.Category> catRepository, ILogger<ProductService> logger)
        {
            _logger = logger;
            _productRepository = repository;
            _categoryRepository = catRepository;
        }
        public async Task<List<Product>> Get()
        {
            var lstProducts = await _productRepository.Get();

            return lstProducts.Select(p => p?.ToDto()).ToList();
        }

        public async Task<(List<Product>, List<string>)> Products(Guid CategoryId)
        {
            var msg = string.Empty;
            var lstMessages = new List<string>();

            var lstProducts = new List<Product>();

            var cat = await _categoryRepository.Get(CategoryId);

            if (cat == null)
            {
                msg = string.Format($"There's no category for this Id ({CategoryId})");
                lstMessages.Add(msg);
                _logger.LogError(msg);
            }
            else
            {
                var lstEfProducts = _productRepository.Find(p => p.CategoryId == CategoryId);

                lstProducts = lstEfProducts.Select(p => p?.ToDto()).ToList();

                return (lstProducts, lstMessages);
            }

            return (null, lstMessages);
        }

        public async Task<Product> Get(Guid Id)
        {
            var product = await _productRepository.Get(Id);

            return product?.ToDto();
        }

        public async Task<(Product, List<string>)> Save(Product product)
        {
            var msg = string.Empty;
            var lstMessages = new List<string>();

            if (string.IsNullOrEmpty(product.Name))
            {
                msg = "Product name can't be empty";
                lstMessages.Add(msg);
                _logger.LogError(msg);
            }

            var lstProducts = _productRepository.Find(p => p.Name.Equals(product.Name));
            if (lstProducts != null && lstProducts.Count > 0)
            {
                msg = string.Format($"Another Product Exist with same name {product.Name}, Id: {lstProducts.FirstOrDefault()?.Id}");
                lstMessages.Add(msg);
                _logger.LogError(msg);
            }

            var existingCategory = await _categoryRepository.Get(product.CategoryId);
            if (existingCategory == null)
            {
                msg = string.Format($"Invalid Category Id provided {product.CategoryId}");
                lstMessages.Add(msg);
                _logger.LogError(msg);
            }

            if (product.Price < 0)
            {
                msg = $"Price can not be negative ({product.Price})";
                lstMessages.Add(msg);
                _logger.LogError(msg);
            }

            if (lstMessages.Count > 0)
                return (null, lstMessages);

            var productToSave = product.ToDto();
            productToSave.Id = Guid.NewGuid();
            productToSave.LastModifiedDateTime = new DateTime();

            var savedProduct = await _productRepository.Save(productToSave);
            return (savedProduct?.ToDto(), lstMessages);
        }

        public async Task<(Product, List<string>)> Update(Product product)
        {
            var msg = string.Empty;
            var lstMessages = new List<string>();

            if (string.IsNullOrEmpty(product.Name))
            {
                msg = "Product name can't be made empty";
                lstMessages.Add(msg);
                _logger.LogError(msg);
            }

            var lstProducts = _productRepository.Find(p => p.Name.Equals(product.Name, StringComparison.InvariantCultureIgnoreCase)
                    && p.Id != product.Id);
            if (lstProducts != null && lstProducts.Count > 0)
            {
                msg = string.Format($"Another Product Exist with same name [{product.Name}, Existing Id: {lstProducts.FirstOrDefault()?.Id}]");
                lstMessages.Add(msg);
                _logger.LogError(msg);
            }

            var existingCategory = await _categoryRepository.Get(product.CategoryId);
            if (existingCategory == null)
            {
                msg = string.Format($"Provided new Category Id is invalid {product.CategoryId}");
                lstMessages.Add(msg);
                _logger.LogError(msg);
            }

            if (product.Price < 0)
            {
                msg = $"Price can not be negative ({product.Price})";
                lstMessages.Add(msg);
                _logger.LogError(msg);
            }

            if (lstMessages.Count > 0)
                return (null, lstMessages);

            //TODO: Usually, we should limit the fields that are to be updated
            //      This can be taken call based on transaction history
            //      Otherwise, existing history of transactions will be mapped to this
            //      changed product.
            //      Example: Say product already exist with name "Mouse" and sales track of five thousand units 
            //          and assume, later we changed it to "Gaming monitor" because that mouse no-longer exist
            //          Then, there's a good possibility of mgmt getting false report of "Gaming monitor" of 5k units sold

            var productToUpdate = product.ToDto();
            productToUpdate.LastModifiedDateTime = new DateTime();

            var updatedProduct = await _productRepository.Update(productToUpdate);
            return (updatedProduct?.ToDto(), lstMessages);
        }

        public bool Delete(Guid Id)
        {
            // TODO: If there's transaction history,
            //          this Delete might need to be
            //          re-designed

            if (_productRepository.Get(Id).Result != null)
                return _productRepository.Delete(Id);

            return false;
        }
    }
}
