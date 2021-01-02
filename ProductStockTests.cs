namespace INStock.Tests
{
    using System;
    using System.Linq;

    using INStock.Models;

    using NUnit.Framework;

    [TestFixture]
    public class ProductStockTests
    {
        private Product product;
        private Product product2;
        private Product product3;
        private ProductStock productStock;

        [SetUp]
        public void SetUp()
        {
            product = new Product("Test", 23.5m, 6);
            product2 = new Product("Test2", 7.5m, 11);
            product3 = new Product("Test3", 56.5m, 3);
            productStock = new ProductStock();
        }

        //Add
        [Test]
        public void AddShouldSaveTheProduct()
        {
            var expectedLabel = "Test";
            var expectedPrice = 23.5;
            var expectedQty = 6;
            var expectedCount = 1;

            productStock.Add(product);

            var productToCheck = productStock.FindByLabel("Test");

            Assert.That(productToCheck.Label, Is.EqualTo(expectedLabel));
            Assert.That(productToCheck.Price, Is.EqualTo(expectedPrice));
            Assert.That(productToCheck.Quantity, Is.EqualTo(expectedQty));
            Assert.That(productStock.Count, Is.EqualTo(expectedCount));
        }

        [Test]
        public void AddingTwoIdenticalProductsShouldThrowException()
        {
            productStock.Add(product);

            Assert.Throws<ArgumentException>(() =>
            {
                productStock.Add(product);
            });
        }

        [Test]
        public void AddingTwoDifferentProductsShouldSaveThem()
        {

            var expectedLabelFirst = "Test";
            var expectedLabelSecond = "Test2";
            var expectedCount = 2;


            productStock.Add(product);
            productStock.Add(product2);

            var productToCheck = productStock.FindByLabel("Test");
            var product2ToCheck = productStock.FindByLabel("Test2");

            Assert.That(productToCheck.Label, Is.EqualTo(expectedLabelFirst));
            Assert.That(product2ToCheck.Label, Is.EqualTo(expectedLabelSecond));
            Assert.That(productStock.Count, Is.EqualTo(expectedCount));
        }
        //Remove
        [Test]
        public void RemoveShouldRemoveTheProductIfItExists()
        {
            AddProducts();

            var removed = productStock.Remove(product);

            Assert.That(removed, Is.True);
            Assert.That(productStock.Contains(product), Is.False);
        }
        [Test]
        public void RemovingNotExistingProductShouldReturnFalse()
        {
            AddProducts();

            var removed = productStock.Remove(new Product("Some name",1.0m,100));

            Assert.That(removed, Is.False);
        }
        [Test]
        public void RemovingInvalidProductShouldThrowException()
        {
            Assert.Throws<ArgumentException>(() =>
            {
                productStock.Remove(null);
            });
        }
        //Contains

        [Test]
        public void ContainsShouldReturnTrueIfProductExists()
        {
            AddProducts();

            var containsProduct = productStock.Contains(product);

            Assert.That(containsProduct, Is.True);
        }

        [Test]
        public void ContainsShouldReturnFalseIfProductDoesNotExist()
        {
            productStock.Add(product);

            var containsProduct = productStock.Contains(product2);

            Assert.That(containsProduct, Is.False);
        }

        [Test]
        public void ContainsShouldThrowExceptionInCaseOfInvalidInput()
        {
            Assert.Throws<ArgumentException>(() =>
            {
                productStock.Contains(null);
            });
        }

        //Count
        [Test]
        public void CountShouldReturnTheNumberofProducts()
        {
            var expectedCount = 3;

            AddProducts();

            Assert.AreEqual(expectedCount, this.productStock.Count);
        }

        //Find - IT IS ONE BASED
        [Test]
        public void FindShouldReturnTheRightProduct()
        {
            var expectedLabel = "Test2";
            var expectedPrice = 7.5;
            var expectedQty = 11;

            AddProducts();

            var productToCheck = productStock.Find(1);

            Assert.That(productToCheck.Label, Is.EqualTo(expectedLabel));
            Assert.That(productToCheck.Price, Is.EqualTo(expectedPrice));
            Assert.That(productToCheck.Quantity, Is.EqualTo(expectedQty));
        }



        [TestCase(4)]
        [TestCase(-2)]
        public void FindingProductByInvalidIndexShouldThrowException(int index)
        {
            AddProducts();

            Assert.Throws<IndexOutOfRangeException>(() =>
            {
                productStock.Find(index);
            });
        }

        //FingByLabel
        [Test]
        public void FingByLabelShouldReturnTheRightProduct()
        {
            var expectedLabel = "Test2";
            var expectedPrice = 7.5;
            var expectedQty = 11;

            AddProducts();

            var productToCheck = productStock.FindByLabel("Test2");

            Assert.That(productToCheck.Label, Is.EqualTo(expectedLabel));
            Assert.That(productToCheck.Price, Is.EqualTo(expectedPrice));
            Assert.That(productToCheck.Quantity, Is.EqualTo(expectedQty));
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("    ")]
        [TestCase("NotExistingProduct")]
        public void FindByLabelShouldThrowExceptionInCaseOfInvalidData(string label)
        {
            AddProducts();

            Assert.Throws<ArgumentException>(() =>
            {
                productStock.FindByLabel(label);
            });
        }

        //FindAllInRange
        [Test]
        public void FindAllInPriceRangeShouldReturnTheCorrectCollection()
        {
            var expectedCount = 3;
            AddMultipleProducts();

            var result = productStock.FindAllInRange(20, 40).ToList();

            Assert.That(result.Count, Is.EqualTo(expectedCount));
            Assert.That(result[0].Label, Is.EqualTo("4"));
            Assert.That(result[1].Label, Is.EqualTo("3"));
            Assert.That(result[2].Label, Is.EqualTo("2"));
        }

        [Test]
        public void FindAllInPriceRangeShouldReturnEmptyCollection()
        {
            var expectedCount = 0;
            AddMultipleProducts();

            var result = productStock.FindAllInRange(100, 200).ToList();

            Assert.That(result.Count, Is.EqualTo(expectedCount));
            Assert.That(result, Is.Empty);
        }

        //FindAllByPrice
        [Test]
        public void FindAllByPriceShouldReturnTheRightCollection()
        {
            var expectedCount = 2;
            AddMultipleProducts();

            var result = productStock.FindAllByPrice(50).ToList();

            Assert.That(result.Count, Is.EqualTo(expectedCount));
            Assert.That(result[0].Label, Is.EqualTo("5"));
            Assert.That(result[1].Label, Is.EqualTo("6"));
        }

        [Test]
        public void FindAllByPriceShouldReturnEmptyCollection()
        {
            var expectedCount = 0;
            AddMultipleProducts();

            var result = productStock.FindAllByPrice(100).ToList();

            Assert.That(result.Count, Is.EqualTo(expectedCount));
            Assert.That(result, Is.Empty);
        }

        //FindMostExpensive
        [Test]
        public void FindMostExpensiveProductShouldReturnTheRightAnswer()
        {
            AddMultipleProducts();

            var result = productStock.FindMostExpensiveProduct();

            Assert.That(result.Label, Is.EqualTo("5")); //Most expensive in order of input
        }

        //FindAllByQuantity
        [Test]
        public void FindAllByQuantityShouldReturnTheRightCollection()
        {
            var expectedCount = 3;
            AddMultipleProducts();

            var result = productStock.FindAllByQuantity(9).ToList();

            Assert.AreEqual(expectedCount, result.Count);
            Assert.That(result[0].Label, Is.EqualTo("4"));
            Assert.That(result[1].Label, Is.EqualTo("5"));
            Assert.That(result[2].Label, Is.EqualTo("6"));
        }

        [Test]
        public void FindAllByQuantityShouldReturnEmptyCollection()
        {
            var expectedCount = 0;
            AddMultipleProducts();

            var result = productStock.FindAllByQuantity(100).ToList();

            Assert.That(result.Count, Is.EqualTo(expectedCount));
            Assert.That(result, Is.Empty);
        }
        //GetEnumerator

        [Test]
        public void GetEnumeratorShouldReturnTheRightInsertionOrder()
        {
            AddMultipleProducts();

            var result = productStock.ToList();

            Assert.That(result[0].Label, Is.EqualTo("1"));
            Assert.That(result[1].Label, Is.EqualTo("2"));
            Assert.That(result[2].Label, Is.EqualTo("3"));
            Assert.That(result[3].Label, Is.EqualTo("4"));
            Assert.That(result[4].Label, Is.EqualTo("5"));
            Assert.That(result[5].Label, Is.EqualTo("6"));
        }

        //Indexator - IT IS ZERO BASED

        [Test]
        public void GetByIndexReturnTheRightProduct()
        {
            var expectedLabel = "Test2";
            var expectedPrice = 7.5;
            var expectedQty = 11;

            AddProducts();

            var productToCheck = productStock[1];

            Assert.That(productToCheck.Label, Is.EqualTo(expectedLabel));
            Assert.That(productToCheck.Price, Is.EqualTo(expectedPrice));
            Assert.That(productToCheck.Quantity, Is.EqualTo(expectedQty));
        }
        [TestCase(4)]
        [TestCase(-2)]
        public void GetByInvalidIndexShouldThrowException(int index)
        {
            AddProducts();

            Assert.Throws<IndexOutOfRangeException>(() =>
            {
                var productToCheck = productStock[index];
            });
        }
        [Test]
        public void SetByIndexShouldChangeTheProductData()
        {
            AddProducts();
            var productToChange = new Product("Changed label", 99.9m, 1);
            productStock[1] = productToChange;

            var productToCheck = productStock[1];

            Assert.That(productToCheck.Label, Is.EqualTo("Changed label"));
            Assert.That(productToCheck.Price, Is.EqualTo(99.9m));
            Assert.That(productToCheck.Quantity, Is.EqualTo(1));
        }
        [TestCase(-1)]
        [TestCase(3)]
        public void SetbyInvalidIndexShouldThrowException(int index)
        {
            AddProducts();

            Assert.Throws<IndexOutOfRangeException>(() =>
            {
                productStock[index] = new Product("Changed label", 99.9m, 1);
            });
        }


        //Helping methods
        private void AddProducts()
        {
            productStock.Add(product);
            productStock.Add(product2);
            productStock.Add(product3);
        }
        private void AddMultipleProducts()
        {
            productStock.Add(new Product("1", 10, 5));
            productStock.Add(new Product("2", 20, 6));
            productStock.Add(new Product("3", 30, 7));
            productStock.Add(new Product("4", 40, 9));
            productStock.Add(new Product("5", 50, 9));
            productStock.Add(new Product("6", 50, 9));
        }
    }
}
