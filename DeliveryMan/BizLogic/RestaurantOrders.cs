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
        public static bool waiting(this RestaurantOrder order)
        {
            if (order.status == "Waiting")
                return true;
            else
                return false;
        }

        public static bool pending(this RestaurantOrder order)
        {
            if (order.status == "Pending")
                return true;
            else
                return false;
        }

        public static bool inProgress(this RestaurantOrder order)
        {
            if (order.status == "InProgress")
                return true;
            else
                return false;
        }

        public static bool delivered(this RestaurantOrder order)
        {
            if (order.status == "Delivered")
                return true;
            else
                return false;
        }
    }
}
