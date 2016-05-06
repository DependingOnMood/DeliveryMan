using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BizLogic
{
    public static class RestaurantCancelOrder
    {
        public static void isWaiting(this RestaurantOrder order)
        {
            if (order.status == "Waiting")
                isWaiting = true;
            else
                isWaiting = false;
        }

        public static void isPending(this RestaurantOrder order)
        {
            if (order.status == "Pending")
                isPending = true;
            else
                isPending = false;
        }

        public static void isInProgress(this RestaurantOrder order)
        {
            if (order.status == "InProgress")
                isInProgress = true;
            else
                isInProgress = false;
        }
        public static void isDelivered(this RestaurantOrder order)
        {
            if (order.status == "Delivered")
                isDelivered = true;
            else
                isDelivered = false;
        }

        public static void cancelLock(this RestaurantOrder order)
        {
            if (order.status == "InProgress")
                cancelLock = true;
            else
                cancelLock = false;
        }

        public static double cancellationFee(this RestaurantOrder order)
        {
            if (isWaiting(order))
            {
                return 0;
            }

            if (isPending(order)
            {
                if (order.PlacedTime.AddMinutes(5) <= DateTime.Now)
                {
                    return 0;
                }
                else
                {
                    return feeCalculation(distance, order.DeliveryFee);
                }
            }
        }

        public void feeCalculation(double distance, double deliveryFee)
        {

        }
    }
}
