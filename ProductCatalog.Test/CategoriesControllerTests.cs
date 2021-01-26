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
    public class CategoriesControllerTests
    {
        CategoriesController _controller;

        [Fact]
        public void GetAllReturnsList()
        {
            var expected = new List<Category>() {
                new Category { Id = Guid.NewGuid(), Name = "Category-1" },
                new Category { Id = Guid.NewGuid(), Name = "Category-2" },
                new Category { Id = Guid.NewGuid(), Name = "Category-3" }
            };

            var categoryService = new Mock<ICategoryService>();
            categoryService.Setup(bal => bal.Get()).Returns(Task.FromResult(expected));

            var logger = new Mock<ILogger<CategoriesController>>();

            _controller = new CategoriesController(categoryService.Object, logger.Object);

            var actual = _controller.Get().Result;

            Assert.Equal(expected.Count, actual.Count());

            var expOne = expected.FirstOrDefault();
            var actOne = actual.FirstOrDefault(a => a.Id == expOne.Id);

            Assert.NotNull(actOne);
        }


        [Fact]
        public void GetOneReturnsItem()
        {
            var expected = new Category { Id = Guid.NewGuid(), Name = "Category-1" };

            var categoryService = new Mock<ICategoryService>();
            categoryService.Setup(bal => bal.Get(expected.Id)).Returns(Task.FromResult(expected));

            var logger = new Mock<ILogger<CategoriesController>>();

            _controller = new CategoriesController(categoryService.Object, logger.Object);

            var iaResult = _controller.Get(expected.Id).Result as OkObjectResult;

            Assert.NotNull(iaResult);
            Assert.Equal((int)HttpStatusCode.OK, iaResult.StatusCode);

            var actual = iaResult.Value as Category;
            Assert.Equal(expected.Id, actual.Id);
            Assert.Equal(expected.Name, actual.Name);
        }

        [Fact]
        public void GetOneReturnsNotFound()
        {
            var expected = new Category { Id = Guid.NewGuid(), Name = "Category-1" };

            var categoryService = new Mock<ICategoryService>();
            categoryService.Setup(bal => bal.Get(expected.Id)).Returns(Task.FromResult(expected));

            var logger = new Mock<ILogger<CategoriesController>>();

            _controller = new CategoriesController(categoryService.Object, logger.Object);

            var iaResult = _controller.Get(Guid.Empty).Result as NotFoundObjectResult;

            Assert.NotNull(iaResult);
            Assert.Equal((int)HttpStatusCode.NotFound, iaResult.StatusCode);
            Assert.Contains("No Category item found", iaResult.Value as string);
        }

        [Fact]
        public void PostCategorySaveSuccess()
        {
            var newCatItem = new Category { Id = Guid.NewGuid(), Name = "Category-1" };
            var expected = (newCatItem, new List<string>());

            var categoryService = new Mock<ICategoryService>();
            categoryService.Setup(bal => bal.Save(newCatItem)).Returns(Task.FromResult(expected));

            var logger = new Mock<ILogger<CategoriesController>>();

            _controller = new CategoriesController(categoryService.Object, logger.Object);

            var iaResult = _controller.Post(newCatItem).Result as OkObjectResult;

            Assert.NotNull(iaResult);
            Assert.Equal((int)HttpStatusCode.OK, iaResult.StatusCode);

            var actual = iaResult.Value as Category;
            Assert.Equal(expected.newCatItem.Id, actual.Id);
            Assert.Equal(expected.newCatItem.Name, actual.Name);
        }


        [Fact]
        public void PostCategorySaveFailed()
        {
            Category newCatItem = null;
            var expected = (newCatItem, new List<string>() { "Save Failed ..." });

            var categoryService = new Mock<ICategoryService>();
            categoryService.Setup(bal => bal.Save(new Category { Name = "xyz" })).Returns(Task.FromResult(expected));

            var logger = new Mock<ILogger<CategoriesController>>();

            _controller = new CategoriesController(categoryService.Object, logger.Object);

            var iaResult = _controller.Post(new Category { }).Result as BadRequestObjectResult;

            Assert.NotNull(iaResult);
            Assert.Equal((int)HttpStatusCode.BadRequest, iaResult.StatusCode);
            Assert.Null(iaResult.Value);
        }
    }
}
