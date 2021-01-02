using System;

using INStock.Contracts;

namespace INStock.Models
{
    public class Product : IProduct
    {
        private string label;
        private decimal price;
        private int quantity;

        public Product(string label, decimal price, int qty)
        {
            this.Label = label;
            this.Price = price;
            this.Quantity = qty;
        }
        public string Label
        {
            get
            {
                return this.label;
            }
            private set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentException("Label cannot be null!");
                }
                this.label = value;
            }
        }

        public decimal Price
        {
            get
            {
                return this.price;
            }
            private set
            {
                if (value <= 0)
                {
                    throw new ArgumentException("Price must be positive number!");
                }
                this.price = value;
            }
        }

        public int Quantity
        {
            get
            {
                return this.quantity;
            }
            private set
            {
                if (value < 0)
                {
                    throw new ArgumentException("Quantity cannot be negative number!");
                }
                this.quantity = value;
            }
        }

        public int CompareTo(IProduct other)
        {
            return this.Price.CompareTo(other.Price);
        }
    }
}
