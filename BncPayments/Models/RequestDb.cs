using BncPayments.Utils;
using System.ComponentModel.DataAnnotations;

namespace BncPayments.Models
{
    public class RequestDb
    {
        [Key]
        public long Id { get; set; }
        [Required]
        public Methods Method { get; set; }
        [Required]
        public string? Url { get; set; }
        [Required]
        [DataType(DataType.DateTime)]
        public DateTime Timestamp { get; set; } = DateTime.Now;
        [Required]
        [MaxLength]
        public string? RequestBody { get; set; }

        // Relación con WorkingKey
        public long WorkingKeyId { get; set; }
        public virtual WorkingKey WorkingKey { get; set; } = null!;
    }
}
