using System;
using System.Collections.Generic;
using P03_SalesDatabase.Data.Models;
using P03_SalesDatabase.Data.Seeding.Contracts;

namespace P03_SalesDatabase.Data.Seeding
{
    public class StoreSeeder : ISeeder
    {
        private readonly SalesContext context;
        private readonly Random random;

        public StoreSeeder(SalesContext context, Random random)
        {
            this.context = context;
            this.random = random;
        }

        public void Seed()
        {
            var stores = new[] { "Sofia", "Pleven", "Petrich", "Plovdiv", "Vidin", "Varna", "Burgas", "Ruse", "Lom", "Bania" };

            List<Store> randomStores = new List<Store>(); 

            for (int i = 0; i < stores.Length; i++)
            {
                Store store = new Store()
                {
                    Name = stores[i]
                };

                if (randomStores.Count > 1)
                {
                    var randomIndex = this.random.Next(0, randomStores.Count);
                    var temp = randomStores[randomIndex];
                    randomStores[randomIndex] = store;
                    randomStores.Add(temp);
                }
                else
                {
                    randomStores.Add(store);
                }
            }

            this.context.Stores.AddRange(randomStores);
            this.context.SaveChanges();
        }
    }
}
