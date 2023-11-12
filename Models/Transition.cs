using System;
using System.Collections.Generic;

namespace BTL_Web.Models;

public partial class Transition
{
    public int TransitionId { get; set; }

    public string? Diachi { get; set; }

    public string? Sodienthoai { get; set; }

    public string? Name { get; set; }

    public int? OrderId { get; set; }

    public int? UserId { get; set; }

    public virtual Order? Order { get; set; }
}
