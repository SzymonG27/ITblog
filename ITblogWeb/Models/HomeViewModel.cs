using ITblogWeb.Models.Post;

namespace ITblogWeb.Models
{
    public class HomeViewModel
    {
        public IEnumerable<ResponsePost>? ResponsePosts { get; set; }
        public int? PostId { get; set; }
    }
}
