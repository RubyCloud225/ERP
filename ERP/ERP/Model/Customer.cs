using System;
using System.ComponentModel.DataAnnotations;

namespace ERP.Model
{
    public class Customer
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        public required string Name { get; set; }
        // Add other properties as needed
    }
}
