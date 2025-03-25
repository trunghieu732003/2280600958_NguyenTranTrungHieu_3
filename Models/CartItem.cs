using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace _2280600958_NguyenTranTrungHieu_3.Models
{
    public class CartItem
    {
        [Key]
        public int Id { get; set; } // Khóa chính

        public int ShoppingCartId { get; set; } // Khóa ngoại đến ShoppingCart

        [ForeignKey("ShoppingCartId")]
        public ShoppingCart ShoppingCart { get; set; }

        public int ProductId { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
    }
}
