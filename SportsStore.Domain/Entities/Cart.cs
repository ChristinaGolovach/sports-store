using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace SportsStore.Domain.Entities
{
    //[ModelBinder (typeof(CartModelBinder))]
    // альтерантива global.asax
    public class Cart
    {
        private List<CartLine> lineCollection = new List<CartLine>();

        public IEnumerable<CartLine> CartLines
        {
            get { return lineCollection; }
        }

        public void AddItem (Product product, int quantity)
        {

            CartLine existsCartLine = lineCollection
                                      .Where(cart => cart.Product.ProductId == product.ProductId)
                                      .FirstOrDefault();

            if (existsCartLine == null)
            {
                lineCollection.Add(new CartLine
                {
                    Product = product,
                    Quantity = quantity
                });
            }
            else
            {
                existsCartLine.Quantity += quantity;
            }
        }

        public void RemoveLine(int id)//(Product product)//(CartLine cartLine)
        {
            //lineCollection.Remove(cartLine);
            lineCollection.RemoveAll(line => line.Product.ProductId == id); // product.ProductId);
        }

        public decimal CalculateTotalSum()
        {
            decimal sum = lineCollection.Sum(line =>  line.Product.Price * line.Quantity);
            return sum;
        }

        public void Clear()
        {
            lineCollection.Clear();
        }
    }
}
