using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Linq;
using SportsStore.Domain.Abstract;
using SportsStore.Domain.Entities;
using System.Collections.Generic;
using SportsStore.WebUI.Controllers;

namespace SportsStore.UnitTests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void Can_Paginate()
        {
            //Arange
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup((p => p.Products))
                .Returns(new List<Product> {
                    new Product { ProductId=1, Name="N1"},
                    new Product { ProductId=2, Name="N2"},
                    new Product { ProductId=3, Name="N3"},
                    new Product { ProductId=4, Name="N4"},
                    new Product { ProductId=5, Name="N5"}
                }.AsQueryable());

            ProductController productController = new ProductController(mock.Object);

            productController.pageSize = 3;

            //Act
            IEnumerable<Product> result = (IEnumerable<Product>)productController.List(2).Model;

            //Assert
            Product[] prodArray = result.ToArray();
            Assert.IsTrue(prodArray.Length == 2);
            Assert.AreEqual(prodArray[0].Name, "P4");
            Assert.AreEqual(prodArray[1].Name, "P5");


        }

        
    }
}
