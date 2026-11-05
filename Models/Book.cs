using System.ComponentModel.DataAnnotations;

namespace SecureBooksApi.Models
{
    public class Book
    {
        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Author { get; set; } = string.Empty;
    }
}