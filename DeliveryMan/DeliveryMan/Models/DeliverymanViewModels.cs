using DataLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DeliveryMan.Models
{
    public class DeliverymanMyOrdersViewModel
    {
        public decimal Balance { get; set; }

        public IEnumerable<Order> pendingOrders { get; set; }

        public IEnumerable<Order> inProgressOrders { get; set; }

        public IEnumerable<Order> deliveredOrders { get; set; }

    }

    public class DeliverymanCancelPickupViewModel
    {
        public int OrderId { get; set; }

        public string OrderNote { get; set; }

        public string RestaurantName { get; set; }

        public Status OrderStatus { get; set; }

        public string ETA { get; set; }

        public DateTime PlacedTime { get; set; }

        public DateTime? PickUpTime { get; set; }

        public decimal DeliveryFee { get; set; }

        public decimal CancellationFee { get; set; }
    }
}