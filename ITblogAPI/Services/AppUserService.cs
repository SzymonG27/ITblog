using ITblogAPI.Data;
using ITblogAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace ITblogAPI.Services
{
    public class AppUserService : IAppUserService
    {
        private readonly AppIdentityDbContext dbContext;

        public AppUserService(AppIdentityDbContext _dbContext)
        {
            dbContext = _dbContext;
        }

        public async Task Delete(string id)
        {
            var userToDelete = await dbContext.Users.FindAsync(id);
            if (userToDelete != null)
            {
                dbContext.Users.Remove(userToDelete);
                await dbContext.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<AppUser>> Get()
        {
            var users = await dbContext.Users.ToListAsync();
            return users;
        }

        public async Task<AppUser> Get(string id)
        {
            var user = await dbContext.Users.FindAsync(id);
            if (user != null)
            {
                return user;
            }
            return null!;
        }

        public async Task<AppUser> CheckUser(string name, string mail) //NOT RESTful, will be changed
        {
            var isUserNameExist = await dbContext.Users.FirstOrDefaultAsync(n => n.UserName == name);
            if (isUserNameExist != null)
            {
                return new AppUser { 
                    UserName = name
                };
            }
            var isMailExist = await dbContext.Users.FirstOrDefaultAsync(n => n.Email == mail);
            if (isMailExist != null)
            {
                return new AppUser
                {
                    Email = mail
                };
            }
            return null!;
        }

        public async Task Update(AppUser model)
        {
            dbContext.Entry(model).State = EntityState.Modified;
            await dbContext.SaveChangesAsync();
        }
    }
}
