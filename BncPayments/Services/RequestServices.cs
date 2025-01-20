using BncPayments.Models;

namespace BncPayments.Services
{
    public interface IRequestServices
    {
        Task<long> Create(RequestDb model);
        Task<RequestDb> GetRequest(long id);
    }
    public class RequestServices : IRequestServices
    {
        private readonly DbEpaymentsContext _dbContext;

        public RequestServices(DbEpaymentsContext context)
        {
            _dbContext = context;
        }

        public async Task<RequestDb> GetRequest(long id)
        {
            var request = await _dbContext.Requests.FindAsync(id);
            return request;
        }

        public async Task<long> Create(RequestDb model)
        {
            try
            {
                _dbContext.Requests.Add(model);
                await _dbContext.SaveChangesAsync();
                return model.Id;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }
    }
}
