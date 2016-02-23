using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SportsStore.Domain.Abstract;
using SportsStore.Domain.Entities;
using SportStore.WebUI.Controllers;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using SportsStore.WebUI.Controllers;
using SportsStore.WebUI.Models;
using SportsStore.WebUI.HtmlHelpers;

namespace SportsStore.Tests {
    [TestClass]
    public class UnitTest1 {
        [TestMethod]
        public void Can_Paginate() {
            //arrange
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[] {
                new Product {ProductID = 1,  Name = "P1" },
                new Product {ProductID = 2,  Name = "P2" },
                new Product {ProductID = 3,  Name = "P3" },
                new Product {ProductID = 4,  Name = "P4" },
                new Product {ProductID = 5,  Name = "P5" }
            });

            ProductController controller = new ProductController(mock.Object) {PageSize = 3};

            //act
            ProductListViewModel result = (ProductListViewModel)controller.List(null, 2).Model;

            //assert
            Product[] prodArray = result.Products.ToArray();
            Assert.IsTrue(prodArray.Length == 2);
            Assert.AreEqual(prodArray[0].Name, "P4");
            Assert.AreEqual(prodArray[1].Name, "P5");
        }


        [TestMethod]
        public void Can_Generate_Page_Links() {
            //arrange - define an HTML helper - we need to do this
            // in order to apply the extension method
            HtmlHelper myhelper = null;

            PagingInfo pagingInfo = new PagingInfo {
                CurrentPage = 2,
                TotalItems = 28,
                ItemsPerPage = 10
            };

            Func<int, string> pageUrlDelegate = i => "Page" + i;

            //act
            MvcHtmlString result = myhelper.PageLinks(pagingInfo, pageUrlDelegate);

            //assert
            Assert.AreEqual(@"<a class=""btn btn-default"" href=""Page1"">1</a><a class=""btn btn-default btn-primary selected"" href=""Page2"">2</a><a class=""btn btn-default"" href=""Page3"">3</a>", result.ToString());
        }

        [TestMethod]
        public void Can_Send_Pagination_View_Model() {
            //arrange
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[] {
                new Product {ProductID = 1, Name = "P1" },
                new Product {ProductID = 2, Name = "P2" },
                new Product {ProductID = 3, Name = "P3" },
                new Product {ProductID = 4, Name = "P4" },
                new Product {ProductID = 5, Name = "P5" },
            });

            ProductController controller = new ProductController(mock.Object) {PageSize = 3};

            //act
            ProductListViewModel result = (ProductListViewModel)controller.List(null, 2).Model;

            //assert
            PagingInfo pageInfo = result.PagingInfo;
            Assert.AreEqual(pageInfo.CurrentPage, 2);
            Assert.AreEqual(pageInfo.ItemsPerPage, 3);
            Assert.AreEqual(pageInfo.TotalItems, 5);
            Assert.AreEqual(pageInfo.TotalPages, 2);
        }

        [TestMethod]
        public void Can_Filter_Products()
        {
            //arrange
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[]
            {
                new Product() {ProductID = 1, Name = "P1", Category = "Cat1"},
                new Product() {ProductID = 2, Name = "P2", Category = "Cat2"},
                new Product() {ProductID = 3, Name = "P3", Category = "Cat1"},
                new Product() {ProductID = 4, Name = "P4", Category = "Cat2"},
                new Product() {ProductID = 5, Name = "P5", Category = "Cat3"}
            });

            ProductController controller = new ProductController(mock.Object) {PageSize = 3};

            //act
            Product[] result = ((ProductListViewModel) controller.List("Cat2", 1).Model).Products.ToArray();

            //assert
            Assert.AreEqual(result.Length, 2);
            Assert.IsTrue(result[0].Name == "P2" && result[0].Category == "Cat2");
            Assert.IsTrue(result[1].Name == "P4" && result[1].Category == "Cat2");
        }

        [TestMethod]
        public void Can_Create_Categories()
        {
            //arrange
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[]
            {
                new Product() {ProductID = 1, Name = "P1", Category = "Apples"},
                new Product() {ProductID = 2, Name = "P2", Category = "Apples"},
                new Product() {ProductID = 3, Name = "P3", Category = "Plums"},
                new Product() {ProductID = 4, Name = "P4", Category = "Oranges"}
            });

            NavController targetController = new NavController(mock.Object);

            //act
            string[] results = ((IEnumerable<string>) targetController.Menu().Model).ToArray();

            //assert
            Assert.AreEqual(results.Length, 3);
            Assert.AreEqual(results[0], "Apples");
            Assert.AreEqual(results[1], "Oranges");
            Assert.AreEqual(results[2], "Plums");
        }

        [TestMethod]
        public void Indicates_Selected_Category() {
            //arrange
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[]
            {
                new Product() {ProductID = 1, Name = "P1", Category = "Apples"},
                new Product() {ProductID = 4, Name = "P2", Category = "Oranges"}
            });

            NavController targetController = new NavController(mock.Object);

            string categoryToSelect = "Apples";

            //act
            string result = targetController.Menu(categoryToSelect).ViewBag.SelectedCategory;

            //assert
            Assert.AreEqual(categoryToSelect, result);
        }

        [TestMethod]
        public void Can_Generate_Category_Specific_Product_Count() {
            //arrange
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[] {
                new Product {ProductID = 1, Name = "P1", Category = "Cat1"},
                new Product {ProductID = 2, Name = "P2", Category = "Cat2"},
                new Product {ProductID = 3, Name = "P3", Category = "Cat1"},
                new Product {ProductID = 4, Name = "P4", Category = "Cat2"},
                new Product {ProductID = 5, Name = "P5", Category = "Cat3"},
            });

            ProductController targetController = new ProductController(mock.Object) { PageSize = 3 };

            //act
            int result1 = ((ProductListViewModel)targetController.List("Cat1").Model).PagingInfo.TotalItems;
            int result2 = ((ProductListViewModel)targetController.List("Cat2").Model).PagingInfo.TotalItems;
            int result3 = ((ProductListViewModel)targetController.List("Cat3").Model).PagingInfo.TotalItems;
            int result4 = ((ProductListViewModel)targetController.List(null).Model).PagingInfo.TotalItems;

            //assert
            Assert.AreEqual(result1, 2);
            Assert.AreEqual(result2, 2);
            Assert.AreEqual(result3, 1);
            Assert.AreEqual(result4, 5);
        }
    }
}
