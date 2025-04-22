using System;
using System.Collections.Generic;

namespace Store.Models;

public partial class ProductReview
{
    public int Id { get; set; }

    public int ProductId { get; set; }

    public string UserId { get; set; } = null!;

    public int Rating { get; set; }

    public string? Title { get; set; }

    public string? Comment { get; set; }

    public DateTime ReviewDate { get; set; }

    public virtual Product Product { get; set; } = null!;

    public virtual AspNetUser User { get; set; } = null!;
}
