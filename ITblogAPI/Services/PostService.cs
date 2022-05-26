using ITblogAPI.Data;
using ITblogAPI.Infrastructure;
using ITblogAPI.Models;
using Microsoft.AspNetCore.Mvc;
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

        public async Task<IEnumerable<Post>> Get([FromQuery] Pagination pagination)
        {
            var posts = dbContext.Posts
                    .OrderByDescending(post => post.CreatedDate);

            var paginationData = new PaginationData(posts.Count(), pagination.Page, pagination.ItemsPerPage);
            //Can serialize here for project
            var items = await posts
                        .Skip((pagination.Page - 1) * pagination.ItemsPerPage)
                        .Take(pagination.ItemsPerPage)
                        .ToListAsync();

            return items;
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

        public async Task<IEnumerable<Post>> Get(string category, [FromQuery] Pagination pagination)
        {
            if (category == null)
            {
                return await dbContext.Posts.ToListAsync();
            }
            var posts = dbContext.Posts
                    .Where(post => post.Category == category)
                    .OrderByDescending(post => post.CreatedDate);

            var paginationData = new PaginationData(posts.Count(), pagination.Page, pagination.ItemsPerPage);
            //Can serialize here for project
            var items = await posts
                        .Skip((pagination.Page - 1) * pagination.ItemsPerPage)
                        .Take(pagination.ItemsPerPage)
                        .ToListAsync();

            return items;
        }

        public async Task Update(Post post)
        {
            dbContext.Entry(post).State = EntityState.Modified;
            await dbContext.SaveChangesAsync();
        }
    }
}
