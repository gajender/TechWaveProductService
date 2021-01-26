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
    public class ProductServiceTests
    {
        [Fact]
        public void UpdateProductFailed()
        {
            EF.Category catNotExist = null;
            var expected = new Product { Id = Guid.NewGuid(), CategoryId = Guid.NewGuid(), Name = "Product Name" };

            var productRepository = new Mock<IRepository<EF.Product>>();
            var categoryRepository = new Mock<IRepository<EF.Category>>();
            categoryRepository.Setup(c => c.Get(It.IsAny<Guid>())).Returns(Task.FromResult(catNotExist));
            var logger = new Mock<ILogger<ProductService>>();

            var productService = new ProductService(productRepository.Object, categoryRepository.Object, logger.Object);

            var result = productService.Update(expected).Result;

            Assert.Null(result.Item1);
            Assert.True(result.Item2.Count > 0);
            Assert.Contains(result.Item2, e => e.Contains("Provided new Category Id is invalid"));
        }

        [Fact]
        public void UpdateProductSuccess()
        {
            var existingId = Guid.NewGuid();
            var expected = new Product { Id = existingId, Name = "Product New Name" };
            var old = new EF.Product { Id = existingId, Name = "Old Name" };

            var productRepository = new Mock<IRepository<EF.Product>>();
            productRepository.Setup(p => p.Get(It.IsAny<Guid>())).Returns(Task.FromResult(old));
            productRepository.Setup(p => p.Update(It.IsAny<EF.Product>())).Returns(Task.FromResult(expected.ToDto()));

            var categoryRepository = new Mock<IRepository<EF.Category>>();
            categoryRepository.Setup(c => c.Get(It.IsAny<Guid>())).Returns(Task.FromResult(new EF.Category { }));

            var logger = new Mock<ILogger<ProductService>>();

            var productService = new ProductService(productRepository.Object, categoryRepository.Object, logger.Object);

            var actual = productService.Update(expected).Result;

            Assert.NotNull(actual.Item1);
            Assert.Equal(expected.Id, actual.Item1.Id);
            Assert.NotEqual(old.Name, actual.Item1.Name);
            Assert.Equal(expected.Name, actual.Item1.Name);
        }

        [Fact]
        public void DeleteProductSuccess()
        {
            var expected = new EF.Product { Id = Guid.NewGuid(), Name = "Product Name" };

            var productRepository = new Mock<IRepository<EF.Product>>();
            productRepository.Setup(p => p.Get(It.IsAny<Guid>())).Returns(Task.FromResult(expected));
            productRepository.Setup(p => p.Delete(It.IsAny<Guid>())).Returns(true);
            var categoryRepository = new Mock<IRepository<EF.Category>>();

            var logger = new Mock<ILogger<ProductService>>();

            var productService = new ProductService(productRepository.Object, categoryRepository.Object, logger.Object);

            var actual = productService.Delete(expected.Id);

            Assert.True(actual);
        }

        [Fact]
        public void DeleteProductFailed()
        {
            var productRepository = new Mock<IRepository<EF.Product>>();
            productRepository.Setup(p => p.Find(It.IsAny<Func<EF.Product, bool>>())).Returns(new List<EF.Product> { new EF.Product { } });
            var categoryRepository = new Mock<IRepository<EF.Category>>();
            var logger = new Mock<ILogger<ProductService>>();

            var productService = new ProductService(productRepository.Object, categoryRepository.Object, logger.Object);

            var actual = productService.Delete(Guid.NewGuid());

            Assert.False(actual);
        }
    }
}
