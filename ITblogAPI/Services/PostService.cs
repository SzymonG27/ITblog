using ITblogAPI.Data;
using ITblogAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace ITblogAPI.Services
{
    public class PostService : IPostService
    {
        private readonly ApplicationDbContext dbContext;

        public PostService(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }


         public async Task<Post> Create(Post post)
        {
            dbContext.Posts.Add(post);
            await dbContext.SaveChangesAsync();
            return post;
        }

        public async Task Delete(int id)
        {
            var postToDelete = await dbContext.Posts.FindAsync(id);
            if (postToDelete != null)
            {
                dbContext.Posts.Remove(postToDelete);
                await dbContext.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Post>> Get()
        {
            var posts = await dbContext.Posts.ToListAsync();
            return posts;
        }

        public async Task<Post> Get(int id)
        {
            var post = await dbContext.Posts.FindAsync(id);
            if (post != null)
            {
                return post;
            }
            return null!;
        }

        public async Task Update(Post post)
        {
            dbContext.Entry(post).State = EntityState.Modified;
            await dbContext.SaveChangesAsync();
        }
    }
}
