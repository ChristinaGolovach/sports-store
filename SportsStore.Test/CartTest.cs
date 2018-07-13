using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using SportsStore.Domain.Entities;
using SportsStore.Domain.Abstract;
using SportsStore.WebUI.Controllers;
using SportsStore.WebUI.Models;
using Moq;
using System.Collections.Generic;
using System.Web.Mvc;

namespace SportsStore.Test
{
    [TestClass]
    public class CartTest
    {
        [TestMethod]
        public void Can_Add_New_Lines()
        {
            Product p1 = new Product { ProductId = 1, Name = "P1" };
            Product p2 = new Product { ProductId = 2, Name = "P2" };

            Cart cart = new Cart();

            cart.AddItem(p1, 2);
            cart.AddItem(p2, 3);

            CartLine[] products = cart.CartLines.ToArray();

            Assert.AreEqual(cart.CartLines.Count(), 2);
            Assert.AreEqual(products[0].Product.Name, "P1");            
        }

        [TestMethod]
        public void Can_Add_Quantity_For_Existing_Lines()
        {
            Product p1 = new Product { ProductId = 1, Name = "P1" };
            Product p2 = new Product { ProductId = 2, Name = "P2" };

            Cart cart = new Cart();

            cart.AddItem(p1, 1);            
            cart.AddItem(p2, 2);
            cart.AddItem(p1, 3);

            CartLine[] products = cart.CartLines.ToArray();

            Assert.AreEqual(products.Length, 2);
            Assert.AreEqual(products[0].Quantity, 4);           
        }

        [TestMethod]
        public void Can_Remove_Line()
        {
            Product p1 = new Product { ProductId = 1, Name = "P1" };
            Product p2 = new Product { ProductId = 2, Name = "P2" };

            Cart cart = new Cart();

            cart.AddItem(p1, 1);
            cart.AddItem(p2, 2);
            cart.AddItem(p1, 3);

            cart.RemoveLine(p1);

            Assert.AreEqual(cart.CartLines.Count(), 1);
            Assert.AreEqual(cart.CartLines.Where(p => p.Product == p1).Count(), 0);
        }

        [TestMethod]
        public void Calculate_Cart_Total()
        {
            Product p1 = new Product { ProductId = 1, Name = "P1", Price=10 };
            Product p2 = new Product { ProductId = 2, Name = "P2", Price=5 };

            Cart cart = new Cart();

            cart.AddItem(p1, 1);
            cart.AddItem(p2, 2);
            cart.AddItem(p1, 3);

            Assert.AreEqual(cart.CalculateTotalSum(), 50);
        }

        [TestMethod]
        public void Calculate_Clear()
        {
            Product p1 = new Product { ProductId = 1, Name = "P1", Price = 10 };
            Product p2 = new Product { ProductId = 2, Name = "P2", Price = 5 };

            Cart cart = new Cart();

            cart.AddItem(p1, 1);
            cart.AddItem(p2, 2);
            cart.AddItem(p1, 3);

            cart.Clear();

            Assert.AreEqual(cart.CartLines.Count(), 0);
        }

        [TestMethod]
        public void Can_Add_To_Cart()
        {
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(p => p.Products).Returns(new List<Product>
            {
                new Product {ProductId = 1, Name = "P1", Category = "Apples"},
                new Product {ProductId = 2, Name = "P1", Category = "Apples"}


            }.AsQueryable());

            Cart cart = new Cart();

            CartController controller = new CartController(mock.Object, null);
            controller.AddToCart(cart, 3, null);
            controller.AddToCart(cart, 1, null);            

            Assert.AreEqual(cart.CartLines.Count(), 1);
            Assert.AreEqual(cart.CartLines.ToArray()[0].Product.Name, "P1");
        }

        [TestMethod]
        public void Adding_Product_To_Cart_Goes_To_Cart_Screen()
        {
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(p => p.Products).Returns(new List<Product>
            {
                new Product {ProductId = 1, Name = "P1", Category = "Apples"}
            }.AsQueryable());

            Cart cart = new Cart();

            CartController controller = new CartController(mock.Object, null);

            RedirectToRouteResult result =  controller.AddToCart(cart, 2, "myUrl");

            Assert.AreEqual(cart.CartLines.Count(), 0);
            Assert.AreEqual(result.RouteValues["action"], "Index");
            Assert.AreEqual(result.RouteValues["returnUrl"], "myUrl");
        }

        [TestMethod]
        public void Can_View_Cart()
        {
            Cart cart = new Cart();
            CartController controller = new CartController(null, null);
            CartIndexViewModel result = (CartIndexViewModel)controller.Index(cart, "myUrl").ViewData.Model;

            Assert.AreEqual(result.Cart, cart);
            Assert.AreEqual(result.ReturnUrl, "myUrl");
            
        }
        
        [TestMethod]
        public void Cannot_Checkout_Empty_Cart()
        {
            //Arrange
            Mock<IOrderProcessor> mock = new Mock<IOrderProcessor>();

            Cart cart = new Cart();
            ShippingDetails details = new ShippingDetails();
            CartController target = new CartController(null, mock.Object);

            //Act
            ViewResult result = target.Checkout(cart, details);

            //Assert
            mock.Verify(m => m.ProceesOrder(It.IsAny<Cart>(),It.IsAny<ShippingDetails>()), Times.Never);
            Assert.AreEqual("", result.ViewName);
            Assert.AreEqual(false, result.ViewData.ModelState.IsValid);
           
       
        }
    }
}
