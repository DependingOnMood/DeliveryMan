using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BizLogic
{
    class CreateOrderLogic
    {
        public decimal computePrice(double distance, decimal orderFee) {
            decimal price = 0;
            if (distance >= 0 && distance < 5)
            {
                price = 0.15M * orderFee;
            }
            else if (distance >= 5 && distance < 10)
            {
                price = 0.20M * orderFee;
            }
            else {
                price = 0.25M * orderFee;
            }

            return price;
        }

        public double getRealDistance(String addr1, String addr2) {
            double res = 0;
            GoogleMapHelper helper = new GoogleMapHelper();
            String str = helper.getRoute(addr1, addr2);
            res = Double.Parse(str.Split(' ')[3]);
            return res;
        }

        public DateTime getETA(String addr1, String addr2) {
            DateTime res = new DateTime();
            GoogleMapHelper helper = new GoogleMapHelper();
            String str = helper.getRoute(addr1, addr2);
            TimeSpan span = TimeSpan.Parse(str.Split(' ')[1]);
            res.Add(span);
            return res;
        }

    }
}
