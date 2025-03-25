using _2280600958_NguyenTranTrungHieu_3.Extensions;
using _2280600958_NguyenTranTrungHieu_3.Models;
using _2280600958_NguyenTranTrungHieu_3.Models.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace _2280600958_NguyenTranTrungHieu_3.Controllers
{
    [Authorize]
    public class ShoppingCartController : Controller
    {
        private readonly IProductRepository _productRepository;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ShoppingCartController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IProductRepository productRepository)
        {
            _productRepository = productRepository;
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> AddToCart(int productId, int quantity)
{
    var product = await _productRepository.GetByIdAsync(productId);
    if (product == null) return NotFound();

    var cart = await GetCartForUser();
    if (cart == null) return Unauthorized();

    var existingItem = cart.Items.FirstOrDefault(i => i.ProductId == productId);
    if (existingItem != null)
    {
        existingItem.Quantity += quantity;
    }
    else
    {
        cart.Items.Add(new CartItem
        {
            ProductId = productId,
            Name = product.Name,
            Price = product.Price,
            Quantity = quantity
        });
    }

    _context.Update(cart);
    await _context.SaveChangesAsync();

    return RedirectToAction("Index");
}


        public async Task<IActionResult> Index()
        {
            var cart = await GetCartForUser();
            return View(cart ?? new ShoppingCart());
        }


        public async Task<IActionResult> RemoveFromCart(int productId)
        {
            var cart = await GetCartForUser();
            if (cart == null) return Unauthorized();

            var item = cart.Items.FirstOrDefault(i => i.ProductId == productId);
            if (item != null)
            {
                cart.Items.Remove(item);
                _context.Update(cart);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index");
        }


        public IActionResult Checkout()
        {
            var cart = HttpContext.Session.GetObjectFromJson<ShoppingCart>("Cart");
            if (cart == null || !cart.Items.Any()) return RedirectToAction("Index");
            ViewBag.TotalPrice = cart.Items.Sum(i => i.Price * i.Quantity);
            return View(new Order());
        }

        [HttpPost]
        public async Task<IActionResult> Checkout(Order order)
        {
            var cart = HttpContext.Session.GetObjectFromJson<ShoppingCart>("Cart");
            if (cart == null || !cart.Items.Any()) return RedirectToAction("Index");

            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            order.UserId = user.Id;
            order.OrderDate = DateTime.UtcNow;
            order.TotalPrice = cart.Items.Sum(i => i.Price * i.Quantity);
            order.OrderDetails = cart.Items.Select(i => new OrderDetail { ProductId = i.ProductId, Quantity = i.Quantity, Price = i.Price }).ToList();

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();
            HttpContext.Session.Remove("Cart");

            return View("OrderCompleted", order.Id);
        }

        public IActionResult OrderCompleted(int orderId)
        {
            var order = _context.Orders.FirstOrDefault(o => o.Id == orderId);
            if (order == null) return NotFound();
            return View(order);
        }
        private async Task<ShoppingCart> GetCartForUser()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return null;

            var cart = _context.ShoppingCarts
                .Include(c => c.Items)
                .FirstOrDefault(c => c.UserId == user.Id);

            if (cart == null)
            {
                cart = new ShoppingCart { UserId = user.Id };
                _context.ShoppingCarts.Add(cart);
                await _context.SaveChangesAsync();
            }

            return cart;
        }


    }
}