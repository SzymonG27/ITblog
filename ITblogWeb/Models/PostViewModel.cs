using ITblogWeb.Models.Post;

namespace ITblogWeb.Models
{
    public class PostViewModel //TODO: photo sender to api
    {
        public ResponsePost? Post { get; set; }
        public IFormFile? Photo { get; set; }
    }
}
