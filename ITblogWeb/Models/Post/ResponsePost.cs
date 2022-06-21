using System.ComponentModel.DataAnnotations;

namespace ITblogWeb.Models.Post
{
    public class ResponsePost
    {
        [Key]
        public int Id { get; set; }

        public string? AuthorId { get; set; }

        [Required]
        public string? Category { get; set; }

        [Required]
        [MaxLength(50)]
        public string? Title { get; set; }

        [Required]
        public string? Content { get; set; }

        public DateTime CreatedDate { get; set; }

        public string? ImageName { get; set; }

        public int Likes { get; set; }
    }
}
