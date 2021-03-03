using System;
using System.Collections.Generic;
using System.Reflection;
using P03_SalesDatabase.Data;
using P03_SalesDatabase.Data.Seeding;
using P03_SalesDatabase.Data.Seeding.Contracts;

namespace P03_SalesDatabase
{
    public class StartUp
    {
        static void Main()
        {
            SalesContext context = new SalesContext();
            Random random = new Random();
            //context.Database.EnsureDeleted();
            //context.Database.EnsureCreated();

            //Reflection ???

            //var types = Assembly.GetExecutingAssembly().GetTypes();

            //foreach (var item in types)
            //{
            //    if (item.Name.Contains("Seeder") && item.IsClass)
            //    {
            //        ISeeder seeder = item.Name switch
            //        {
            //            "CustomerSeeder" => new CustomerSeeder(context, random),
            //            "ProductSeeder" => new ProductSeeder(context, random),
            //            "StoreSeeder" => new StoreSeeder(context, random),
            //            "SalesSeeder" => new SalesSeeder(context, random),
            //            _=> throw new ArgumentException()
            //        };

            //        seeder.Seed();

            //        //var persistMethod = typeof(ISeeder)
            //        //         .GetMethod("Persist", BindingFlags.Instance | BindingFlags.NonPublic)
            //        //         .MakeGenericMethod(item);

            //        //persistMethod.Invoke(item, new object[] { context, random });
            //    }
            //}

            //foreach (var item in types)
            //{
            //    if (item.Name.Contains("SalesSeeder") && item.IsClass)
            //    {
            //        ISeeder seeder = item.Name switch
            //        {   
            //            "SalesSeeder" => new SalesSeeder(context, random),
            //            _ => throw new ArgumentException()
            //        };

            //        seeder.Seed();

            //    }
            //}

            ICollection<ISeeder> seeders = new List<ISeeder>();
            seeders.Add(new StoreSeeder(context, random));
            seeders.Add(new ProductSeeder(context, random));
            seeders.Add(new CustomerSeeder(context, random));
            seeders.Add(new SalesSeeder(context, random));

            foreach (var seed in seeders)
            {
                seed.Seed();
            }


        }
    }
}
