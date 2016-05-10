using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using DataLayer;
using BizLogic;
using DeliveryMan.Models;

namespace DeliveryMan.Controllers
{
    public class RestaurantController : Controller
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

        // GET: Restaurant/CreateOrder
        public ActionResult CreateOrder()
        {
            if (User.Identity.IsAuthenticated)
            {
                ViewBag.UserType = GetRole();
            }

            return View();
        }

        // POST: Restaurant/CreateOrder
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateOrder([Bind(Include = "Note, AddressLine1, AddressLine2, City, State, ZipCode, PhoneNumber, OrderFee")]
            RestaurantCreateOrderViewModel model)
        {
            GoogleMapHelper helper = null;
            Restaurant res = (from r in db.restaurants
                              where r.Contact.Email.Equals(User.Identity.Name)
                              select r).FirstOrDefault();
            if (res == null)
            {
                return HttpNotFound();
            }
            Contact contact = (from c in db.contacts
                               where c.PhoneNumber.Equals(model.PhoneNumber)
                               select c).FirstOrDefault();
            String totalAddress = model.AddressLine1 + " " + model.AddressLine2 + " " + model.City + " " + model.State + " " + model.ZipCode;
            if (contact == null)
            {
                contact = new Contact()
                {
                    PhoneNumber = model.PhoneNumber,
                    Role = Role.CUSTOMER,
                    AddressLine1 = model.AddressLine1,
                    AddressLine2 = model.AddressLine2,
                    City = model.City,
                    State = model.State,
                    ZipCode = model.ZipCode,
                };
                helper = new GoogleMapHelper();
                String latAndLong = helper.getLatandLngByAddr(totalAddress);
                contact.Latitude = Decimal.Parse(latAndLong.Split(' ')[0]);
                contact.Longitude = Decimal.Parse(latAndLong.Split(' ')[1]);
                db.contacts.Add(contact);
            }
            else
            {
                bool changed = false;
                if (!contact.AddressLine1.Equals(model.AddressLine1))
                {
                    contact.AddressLine1 = model.AddressLine1;
                    changed = true;
                }
                if (model.AddressLine2 != null && contact.AddressLine2 != null && !contact.AddressLine2.Equals(model.AddressLine2))
                {
                    contact.AddressLine2 = model.AddressLine2;
                    changed = true;
                }
                if (!contact.City.Equals(model.City) || !contact.State.Equals(model.State) || !contact.ZipCode.Equals(model.ZipCode))
                {
                    contact.City = model.City;
                    contact.State = model.State;
                    contact.ZipCode = model.ZipCode;
                    changed = true;
                }
                if (changed)
                {
                    helper = new GoogleMapHelper();
                    String latAndLong = helper.getLatandLngByAddr(totalAddress);
                    contact.Latitude = Decimal.Parse(latAndLong.Split(' ')[0]);
                    contact.Longitude = Decimal.Parse(latAndLong.Split(' ')[1]);
                }
            }
            db.SaveChanges();
            helper = new GoogleMapHelper();
            string loc1 = res.Contact.getAddress();
            string loc2 = totalAddress;
            double distance = CreateOrderLogic.getRealDistance(loc1, loc2);
            Order order = new Order()
            {
                RestaurantId = res.Id,
                Restaurant = res,
                Status = Status.WAITING,
                Note = model.Note,
                PlacedTime = DateTime.Now,
                ContactId = contact.PhoneNumber,
                Contact = contact,
                DeliveryFee = CreateOrderLogic.computePrice(distance, model.OrderFee),
                ETA = CreateOrderLogic.getETA(loc1, loc2),
            };
            db.orders.Add(order);
            db.SaveChanges();
            return RedirectToAction("Orders");
        }

        // GET: Restaurant
        public ActionResult Orders()
        {
            if (User.Identity.IsAuthenticated)
            {
                ViewBag.UserType = GetRole();
            }

            Restaurant res = (from r in db.restaurants
                              where r.Contact.Email.Equals(User.Identity.Name)
                              select r).FirstOrDefault();
            if (res == null)
            {
                return HttpNotFound();
            }
            IEnumerable<Order> wo = from o in db.orders
                                    where o.RestaurantId == res.Id
                                    where o.Status == Status.WAITING
                                    orderby o.PlacedTime descending
                                    select o;
            IEnumerable<Order> po = from o in db.orders
                                    where o.RestaurantId == res.Id
                                    where o.Status == Status.PENDING
                                    orderby o.PlacedTime descending
                                    select o;
            IEnumerable<Order> io = from o in db.orders
                                    where o.RestaurantId == res.Id
                                    where o.Status == Status.INPROGRESS
                                    orderby o.PlacedTime descending
                                    select o;
            RestaurantOrdersViewModel rovm = new RestaurantOrdersViewModel()
            {
                WaitingOrders = wo,
                PendingOrders = po,
                InProgressOrders = io,
            };
            return View(rovm);
        }

        // GET: Restaurant/PickUpOrder/5
        public ActionResult PickUpOrder(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Restaurant res = (from r in db.restaurants
                              where r.Contact.Email.Equals(User.Identity.Name)
                              select r).FirstOrDefault();
            if (res == null)
            {
                return HttpNotFound();
            }
            Order order = (from o in db.orders
                           where o.RestaurantId == res.Id
                           where o.Id == id
                           where o.Status == Status.PENDING
                           select o).FirstOrDefault();
            if (order == null)
            {
                return HttpNotFound();
            }
            order.Status = Status.INPROGRESS;
            db.SaveChanges();
            return RedirectToAction("Orders");
        }

        // GET: Restaurant/OrderDetails/5
        public ActionResult OrderDetails(int? id)
        {
            if (User.Identity.IsAuthenticated)
            {
                ViewBag.UserType = GetRole();
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Restaurant res = (from r in db.restaurants
                              where r.Contact.Email.Equals(User.Identity.Name)
                              select r).FirstOrDefault();
            if (res == null)
            {
                return HttpNotFound();
            }
            Order order = (from o in db.orders
                           where o.RestaurantId == res.Id
                           where o.Id == id
                           select o).FirstOrDefault();
            if (order == null)
            {
                return HttpNotFound();
            }



            return View(order);
        }

        // GET: Restaurant/EditOrder/5
        public ActionResult EditOrder(int? id)
        {
            if (User.Identity.IsAuthenticated)
            {
                ViewBag.UserType = GetRole();
            }

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Restaurant res = (from r in db.restaurants
                              where r.Contact.Email.Equals(User.Identity.Name)
                              select r).FirstOrDefault();
            if (res == null)
            {
                return HttpNotFound();
            }
            Order order = (from o in db.orders
                           where o.RestaurantId == res.Id
                           where o.Id == id
                           select o).FirstOrDefault();
            RestaurantEditOrderViewModel model = new RestaurantEditOrderViewModel()
            {
                OrderId = order.Id,
                AddressLine1 = order.Contact.AddressLine1,
                AddressLine2 = order.Contact.AddressLine2,
                City = order.Contact.City,
                State = order.Contact.State,
                ZipCode = order.Contact.ZipCode,
            };
            return View(model);
        }

        // POST: Restaurant/EditOrder/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditOrder([Bind(Include = "OrderId, AddressLine1, AddressLine2, City, State, ZipCode")]
            RestaurantEditOrderViewModel model)
        {
            if (User.Identity.IsAuthenticated)
            {
                ViewBag.UserType = GetRole();
            }

            Restaurant res = (from r in db.restaurants
                              where r.Contact.Email.Equals(User.Identity.Name)
                              select r).FirstOrDefault();
            if (res == null)
            {
                return HttpNotFound();
            }
            Order order = (from o in db.orders
                           where o.RestaurantId == res.Id
                           where o.Id == model.OrderId
                           select o).FirstOrDefault();
            order.Contact.AddressLine1 = model.AddressLine1;
            order.Contact.AddressLine2 = model.AddressLine2;
            order.Contact.City = model.City;
            order.Contact.State = model.State;
            order.Contact.ZipCode = model.ZipCode;
            db.SaveChanges();
            return RedirectToAction("Orders");
        }

        // GET: Restaurant/CancelOrder/5
        public ActionResult CancelOrder(int? id)
        {
            if (User.Identity.IsAuthenticated)
            {
                ViewBag.UserType = GetRole();
            }

            var orderDetails = (from o in db.orders
                                where o.Restaurant.Contact.Email.Equals(User.Identity.Name)
                                where o.Id == id
                                select o).FirstOrDefault();

            CancelOrderViewModel cancelOrderVM = new CancelOrderViewModel();

            cancelOrderVM.OrderId = orderDetails.Id;
            cancelOrderVM.OrderName = orderDetails.Note;
            cancelOrderVM.OrderStatus = orderDetails.Status;

            cancelOrderVM.ETA = orderDetails.ETA;
            cancelOrderVM.PlacedTime = orderDetails.PlacedTime;
            cancelOrderVM.PickUpTime = orderDetails.PickUpTime;
            cancelOrderVM.DeliveryFee = orderDetails.DeliveryFee;

            // calculate cancellation fee
            cancelOrderVM.CancellationFee = RestaurantCancelOrder.cancellationFee(orderDetails);

            return View("CancelOrder", cancelOrderVM);
        }

        // POST: Restaurant/CancelOrder/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CancelOrder(CancelOrderViewModel model, int id)
        {
            if (User.Identity.IsAuthenticated)
            {
                ViewBag.UserType = GetRole();
            }

            Order curOrder = (from o in db.orders
                              where o.Restaurant.Contact.Email == User.Identity.Name
                              && o.Id == id
                              select o).FirstOrDefault();

            if (curOrder == null)
            {
                return HttpNotFound();
            }

            if (curOrder.Status != Status.DELIVERED)
            {
                db.orders.Remove(curOrder);
                db.SaveChanges();

                return RedirectToAction("Orders");
            }
            else
            {
                ModelState.AddModelError("", "A Delivered Order can not be cancelled");
                return View(model);
            }
        }

        // GET: Restaurant/ReviewOrder/5
        public ActionResult ReviewOrder(int? id)
        {
            if (User.Identity.IsAuthenticated)
            {
                ViewBag.UserType = GetRole();
            }

            var orderDetails = (from o in db.orders
                                where o.Restaurant.Contact.Email == User.Identity.Name
                                where o.Id == id
                                select o).FirstOrDefault();

            ReviewOrderViewModel reviewOrderVM = new ReviewOrderViewModel();

            reviewOrderVM.OrderName = orderDetails.Note;
            reviewOrderVM.PlacedTime = orderDetails.PlacedTime;
            reviewOrderVM.PickUpTime = orderDetails.PickUpTime;
            reviewOrderVM.DeliveredTime = orderDetails.DeliveredTime;
            reviewOrderVM.OrderId = orderDetails.Id;

            return View("ReviewOrder", reviewOrderVM);
        }

        // POST: Restaurant/ReviewOrder/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ReviewOrder(ReviewOrderViewModel model, int? id)
        {
            if (ModelState.IsValid && id != null)
            { // check model state
              //   if (model.DeliveredTime != null)
              //  { // only allow delivered order to be reviewed
                Review newReview = new Review();
                newReview.OrderId = (int)id;
                newReview.ReviewText = model.ReviewText;
                newReview.Rating = model.Rating;

                // calculate deliveryman's current rating
                var orderDeliveryman = (from o in db.orders
                                        select o).FirstOrDefault();

                Deliveryman deliveryman = db.deliverymen.Find(orderDeliveryman.DeliverymanId);

                // calculate cumulative rating
                decimal rating = ReviewRating.calculateRating(deliveryman, newReview.Rating);

                // update deliveryman table
                deliveryman.Rating = rating;
                deliveryman.TotalStarsEarned = deliveryman.TotalStarsEarned + newReview.Rating;
                deliveryman.TotalDeliveryCount = deliveryman.TotalDeliveryCount + 1;

                db.reviews.Add(newReview);

                db.SaveChanges();

                // update deliveryman ranking
                var rankedDmans = (from d in db.deliverymen
                                   select d).OrderByDescending(x => x.Rating);

                Deliveryman prevDman = new Deliveryman();

                for (int i = 0; i < rankedDmans.Count(); i++)
                {
                    Deliveryman curDman = rankedDmans.Skip(i).First();

                    Deliveryman rankedDman = db.deliverymen.Find(curDman.Id);

                    rankedDman.Ranking = DeliverymanRanking.getRank(i, curDman, prevDman);

                    prevDman = curDman;
                }

                db.SaveChanges();

                return RedirectToAction("Orders");
            }
            //   else
            //    {
            //        ModelState.AddModelError("", "You cannot review an order that has not yet been delivered!");
            //        return View(model);
            //    }

            return View(model);
        }

        // GET: Restaurant/OrdersHistory
        public ActionResult OrdersHistory()
        {
            if (User.Identity.IsAuthenticated)
            {
                ViewBag.UserType = GetRole();
            }

            Restaurant res = (from r in db.restaurants
                              where r.Contact.Email.Equals(User.Identity.Name)
                              select r).FirstOrDefault();
            if (res == null)
            {
                return HttpNotFound();
            }
            IEnumerable<Order> orders = (from o in db.orders
                                         where o.RestaurantId == res.Id
                                         where o.Status == Status.DELIVERED
                                         orderby o.PlacedTime descending
                                         select o).ToList<Order>();
            List<RestaurantOrdersHistoryViewModel> models = new List<RestaurantOrdersHistoryViewModel>();
            foreach (Order o in orders)
            {
                RestaurantOrdersHistoryViewModel model = new RestaurantOrdersHistoryViewModel()
                {
                    OrderId = o.Id,
                    Name = o.Deliveryman.getName(),
                    Note = o.Note,
                    PlacedTime = o.PlacedTime,
                    PickUpTime = (DateTime)o.PickUpTime,
                    DeliveredTime = (DateTime)o.DeliveredTime,
                    DeliveryFee = o.DeliveryFee,
                };
                Review rev = (from rv in db.reviews
                              where rv.OrderId == o.Id
                              select rv).FirstOrDefault();
                if (rev == null)
                {
                    model.IsReviewed = false;
                }
                else
                {
                    model.IsReviewed = true;
                }
                models.Add(model);
            }
            return View(models);
        }

        // GET: Restaurant/Blacklist
        public ActionResult Blacklist()
        {
            if (User.Identity.IsAuthenticated)
            {
                ViewBag.UserType = GetRole();
            }

            Restaurant res = (from r in db.restaurants
                              where r.Contact.Email.Equals(User.Identity.Name)
                              select r).FirstOrDefault();
            if (res == null)
            {
                return HttpNotFound();
            }
            IEnumerable<Deliveryman> badGuys = from bl in db.blacklists
                                               where bl.RestaurantId == res.Id
                                               select bl.Deliveryman;
            return View(badGuys);
        }

        // GET: Restaurant/DeliverymanDetails/5
        public ActionResult DeliverymanDetails(int? id)
        {
            if (User.Identity.IsAuthenticated)
            {
                ViewBag.UserType = GetRole();
            }

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Restaurant res = (from r in db.restaurants
                              where r.Contact.Email.Equals(User.Identity.Name)
                              select r).FirstOrDefault();
            if (res == null)
            {
                return HttpNotFound();
            }
            Deliveryman deliveryman = (from dm in db.deliverymen
                                       where dm.Id == id
                                       select dm).FirstOrDefault();
            if (deliveryman == null)
            {
                return HttpNotFound();
            }
            Order order = (from o in db.orders
                           where o.DeliverymanId == deliveryman.Id
                           where o.RestaurantId == res.Id
                           select o).FirstOrDefault();
            if (order != null)
            {
                return View(deliveryman);
            }
            else
            {
                return HttpNotFound();
            }
        }

        // GET: Restaurant/AddToBlacklist/5
        public ActionResult AddToBlacklist(int? id)
        {
            if (User.Identity.IsAuthenticated)
            {
                ViewBag.UserType = GetRole();
            }

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            return View();
        }

        // POST: Restaurant/AddToBlacklist/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddToBlacklist(int id)
        {
            if (User.Identity.IsAuthenticated)
            {
                ViewBag.UserType = GetRole();
            }

            Restaurant res = (from r in db.restaurants
                              where r.Name.Equals(User.Identity.Name)
                              select r).FirstOrDefault();
            if (res == null)
            {
                return HttpNotFound();
            }
            if (ModelState.IsValid)
            {
                Deliveryman badGuy = db.deliverymen.Find(id);
                if (badGuy == null)
                {
                    return HttpNotFound();
                }
                else
                {
                    Blacklist bl = new Blacklist()
                    {
                        RestaurantId = res.Id,
                        DeliverymanId = badGuy.Id,
                    };
                    db.blacklists.Add(bl);
                    db.SaveChanges();
                }
                return RedirectToAction("Blacklist");
            }
            //ViewBag.CId = new SelectList(db.contact, "CId", "Name", restaurant.CId);
            return View();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
