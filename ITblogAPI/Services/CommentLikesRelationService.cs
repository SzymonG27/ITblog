using ITblogAPI.Data;
using ITblogAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace ITblogAPI.Services
{
    public class CommentLikesRelationService : ICommentLikesRelationService
    {
        private readonly ApplicationDbContext dbContext;

        public CommentLikesRelationService(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<IEnumerable<CommentLikesRelation>> Get()
        {
            var commentRelations = await dbContext.CommentLikesRelations.ToListAsync();
            return commentRelations;
        }

        public async Task<CommentLikesRelation> Get(int id)
        {
            var commentRelation = await dbContext.CommentLikesRelations.FindAsync(id);
            if (commentRelation != null)
            {
                return commentRelation;
            }
            return null!;
        }

        public async Task<CommentLikesRelation> GetRecord(string userId, int commentId)
        {
            var commentRelation = await dbContext.CommentLikesRelations.FirstOrDefaultAsync(r => r.UserId == userId && r.CommentId == commentId);
            if (commentRelation != null)
            {
                return commentRelation;
            }
            return null!;
        }

        public async Task<CommentLikesRelation> Create(CommentLikesRelation model)
        {
            await dbContext.CommentLikesRelations.AddAsync(model);
            await dbContext.SaveChangesAsync();
            return model;
        }

        public async Task Delete(int rowId)
        {
            var relation = await dbContext.CommentLikesRelations.FirstOrDefaultAsync(r => r.Id == rowId);
            if (relation != null)
            {
                dbContext.CommentLikesRelations.Remove(relation);
                await dbContext.SaveChangesAsync();
            }
        }

        public async Task<bool> IsRelation(CommentLikesRelation model)
        {
            var relation = await dbContext.CommentLikesRelations.FirstOrDefaultAsync(r => r.UserId == model.UserId && r.CommentId == model.CommentId);
            if (relation != null)
            {
                return true;
            }
            return false;
        }
    }
}
