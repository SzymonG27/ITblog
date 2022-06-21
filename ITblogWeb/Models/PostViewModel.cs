using ITblogWeb.Models.Post;

namespace ITblogWeb.Models
{
    public class PostViewModel
    {
        public ResponsePost? Post { get; set; }
        public IFormFile? Photo { get; set; }
    }
}
