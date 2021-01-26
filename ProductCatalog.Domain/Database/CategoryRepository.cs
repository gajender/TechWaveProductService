using Microsoft.EntityFrameworkCore;
using ProductCatalog.Domain.Database.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductCatalog.Domain.Database
{
    public class CategoryRepository : IRepository<Category>
    {
        private readonly Context _context;
        public CategoryRepository(Context context)
        {
            _context = context;
        }
        public async Task<List<Category>> Get()
        {
            return await _context.Categories.ToListAsync();
        }

        public async Task<Category> Get(Guid Id)
        {
            return await _context.Categories.FindAsync(Id);
        }

        public List<Category> Find(Func<Category, bool> p)
        {
            return _context.Categories.Where(p).ToList();
        }

        public async Task<Category> Save(Category t)
        {
            var savedObj = _context.Categories.Add(t);

            await _context.SaveChangesAsync();

            return savedObj.Entity;
        }

        public async Task<Category> Update(Category t)
        {
            var existing = await _context.Categories.FindAsync(t.Id);

            existing.Name = t.Name;

            await _context.SaveChangesAsync();

            return existing;
        }

        public bool Delete(Guid Id)
        {
            var existing = _context.Categories.Find(Id);

            _context.Remove(existing);

            return _context.SaveChanges() > 0;
        }
    }
}
