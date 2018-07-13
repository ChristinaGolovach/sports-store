using System.Linq;
using System.Web.Mvc;
using SportsStore.Domain.Abstract;
using SportsStore.Domain.Entities;
using SportsStore.WebUI.Models;

namespace SportsStore.WebUI.Controllers
{
    public class CartController : Controller
    {
        IProductRepository productRepository;
        IOrderProcessor orderProcessor;

        public CartController(IProductRepository repository, IOrderProcessor processor)
        {
            productRepository = repository;
            orderProcessor = processor;
        }

        public RedirectToRouteResult AddToCart(Cart cart, int productId, string returnUrl)
        {           
            Product product = productRepository.Products.FirstOrDefault(p => p.ProductId == productId);
            if (product != null)
                cart.AddItem(product, 1);
            return RedirectToAction("Index", new { returnUrl });
        }
            
        public RedirectToRouteResult RemoveFromCart(Cart cart,int productId, string returnUrl)
        {
            Product product = productRepository.Products.FirstOrDefault(p => p.ProductId == productId);
            if (product !=null)
                cart.RemoveLine(product.ProductId);
            return RedirectToAction("Index", new { returnUrl });
        }       


        public ViewResult Index(Cart cart, string returnUrl)
        {
            CartIndexViewModel cartViewModel = new CartIndexViewModel
            {
                Cart = cart,
                ReturnUrl = returnUrl
            };
            return View(cartViewModel);
        }

        public PartialViewResult Summary (Cart cart)
        {
            return PartialView(cart);
        }

        public ViewResult Checkout()
        {
            return View(new ShippingDetails());
        }

        [HttpPost]
        public ViewResult Checkout(Cart cart, ShippingDetails shippingDetails)
        {
            if (cart.CartLines.Count() == 0)
            {
                ModelState.AddModelError("", "Sorry, your cart is empty");
            }
            if (ModelState.IsValid)
            {
                orderProcessor.ProceesOrder(cart, shippingDetails);
                cart.Clear();
                return View("Completed");
            }
            else
            {
                return View(shippingDetails);
            }
            
        }
    }
}