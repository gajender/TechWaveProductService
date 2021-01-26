using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using ProductCatalog.API.Controllers;
using ProductCatalog.Domain.Models;
using ProductCatalog.Domain.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace ProductCatalog.Test
{
    public class ProductsControllerTests
    {
        ProductsController _controller;

        [Fact]
        public void GetAllReturnsList()
        {
            var expected = new List<Product>() {
                new Product { Id = Guid.NewGuid(), Name = "Product-1" },
                new Product { Id = Guid.NewGuid(), Name = "Product-2" },
                new Product { Id = Guid.NewGuid(), Name = "Product-3" }
            };

            var productService = new Mock<IProductService>();
            productService.Setup(bal => bal.Get()).Returns(Task.FromResult(expected));

            var logger = new Mock<ILogger<ProductsController>>();

            _controller = new ProductsController(productService.Object, logger.Object);

            var actual = _controller.Get().Result;

            Assert.Equal(expected.Count, actual.Count());

            var expOne = expected.FirstOrDefault();
            var actOne = actual.FirstOrDefault(a => a.Id == expOne.Id);

            Assert.NotNull(actOne);
        }


        [Fact]
        public void GetOneReturnsItem()
        {
            var expected = new Product { Id = Guid.NewGuid(), Name = "Product-1" };

            var productService = new Mock<IProductService>();
            productService.Setup(bal => bal.Get(expected.Id)).Returns(Task.FromResult(expected));

            var logger = new Mock<ILogger<ProductsController>>();

            _controller = new ProductsController(productService.Object, logger.Object);

            var iaResult = _controller.Get(expected.Id).Result as OkObjectResult;

            Assert.NotNull(iaResult);
            Assert.Equal((int)HttpStatusCode.OK, iaResult.StatusCode);

            var actual = iaResult.Value as Product;
            Assert.Equal(expected.Id, actual.Id);
            Assert.Equal(expected.Name, actual.Name);
        }

        [Fact]
        public void GetOneReturnsNotFound()
        {
            var expected = new Product { Id = Guid.NewGuid(), Name = "Product-1" };

            var productService = new Mock<IProductService>();
            productService.Setup(bal => bal.Get(expected.Id)).Returns(Task.FromResult(expected));

            var logger = new Mock<ILogger<ProductsController>>();

            _controller = new ProductsController(productService.Object, logger.Object);

            var iaResult = _controller.Get(Guid.Empty).Result as NotFoundObjectResult;

            Assert.NotNull(iaResult);
            Assert.Equal((int)HttpStatusCode.NotFound, iaResult.StatusCode);
            Assert.Contains(Guid.Empty.ToString(), iaResult.Value as string);
        }

        [Fact]
        public void PostProductSaveSuccess()
        {
            var newCatItem = new Product { Id = Guid.NewGuid(), Name = "Product-1" };
            var expected = (newCatItem, new List<string>());

            var productService = new Mock<IProductService>();
            productService.Setup(bal => bal.Save(newCatItem)).Returns(Task.FromResult(expected));

            var logger = new Mock<ILogger<ProductsController>>();

            _controller = new ProductsController(productService.Object, logger.Object);

            var iaResult = _controller.Post(newCatItem).Result as OkObjectResult;

            Assert.NotNull(iaResult);
            Assert.Equal((int)HttpStatusCode.OK, iaResult.StatusCode);

            var actual = iaResult.Value as Product;
            Assert.Equal(expected.newCatItem.Id, actual.Id);
            Assert.Equal(expected.newCatItem.Name, actual.Name);
        }


        [Fact]
        public void PostProductSaveFailed()
        {
            Product newProdItem = null;
            var expected = (newProdItem, new List<string>() { "Save Failed ..." });

            var productService = new Mock<IProductService>();
            productService.Setup(bal => bal.Save(new Product { Name = "xyz" })).Returns(Task.FromResult(expected));

            var logger = new Mock<ILogger<ProductsController>>();

            _controller = new ProductsController(productService.Object, logger.Object);

            var iaResult = _controller.Post(new Product { }).Result as BadRequestObjectResult;

            Assert.NotNull(iaResult);
            Assert.Equal((int)HttpStatusCode.BadRequest, iaResult.StatusCode);
            Assert.Null(iaResult.Value);
        }
    }
}
