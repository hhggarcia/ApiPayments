using BncPayments.Models;

namespace BncPayments.Services
{
    public interface IRequestServices
    {
        Task<long> Create(RequestDb model);
    }
    public class RequestServices : IRequestServices
    {
        private readonly EpaymentsContext _dbContext;

        public RequestServices(EpaymentsContext context)
        {
            _dbContext = context;
        }

        public async Task<long> Create(RequestDb model)
        {
            _dbContext.Requests.Add(model);
            return await _dbContext.SaveChangesAsync();
        }
    }
}
