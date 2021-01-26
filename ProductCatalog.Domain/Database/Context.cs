using Microsoft.EntityFrameworkCore;
using ProductCatalog.Domain.Database.Entities;

namespace ProductCatalog.Domain.Database
{
    public class Context : DbContext
    {
        public Context(DbContextOptions<Context> options)
            : base(options)
        {

        }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
    }
}
