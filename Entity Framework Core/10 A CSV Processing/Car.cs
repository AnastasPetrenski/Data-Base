using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace CSV
{
    public class Car
    {
        public int Year { get; set; }

        [Required]
        public string Make { get; set; }

        [Required]
        public string Model { get; set; }

        public string Description { get; set; }

        public decimal Price { get; set; }

        public override string ToString()
        {
            string emptyDescription = Description != string.Empty ? Description : "No description";

            var sb = new StringBuilder();
            sb
                .AppendLine($"Car year: {Year}")
                .AppendLine($"Maker: {Make}")
                .AppendLine($"Model: {Model}")
                .AppendLine($"Descpription: {emptyDescription}")
                .AppendLine($"Price: {Price:f2}");

            return sb.ToString();
        }
    }
}
