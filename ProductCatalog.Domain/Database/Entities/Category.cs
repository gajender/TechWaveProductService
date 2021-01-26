using System;
using System.ComponentModel.DataAnnotations;

namespace ProductCatalog.Domain.Database.Entities
{
    public class Category
    {
        public Guid Id { get; set; }
        [Required]
        public string Name { get; set; }
        public DateTime LastModifiedDateTime { get; set; }
    }
}
