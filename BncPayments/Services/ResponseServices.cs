using BncPayments.Models;

namespace BncPayments.Services
{
    public interface IResponseServices
    {

    }
    public class ResponseServices : IResponseServices
    {
        private readonly EpaymentsContext _dbContext;

        public ResponseServices(EpaymentsContext context)
        {
            _dbContext = context;
        }

        public async Task<long> Create(ResponseDb model)
        {
            _dbContext.Responses.Add(model);
            return await _dbContext.SaveChangesAsync();
        }
    }
}
