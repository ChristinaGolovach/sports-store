using System.Linq;
using System.Web.Mvc;
using SportsStore.Domain.Abstract;
using SportsStore.Domain.Entities;
using SportsStore.WebUI.Models;
using SportsStore.WebUI.HtmlHelpers;

namespace SportsStore.WebUI.Controllers
{
    public class ProductController : Controller
    {
        private IProductRepository productRepository;
        public int pageSize=2;

        public ProductController(IProductRepository repository)
        {
            productRepository = repository;
        }

        public ViewResult List(string category,  int page = 1)
        {
            ProductsListViewModel productViewModel = new ProductsListViewModel
            {
                Products = productRepository.Products
                                            .Where(p => p.Category == null || p.Category == category)
                                            .OrderBy(p => p.ProductId)
                                            .Skip((page - 1) * pageSize)
                                            .Take(pageSize),
                PgingInfo = new PagingInfo
                {
                    CurrentPage = page,
                    ItemsPerPage = pageSize,
                    TotalItems = category == null ? productRepository.Products.Count() : productRepository.Products.Where(p => p.Category == category).Count()
                },

                CurrentCategory = category                
            };

            return View(productViewModel);           
        }
        
        public FileContentResult GetImage(int productId)
        {
            Product product =  productRepository.Products.FirstOrDefault(p => p.ProductId == productId);
            if (product != null)
            {
                return File(product.ImageData, product.ImageMimeType);
            }
            else
            {
                return null;
            }
        }
        
    }
}