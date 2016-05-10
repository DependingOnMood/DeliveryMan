using DataLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DeliveryMan.Models
{
    public class DeliverymanMyOrdersViewModel
    {
        public IEnumerable<Order> pendingOrders { get; set; }

        public IEnumerable<Order> inProgressOrders { get; set; }

        public IEnumerable<Order> deliveredOrders { get; set; }

    }
}