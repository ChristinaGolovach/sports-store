using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Linq;
using SportsStore.Domain.Abstract;
using SportsStore.Domain.Entities;
using System.Collections.Generic;
using System.Web.Mvc;
using SportsStore.WebUI.Controllers;
using SportsStore.WebUI.Models;
using SportsStore.WebUI.HtmlHelpers;

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
            ProductsListViewModel result = (ProductsListViewModel)productController.List(null,2).Model;

            //Assert
            Product[] prodArray = result.Products.ToArray();
            Assert.IsTrue(prodArray.Length == 2);
            Assert.AreEqual(prodArray[0].Name, "N4");
            Assert.AreEqual(prodArray[1].Name, "N5");

        }

        
        [TestMethod]
        public void Can_Generate_Page_Links()
        {
            //Arrange
            HtmlHelper myHelper = null;
            PagingInfo pagingInfo = new PagingInfo
            {
                CurrentPage = 2,
                ItemsPerPage = 10,
                TotalItems = 28
            };

            Func<int, string> pageUrlDelegate = i => "Page" + i;

            //ACT
            MvcHtmlString result = myHelper.PageLinks(pagingInfo, pageUrlDelegate);


            //Asert
            Assert.AreEqual(result.ToString(), @"<a href=""Page1"">1</a>"
                                                + @"<a class=""selected"" href=""Page2"">2</a>"
                                                + @"<a href=""Page3"">3</a>");
        }

        [TestMethod]
        public void Can_Send_Pagination_View_Model()
        {
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(p => p.Products).Returns(new List<Product>
            {
                new Product {ProductId = 1, Name = "P1"},
                new Product {ProductId = 2, Name = "P2"},
                new Product {ProductId = 3, Name = "P3"},
                new Product {ProductId = 4, Name = "P4"},
                new Product {ProductId = 5, Name = "P5"}
            }.AsQueryable());

            ProductController controller = new ProductController(mock.Object);
            controller.pageSize = 3;

            //Act
            ProductsListViewModel result = (ProductsListViewModel)controller.List(null,2).Model;

            //Assert
            PagingInfo pagingInfo = result.PgingInfo;
            Assert.AreEqual(pagingInfo.CurrentPage, 2);
            Assert.AreEqual(pagingInfo.ItemsPerPage, 3);
            Assert.AreEqual(pagingInfo.TotalItems, 5);
            Assert.AreEqual(pagingInfo.TotalPages, 2);
        }

        [TestMethod]
        public void Can_Filter_Products()
        {
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(p => p.Products)
                .Returns( new List<Product> {
                    new Product { ProductId= 1, Category ="C1"},
                    new Product { ProductId= 2, Category ="C2"},
                    new Product { ProductId= 3, Category ="C1"},
                    new Product { ProductId= 4, Category ="C4"}
                }.AsQueryable());


            ProductController controller = new ProductController(mock.Object);

            //Act
            Product[] result = ((ProductsListViewModel)controller.List("C1", 1).Model).Products.ToArray();
             
            //Assert
            Assert.AreEqual(result.Length, 2);
            Assert.IsTrue(result[0].ProductId == 1 && result[0].Category == "C1");
            Assert.IsTrue(result[1].ProductId == 3 && result[1].Category == "C1");
        }

        [TestMethod]
        public void Can_Create_Categories()
        {
            //Arange
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(p => p.Products).Returns(new List<Product> {
                new Product {ProductId =1, Category="B"},
                new Product {ProductId =2, Category="A"},
                new Product {ProductId =3, Category="F"},
                new Product {ProductId =4, Category="C"},
                new Product {ProductId =5, Category="D"},
                new Product {ProductId =6, Category="A"}
            }.AsQueryable());

            NavController navController = new NavController(mock.Object);

            //Act
            string[] categories = ((IEnumerable<string>)navController.Menu().Model).ToArray();

            //Assert
            Assert.AreEqual(categories.Length, 5);
            Assert.AreEqual(categories[0], "A");
        }


        [TestMethod]
        public void Indicates_Selected_Category()
        {
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(p => p.Products).Returns(new List<Product>
            {
                new Product { ProductId = 1 , Category ="Apple"},
                new Product { ProductId = 2 , Category ="Orange"},
                new Product { ProductId = 3 , Category ="Banana"},
                new Product { ProductId = 4 , Category ="Apple"},
            }.AsQueryable());

            NavController navController = new NavController(mock.Object);
            string categoryToSelect = "Apple";

            string result = navController.Menu(categoryToSelect).ViewBag.SelectedCategory;

            Assert.AreEqual(categoryToSelect, result);
        }

        [TestMethod]
        public void Generate_Category_Specific_Product_Count()
        {
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(p => p.Products).Returns(new List<Product>
            {
                new Product { ProductId = 1 , Category ="Cat1"},
                new Product { ProductId = 2 , Category ="Cat2"},
                new Product { ProductId = 3 , Category ="Cat1"},
                new Product { ProductId = 4 , Category ="Cat3"},
                new Product { ProductId = 5 , Category ="Cat1"},
                new Product { ProductId = 6 , Category ="Cat3"},
            }.AsQueryable());

            ProductController controller = new ProductController(mock.Object);
            controller.pageSize = 2;

            string category1 = "Cat1";
            int result1= ((ProductsListViewModel)controller.List(category1).Model).PgingInfo.TotalPages;

            string category2 = "Cat2";
            int result2 = ((ProductsListViewModel)controller.List(category2).Model).PgingInfo.TotalPages;

            string categoryAll = "null";
            int resultAll = ((ProductsListViewModel)controller.List(categoryAll).Model).PgingInfo.TotalPages;


            Assert.AreEqual(result1, 2);
            Assert.AreEqual(result2, 1);
            Assert.AreEqual(resultAll, 0); //I do not know why they get all products in this case?

        }
    }
}
