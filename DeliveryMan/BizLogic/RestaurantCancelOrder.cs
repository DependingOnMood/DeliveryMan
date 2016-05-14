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
                    DateTime pickUpTime = order.PickUpTime.Value;
                    if (pickUpTime.AddMinutes(5) >= DateTime.Now)
                    {
                        return 0M;
                    }
                    else
                    {
                        return pendingFeeCalculation(order.DeliveryFee);
                    }
                case Status.INPROGRESS:
                    return inprogressFeeCalculation(order.DeliveryFee); ;
                case Status.DELIVERED:
                    return 0M;
                default:
                    return 0M;
            }
        }

        private static decimal pendingFeeCalculation(decimal deliveryFee)
        {
            return deliveryFee * Convert.ToDecimal(0.8);
        }

        private static decimal inprogressFeeCalculation(decimal deliveryFee)
        {
            return deliveryFee * Convert.ToDecimal(1.5);
        }
    }
}
