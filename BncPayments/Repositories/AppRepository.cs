using BncPayments.Models;
using BncPayments.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace BncPayments.Repositories
{
    public interface IAppRepository
    {
        Task<string> Create(Application model);
        Task<bool> Delete(Guid id);
        Task<string> Edit(Application model);
        Task<Application> ExistsApp(LoginVM model);
        Task<List<Application>> GetApps();
    }
    public class AppRepository : IAppRepository
    {
        private readonly DbEpaymentsContext _context;

        public AppRepository(DbEpaymentsContext context)
        {
            _context = context;
        }

        // gets users
        public async Task<List<Application>> GetApps() => await _context.Applications.ToListAsync();

        // create app
        public async Task<string> Create(Application model)
        {
            _context.Applications.Add(model);
            await _context.SaveChangesAsync();

            return model.IdApplication;
        }

        // edit user
        public async Task<string> Edit(Application model)
        {
            _context.Applications.Update(model);
            await _context.SaveChangesAsync();

            return model.IdApplication;
        }
        // delete user

        public async Task<bool> Delete(Guid id)
        {
            var app = await _context.Applications.FindAsync(id);

            if (app != null)
            {
                _context.Applications.Remove(app);
                await _context.SaveChangesAsync();
                return true;    
            }

            return false;
        }

        public async Task<Application> ExistsApp(LoginVM model)
        {
            var result = await _context.Applications.FirstOrDefaultAsync(c => c.Name.Equals(model.Username) &&  c.Password.Equals(model.Password));

            return result;
        }
    }
}
