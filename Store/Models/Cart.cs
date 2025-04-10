using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace Store.Models;

public partial class Cart
{
    public int Id { get; set; }

    public string UserId { get; set; } = null!;

    [ForeignKey("UserId")] // Explicitly specify the foreign key property
    public virtual IdentityUser User { get; set; } // Navigation property to IdentityUser

    public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
}