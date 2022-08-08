using System.ComponentModel.DataAnnotations;

namespace ITblogAPI.Models
{
    public class PostLikesRelation
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int PostId { get; set; }

        [Required]
        public string? UserId { get; set; }
    }
}
