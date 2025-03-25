using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace _2280600958_NguyenTranTrungHieu_3.Models
{
    public class ShoppingCart
    {
        [Key]
        public int Id { get; set; } // Thêm ID làm khóa chính

        public string UserId { get; set; } // Liên kết với người dùng

        [ForeignKey("UserId")]
        public ApplicationUser ApplicationUser { get; set; } // Khóa ngoại đến User

        public List<CartItem> Items { get; set; } = new List<CartItem>();

        public void AddItem(CartItem item)
        {
            var existingItem = Items.FirstOrDefault(i => i.ProductId == item.ProductId);
            if (existingItem != null)
            {
                existingItem.Quantity += item.Quantity;
            }
            else
            {
                Items.Add(item);
            }
        }

        public void RemoveItem(int productId)
        {
            Items.RemoveAll(i => i.ProductId == productId);
        }
    }
}
