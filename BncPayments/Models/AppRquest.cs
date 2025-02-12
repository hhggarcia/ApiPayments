using System;
using System.Collections.Generic;

namespace BncPayments.Models;

public partial class AppRquest
{
    public long Id { get; set; }

    public string IdApp { get; set; } = null!;

    public long IdRequest { get; set; }

    public virtual Application IdAppNavigation { get; set; } = null!;

    public virtual RequestDb IdRequestNavigation { get; set; } = null!;
}
