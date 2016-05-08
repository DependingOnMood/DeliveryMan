using DataLayer;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DeliveryMan.Models
{
    public class CancelOrderViewModel
    {
        [Display(Name = "Order ID")]
        public int OrderId { get; set; }

        [Display(Name = "Order Name")]
        public string OrderName { get; set; }

        [Display(Name = "Order Status")]
        public Status OrderStatus { get; set; }

        [Display(Name = "ETA")]
        public string ETA { get; set; }

        [Display(Name = "Placed Time")]
        public DateTime PlacedTime { get; set; }

        [Display(Name = "Picked-up Time")]
        public DateTime? PickUpTime { get; set; }

        [Display(Name = "Delivery Fee")]
        public decimal DeliveryFee { get; set; }

        [Display(Name = "Cancellation Fee")]
        public decimal CancellationFee { get; set; }

    }
}