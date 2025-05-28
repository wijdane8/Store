// File: Store/Models/Wishlist.cs
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Store.Models
{
    public class Wishlist
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } = null!;

        
        [ForeignKey("UserId")]
        public virtual ApplicationUser User { get; set; } = null!; 
       

        [Required]
        public int ProductId { get; set; }
        [ForeignKey("ProductId")]
        public virtual Product Product { get; set; } = null!;

        [Column(TypeName = "datetime")]
        public DateTime AddedDate { get; set; } = DateTime.Now; 
    }
}