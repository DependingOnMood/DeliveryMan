using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DeliveryMan.Models
{
    public class DeliverymanRankingViewModel
    {
        [Display(Name = "Rank score")]
        public int Rank { get; set; }

        [Display(Name = "Deliveryman Name")]
        public string DeliverymanName { get; set; }

        [Display(Name = "Orders reviewed")]
        public int TotalOrders { get; set; }

        [Display(Name = "Average Rating")]
        public decimal Rating { get; set; }

        [Display(Name = "Sample review")]
        public string ReviewText { get; set; }

        public string IconUrl { get; set; }
    }
}