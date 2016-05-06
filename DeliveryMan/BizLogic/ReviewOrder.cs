using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BizLogic
{
    public static class ReviewOrder
    {
        public static double calculatScore(this Review review)
        {
            double score = 120 - Math.Round(1 / (1 + Math.Pow(10, (review.Rating / 400))) * 120);
            return score;
        }

        public static double calculateWeight(this Review review)
        {
            return review.ratingScore / 10; 
        }

        public static ratingScore (this Review review)
        {
            double score = calculateScore(review.Rating);
            double weight = calculateWeight(review.Rating);

            double result = (120 - Math.Round(1 / (1 + Math.Pow(10, ((score -weight) / 400))) * 120))
                - (120 - Math.Round(1 / (1 + Math.Pow(10, ((score - weight) / 400))) * 120));

            return result;
        }

        public static double ratingS(this RestaurantOrder order)
        {
            if (order == null)
            {
                return 0;
            }
            else {
                double max = Math.Max(order.toList());
                return max * order.Rating;
            }
        }

        public static bool overflow(this Restaurant order)
        {
            if (order.Rating != 0 && order.Rating != 1 && order.Rating != 2
                && order.Rating != 3 && order.Rating != 4 && order.Rating != 5)
                return true;
            else
                return false;
        }

        public static double getEloRating(this Retaurant order)
        {
            EloRating(order.Rating);
        }
    }
}
