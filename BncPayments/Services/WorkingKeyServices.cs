using BncPayments.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace BncPayments.Services
{
    public interface IWorkingKeyServices
    {
        Task<long> CreateWorkingKey(WorkingKey model);
        Task<string> GetWorkingKey();
        Task<WorkingKey> GetWorkingKeyObject();
        void SetWorkingKey(string workingKey);
    }
    public class WorkingKeyServices : IWorkingKeyServices
    {
        private readonly EpaymentsContext _dbContext;
        private readonly ILogger<WorkingKeyServices> _logger;

        public WorkingKeyServices(EpaymentsContext context,
            ILogger<WorkingKeyServices> logger)
        {
            _dbContext = context;
            _logger = logger;
        }

        public async Task<string> GetWorkingKey()
        {
            _logger.LogInformation("Getting working key from database.");

            var keyEntity = await _dbContext.WorkingKeys.FirstOrDefaultAsync(c => c.Activo);
            return keyEntity?.Key ?? string.Empty;
        }

        public async Task<WorkingKey> GetWorkingKeyObject()
        {
            _logger.LogInformation("Getting working key from database.");

            var keyEntity = await _dbContext.WorkingKeys.FirstOrDefaultAsync(c => c.Activo);
            return keyEntity;
        }

        public async Task<long> CreateWorkingKey(WorkingKey model)
        {
            _logger.LogInformation("Create working key from database.");

            var workingsPast = await _dbContext.WorkingKeys.ToListAsync();
            model.Version = 1;
            model.Activo = true;

            if (workingsPast.Any())
            {
                var lastWorking = workingsPast.LastOrDefault();
                if (lastWorking != null) 
                {
                    lastWorking.Activo = false;
                    lastWorking.FechaExpiracion = DateTime.Now;
                    model.Version = lastWorking.Version + 1;

                    _dbContext.WorkingKeys.Update(lastWorking);
                }

                _dbContext.WorkingKeys.Add(model);
                return await _dbContext.SaveChangesAsync();
            }
            _dbContext.WorkingKeys.Add(model);
            return await _dbContext.SaveChangesAsync();
        }

        public void SetWorkingKey(string workingKey)
        {
            if (string.IsNullOrEmpty(workingKey))
            {
                _logger.LogWarning("Attempted to set an empty or null working key.");
                return;
            }

            _logger.LogInformation("Setting working key in database.");

            var keyEntity = _dbContext.WorkingKeys.FirstOrDefault(c => c.Activo);
            if (keyEntity == null)
            {
                keyEntity = new WorkingKey 
                { Key = workingKey, 
                    Version = _dbContext.WorkingKeys.Count() + 1,  
                    Activo = true 
                };

                _dbContext.WorkingKeys.Add(keyEntity);
            }
            else
            {
                keyEntity.Key = workingKey;
            }
            _dbContext.SaveChanges();
        }
    }
}
