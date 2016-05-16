using DataLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BizLogic
{
    public static class Score
    {
        public static int calculateScore(this Review review)
        {
            /// <summary>
            ////when deliverman got 1st review, he/she will get a start score 1500
            ////in this and later review, 
            ////score +3 for a 5-star, +1 for a 4-star, 0 for a 3-star, -1 for a 2-star, -3 for a 1-star
            /// <summary>
            /// <param name="the deliveryman in question"></param>
            /// <param name=" his/her current review rating from restaurant"></param>
            /// <returns>new score</returns>

            int oldScore = review.Order.Deliveryman.Ranking;
            int newScore = oldScore;
            int curScore = 0;

            switch (review.Rating)
            {
                case 1:
                    curScore = -3;
                    break;
                case 2:
                    curScore = -1;
                    break;
                case 4:
                    curScore = 1;
                    break;
                case 5:
                    curScore = 3;
                    break;
                default:
                    curScore = 0;
                    break;
            }

            if (oldScore == 0)
            { // meaning this is the deliveryman's first review
                newScore = 1500 + curScore;
            }

            else
            { // calculate new score
                newScore = oldScore + curScore;
            }

            return newScore;
        }
    }
}
