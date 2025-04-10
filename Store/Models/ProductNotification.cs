using System;
using System.Collections.Generic;

namespace Store.Models;

public partial class ProductNotification
{
    public int Id { get; set; }

    public int ProductId { get; set; }

    public string Email { get; set; } = null!;

    public DateTime NotificationDate { get; set; }

    public bool IsNotified { get; set; }

    public virtual Product Product { get; set; } = null!;
}
