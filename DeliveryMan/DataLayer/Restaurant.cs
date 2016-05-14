﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer
{
    public class Restaurant
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string ContactId { get; set; }

        public virtual Contact Contact { get; set; }

        public string Name { get; set; }

        [Display(Name = "Insert your icon image URL.")]
        public string IconImageUrl { get; set; }

        public decimal Balance { get; set; }
    }
}
