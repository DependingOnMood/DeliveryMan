using DataLayer;
using DeliveryMan.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DeliveryMan.Controllers
{
    public class HomeController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public int GetRole()
        {
            var q = (
            from c in db.contacts
            where c.Email.Equals(User.Identity.Name)
            select c).FirstOrDefault();

            return (int)q.Role;
            //return 0;

        }

        // rank deliveryman by his/her cumulative rating
        public ActionResult Ranking()
        {
            if (User.Identity.IsAuthenticated)
            {
                ViewBag.UserType = GetRole();
            }

            // get deliveryman ranking
            IEnumerable<Deliveryman> deliveryman = (from d in db.deliverymen
                                                    where d.Ranking != 0
                                                    select d).OrderByDescending(x => x.Ranking);

            int count = deliveryman.Count();
            List<DeliverymanRankingViewModel> rankingVMs = 
                new List<DeliverymanRankingViewModel>(new DeliverymanRankingViewModel[count]);

            for (int i = 0; i < Math.Min(count, 20); i++)
            {
                Deliveryman curDeliveryman = deliveryman.Skip(i).First();
                rankingVMs[i] = new DeliverymanRankingViewModel();

                int avgReview = Convert.ToInt32(curDeliveryman.Rating);

                // get a review reflecting the deliveryman's current rating
                rankingVMs[i].Rank = curDeliveryman.Ranking;
                rankingVMs[i].DeliverymanName = curDeliveryman.FirstName + " " + curDeliveryman.LastName;
                rankingVMs[i].TotalOrders = curDeliveryman.TotalDeliveryCount;
                rankingVMs[i].Rating = curDeliveryman.Rating;

                Review DeliverymanReview = (from r in db.reviews
                                            where r.order.Deliveryman.Id == curDeliveryman.Id
                                            where r.order.Deliveryman.Rating == avgReview
                                            select r).FirstOrDefault();

                if (DeliverymanReview != null)
                {
                    rankingVMs[i].ReviewText = DeliverymanReview.ReviewText;
                }
            }

            return View("Ranking", rankingVMs);
        }

        public ActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                ViewBag.UserType = GetRole();
            }
            return View();
        }

        public ActionResult About()
        {
            if (User.Identity.IsAuthenticated)
            {
                ViewBag.UserType = GetRole();
            }
            return View();
        }

        public ActionResult Contact()
        {
            if (User.Identity.IsAuthenticated)
            {
                ViewBag.UserType = GetRole();
            }
            return View();
        }
    }
}