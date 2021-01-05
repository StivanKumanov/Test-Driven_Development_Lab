using Chainblock.Contracts;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Chainblock.Models
{
    public class ChainBlock : IChainblock
    {
        private Dictionary<int, ITransaction> transactionsById;
        private Dictionary<double, List<ITransaction>> transactionsByAmount;
        private SortedDictionary<double, List<ITransaction>> transactionsByAmountDescending;
        private Dictionary<TransactionStatus, List<ITransaction>> transactionsByStatus;
        private Dictionary<string, List<ITransaction>> transactionsBySender;
        private Dictionary<string, List<ITransaction>> transactionsByReceiver;
        private HashSet<int> ids;
        public ChainBlock()
        {
            this.transactionsById = new Dictionary<int, ITransaction>();
            this.ids = new HashSet<int>();
            transactionsByAmount = new Dictionary<double, List<ITransaction>>();
            transactionsByAmountDescending = new SortedDictionary<double, List<ITransaction>>(Comparer<double>.Create((first, second) => second.CompareTo(first)));
            transactionsByStatus = new Dictionary<TransactionStatus, List<ITransaction>>();
            transactionsBySender = new Dictionary<string, List<ITransaction>>();
            transactionsByReceiver = new Dictionary<string, List<ITransaction>>();
        }
        public int Count => this.ids.Count;

        public void Add(ITransaction tx)
        {
            if (tx == null)
            {
                throw new ArgumentNullException();
            }
            if (ids.Contains(tx.Id))
            {
                throw new InvalidOperationException();
            }
            this.ids.Add(tx.Id);
            transactionsById.Add(tx.Id, tx);
            if (!transactionsByAmount.ContainsKey(tx.Amount))
            {
                transactionsByAmount.Add(tx.Amount, new List<ITransaction>());
            }
            if (!transactionsByAmountDescending.ContainsKey(tx.Amount))
            {
                transactionsByAmountDescending.Add(tx.Amount, new List<ITransaction>());
            }
            if (!transactionsByStatus.ContainsKey(tx.Status))
            {
                transactionsByStatus.Add(tx.Status, new List<ITransaction>());
            }
            if (!transactionsBySender.ContainsKey(tx.From))
            {
                transactionsBySender.Add(tx.From, new List<ITransaction>());
            }
            if (!transactionsByReceiver.ContainsKey(tx.To))
            {
                transactionsByReceiver.Add(tx.To, new List<ITransaction>());
            }
            transactionsByAmount[tx.Amount].Add(tx);
            transactionsByAmountDescending[tx.Amount].Add(tx);
            transactionsByStatus[tx.Status].Add(tx);
            transactionsBySender[tx.From].Add(tx);
            transactionsByReceiver[tx.To].Add(tx);
        }

        public void ChangeTransactionStatus(int id, TransactionStatus newStatus)
        {
            if (!ids.Contains(id))
            {
                throw new ArgumentException();
            }
            var transactionToChange = transactionsById[id];
            transactionToChange.Status = newStatus;
        }

        public bool Contains(ITransaction tx)
        {
            if (tx == null)
            {
                throw new ArgumentNullException();
            }
            if (ids.Contains(tx.Id))
            {
                return true;
            }
            return false;
        }

        public bool Contains(int id)
        {
            if (ids.Contains(id))
            {
                return true;
            }
            return false;

        }

        public IEnumerable<ITransaction> GetAllInAmountRange(double lo, double hi)
        {
            var result = new List<ITransaction>();
            foreach (var (amount, tx) in transactionsByAmount)
            {
                if (amount < lo)
                {
                    break;
                }
                if (amount <= hi)
                {
                    result.AddRange(tx);
                }
            }
            return result;
        }

        public IEnumerable<ITransaction> GetAllOrderedByAmountDescendingThenById()
        {
            var result = new List<ITransaction>();
            foreach (var (amount, txs) in transactionsByAmountDescending)
            {
                result.AddRange(txs.OrderByDescending(t => t.Id));
            }
            return result;
        }

        public IEnumerable<string> GetAllReceiversWithTransactionStatus(TransactionStatus status)
        {
            var numberOfOccurances = new Dictionary<string, int>();
            var res = new List<string>();
            if (!this.transactionsByStatus.ContainsKey(status))
            {
                throw new InvalidOperationException();
            }
            foreach (var item in this.transactionsByStatus[status])
            {
                if (!numberOfOccurances.ContainsKey(item.To))
                {
                    numberOfOccurances.Add(item.To, 0);
                }
                numberOfOccurances[item.To]++;
            }
            foreach (var (sender, cnt) in numberOfOccurances.OrderByDescending(t => t.Value))
            {
                for (int i = 0; i < cnt; i++)
                {
                    res.Add(sender);
                }
            }
            return res;
        }

        public IEnumerable<string> GetAllSendersWithTransactionStatus(TransactionStatus status)
        {
            var numberOfOccurances = new Dictionary<string, int>();
            var res = new List<string>();
            if (!this.transactionsByStatus.ContainsKey(status))
            {
                throw new InvalidOperationException();
            }
            foreach (var item in this.transactionsByStatus[status])
            {
                if (!numberOfOccurances.ContainsKey(item.From))
                {
                    numberOfOccurances.Add(item.From, 0);
                }
                numberOfOccurances[item.From]++;
            }
            foreach (var (sender, cnt) in numberOfOccurances.OrderByDescending(t => t.Value))
            {
                for (int i = 0; i < cnt; i++)
                {
                    res.Add(sender);
                }
            }
            return res;
        }

        public ITransaction GetById(int id)
        {
            if (!ids.Contains(id))
            {
                throw new InvalidOperationException();
            }
            return this.transactionsById[id];
        }

        public IEnumerable<ITransaction> GetByReceiverAndAmountRange(string receiver, double lo, double hi)
        {
            if (!this.transactionsByReceiver.ContainsKey(receiver))
            {
                throw new InvalidOperationException();
            }
            var res = this.transactionsByReceiver[receiver].Where(t => t.Amount >= lo && t.Amount < hi).ToList();
            if (res.Count == 0)
            {
                throw new InvalidOperationException();
            }
            return res.OrderByDescending(t => t.Amount).ThenBy(t => t.Id);
        }

        public IEnumerable<ITransaction> GetByReceiverOrderedByAmountThenById(string receiver)
        {
            if (!transactionsByReceiver.ContainsKey(receiver))
            {
                throw new InvalidOperationException();
            }
            return this.transactionsByReceiver[receiver].OrderByDescending(t => t.Amount).ThenBy(t => t.Id);
        }

        public IEnumerable<ITransaction> GetBySenderAndMinimumAmountDescending(string sender, double amount)
        {
            if (!this.transactionsBySender.ContainsKey(sender))
            {
                throw new InvalidOperationException();
            }
            var res = this.transactionsBySender[sender].Where(t => t.Amount > amount).OrderByDescending(t => t.Amount).ToList();
            if (res.Count == 0)
            {
                throw new InvalidOperationException();
            }
            return res;
        }

        public IEnumerable<ITransaction> GetBySenderOrderedByAmountDescending(string sender)
        {
            if (!this.transactionsBySender.ContainsKey(sender))
            {
                throw new InvalidOperationException();
            }
            if (this.transactionsBySender[sender].Count == 0)
            {
                throw new InvalidOperationException();
            }
            return this.transactionsBySender[sender].OrderByDescending(t => t.Amount);
        }

        public IEnumerable<ITransaction> GetByTransactionStatus(TransactionStatus status)
        {
            if (!transactionsByStatus.ContainsKey(status))
            {
                throw new InvalidOperationException();
            }
            return this.transactionsByStatus[status];
        }

        public IEnumerable<ITransaction> GetByTransactionStatusAndMaximumAmount(TransactionStatus status, double amount)
        {
            var result = new List<ITransaction>();
            if (!transactionsByStatus.ContainsKey(status))
            {
                return result;
            }
            result = this.transactionsByStatus[status].Where(t => t.Amount <= amount).ToList();
            return result;
        }

        public void RemoveTransactionById(int id)
        {
            if (!this.ids.Contains(id))
            {
                throw new InvalidOperationException();
            }
            var transactionToRemove = this.transactionsById[id];
            this.ids.Remove(id);
            this.transactionsById.Remove(id);
            this.transactionsByAmount[transactionToRemove.Amount].RemoveAll(t => t.Id == id);
            this.transactionsByAmountDescending[transactionToRemove.Amount].RemoveAll(t => t.Id == id);
            this.transactionsByReceiver[transactionToRemove.To].RemoveAll(t => t.Id == id);
            this.transactionsBySender[transactionToRemove.From].RemoveAll(t => t.Id == id);
            this.transactionsByStatus[transactionToRemove.Status].RemoveAll(t => t.Id == id);
        }

        public IEnumerator<ITransaction> GetEnumerator()
        {
            return this.transactionsById.Values.GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
