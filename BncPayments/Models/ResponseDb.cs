using System.ComponentModel.DataAnnotations;

namespace BncPayments.Models
{
    public class ResponseDb
    {
        [Key]
        public long Id { get; set; }
        public long RequestId { get; set; }
        public virtual RequestDb Request { get; set; }
        [Required]
        [StringLength(50)]
        public string? StatusCode { get; set; }
        [Required]
        public string? ResponseBody { get; set; }
        [Required]
        [DataType(DataType.DateTime)]
        public DateTime Timestamp { get; set; } = DateTime.Now;
    }
}
