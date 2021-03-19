using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using BookShop.Data.Models.Enums;

namespace BookShop.Data.Models
{
    public class Book
    {
        public Book()
        {
            this.AuthorsBooks = new HashSet<AuthorBook>();
        }

        public int Id { get; set; }

        public string Name { get; set; }

        public Genre Genre { get; set; }

        public decimal Price { get; set; }

        [Range(50, 5000)]
        public int Pages { get; set; }

        public virtual DateTime PublishedOn { get; set; }

        public virtual ICollection<AuthorBook> AuthorsBooks { get; set; }
    }
}
