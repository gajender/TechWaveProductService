using EF = ProductCatalog.Domain.Database.Entities;
using M = ProductCatalog.Domain.Models;

namespace ProductCatalog.Domain.Extensions
{
    public static class DtoMapper
    {
        #region Categories 
        public static M.Category ToDto(this EF.Category category)
        {
            if (category == null)
                return null;

            return new M.Category { 
                 Id = category.Id,
                 Name = category.Name
            };
        }

        public static EF.Category ToDto(this M.Category category)
        {
            if (category == null)
                return null;

            return new EF.Category
            {
                Id = category.Id,
                Name = category.Name
            };
        }
        #endregion

        #region Products
        public static M.Product ToDto(this EF.Product product)
        {
            if (product == null)
                return null;

            return new M.Product
            {
                Id = product.Id,
                CategoryId = product.CategoryId,
                Name = product.Name,
                Description = product.Description,
                Brand = product.Brand,
                Price = product.Price
            };
        }

        public static EF.Product ToDto(this M.Product product)
        {
            if (product == null)
                return null;

            return new EF.Product
            {
                Id = product.Id,
                CategoryId = product.CategoryId,
                Name = product.Name,
                Description = product.Description,
                Brand = product.Brand,
                Price = product.Price
            };
        }
        #endregion
    }
}
