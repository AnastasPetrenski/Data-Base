﻿using System;
using System.Linq;
using System.Text;
using FastFood.Data;

namespace FastFood.DataProcessor
{
    public static class Bonus
    {
	    public static string UpdatePrice(FastFoodDbContext context, string itemName, decimal newPrice)
	    {
			var item = context.Items.FirstOrDefault(x => x.Name == itemName);

            var sb = new StringBuilder();

            if (item is null)
            {
                return $"Item {item.Name} not found!";
            }
            else
            {
                var oldPrice = item.Price;

                item.Price = newPrice;

                context.SaveChanges();

                return $"{item.Name} Price updated from ${oldPrice:F2} to ${newPrice:F2}";
            }
	    }
    }
}
