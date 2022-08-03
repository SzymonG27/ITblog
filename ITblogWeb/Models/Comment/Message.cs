using System.ComponentModel.DataAnnotations;

namespace ITblogWeb.Models.Comment
{
    public class Message
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int PostId { get; set; }

        [Required]
        public string? AuthorId { get; set; }

        [Required]
        public DateTime CreatedDate { get; set; }

        [Required]
        public string? Content { get; set; }

        public int Likes { get; set; }
    }
}
