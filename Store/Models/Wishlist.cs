using System;
using System.Collections.Generic;

namespace Store.Models;

public partial class Wishlist
{
    public int Id { get; set; }

    public string UserId { get; set; } = null!;

    public int ProductId { get; set; }

    public DateTime AddedDate { get; set; }

    public virtual Product Product { get; set; } = null!;

    public virtual AspNetUser User { get; set; } = null!;
}
