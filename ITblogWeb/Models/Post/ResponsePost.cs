namespace ITblogWeb.Models.Post
{
    public class ResponsePost
    {
        public int id { get; set; }
        public string? authorId { get; set; }
        public string? category { get; set; }
        public string? title { get; set; }
        public string? content { get; set; }
        public DateTime createdDate { get; set; }
        public string? imageName { get; set; }
        public int likes { get; set; }
    }
}
