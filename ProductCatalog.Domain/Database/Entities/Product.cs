using System;

namespace ProductCatalog.Domain.Database.Entities
{
    public class Product
    {
        public Guid Id { get; set; }
        public Guid CategoryId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Brand { get; set; }
        public float Price { get; set; }
        public DateTime LastModifiedDateTime { get; set; }
    }
}
