﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer
{
        public class Deliveryman
        {
            [Key]
            [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
            public int Id { get; set; }

            public int CId { get; set; }

            public virtual Contact Contact { get; set; }

            public string Name { get; set; }

            [Url]
            [Display(Name = "Insert your icon image URL.")]
            public string IconImageUrl { get; set; }

            public int TotalDeliveryCount { get; set; }

            public int TotalStarsEarned { get; set; }

            public int Rating { get; set; }

            public int Ranking { get; set; }

            public decimal Balance { get; set; }
        }
    }