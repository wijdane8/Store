using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Store.Models
{
    public class ProductReview
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ProductId { get; set; }
        [ForeignKey("ProductId")]
        public virtual Product Product { get; set; } = null!;

        [Required]
        public string UserId { get; set; } = null!;

        [ForeignKey("UserId")]
        public virtual ApplicationUser User { get; set; } = null!; 

        [Range(1, 5)]
        public int Rating { get; set; }

        [StringLength(255)]
        public string Title { get; set; } = null!;

        [StringLength(1000)]
        public string Comment { get; set; } = null!;

        [Column(TypeName = "datetime")]
        public DateTime ReviewDate { get; set; }
    }
}