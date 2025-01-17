using BncPayments.Models;

namespace BncPayments.Services
{
    public interface IResponseServices
    {
        Task<long> Create(ResponseDb model);
    }
    public class ResponseServices : IResponseServices
    {
        private readonly DbEpaymentsContext _dbContext;

        public ResponseServices(DbEpaymentsContext context)
        {
            _dbContext = context;
        }

        public async Task<long> Create(ResponseDb model)
        {
            try
            {
                _dbContext.Responses.Add(model);
                return await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return 0;
            }            
        }
    }
}
