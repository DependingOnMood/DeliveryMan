using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BizLogic
{
    class FindOrders
    {
        public void selectOrderByDistance(int distance ) {
            //need database connection
        }

        public double ComputeDistanceBetweenAandB(String addrA, String addrB) {
            GoogleMapHelper helper = new GoogleMapHelper();
            double res;
            String loc1 = helper.getLatandLngByAddr(addrA);
            String loc2 = helper.getLatandLngByAddr(addrB);
            res =  helper.computeDistanceByLocation(loc1, loc2);
            return res;
        }

    }
}
