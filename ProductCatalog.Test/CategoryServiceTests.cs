using Microsoft.Extensions.Logging;
using Moq;
using ProductCatalog.Domain.Database;
using ProductCatalog.Domain.Extensions;
using ProductCatalog.Domain.Models;
using ProductCatalog.Domain.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using EF = ProductCatalog.Domain.Database.Entities;

namespace ProductCatalog.Test
{
    public class CategoryServiceTests
    {
        [Fact]
        public void UpdateCategoryFailed()
        {
            var expected = new Category { Id = Guid.NewGuid(), Name = "Category Name" };

            var categoryRepository = new Mock<IRepository<EF.Category>>();
            var productRepository = new Mock<IRepository<EF.Product>>();
            var logger = new Mock<ILogger<CategoryService>>();

            var categoryService = new CategoryService(categoryRepository.Object, productRepository.Object, logger.Object);

            var result = categoryService.Update(expected).Result;

            Assert.Null(result.Item1);
            Assert.Contains(result.Item2, e => e.Contains("Invalid Category Id"));
        }

        [Fact]
        public void UpdateCategorySuccess()
        {
            var existingId = Guid.NewGuid();
            var expected = new Category { Id = existingId, Name = "Category New Name" };
            var old = new EF.Category { Id = existingId, Name = "Old Name" };

            var categoryRepository = new Mock<IRepository<EF.Category>>();
            categoryRepository.Setup(c => c.Get(It.IsAny<Guid>())).Returns(Task.FromResult(old));
            categoryRepository.Setup(c => c.Update(It.IsAny<EF.Category>())).Returns(Task.FromResult(expected.ToDto()));

            var productRepository = new Mock<IRepository<EF.Product>>();
            var logger = new Mock<ILogger<CategoryService>>();

            var categoryService = new CategoryService(categoryRepository.Object, productRepository.Object, logger.Object);

            var actual = categoryService.Update(expected).Result;

            Assert.NotNull(actual.Item1);
            Assert.Equal(expected.Id, actual.Item1.Id);
            Assert.NotEqual(old.Name, actual.Item1.Name);
            Assert.Equal(expected.Name, actual.Item1.Name);
        }

        [Fact]
        public void DeleteCategorySuccess()
        {
            var expected = new EF.Category { Id = Guid.NewGuid(), Name = "Category Name" };

            var categoryRepository = new Mock<IRepository<EF.Category>>();
            categoryRepository.Setup(c => c.Get(It.IsAny<Guid>())).Returns(Task.FromResult(expected));
            categoryRepository.Setup(c => c.Delete(It.IsAny<Guid>())).Returns(true);

            var productRepository = new Mock<IRepository<EF.Product>>();
            var logger = new Mock<ILogger<CategoryService>>();

            var categoryService = new CategoryService(categoryRepository.Object, productRepository.Object, logger.Object);

            var actual = categoryService.Delete(expected.Id).Result;

            Assert.True(actual.Item1);
            Assert.True(actual.Item2.Count <= 0);
        }

        [Fact]
        public void DeleteCategoryFailed()
        {
            var categoryRepository = new Mock<IRepository<EF.Category>>();
            var productRepository = new Mock<IRepository<EF.Product>>();
            productRepository.Setup(p => p.Find(It.IsAny<Func<EF.Product, bool>>())).Returns(new List<EF.Product> { new EF.Product { } });
            var logger = new Mock<ILogger<CategoryService>>();

            var categoryService = new CategoryService(categoryRepository.Object, productRepository.Object, logger.Object);

            var actual = categoryService.Delete(Guid.NewGuid()).Result;

            Assert.False(actual.Item1);
            Assert.True(actual.Item2.Count > 0);
        }
    }
}
