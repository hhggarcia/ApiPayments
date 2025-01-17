using BncPayments.Models;

namespace BncPayments.Services
{
    public interface IRequestServices
    {
        Task<long> Create(RequestDb model);
    }
    public class RequestServices : IRequestServices
    {
        private readonly DbEpaymentsContext _dbContext;

        public RequestServices(DbEpaymentsContext context)
        {
            _dbContext = context;
        }

        public async Task<long> Create(RequestDb model)
        {
            try
            {
                _dbContext.Requests.Add(model);
                return await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return 0;
            }
        }
    }
}
