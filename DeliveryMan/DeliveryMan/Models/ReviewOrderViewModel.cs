using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DeliveryMan.Models
{
    public class ReviewOrderViewModel
    {
        [Required]
        [Display(Name = "Order ID")]
        public int OrderId { get; set; }

        [Display(Name = "Order Name")]
        public string OrderName { get; set; }

        [Required]
        [Display(Name = "Placed Time")]
        public DateTime PlacedTime { get; set; }

        [Required]
        [Display(Name = "Picked-up Time")]
        public DateTime PickUpTime { get; set; }

        [Required]
        [Display(Name = "Delivered Time")]
        public DateTime DeliveredTime { get; set; }

        [Display(Name = "Write a review")]
        public string ReviewText { get; set; }

        [Required]
        public int Rating { get; set; }
    }
}