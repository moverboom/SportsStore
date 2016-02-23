using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SportsStore.Domain.Entities;

namespace SportsStore.Tests {
    [TestClass]
    public class CartTest {
        [TestMethod]
        public void Can_Add_New_Lines() {
            //arrange
            Product p1 = new Product { ProductID = 1, Name = "P1" };
            Product p2 = new Product { ProductID = 2, Name = "P2" };

            Cart targetCart = new Cart();

            //act
            targetCart.AddItem(p1, 1);
            targetCart.AddItem(p2, 1);
            Cart.CartLine[] results = targetCart.Lines.ToArray();

            //assert
            Assert.AreEqual(results.Length, 2);
            Assert.AreEqual(results[0].Product, p1);
            Assert.AreEqual(results[1].Product, p2);
        }

        [TestMethod]
        public void Can_Add_Quantity_For_Existing_Lines() {
            //arrange
            Product p1 = new Product { ProductID = 1, Name = "P1" };
            Product p2 = new Product { ProductID = 2, Name = "P2" };

            Cart targetCart = new Cart();

            //act
            targetCart.AddItem(p1, 1);
            targetCart.AddItem(p2, 1);
            targetCart.AddItem(p1, 10);
            Cart.CartLine[] results = targetCart.Lines.OrderBy(c => c.Product.ProductID).ToArray();

            //assert
            Assert.AreEqual(results.Length, 2);
            Assert.AreEqual(results[0].Quantity, 11);
            Assert.AreEqual(results[1].Quantity, 1);
        }

        [TestMethod]
        public void Can_Remoce_Line() {
            //arrange
            Product p1 = new Product { ProductID = 1, Name = "P1" };
            Product p2 = new Product { ProductID = 2, Name = "P2" };
            Product p3 = new Product { ProductID = 3, Name = "P3" };

            Cart targetCart = new Cart();
            targetCart.AddItem(p1, 1);
            targetCart.AddItem(p2, 3);
            targetCart.AddItem(p3, 5);
            targetCart.AddItem(p2, 1);

            //act
            targetCart.RemoveLine(p2);

            //assert
            Assert.AreEqual(targetCart.Lines.Where(c => c.Product == p2).Count(), 0);
            Assert.AreEqual(targetCart.Lines.Count(), 2);
        }

        [TestMethod]
        public void Calculate_Cart_Total() {
            //arrange
            Product p1 = new Product { ProductID = 1, Name = "P1", Price = 100M };
            Product p2 = new Product { ProductID = 2, Name = "P2", Price = 50M };

            Cart targetCart = new Cart();

            //act
            targetCart.AddItem(p1, 1);
            targetCart.AddItem(p2, 1);
            targetCart.AddItem(p1, 3);
            decimal result = targetCart.ComputeTotalValue();

            //assert
            Assert.AreEqual(result, 450M);
        }

        [TestMethod]
        public void Can_Clear_Contents() {
            //arrange
            Product p1 = new Product { ProductID = 1, Name = "P1", Price = 100M };
            Product p2 = new Product { ProductID = 2, Name = "P2", Price = 50M };

            Cart targetCart = new Cart();

            targetCart.AddItem(p1, 1);
            targetCart.AddItem(p2, 1);

            //act
            targetCart.Clear();

            //asserrt
            Assert.AreEqual(targetCart.Lines.Count(), 0);
        }
    }
}
