using DataLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BizLogic
{
    public static class CancelOrderFee
    {
        public static decimal calculateFee(this Order order)
        {
            decimal fee = 0;

            if (order.Status == Status.WAITING)
                fee = 0;
            else if (order.Status == Status.PENDING)
                fee = order.DeliveryFee * Convert.ToDecimal(0.8);
            else if (order.Status == Status.PENDING)
                fee = order.DeliveryFee * Convert.ToDecimal(1.5);

            return fee;
        }
    }
}
