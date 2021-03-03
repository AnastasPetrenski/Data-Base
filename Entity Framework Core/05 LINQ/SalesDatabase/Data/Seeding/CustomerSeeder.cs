using System;
using System.Collections.Generic;
using P03_SalesDatabase.Data.Models;
using P03_SalesDatabase.Data.Seeding.Contracts;

namespace P03_SalesDatabase.Data.Seeding
{
    public class CustomerSeeder : ISeeder
    {
        private readonly SalesContext context;
        private readonly Random random;

        public CustomerSeeder(SalesContext context, Random random)
        {
            this.context = context;
            this.random = random;
        }

        public void Seed()
        {
            var customerNames = new[]
            {
                "Anastas Petrenski",
                "Diego Maradona",
                "Hristo Stoichkov",
                "Dafta Pele",
                "Dimitar Berbatov",
                "Krasimir Balakov",
                "Yordan Lechkov",
                "Emil Kostadinov",
                "Lubomir Penev",
                "Trifon Ivanov"
            };

            var custmomerEmails = new[]
            {
                "petrenski@abv.bg",
                "maradona@gmail.com",
                "stoichkov@mail.bg",
                "pele@yahoo.com",
                "berbatov@abv.bg",
                "balakov@mail.bg",
                "lechkov@abv.bg",
                "kostadinov@mail.bg",
                "penev@gmail.com",
                "ivanov@yahoo.com"
            };

            var creditCardNumbers = new List<string>();
            for (int i = 0; i < 10; i++)
            {
                string custom = string.Empty;
                for (int x = 0; x < 10; x++)
                {
                    var current = this.random.Next(0, 9);
                    custom = custom + current;
                }

                creditCardNumbers.Add(custom);
            }

            for (int i = 0; i < 10; i++)
            {
                Customer customer = new Customer()
                {
                    Name = customerNames[i],
                    Email = custmomerEmails[i],
                    CreditCardNumber = creditCardNumbers[i]
                };

                this.context.Add(customer);
                this.context.SaveChanges();
            }
        }
    }
}
