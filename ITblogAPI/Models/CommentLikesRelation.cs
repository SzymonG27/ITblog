using System.ComponentModel.DataAnnotations;

namespace ITblogAPI.Models
{
    public class CommentLikesRelation
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int CommentId { get; set; }

        [Required]
        public string? UserId { get; set; }
    }
}
