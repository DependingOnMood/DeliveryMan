using DataLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BizLogic
{
    public static class DeliverymanTime
    {

        // calculate the average delivery time for a deliveryman
        public static double averageTime(int times, double totalTime)
        {
            double total = times;
            double averageTime = 0.0;

            if (totalTime != 0 && total != 0)
            {
                averageTime = totalTime / total;

                return averageTime;
            }
            else if (totalTime == 0)
            {
                return 0;
            }
            else
            {
                return 0;
            }
        }
    }
}
