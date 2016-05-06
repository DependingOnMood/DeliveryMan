using DataLayer;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BizLogic
{
    /// <summary>
    /// work in progress
    /// </summary>
    public static class RestaurantOrders
    {
        public static bool waiting(this Order order)
        {
            if (order.Status == Status.WAITING)
                return true;
            else
                return false;
        }

        public static bool pending(this Order order)
        {
            if (order.Status == Status.PENDING)
                return true;
            else
                return false;
        }

        public static bool inProgress(this Order order)
        {
            if (order.Status == Status.INPROGRESS)
                return true;
            else
                return false;
        }

        public static bool delivered(this Order order)
        {
            if (order.Status == Status.DELIVERED)
                return true;
            else
                return false;
        }
    }
}
