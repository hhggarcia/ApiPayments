using System;
using System.Collections.Generic;

namespace BncPayments.Models;

public partial class ResponseDb
{
    public long Id { get; set; }

    public string StatusCode { get; set; } = null!;

    public string ResponseBody { get; set; } = null!;

    public DateTime Timestamp { get; set; }

    public long IdRequest { get; set; }

    public virtual RequestDb IdRequestNavigation { get; set; } = null!;
}
