using System;
using System.Collections.Generic;

namespace BTL_Web.Models;

public partial class Order
{
    public int OrderId { get; set; }

    public int? UserId { get; set; }

    public int? ProductId { get; set; }

    public string? Status { get; set; }

    public string? Adrees { get; set; }

    public string? Number { get; set; }
}
