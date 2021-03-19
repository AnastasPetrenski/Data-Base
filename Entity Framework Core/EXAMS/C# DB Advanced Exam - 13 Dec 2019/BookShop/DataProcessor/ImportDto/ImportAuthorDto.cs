using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace BookShop.DataProcessor.ImportDto
{
    public class ImportAuthorDto
    {
        [JsonProperty("FirstName")]
        [MinLength(3)]
        [MaxLength(30)]
        [Required]
        public string FirstName { get; set; }

        [JsonProperty("LastName")]
        [MinLength(3)]
        [MaxLength(30)]
        [Required]
        public string LastName { get; set; }

        [JsonProperty("Phone")]
        [Required]
        [RegularExpression(@"^[0-9]{3}-[0-9]{3}-[0-9]{4}$")]        
        public string Phone { get; set; }

        [JsonProperty("Email")]
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        
        public ImportAuthorBooksDto[] Books { get; set; }
    }
}

//{
//  "FirstName": "K",
//  "LastName": "Tribbeck",
//  "Phone": "808-944-5051",
//  "Email": "btribbeck0@last.fm",
//  "Books": [
//    {
//      "Id": 79
//    },
//    {
//      "Id": 40
//    }
//  ]
//},
