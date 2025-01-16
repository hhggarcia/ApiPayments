using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace BncPayments.Models
{
    public class WorkingKey
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [Required]
        [MaxLength]
        public string? Key { get; set; }

        [Required]
        [DataType(DataType.DateTime)]

        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        [Required]
        [DataType(DataType.DateTime)]

        public DateTime? FechaExpiracion { get; set; }

        [Required]
        public int Version { get; set; } = 0;
        
        public bool Activo { get; set; }

        public virtual ICollection<RequestDb> Requests { get; set; } = new List<RequestDb>();

    }
}
