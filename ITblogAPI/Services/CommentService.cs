

using ITblogAPI.Data;
using ITblogAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace ITblogAPI.Services
{
    public class CommentService : ICommentService
    {
        private readonly ApplicationDbContext dbContext;

        public CommentService(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<Comment> Create(Comment model)
        {
            dbContext.Comments.Add(model);
            await dbContext.SaveChangesAsync();

            return model;
        }

        public async Task Delete(int id)
        {
            var CommentToDelete = await dbContext.Comments.FindAsync(id);
            if (CommentToDelete != null)
            {
                dbContext.Comments.Remove(CommentToDelete);
                await dbContext.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Comment>> Get()
        {
            var comments = await dbContext.Comments.ToListAsync();
            return comments;
        }

        public async Task<Comment> Get(int id)
        {
            var comment = await dbContext.Comments.FindAsync(id);
            if (comment != null)
            {
                return comment;
            }
            return null!;
        }

        public async Task<IEnumerable<Comment>> GetFromPostId(int postId)
        {
            var comments = await dbContext.Comments.Where(c => c.PostId == postId).ToListAsync();
            return comments;
        }

        public async Task Update(Comment model)
        {
            dbContext.Entry(model).State = EntityState.Modified;
            await dbContext.SaveChangesAsync();
        }
    }
}
