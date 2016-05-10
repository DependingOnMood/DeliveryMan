using DataLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BizLogic
{
    public static class ReviewRating
    {

        /// <summary>
        /// calculate the cumulative rating for deliveryman
        /// </summary>
        /// <param name="the deliveryman in question"></param>
        /// <param name=" his/her current review rating from restaurant"></param>
        /// <returns>new rating</returns>

        public static decimal calculateRating(this Deliveryman deliveryman, int curStars)
        {
            decimal curRating = 0;
            decimal prevRating = deliveryman.Rating;
            int totalStars = deliveryman.TotalStarsEarned;
            int totalDelivery = deliveryman.TotalDeliveryCount;

            if (prevRating == 0)
            { // meaning this is the deliveryman's first review
                curRating = curStars;
            } else
            { // calculate cumulative rating
                curRating = (totalStars + curStars) / (totalDelivery + 1);
            }

            return curRating;
        }

    }
}
