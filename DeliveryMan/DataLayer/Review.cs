﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace DataLayer
{
    public class Review
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int OrderId {get; set;}

        [ForeignKey("OrderId")]
        public virtual Order order { get; set; }

        public string ReviewText { get; set; }

        public int Rating { get; set; }
    }
}