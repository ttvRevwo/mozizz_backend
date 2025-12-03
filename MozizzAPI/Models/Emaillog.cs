using System;
using System.Collections.Generic;

namespace MozizzAPI.Models;

public partial class Emaillog
{
    public int EmailLogId { get; set; }

    public int UserId { get; set; }

    public string? EmailType { get; set; }

    public string? Subject { get; set; }

    public string? Body { get; set; }

    public DateTime SentAt { get; set; }

    public string? Status { get; set; }

    public virtual User User { get; set; } = null!;
}
