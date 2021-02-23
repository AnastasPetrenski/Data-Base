using SoftUni.Data;
using SoftUni.Models;
using System;
using System.Globalization;
using System.Linq;
using System.Text;

namespace SoftUni
{
    public class StartUp
    {
        static void Main()
        {
            SoftUniContext context = new SoftUniContext();

            var result = GetEmployeesByFirstNameStartingWithSa(context);

            Console.WriteLine(result);
        }

        public static string RemoveTown(SoftUniContext context)
        {
            string townToRemove = "Seattle";

            context.Employees
                .Where(e => e.Address.Town.Name == townToRemove)
                .ToList()
                .ForEach(a => a.AddressId = null);

            var addresses = context.Addresses
                .Where(a => a.Town.Name == townToRemove)
                .ToList();

            context.RemoveRange(addresses);

            context.Towns
                .Remove(context.Towns
                        .First(t => t.Name == townToRemove));

            context.SaveChanges();

            return $"{addresses.Count} addresses in {townToRemove} were deleted";
        }

    }
}
