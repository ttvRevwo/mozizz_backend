using System;
using System.Collections.Generic;

namespace MozizzAPI.Models;

public partial class UserVerification
{
    public int Id { get; set; }

    public string Email { get; set; } = null!;

    public string Code { get; set; } = null!;

    public DateTime ExpiresAt { get; set; }
}
