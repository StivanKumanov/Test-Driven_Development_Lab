using Chainblock.Models;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Chainblock.Tests
{
    [TestFixture]
    public class ChainBlockTests
    {
        private Transaction transaction;
        private Transaction transaction2;
        private ChainBlock chainBlock;
        [SetUp]
        public void SetUp()
        {
            this.chainBlock = new ChainBlock();
            this.transaction = new Transaction(123, TransactionStatus.Successfull, "Ivan", "Petkan", 50);
            this.transaction2 = new Transaction(456, TransactionStatus.Failed, "Drago", "Stoqn", 100);
        }
        [Test]
        public void TestIfConstructorWorksCorrectly()
        {
            Assert.AreEqual(0, this.chainBlock.Count);
        }

        //Count
        [Test]
        public void TestIfCountWorksCorrectly()
        {
            var expectedCnt = 2;
            chainBlock.Add(transaction);
            chainBlock.Add(transaction2);

            Assert.AreEqual(expectedCnt, this.chainBlock.Count);
        }
        [Test]
        //Add
        public void TestIfAddSavesTheTransactions()
        {
            chainBlock.Add(transaction);
            chainBlock.Add(transaction2);

            var transactionToCheck = chainBlock.GetById(123);
            var transactionToCheck2 = chainBlock.GetById(456);
            //This test also covers the base functionallity of Contains when the given transaction exists
            Assert.AreEqual(123, transactionToCheck.Id);
            Assert.AreEqual(456, transactionToCheck2.Id);
            Assert.That(chainBlock.Contains(123), Is.True);
            Assert.That(chainBlock.Contains(456), Is.True);
        }

        [Test]
        public void ExceptionShouldBeThrownInCaseOfIdenticalTransactions()
        {
            chainBlock.Add(transaction);

            Assert.Throws<InvalidOperationException>(() =>
            {
                chainBlock.Add(transaction);
            });
        }
        [Test]
        public void ExceptionShouldBeThrownInCaseOfIdenticalIdsButTheRestOfTheDataIsCorrect()
        {
            chainBlock.Add(transaction);
            var transactionWithSameId = new Transaction(123, TransactionStatus.Aborted, "Tosho", "Gosho", 32.2);

            Assert.Throws<InvalidOperationException>(() =>
            {
                chainBlock.Add(transactionWithSameId);
            });
        }
        [Test]
        public void ExceptionShouldBeThrowInCaseOfTransactionWhichIsNull()
        {
            chainBlock.Add(transaction);

            Assert.Throws<ArgumentNullException>(() =>
            {
                chainBlock.Add(null);
            });
        }

        //Contains(Transation transaction)
        [Test]
        public void ContainsShouldReturnTrueIfTransactionExists()
        {
            chainBlock.Add(transaction);

            var contains = chainBlock.Contains(transaction);

            Assert.AreEqual(true, contains);
        }

        [Test]
        public void ContainsShouldReturnFalseIfTransactionDoesNotExists()
        {
            chainBlock.Add(transaction);

            var contains = chainBlock.Contains(transaction2);

            Assert.AreEqual(false, contains);
        }
        [Test]
        public void ContainsShouldThrowExceptionWhenTransactionIsNull()
        {
            chainBlock.Add(transaction);

            Assert.Throws<ArgumentNullException>(() =>
            {
                chainBlock.Contains(null);
            });
        }
        //Contains(int id)

        [Test]
        public void ContainsShouldReturnTrueIfTheGivenIdExists()
        {
            chainBlock.Add(transaction);

            var contains = chainBlock.Contains(123);

            Assert.AreEqual(true, contains);
        }
        [Test]
        public void ContainsShouldReturnFalseIfTheGivenIdDoesNotExists()
        {
            chainBlock.Add(transaction);

            var contains = chainBlock.Contains(456);

            Assert.AreEqual(false, contains);
        }

        //•	ChangeTransactionStatus

        [Test]
        public void ChangeTransactionStatusShouldWorksCorectlyWhenTheDataIsValid()
        {
            chainBlock.Add(transaction);

            chainBlock.ChangeTransactionStatus(123, TransactionStatus.Aborted);

            Assert.AreEqual(TransactionStatus.Aborted, transaction.Status);
        }
        [Test]
        public void ChangeTransactionStatusShouldThrowExceptionWhenIdDoesNotExists()
        {
            chainBlock.Add(transaction);
            var nonExistingId = 888;

            Assert.Throws<ArgumentException>(() =>
            {
                chainBlock.ChangeTransactionStatus(nonExistingId, TransactionStatus.Failed);
            });
        }

        //RemoveTransactionById
        [Test]
        public void RemoveTransactionByIdShouldRemoveTheTransaction()
        {
            chainBlock.Add(transaction);
            chainBlock.Add(transaction2);

            chainBlock.RemoveTransactionById(123);

            Assert.AreEqual(false, chainBlock.Contains(123));
        }
        [Test]
        public void RemoveTransactionByIdShouldShouldThrowExceptionWhenTheIdDoesNotExist()
        {
            chainBlock.Add(transaction);
            chainBlock.Add(transaction2);
            var nonExistingId = 888;

            Assert.Throws<InvalidOperationException>(() =>
            {
                chainBlock.RemoveTransactionById(nonExistingId);
            });
        }
        [Test]
        public void TestIfRemoveWorksCorrectlyWithAllPrivateCollections()
        {
            chainBlock.Add(new Transaction(111, TransactionStatus.Successfull, "1", "SameReceiver", 20));
            chainBlock.Add(new Transaction(112, TransactionStatus.Successfull, "2", "SameReceiver", 20));
            chainBlock.Add(new Transaction(113, TransactionStatus.Successfull, "3", "SameReceiver", 30));

            chainBlock.RemoveTransactionById(111);

            var res = chainBlock.GetByReceiverOrderedByAmountThenById("SameReceiver").ToList();

            Assert.AreEqual(2, res.Count);
            Assert.AreEqual(113, res[0].Id);
            Assert.AreEqual(112, res[1].Id);
        }
        //GetById
        [Test]
        public void GetByIdShouldReturnTheRightTransaction()
        {
            var expectedId = 123;
            var expectedStatus = TransactionStatus.Successfull;
            var expectedReceiver = "Petkan";
            var expectedSender = "Ivan";
            var expectedAmount = 50;

            chainBlock.Add(transaction);
            chainBlock.Add(transaction2);

            var returnedTransaction = chainBlock.GetById(123);

            Assert.AreEqual(expectedId, returnedTransaction.Id);
            Assert.AreEqual(expectedStatus, returnedTransaction.Status);
            Assert.AreEqual(expectedReceiver, returnedTransaction.To);
            Assert.AreEqual(expectedSender, returnedTransaction.From);
            Assert.AreEqual(expectedAmount, returnedTransaction.Amount);
        }

        [Test]
        public void GetByWrongIdShouldThrowException()
        {
            chainBlock.Add(transaction);
            chainBlock.Add(transaction2);
            var nonExistingId = 888;

            Assert.Throws<InvalidOperationException>(() =>
            {
                chainBlock.GetById(nonExistingId);
            });
        }

        //GetbyStatus

        [Test]
        public void GetByStatusShouldReturnTheRightCollection()
        {
            AddMultipleTransactions();
            var expectedCnt = 4;

            var result = chainBlock.GetByTransactionStatus(TransactionStatus.Successfull).ToList();

            Assert.AreEqual(expectedCnt, result.Count);
            Assert.AreEqual(result[0].Id, 123);
            Assert.AreEqual(result[1].Id, 789);
            Assert.AreEqual(result[2].Id, 111);

        }
        [Test]
        public void GetByNoTExistingStatusShouldThrowException()
        {
            AddMultipleTransactions();

            Assert.Throws<InvalidOperationException>(() =>
            {
                chainBlock.GetByTransactionStatus(TransactionStatus.Aborted);
            });
        }

        //GetAllSendersWithGivenTransactionStatus
        [Test]
        public void GettAllSendersWithTransactionStatusShouldReturnTheRightCollection()
        {
            AddMultipleTransactions();
            var expectedCnt = 4;

            var result = chainBlock.GetAllSendersWithTransactionStatus(TransactionStatus.Successfull).ToList();

            Assert.AreEqual(expectedCnt, result.Count);
            Assert.AreEqual(result[0], "1");
            Assert.AreEqual(result[1], "1");
            Assert.AreEqual(result[2], "Ivan");
        }
        [Test]
        public void GetAllSendersWithGivenTransactionStatusShouldThrowExceptionWhenThereAreNotSendersWithGivenStatus()
        {
            chainBlock.Add(transaction);//50
            chainBlock.Add(transaction2);

            Assert.Throws<InvalidOperationException>(() =>
            {
                chainBlock.GetAllSendersWithTransactionStatus(TransactionStatus.Aborted);
            });
        }

        //GetAllReceiversWithGivenTransactionStatus
        [Test]
        public void GettAllReceiversWithTransactionStatusShouldReturnTheRightCollection()
        {
            AddMultipleTransactions();
            var expectedCnt = 4;

            var result = chainBlock.GetAllReceiversWithTransactionStatus(TransactionStatus.Successfull).ToList();

            Assert.AreEqual(expectedCnt, result.Count);
            Assert.AreEqual(result[0], "4");
            Assert.AreEqual(result[1], "4");
            Assert.AreEqual(result[2], "Petkan");
            Assert.AreEqual(result[3], "2");

        }
        [Test]
        public void GetAllReceiversWithGivenTransactionStatusShouldThrowExceptionWhenThereAreNotSendersWithGivenStatus()
        {
            chainBlock.Add(transaction);//50
            chainBlock.Add(transaction2);

            Assert.Throws<InvalidOperationException>(() =>
            {
                chainBlock.GetAllReceiversWithTransactionStatus(TransactionStatus.Aborted);
            });
        }

        //GetAllOrderedByAmountDescendingThenById
        [Test]
        public void GetAllOrderedByAmountDescendingThenByIdShouldReturnTheRightCollection()
        {
            AddMultipleTransactions();

            var result = chainBlock.GetAllOrderedByAmountDescendingThenById().ToList();

            Assert.AreEqual(6, result.Count);
            Assert.AreEqual(result[0].Id, 456);//100
            Assert.AreEqual(result[1].Id, 113);//63
            Assert.AreEqual(result[2].Id, 112);//63
            Assert.AreEqual(result[3].Id, 123);//50
            Assert.AreEqual(result[4].Id, 789);//25
            Assert.AreEqual(result[5].Id, 111);//20
        }

        //GetBySenderOrderedByAmountDescending
        [Test]
        public void GetBySenderOrderedByAmountDescendingShouldReturnTheRightCollection()
        {
            AddMultipleTransactions();

            var result = chainBlock.GetBySenderOrderedByAmountDescending("1").ToList();

            Assert.AreEqual(2, result.Count);
            Assert.AreEqual(result[0].Id, 789);
            Assert.AreEqual(result[1].Id, 111);
        }
        [Test]
        public void GetBySenderOrderedByAmountDescendingShouldThrowExceptionWhenThereAreNotSuchTransactions()
        {
            AddMultipleTransactions();

            Assert.Throws<InvalidOperationException>(() =>
            {
                chainBlock.GetBySenderOrderedByAmountDescending("NotExisting name");
            });
        }
        //•	GetByReceiverOrderedByAmountThenById
        [Test]
        public void GetByReceiverOrderedByAmountThenByIdShouldReturnTheRightCollection()
        {
            AddMultipleTransactions();
            chainBlock.Add(new Transaction(114, TransactionStatus.Successfull, "3", "4", 63));

            var result = chainBlock.GetByReceiverOrderedByAmountThenById("4").ToList();

            Assert.AreEqual(3, result.Count);
            Assert.AreEqual(112, result[0].Id);
            Assert.AreEqual(114, result[1].Id);
            Assert.AreEqual(111, result[2].Id);
        }
        [Test]
        public void GetByReceiverOrderedByAmountThenByIdShouldThrowExceptionInCaseOfNotExistingReceiver()
        {
            AddMultipleTransactions();

            Assert.Throws<InvalidOperationException>(() =>
            {
                chainBlock.GetByReceiverOrderedByAmountThenById("Not existing receiver");
            });
        }

        //GetByTransactionStatusAndMaximumAmount
        [Test]
        public void GetByTransactionStatusAndMaximumAmountShouldReturnTheRightCollection()
        {
            AddMultipleTransactions();
            var expectedCnt = 3;

            var result = chainBlock.GetByTransactionStatusAndMaximumAmount(TransactionStatus.Successfull, 50).ToList();

            Assert.AreEqual(expectedCnt, result.Count);
            Assert.AreEqual(123, result[0].Id);
            Assert.AreEqual(789, result[1].Id);
            Assert.AreEqual(111, result[2].Id);
        }

        [Test]
        public void GetByTransactionStatusAndMaximumAmountShouldReturnEmptyCollectionInCaseOfWrongTransactionStaus()
        {
            AddMultipleTransactions();

            var result = chainBlock.GetByTransactionStatusAndMaximumAmount(TransactionStatus.Unauthorized, 100);

            Assert.That(result, Is.Empty);
        }
        [Test]
        public void GetByTransactionStatusAndMaximumAmountShouldReturnEmptyCollectionInCaseOfWrongMaxAmount()
        {
            AddMultipleTransactions();

            var result = chainBlock.GetByTransactionStatusAndMaximumAmount(TransactionStatus.Successfull, 10);

            Assert.That(result, Is.Empty);
        }
        //GetBySenderAndMinimumAmountDescending
        [Test]
        public void GetBySenderAndMinimumAmountDescendingShouldReturnTheRightCollection()
        {
            AddMultipleTransactions();
            chainBlock.Add(new Transaction(114, TransactionStatus.Successfull, "1", "4", 15));

            var result = chainBlock.GetBySenderAndMinimumAmountDescending("1", 15).ToList();

            Assert.AreEqual(2, result.Count);
            Assert.AreEqual(789, result[0].Id);
            Assert.AreEqual(111, result[1].Id);
        }
        [Test]
        public void GetBySenderAndMinimumAmountDescendingShouldThrowExceptionInCaseOfInvalidSender()
        {
            AddMultipleTransactions();

            Assert.Throws<InvalidOperationException>(() =>
            {
                chainBlock.GetBySenderAndMinimumAmountDescending("Non Existing sender", 10);
            });
        }
        [Test]
        public void GetBySenderAndMinimumAmountDescendingShouldThrowExceptionInCaseOfInvalidMinAmount()
        {
            AddMultipleTransactions();

            Assert.Throws<InvalidOperationException>(() =>
            {
                chainBlock.GetBySenderAndMinimumAmountDescending("1", 120);
            });
        }
        //•	GetByReceiverAndAmountRange
        [Test]
        public void GetByReceiverAndAmountRangeShouldReturnTheRightCollection()
        {
            AddMultipleTransactions();
            chainBlock.Add(new Transaction(114, TransactionStatus.Successfull, "1", "4", 15));
            chainBlock.Add(new Transaction(115, TransactionStatus.Successfull, "1", "4", 30));
            chainBlock.Add(new Transaction(116, TransactionStatus.Successfull, "1", "4", 30));

            var result = chainBlock.GetByReceiverAndAmountRange("4", 20, 63).ToList();

            Assert.AreEqual(3, result.Count);
            Assert.AreEqual(115, result[0].Id);
            Assert.AreEqual(116, result[1].Id);
            Assert.AreEqual(111, result[2].Id);
        }
        [TestCase(64, 100)]
        [TestCase(5, 15)]
        public void GetByReceiverAndAmountRangeShouldThrowExceptionInCaseOfInvalidRange(int lo, int hi)
        {
            AddMultipleTransactions();
            chainBlock.Add(new Transaction(114, TransactionStatus.Successfull, "1", "4", 15));
            chainBlock.Add(new Transaction(115, TransactionStatus.Successfull, "1", "4", 30));

            Assert.Throws<InvalidOperationException>(() =>
            {
                var result = chainBlock.GetByReceiverAndAmountRange("4", lo, hi).ToList();
            });
        }
        [Test]
        public void GetByReceiverAndAmountRangeShouldThrowExceptionInCaseOfInvalidReceiver()
        {
            AddMultipleTransactions();
            chainBlock.Add(new Transaction(114, TransactionStatus.Successfull, "1", "4", 15));
            chainBlock.Add(new Transaction(115, TransactionStatus.Successfull, "1", "4", 30));

            Assert.Throws<InvalidOperationException>(() =>
            {
                var result = chainBlock.GetByReceiverAndAmountRange("Not existing receiver", 10, 30).ToList();
            });
        }
        //GetAllInAmountRange
        [Test]
        public void GetAllInAmountRangeShouldReturnTheRightCollection()
        {
            AddMultipleTransactions();

            var result = chainBlock.GetAllInAmountRange(20, 50).ToList();

            Assert.AreEqual(3, result.Count);
            Assert.AreEqual(123, result[0].Id);
            Assert.AreEqual(789, result[1].Id);
            Assert.AreEqual(111, result[2].Id);
        }
        [TestCase(10,15)]
        [TestCase(101,115)]
        public void GetAllInAmountRangeShouldReturnEmptyCollection(int lo, int hi)
        {
            AddMultipleTransactions();

            var result = chainBlock.GetAllInAmountRange(lo, hi).ToList();

            Assert.That(result, Is.Empty);

        }
        //This method causes problems for methods with staus
        private void AddMultipleTransactions()
        {
            chainBlock.Add(transaction);//50
            chainBlock.Add(transaction2);//100
            chainBlock.Add(new Transaction(789, TransactionStatus.Successfull, "1", "2", 25));
            chainBlock.Add(new Transaction(111, TransactionStatus.Successfull, "1", "4", 20));
            chainBlock.Add(new Transaction(112, TransactionStatus.Successfull, "3", "4", 63));
            chainBlock.Add(new Transaction(113, TransactionStatus.Failed, "5", "6", 63));
        }
    }
}
