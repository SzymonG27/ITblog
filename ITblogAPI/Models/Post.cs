using System.ComponentModel.DataAnnotations;

namespace ITblogAPI.Models
{
    public class Post
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string? AuthorId { get; set; }

        [Required]
        public string? Category { get; set; }

        [Required]
        [MaxLength(100)]
        public string? Title { get; set; }

        [Required]
        public string? Content { get; set; }

        public DateTime CreatedDate { get; set; }

        public string? ImageName { get; set; }

        public int Likes { get; set; }

    }
}
