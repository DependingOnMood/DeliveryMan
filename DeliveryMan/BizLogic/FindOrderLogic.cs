using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BizLogic
{
    public class FindOrderLogic
    {
        public Boolean selectOrderByDistance(double distance, double res) {
            if (distance < 3.01)
            {
                if (res <= distance)
                {
                    return true;
                }
                else {
                    return false;
                }
            }
            else {
                return true;
            }
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
