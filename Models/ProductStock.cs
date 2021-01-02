using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using INStock.Contracts;

namespace INStock.Models
{
    public class ProductStock : IProductStock
    {
        private readonly List<IProduct> productsByIndex;
        private readonly HashSet<string> labels;
        private readonly Dictionary<string, IProduct> productsByLabel;
        private readonly SortedDictionary<decimal, List<IProduct>> productsByPrice;
        private readonly Dictionary<int, List<IProduct>> productsByQuantity;

        public ProductStock()
        {
            productsByIndex = new List<IProduct>();
            labels = new HashSet<string>();
            productsByLabel = new Dictionary<string, IProduct>();
            productsByPrice = new SortedDictionary<decimal, List<IProduct>>(Comparer<decimal>.Create((first, second) => second.CompareTo(first)));
            productsByQuantity = new Dictionary<int, List<IProduct>>();
        }

        public int Count => this.productsByIndex.Count;
        public IProduct this[int index]
        {
            get
            {
                if (index < 0 || index >= this.Count)
                {
                    throw new IndexOutOfRangeException();
                }
                return this.productsByIndex[index];
            }
            set
            {
                if (index < 0 || index >= this.Count)
                {
                    throw new IndexOutOfRangeException();
                }
                RemoveFromCollections(this.productsByIndex[index]);

                InitializeCollectoins(value);

                this.productsByIndex[index] = value;
            }
        }


        public void Add(IProduct product)
        {
            if (labels.Contains(product.Label))
            {
                throw new ArgumentException();
            }
            productsByIndex.Add(product);
            labels.Add(product.Label);
            productsByLabel.Add(product.Label, product);

            InitializeCollectoins(product);

            productsByPrice[product.Price].Add(product);
            productsByQuantity[product.Quantity].Add(product);
        }


        public bool Contains(IProduct product)
        {
            if (product == null)
            {
                throw new ArgumentException();
            }
            return this.labels.Contains(product.Label);
        }

        public IProduct Find(int index) 
        {
            if (index >= this.Count || index  <= 0)
            {
                throw new IndexOutOfRangeException();
            }
            return productsByIndex[index ];
        }

        public IEnumerable<IProduct> FindAllByPrice(double price)
        {
            if (!productsByPrice.ContainsKey((decimal)price))
            {
                return new List<IProduct>();
            }
            return this.productsByPrice[(decimal)price];
        }

        public IEnumerable<IProduct> FindAllByQuantity(int quantity)
        {
            if (!productsByQuantity.ContainsKey(quantity))
            {
                return new List<IProduct>();
            }
            return this.productsByQuantity[quantity];
        }

        public IEnumerable<IProduct> FindAllInRange(double lo, double hi)
        {
            var result = new List<IProduct>();
            foreach (var (price, products) in this.productsByPrice)
            {
                if (price < (decimal)lo)
                {
                    break;
                }
                if (price <= (decimal)hi)
                {
                    result.AddRange(products);
                }
            }
            return result;
        }

        public IProduct FindByLabel(string label)
        {
            if (string.IsNullOrEmpty(label) || !labels.Contains(label))
            {
                throw new ArgumentException();
            }
            return productsByLabel[label];
        }

        public IProduct FindMostExpensiveProduct()
        {
            return this.productsByPrice.First().Value.First();
        }

        public bool Remove(IProduct product)
        {
            if (product == null)
            {
                throw new ArgumentException();
            }
            if (!labels.Contains(product.Label))
            {
                return false;
            }
            this.productsByIndex.RemoveAll(p => p.Label == product.Label);
            RemoveFromCollections(product);

            return true;
        }
        public IEnumerator<IProduct> GetEnumerator()
        {
            return this.productsByIndex.GetEnumerator();
        }


        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
        private void InitializeCollectoins(IProduct product)
        {
            if (!productsByPrice.ContainsKey(product.Price))
            {
                productsByPrice.Add(product.Price, new List<IProduct>());
            }
            if (!productsByQuantity.ContainsKey(product.Quantity))
            {
                productsByQuantity.Add(product.Quantity, new List<IProduct>());
            }
        }
        private void RemoveFromCollections(IProduct product)
        {
            var productLabel = product.Label;

            this.labels.Remove(productLabel);
            this.productsByLabel.Remove(productLabel);
            var allProductsWithGivenPrice = this.productsByPrice[product.Price];
            allProductsWithGivenPrice.RemoveAll(p => p.Label == productLabel);
            var allProductsWithGivenQty = this.productsByQuantity[product.Quantity];
            allProductsWithGivenQty.RemoveAll(p => p.Quantity == product.Quantity);
        }
    }
}
