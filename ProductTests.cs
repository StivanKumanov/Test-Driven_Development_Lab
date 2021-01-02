namespace INStock.Tests
{
    using INStock.Models;
    using NUnit.Framework;

    [TestFixture]
    public class ProductTests
    {
        private Product product;
        [Test]
        public void ConstructorShouldCreateValidProduct()
        {
            var expectedLabel = "Test";
            var expectedPrice = 23.5m;
            var expectedQty = 6;

             product = new Product("Test", 23.5m, 6);

            Assert.That(product, Is.Not.Null);
            Assert.That(product.Label, Is.EqualTo(expectedLabel));
            Assert.That(product.Price, Is.EqualTo(expectedPrice));
            Assert.That(product.Quantity, Is.EqualTo(expectedQty));
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("    ")]
        public void CreatingProductWithInvalidLabelShouldThrownException(string label)
        {
            Assert.That(() =>
            {
                product = new Product(label, 23.5m, 6);
            }, Throws.ArgumentException);
        }
        [TestCase(-1.0)]
        [TestCase(0)]
        public void CreatingProductWithInvalidPriceShouldThrownException(decimal price)
        {
            Assert.That(() =>
            {
                product = new Product("Test", price, 6);
            }, Throws.ArgumentException);
        }

        [Test]
        public void CreatingProductWithInvalidQuantityShouldThrownException()
        {
            Assert.That(() =>
            {
                product = new Product("Test", 23.5m, -5);
            }, Throws.ArgumentException);
        }
        [Test]
        public void CheckIfCompareTwoWorksCorrectlyWithRightOrderOfProducts()
        {
            product = new Product("Test", 35.5m, 6);
            var product2 = new Product("Test2", 15m, 2);

           var res =  product.CompareTo(product2);

            Assert.That(res, Is.EqualTo(1));
        }
        [Test]
        public void CheckIfCompareTwoWorksCorrectlyWithReverseOrderOfProducts()
        {
            product = new Product("Test", 35.5m, 6);
            var product2 = new Product("Test2", 15m, 2);

            var res = product2.CompareTo(product);

            Assert.That(res, Is.EqualTo(-1));
        }
        [Test]
        public void CheckIfCompareTwoWorksCorrectlyWithEqualProducts()
        {
            product = new Product("Test",  15m, 6);
            var product2 = new Product("Test2", 15m, 2);

            var res = product.CompareTo(product2);

            Assert.That(res, Is.EqualTo(0));
        }
    }
}