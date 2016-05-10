using DataLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BizLogic
{
    // compare and return deliveryman ranking
    public static class DeliverymanRanking
    {
        public static int getRank(int i, Deliveryman curDman, Deliveryman prevDman)
        {
            if (prevDman.Rating == curDman.Rating)
            {
                return i;
            }
            else
            {
                return i + 1;
            }
        }

    }
}
