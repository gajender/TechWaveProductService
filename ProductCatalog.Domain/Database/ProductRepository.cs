using Microsoft.EntityFrameworkCore;
using ProductCatalog.Domain.Database.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductCatalog.Domain.Database
{
    public class ProductRepository : IRepository<Product>
    {
        private readonly Context _context;
        public ProductRepository(Context context)
        {
            _context = context;
        }

        public async Task<List<Product>> Get()
        {
            return await _context.Products.ToListAsync();
        }

        public async Task<Product> Get(Guid Id)
        {
            return await _context.Products.FindAsync(Id);
        }

        public List<Product> Find(Func<Product, bool> p)
        {
            return _context.Products.Where(p).ToList();
        }

        public async Task<Product> Save(Product t)
        {
            var savedObj = _context.Products.Add(t);

            await _context.SaveChangesAsync();

            return savedObj.Entity;
        }

        public async Task<Product> Update(Product product)
        {
            var existingProduct = await _context.Products.FindAsync(product.Id);

            existingProduct.Name = product.Name;
            existingProduct.Description = product.Description;
            existingProduct.Brand = product.Brand;
            existingProduct.CategoryId = product.CategoryId;
            existingProduct.Price = product.Price;

            await _context.SaveChangesAsync();

            return existingProduct;
        }
        public bool Delete(Guid Id)
        {
            var existing = _context.Products.Find(Id);

            _context.Remove(existing);

            return _context.SaveChanges() > 0;
        }
    }
}
