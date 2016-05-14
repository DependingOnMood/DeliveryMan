using DataLayer;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DeliveryMan.Models
{
    public class RestaurantOrdersViewModel
    {
        public decimal Balance { get; set; }

        public IEnumerable<Order> WaitingOrders { get; set; }

        public IEnumerable<Order> PendingOrders { get; set; }

        public IEnumerable<Order> InProgressOrders { get; set; }
    }

    public class RestaurantCreateOrderViewModel
    {
        [Required]
        public string Note { get; set; }

        [Required]
        public string AddressLine1 { get; set; }

        public string AddressLine2 { get; set; }

        [Required]
        public string City { get; set; }

        [Required]
        public string State { get; set; }

        [Required]
        [DataType(DataType.PostalCode)]
        public string ZipCode { get; set; }

        [Required]
        [Phone]
        public string PhoneNumber { get; set; }

        [Required]
        public decimal OrderFee { get; set; }
    }

    public class RestaurantEditOrderViewModel
    {
        [Required]
        public int OrderId { get; set; }

        [Required]
        public string AddressLine1 { get; set; }

        public string AddressLine2 { get; set; }

        [Required]
        public string City { get; set; }

        [Required]
        public string State { get; set; }

        [Required]
        [DataType(DataType.PostalCode)]
        public string ZipCode { get; set; }
    }

    public class RestaurantOrdersHistoryViewModel
    {
        [Required]
        public int OrderId { get; set; }

        [Required]
        public bool IsReviewed { get; set; }

        [Required]
        [Display(Name = "Deliveryman Name")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Description")]
        public string Note { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        public DateTime PlacedTime { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        public DateTime PickUpTime { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        public DateTime DeliveredTime { get; set; }

        [Required]
        [Display(Name = "Delivery Fee")]
        public decimal DeliveryFee { get; set; }
    }

    public class RestaurantAddBalanceViewModel
    {
        [Required]
        public int RestaurantId { get; set; }

        [Required]
        public decimal Balance { get; set; }
    }
}