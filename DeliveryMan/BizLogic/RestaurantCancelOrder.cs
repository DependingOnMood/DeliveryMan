using DataLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BizLogic
{
    public static class RestaurantCancelOrder
    {
        public static decimal cancellationFee(this Order order)
        {
            switch (order.Status)
            {
                case Status.WAITING:
                    return 0M;
                case Status.PENDING:
                    if (order.PlacedTime.AddMinutes(5) <= DateTime.Now)
                    {
                        return 0M;
                    }
                    else
                    {
                        return feeCalculation(order.DeliveryFee);
                    }
                case Status.INPROGRESS:
                    return 0M;
                case Status.DELIVERED:
                    return 0M;
                default:
                    return 0M;
            }
        }

        private static decimal feeCalculation(decimal deliveryFee)
        {
            return deliveryFee / 2;
        }
    }
}
