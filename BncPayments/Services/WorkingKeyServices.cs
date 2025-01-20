using BncPayments.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace BncPayments.Services
{
    public class WorkingKeyServices
    {
        private readonly DbEpaymentsContext _dbContext;
        private readonly ILogger<WorkingKeyServices> _logger;

        public WorkingKeyServices(DbEpaymentsContext context, 
            ILogger<WorkingKeyServices> logger)
        {
            _dbContext = context;
            _logger = logger;
        }

        public async Task<string> GetWorkingKey()
        {
            try
            {

                _logger.LogInformation("Getting working key from database.");
                var keyEntity = await _dbContext.WorkingKeys.FirstOrDefaultAsync(c => c.Activo);
                var key = keyEntity?.Key ?? string.Empty;
                return key;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while getting the working key.");
                return string.Empty;
            }
        }

        public async Task<WorkingKey> GetWorkingKeyObject()
        {
            try
            {
                _logger.LogInformation("Getting working key from database.");
                var keyEntity = await _dbContext.WorkingKeys.FirstOrDefaultAsync(c => c.Activo);
                return keyEntity ?? new WorkingKey();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while getting the working key object.");
                return new WorkingKey();
            }
        }

        public async Task<long> CreateWorkingKey(WorkingKey model)
        {
            
            try
            {
                _logger.LogInformation("Creating working key in database.");

                var lastWorking = await _dbContext.WorkingKeys.FirstOrDefaultAsync(c => c.Activo);
                if (lastWorking != null)
                {
                    lastWorking.Activo = false;
                    lastWorking.FechaExpiracion = DateTime.Now;
                    model.Version = lastWorking.Version + 1;

                    _dbContext.WorkingKeys.Update(lastWorking);
                }
                else
                {
                    model.Version = 1;
                }

                model.Activo = true;
                _dbContext.WorkingKeys.Add(model);
                await _dbContext.SaveChangesAsync();
               
                return model.Id;
            }
            catch (Exception ex)
            {
               
                _logger.LogError(ex, "An error occurred while creating the working key.");
                return 0;
            }
        }

        public void SetWorkingKey(string workingKey)
        {
            try
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
                    {
                        Key = workingKey,
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while setting the working key.");
            }
        }
    }
}