using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using SportsStore.Domain.Abstract;

namespace SportsStore.WebUI.Controllers
{
    public class NavController : Controller
    {
        private IProductRepository productRepository;

        public NavController(IProductRepository repository)
        {
            productRepository = repository;                
        }

        public PartialViewResult Menu(string category = null)
        {
            ViewBag.SelectedCategory = category;
            IEnumerable<string> categories = productRepository.Products
                                                              .Select(p => p.Category)
                                                              .Distinct()
                                                              .OrderBy(p => p);
            return PartialView(categories);
        }
    }
}