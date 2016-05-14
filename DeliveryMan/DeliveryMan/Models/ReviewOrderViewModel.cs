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

        [Display(Name = "Placed Time")]
        [DataType(DataType.DateTime)]
        public DateTime PlacedTime { get; set; }

        [Display(Name = "Picked-up Time")]
        [DataType(DataType.DateTime)]
        public DateTime? PickUpTime { get; set; }

        [Display(Name = "Delivered Time")]
        [DataType(DataType.DateTime)]
        public DateTime? DeliveredTime { get; set; }

        [Display(Name = "Write a review")]
        public string ReviewText { get; set; }

        [Required]
        public int Rating { get; set; }

        public bool Blacklist { get; set; }
    }
}