using System.Collections.Generic;
using System.Linq;
using GamingGearStore.Data;
using GamingGearStore.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace GamingGearStore.Controllers
{
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CartController(ApplicationDbContext context)
        {
            _context = context;
        }

        // 🛒 Lấy giỏ hàng từ Session
        private List<CartItem> GetCart()
        {
            var session = HttpContext.Session.GetString("Cart");

            if (!string.IsNullOrEmpty(session))
            {
                return JsonConvert.DeserializeObject<List<CartItem>>(session) ?? new List<CartItem>();
            }

            return new List<CartItem>();
        }

        // 💾 Lưu giỏ hàng
        private void SaveCart(List<CartItem> cart)
        {
            HttpContext.Session.SetString("Cart", JsonConvert.SerializeObject(cart));
        }

        // 👀 Xem giỏ hàng
        public IActionResult Index()
        {
            var cart = GetCart();
            ViewBag.Total = cart.Sum(p => p.Price * p.Quantity);
            return View(cart);
        }

        // ➕ Thêm vào giỏ
        public IActionResult AddToCart(int id)
        {
            var product = _context.Products.Find(id);

            if (product == null)
            {
                return NotFound();
            }

            var cart = GetCart();
            var item = cart.FirstOrDefault(p => p.ProductId == id);

            if (item == null)
            {
                cart.Add(new CartItem
                {
                    ProductId = product.Id,
                    ProductName = product.Name ?? "Không tên",
                    Price = product.Price,
                    Quantity = 1,
                    Image = product.Image
                });
            }
            else
            {
                item.Quantity++;
            }

            SaveCart(cart);
            return RedirectToAction("Index");
        }

        // ➖ Giảm số lượng
        public IActionResult Decrease(int id)
        {
            var cart = GetCart();
            var item = cart.FirstOrDefault(p => p.ProductId == id);

            if (item != null)
            {
                item.Quantity--;

                if (item.Quantity <= 0)
                {
                    cart.Remove(item);
                }
            }

            SaveCart(cart);
            return RedirectToAction("Index");
        }

        // 🔄 Cập nhật số lượng
        public IActionResult Update(int id, int quantity)
        {
            var cart = GetCart();
            var item = cart.FirstOrDefault(p => p.ProductId == id);

            if (item != null)
            {
                if (quantity <= 0)
                {
                    cart.Remove(item);
                }
                else
                {
                    item.Quantity = quantity;
                }
            }

            SaveCart(cart);
            return RedirectToAction("Index");
        }

        // ❌ Xóa sản phẩm
        public IActionResult Remove(int id)
        {
            var cart = GetCart();
            cart.RemoveAll(p => p.ProductId == id);

            SaveCart(cart);
            return RedirectToAction("Index");
        }

        // 🧹 Xóa toàn bộ giỏ
        public IActionResult Clear()
        {
            SaveCart(new List<CartItem>());
            return RedirectToAction("Index");
        }
    }
}