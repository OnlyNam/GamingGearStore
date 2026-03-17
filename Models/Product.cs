using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GamingGearStore.Models
{
    public class Product
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Tên sản phẩm không để trống")]
        public string? Name { get; set; }
        public string? Description { get; set; }
        [Range(0, double.MaxValue, ErrorMessage = "Giá phải >= 0")]
        public decimal Price { get; set; }

        public string? Image { get; set; }

        public int CategoryId { get; set; }
        public Category? Category { get; set; }

        // Upload file
        [NotMapped]
        public IFormFile? ImageFile { get; set; }
    }
}