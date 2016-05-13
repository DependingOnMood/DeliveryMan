using DataLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BizLogic
{
    public static class DeliverymanRanking
    {
        // get average review, and if one dosen't exist, return the last review
        public static string getReview(Review avgReview, Review lastReview)
        {
            string theReviewText;

            if (avgReview != null)
            { // found a review text matching his average rating
                theReviewText = avgReview.ReviewText;

                return theReviewText;
            }
            else if (lastReview != null)
            {
                theReviewText = lastReview.ReviewText;

                return theReviewText;
            }
            else
            {
                theReviewText = "No Reviews Available!";

                return theReviewText;
            }
        }

        // compare and return deliveryman ranking
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
