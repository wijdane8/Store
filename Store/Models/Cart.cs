using System;
using System.Collections.Generic;

namespace Store.Models;

public partial class Cart
{
    public int Id { get; set; }

    public string UserId { get; set; } = null!;

    public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();

    public virtual AspNetUser User { get; set; } = null!;
}
