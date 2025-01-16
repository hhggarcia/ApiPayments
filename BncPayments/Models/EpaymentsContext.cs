using Microsoft.EntityFrameworkCore;

namespace BncPayments.Models
{
    public class EpaymentsContext : DbContext
    {
        public EpaymentsContext(DbContextOptions<EpaymentsContext> options): base(options)
        {
            
        }

        public DbSet<WorkingKey> WorkingKeys { get; set; }
        public DbSet<RequestDb> Requests { get; set; }
        public DbSet<ResponseDb> Responses { get; set; }
    }
}
