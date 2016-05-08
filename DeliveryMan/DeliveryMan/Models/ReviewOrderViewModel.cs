using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DeliveryMan.Models
{
    public class ReviewOrderViewModel
    {
        [Required]
        public int Id { get; set; }

        public string ReviewText { get; set; }

        [Required]
        public int Rating { get; set; }

        [Required]
        public DateTime PlacedTime { get; set; }

        [Required]
        public DateTime PickUpTime { get; set; }

        [Required]
        public DateTime DeliveredTime { get; set; }
    }
}
}