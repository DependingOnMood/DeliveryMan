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

        public ActionResult Ranking()
        {
            // get deliveryman ranking
            IEnumerable<Deliveryman> deliveryman = (from d in db.deliverymen
                                                    select d).OrderByDescending(x => x.Ranking);

            List<DeliverymanRankingViewModel> rankingVMs = new List<DeliverymanRankingViewModel>();

            for (int i = 0; i < rankingVMs.Count(); i++)
            {
                Deliveryman curDeliveryman = deliveryman.Skip(i).First();
                DeliverymanRankingViewModel curRankingVM = rankingVMs[i];

                int avgReview = Convert.ToInt32(curDeliveryman.Rating);

                // get a review reflecting the deliveryman's current rating
                Review DeliverymanReview = (from r in db.reviews
                                    where r.Restaurant.Contact.Email == User.Identity.Name
                                    where r.Id == id
                                    select r).FirstOrDefault();

                curRankingVM.Rank = curDeliveryman.Ranking;
                curRankingVM.DeliverymanName = curDeliveryman.FirstName + " " + curDeliveryman.LastName;
                curRankingVM.TotalOrders = curDeliveryman.TotalDeliveryCount;

                curRankingVM.Rating = orderDetails.PickUpTime;
                curRankingVM.ReviewText = orderDetails.DeliveredTime;
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