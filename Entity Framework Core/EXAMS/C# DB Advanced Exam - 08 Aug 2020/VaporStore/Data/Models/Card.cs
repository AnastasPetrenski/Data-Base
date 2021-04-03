﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using VaporStore.Data.Models.Enums;

namespace VaporStore.Data.Models
{
    public class Card
    {
        public Card()
        {
            this.Purchases = new HashSet<Purchase>();
        }

        [Key]
        public int Id { get; set; }

        [Required]
        [RegularExpression(@"^[\d]{4} [\d]{4} [\d]{4} [\d]{4}$")]
        public string Number { get; set; }

        [Required]
        [RegularExpression(@"^[\d]{3}$")]
        public string Cvc { get; set; }

        [Required]
        [EnumDataType(typeof(CardType))]
        public CardType Type { get; set; }

        public int UserId { get; set; }
        [Required]
        public virtual User User { get; set; }

        public virtual ICollection<Purchase> Purchases { get; set; }
    }
}