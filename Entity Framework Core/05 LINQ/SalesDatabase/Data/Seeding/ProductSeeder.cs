using System;
using System.Collections.Generic;
using P03_SalesDatabase.Data.Models;
using P03_SalesDatabase.Data.Seeding.Contracts;

namespace P03_SalesDatabase.Data.Seeding
{
    public class ProductSeeder : ISeeder
    {
        private readonly SalesContext context;
        private readonly Random random; 


        public ProductSeeder(SalesContext context, Random random)
        {
            this.context = context;
            this.random = random;
        }

        public void Seed()
        {
            var products = new[] { "apple", "banana", "orange", "kiwi", "blueberry", 
                                    "strawberry", "pineapple", "mango", "peach", "apricot"};
            var quantities = new List<double>() { 1, 2, 3.5, 4, 5.5, 6, 7, 8.6, 9.73, 10};
            var prices = new[] { 2.49m, 1.69m, 3.49m, 5.19m, 0.99m, 4.49m, 7.89m, 12.99m, 7.39m, 9.69m };

            List<Product> randomProducts = new List<Product>();

            for (int i = 0; i < products.Length; i++)
            {
                Product product = new Product()
                {
                    Name = products[random.Next(0, 9)],
                    Quantity = quantities[random.Next(0, 9)],
                    Price = prices[random.Next(0, 9)]
                };

                randomProducts.Add(product);
            }

            this.context.AddRange(randomProducts);
            this.context.SaveChanges();
        }
    }
}
