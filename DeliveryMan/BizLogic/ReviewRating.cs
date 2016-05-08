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
        public static double calculateScore(this Review review)
        {
            double score = 120 - Math.Round(1 / (1 + Math.Pow(10, (review.Rating / 400))) * 120);
            return score;
        }

        public static double calculateWeight(this Review review)
        {
            return review.Rating / 10; 
        }
    }
}
