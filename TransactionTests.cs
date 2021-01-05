using System;

using Chainblock.Models;

using NUnit.Framework;

namespace Chainblock.Tests
{
    [TestFixture]
    public class TransactionTests
    {
        private Transaction transaction;
        [Test]
        public void TestIfConstructorWorksCorrectly()
        {
            var Id = 123;
            var sender = "Vancho";
            var receiver = "Pencho";
            var status = TransactionStatus.Successfull;
            var amount = 5.0;

            transaction = new Transaction(Id, status, sender, receiver, amount);

            Assert.AreEqual(Id, transaction.Id);
            Assert.AreEqual(status, transaction.Status);
            Assert.AreEqual(sender, transaction.From);
            Assert.AreEqual(receiver, transaction.To);
            Assert.AreEqual(amount, transaction.Amount);
        }
        [TestCase(0)]
        [TestCase(-5)]
        public void IdShouldThrowExceptionIfValueIsNonPositiveNumber(int Id)
        {
            var sender = "Vancho";
            var receiver = "Pencho";
            var status = TransactionStatus.Successfull;
            var amount = 5.0;


            Assert.Throws<ArgumentException>(() =>
            {
                transaction = new Transaction(Id, status, sender, receiver, amount);
            });
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("    ")]
        public void FromShouldThrowExceptionInCaseOfInvalidValue(string from)
        {
            var Id = 132;
            var receiver = "Pencho";
            var status = TransactionStatus.Successfull;
            var amount = 5.0;

            Assert.Throws<ArgumentException>(() =>
            {
                transaction = new Transaction(Id, status, from, receiver, amount);
            });
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("    ")]
        public void ToShouldThrowExceptionInCaseOfInvalidValue(string to)
        {
            var Id = 123;
            var sender = "Ivan";
            var status = TransactionStatus.Successfull;
            var amount = 5.0;

            Assert.Throws<ArgumentException>(() =>
            {
                transaction = new Transaction(Id, status, sender, to, amount);
            });
        }
        [TestCase(0.0)]
        [TestCase(-5.5)]
        public void AmountShouldThrowExceptionInCaseOfNonPositiveValue(double amount)
        {
            var Id = 123;
            var sender = "Ivan";
            var receiver = "Petko";
            var status = TransactionStatus.Successfull;

            Assert.Throws<ArgumentException>(() =>
            {
                transaction = new Transaction(Id, status, sender, receiver, amount);
            });
        }
    }
}
