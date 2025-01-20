using System;
using System.Collections.Generic;

namespace BncPayments.Models;

public partial class WorkingKey
{
    public long Id { get; set; }

    public string Key { get; set; } = null!;

    public DateTime FechaCreacion { get; set; } = DateTime.Now;

    public DateTime? FechaExpiracion { get; set; }

    public int Version { get; set; }

    public bool Activo { get; set; }

    public virtual ICollection<RequestDb> Requests { get; set; } = new List<RequestDb>();
}
