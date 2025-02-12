using BncPayments.Utils;
using System;
using System.Collections.Generic;

namespace BncPayments.Models;

public partial class RequestDb
{
    public long Id { get; set; }

    public Methods Method { get; set; }

    public string Url { get; set; } = null!;

    public DateTime Timestamp { get; set; }

    public string RequestBody { get; set; } = null!;

    public long IdWorkingKey { get; set; }

    public virtual ICollection<AppRquest> AppRquests { get; set; } = new List<AppRquest>();

    public virtual WorkingKey IdWorkingKeyNavigation { get; set; } = null!;

    public virtual ICollection<ResponseDb> Responses { get; set; } = new List<ResponseDb>();
}
