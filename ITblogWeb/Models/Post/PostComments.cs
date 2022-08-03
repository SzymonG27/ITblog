using ITblogWeb.Models.Comment;

namespace ITblogWeb.Models.Post
{
    public class PostComments
    {
        public ResponsePost? ResponsePost { get; set; }
        public IEnumerable<Message>? Messages { get; set; }
    }
}
