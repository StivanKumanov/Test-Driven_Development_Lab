using System;

using Chainblock.Contracts;

namespace Chainblock.Models
{
    public class Transaction : ITransaction
    {
        private int id;
        private TransactionStatus transactionStatus;
        private string from;
        private string to;
        private double amount;

        public Transaction(int id, TransactionStatus ts,string from, string to, double amount)
        {
            this.Id = id;
            this.Status = ts;
            this.From = from;
            this.To = to;
            this.Amount = amount;
        }
        public int Id
        {
            get
            {
                return this.id;
            }
            set
            {
                if (value <= 0)
                {
                    throw new ArgumentException();
                }
                this.id = value;
            }
        }
        public TransactionStatus Status
        {
            get
            {
                return this.transactionStatus;
            }
            set
            {
                this.transactionStatus = value;
            }
        }
        public string From
        {
            get
            {
                return this.from;
            }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentException();
                }
                this.from = value;
            }
        }
        public string To
        {
            get
            {
                return this.to;
            }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentException();
                }
                this.to = value;
            }
        }
        public double Amount
        {
            get
            {
                return this.amount;
            }
            set
            {
                if (value <= 0)
                {
                    throw new ArgumentException();
                }
                this.amount = value;
            }
        }
    }
}
