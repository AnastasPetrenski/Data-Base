using P03_SalesDatabase.Data.Models;
using P03_SalesDatabase.Data.Seeding.Contracts;
using System;

namespace P03_SalesDatabase.Data.Seeding
{
    public class SalesSeeder : ISeeder
    {
        private readonly SalesContext context;
        private readonly Random random;

        public SalesSeeder(SalesContext context, Random random)
        {
            this.context = context;
            this.random = random;
        }

        public void Seed()
        {
            for (int i = 0; i < 100; i++)
            {
                Sale sale = new Sale()
                {
                    //Date = DateTime.UtcNow.AddDays(i % 10),
                    ProductId = this.random.Next(1, 10),
                    CustomerId = this.random.Next(1, 10),
                    StoreId = this.random.Next(1, 10)
                };

                this.context.Sales.Add(sale);
                this.context.SaveChanges();
            }
        }
    }
}
