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
    public interface ICategoryService
    {
        Task<List<Category>> Get();
        Task<Category> Get(Guid Id);
        Task<(Category, List<string>)> Save(Category category);
        Task<(Category, List<string>)> Update(Category category);
        Task<(bool, List<string>)> Delete(Guid Id);
    }

    public class CategoryService : ICategoryService
    {
        private readonly ILogger<CategoryService> _logger;
        private readonly IRepository<EF.Category> _categoryRepository;
        private readonly IRepository<EF.Product> _productRepository;
        public CategoryService(IRepository<EF.Category> categoryRepository, IRepository<EF.Product> productRepository, ILogger<CategoryService> logger)
        {
            _categoryRepository = categoryRepository;
            _productRepository = productRepository;
            _logger = logger;
        }
        public async Task<List<Category>> Get()
        {
            var lstCategories = await _categoryRepository.Get();

            return lstCategories.Select(c => c?.ToDto()).ToList();
        }

        public async Task<Category> Get(Guid Id)
        {
            var category = await _categoryRepository.Get(Id);

            return category?.ToDto();
        }

        public async Task<(Category, List<string>)> Save(Category category)
        {
            var msg = string.Empty;
            var lstMessages = new List<string>();

            if (string.IsNullOrEmpty(category.Name))
            {
                msg = "Category Name can't be empty";
                lstMessages.Add(msg);
                _logger.LogError(msg);
            }

            var lstCategories = _categoryRepository.Find(c => c.Name.Equals(category.Name));
            if (lstCategories != null && lstCategories.Count > 0)
            {
                msg = string.Format($"Another category Exist with same name {category.Name}, Id: {lstCategories.FirstOrDefault().Id}");
                _logger.LogWarning(msg);
                lstMessages.Add(msg);
            }

            if (lstMessages.Count > 0)
                return (null, lstMessages);

            var categoryToSave = category.ToDto();
            categoryToSave.Id = Guid.NewGuid();
            categoryToSave.LastModifiedDateTime = new DateTime();

            var savedCategory = await _categoryRepository.Save(categoryToSave);

            return (savedCategory?.ToDto(), lstMessages);
        }

        public async Task<(Category, List<string>)> Update(Category category)
        {
            var msg = string.Empty;
            var lstMessages = new List<string>();

            if (string.IsNullOrEmpty(category.Name))
            {
                msg = "Category Name can't be empty";
                lstMessages.Add(msg);
                _logger.LogError(msg);
            }

            var existing = await _categoryRepository.Get(category.Id);
            if (existing == null)
            {
                msg = string.Format($"Invalid Category Id ({category.Id}) provided");
                lstMessages.Add(msg);
                _logger.LogError(msg);
            }

            var lstCategories = _categoryRepository.Find(c => c.Name.Equals(category.Name, StringComparison.InvariantCultureIgnoreCase)
                    && c.Id != category.Id);
            if (lstCategories != null && lstCategories.Count > 0)
            {
                msg = string.Format($"Another category already exist with new name [{category.Name}, Existing Id: {lstCategories.FirstOrDefault().Id}]");
                _logger.LogWarning(msg);
                lstMessages.Add(msg);
            }

            var lstProducts = _productRepository.Find(p => p.CategoryId == category.Id);
            if (lstProducts != null && lstProducts.Count > 0)
            {
                msg = $"Can't update as One (or) more product(s) are mapped to this Category Id ({category.Id} - Product: {lstProducts.FirstOrDefault()?.Name})";
                lstMessages.Add(msg);
                _logger.LogError(msg);
            }

            if (lstMessages.Count > 0)
                return (null, lstMessages);

            var categoryToUpdate = category.ToDto();
            categoryToUpdate.LastModifiedDateTime = new DateTime();

            var updatedCategory = await _categoryRepository.Update(categoryToUpdate);
            return (updatedCategory?.ToDto(), lstMessages);
        }

        public async Task<(bool, List<string>)> Delete(Guid Id)
        {
            var msg = string.Empty;
            var lstMessages = new List<string>();

            var catgory = await _categoryRepository.Get(Id);
            if (catgory == null)
                lstMessages.Add($"Invalid Category Id {Id}");

            var lstProducts = _productRepository.Find(p => p.CategoryId == Id);
            if (lstProducts != null && lstProducts.Count > 0)
            {
                msg = $"Can't Delete as One (or) more product(s) are mapped to this Category Id ({Id} - Product: {lstProducts.FirstOrDefault()?.Name})";
                lstMessages.Add(msg);
                _logger.LogError(msg);
            }

            if (lstMessages.Count > 0)
                return (false, lstMessages);

            var bDeleted = _categoryRepository.Delete(Id);

            return await Task.FromResult((bDeleted, lstMessages));
        }
    }
}
