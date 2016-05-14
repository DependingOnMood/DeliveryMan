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
        public static decimal calculateScore(this Deliveryman deliveryman, int curStars)
        {
            /// <summary>
            ////when deliverman got 1st review, he/she will get a start score 1500
            ////in this and later review, 
            ////score +3 for a 5-star, +1 for a 4-star, -1 for a 2-star, -3 for a 1-star
            /// <summary>
            /// <param name="the deliveryman in question"></param>
            /// <param name=" his/her current review rating from restaurant"></param>
            /// <returns>new score</returns>
            int oldScore = deliveryman.Ranking;
            int newScore = oldScore;
            int curScore = 0;

            if (curStars == 1) { curScore = -3; }
            else if (curStars == 2) { curScore = -1; }
            else if (curStars == 4) { curScore = 1; }
            else if (curStars == 5) { curScore = 3; }


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
