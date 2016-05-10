using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DeliveryMan.Models
{
    public class DeliverymanRankingViewModel
    {
        [Display(Name = "Rank")]
        public int Rank { get; set; }

        [Display(Name = "Deliveryman Name")]
        public string DeliverymanName { get; set; }

        [Display(Name = "Orders delivered")]
        public int TotalOrders { get; set; }

        [Display(Name = "Rating")]
        public decimal Rating { get; set; }

        [Display(Name = "Sample reviews")]
        public string ReviewText { get; set; }
    }
}