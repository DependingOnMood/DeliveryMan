using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DeliveryMan.Models
{
    public class BlackListViewModel
    {
        [Display(Name = "Deliveryman Name")]
        public string DeliverymanName { get; set; }

        [Display(Name = "Orders Delivered")]
        public int TotalOrders { get; set; }

        [Display(Name = "Average delivery time")]
        public int AverageDeliveryTime { get; set; }

        [Display(Name = "Average Rating")]
        public decimal Rating { get; set; }

        public int? OrderId { get; set; }

        public int? RestaurantId { get; set; }

        public int? DeliverymanId { get; set; }

    }
}