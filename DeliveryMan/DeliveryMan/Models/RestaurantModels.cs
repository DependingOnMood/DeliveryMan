using DataLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DeliveryMan.Models
{
    public class RestaurantOrdersViewModel
    {
        public IEnumerable<Order> WaitingOrders { get; set; }

        public IEnumerable<Order> PendingOrders { get; set; }

        public IEnumerable<Order> InProgressOrders { get; set; }
    }
}