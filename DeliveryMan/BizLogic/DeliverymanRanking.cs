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
                if (avgReview.ReviewText != null)
                { // if restaurant wrote a review text
                    theReviewText = avgReview.ReviewText;

                    return theReviewText;
                }
                else
                { // no review text
                    return "Restaurant did not write a Review!";
                }
            }
            else if (lastReview != null)
            {
                if (lastReview.ReviewText != null)
                { // review text exist
                    theReviewText = lastReview.ReviewText;

                    return theReviewText;
                }
                else
                {
                    return "Restaurant did not write a Review!";
                }
            }
            else
            { // none of the above
                return "No Reviews Available!";
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
