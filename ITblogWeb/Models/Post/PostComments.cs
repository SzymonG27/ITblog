using ITblogWeb.Models.Comment;
using System.ComponentModel.DataAnnotations;

namespace ITblogWeb.Models.Post
{
    public class PostComments
    {
        public ResponsePost? ResponsePost { get; set; }
        public IEnumerable<Message>? Messages { get; set; }
        public int MessageId { get; set; }

        [MinLength(5, ErrorMessage = "Musisz wpisać minimum 5 znaków aby móc opublikować komentarz!")]
        [MaxLength(150, ErrorMessage = "Przekroczyłeś liczbę dozwolonych znaków dla komentarza (max 150)")]
        public string? CommentToAdd { get; set; }
    }
}
