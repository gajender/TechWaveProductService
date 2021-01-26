using System;
using System.ComponentModel.DataAnnotations;

namespace ProductCatalog.Domain.Models
{
    public class Product
    {
        public Guid Id { get; set; }
        [Required]
        public Guid CategoryId { get; set; }
        [Required]
        public string Name { get; set; }
        public string Description { get; set; }
        public string Brand { get; set; }
        public float Price { get; set; }
    }
}
