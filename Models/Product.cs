using System;
using System.Collections.Generic;

namespace BTL_Web.Models;

public partial class Product
{
    public int ProductId { get; set; }

    public string? ProductName { get; set; }

    public double? ProductPrice { get; set; }

    public string? ProductDes { get; set; }

    public string? ProductImg { get; set; }
}
