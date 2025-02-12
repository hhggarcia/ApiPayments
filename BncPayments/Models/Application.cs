using System;
using System.Collections.Generic;

namespace BncPayments.Models;

public partial class Application
{
    public string IdApplication { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string Name { get; set; } = null!;

    public virtual ICollection<AppRquest> AppRquests { get; set; } = new List<AppRquest>();
}
