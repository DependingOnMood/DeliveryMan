﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BizLogic
{
    public static class CreateOrderLogic
    {
        public static decimal computePrice( double distance, decimal orderFee)
        {
            decimal price = 0;
            if (distance >= 0 && distance < 1)
            {
                price = 0.15M * orderFee;
            }
            else if (distance >= 1 && distance < 3.01)
            {
                price = 0.20M * orderFee;
            }
            else {
                price = 0.25M * orderFee;
            }

            return price;
        }

        public static double getRealDistance( String addr1, String addr2)
        {
            GoogleMapHelper helper = new GoogleMapHelper();
            String str = helper.getRoute(addr1, addr2);
            str = str.Split('#')[3];
            double res = Double.Parse(str.Split(' ')[0]);
            return res;
        }

        public static String getETA( String addr1, String addr2)
        {
            GoogleMapHelper helper = new GoogleMapHelper();
            String str = helper.getRoute(addr1, addr2);
            str = str.Split('#')[1];           
            return str;
        }

    }
}
