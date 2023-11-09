using System;
using System.Collections.Generic;

namespace BTL_Web.Models;

public partial class User
{
    public int UserId { get; set; }

    public string? Email { get; set; }

    public string? Password { get; set; }

    public string? Roles { get; set; }
}
