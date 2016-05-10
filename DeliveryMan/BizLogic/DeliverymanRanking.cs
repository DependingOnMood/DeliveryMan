using DataLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BizLogic
{
    // sort deliveryman by descending
    public static class DeliverymanRanking
    {
        public static List<Deliveryman> RankDeliveryman(this IQueryable<Deliveryman> allDeliveryman)
        {
            List<Deliveryman> deliverymanList = allDeliveryman.ToList();

            deliverymanList.Sort();

            return deliverymanList;
        }
    }
}
