using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using DataLayer;
using DeliveryMan.Models;

namespace DeliveryMan.Controllers
{
    public class RestaurantController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Restaurant/CreateOrder
        public ActionResult CreateOrder()
        {
            return View();
        }

        // POST: Restaurant/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateOrder([Bind(Include = "")] Order order)
        {
            return RedirectToAction("Orders");
        }

        // GET: Restaurant
        public ActionResult Orders()
        {
            Restaurant res = (from r in db.restaurants
                              where r.Name.Equals(User.Identity.Name)
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

        // GET: Restaurant/OrderDetails/5
        public ActionResult OrderDetails(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Restaurant res = (from r in db.restaurants
                              where r.Name.Equals(User.Identity.Name)
                              select r).FirstOrDefault();
            if (res == null)
            {
                return HttpNotFound();
            }
            Order order = (from o in db.orders
                           where o.RestaurantId == res.Id
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
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Restaurant res = (from r in db.restaurants
                              where r.Name.Equals(User.Identity.Name)
                              select r).FirstOrDefault();
            if (res == null)
            {
                return HttpNotFound();
            }
            Order order = (from o in db.orders
                           where o.RestaurantId == res.Id
                           select o).FirstOrDefault();

            if (order == null)
            {
                return HttpNotFound();
            }

            //ViewBag.CId = new SelectList(db.contact, "CId", "Name", restaurant.CId);
            return View(order);
        }

        // POST: Restaurant/EditOrder/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditOrder([Bind(Include = "")] Order order)
        {
            if (ModelState.IsValid)
            {
                db.Entry(order).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Orders");
            }
            //ViewBag.CId = new SelectList(db.contact, "CId", "Name", restaurant.CId);
            return View(order);
        }

        // GET: Restaurant/CancelOrder/5
        public ActionResult CancelOrder(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Restaurant res = (from r in db.restaurants
                              where r.Name.Equals(User.Identity.Name)
                              select r).FirstOrDefault();
            if (res == null)
            {
                return HttpNotFound();
            }
            Order order = (from o in db.orders
                           where o.RestaurantId == res.Id
                           select o).FirstOrDefault();

            if (order == null)
            {
                return HttpNotFound();
            }
            return View(order);
        }

        // POST: Restaurant/CancelOrder/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult CancelOrder(int id)
        {
            Restaurant res = (from r in db.restaurants
                              where r.Name.Equals(User.Identity.Name)
                              select r).FirstOrDefault();
            if (res == null)
            {
                return HttpNotFound();
            }
            Order order = (from o in db.orders
                           where o.RestaurantId == res.Id
                           select o).FirstOrDefault();

            if (order == null)
            {
                return HttpNotFound();
            }
            db.orders.Remove(order);
            db.SaveChanges();
            return RedirectToAction("Orders");
        }

        // GET: Restaurant/ReviewOrder/5
        public ActionResult ReviewOrder()
        {
            return View();
        }

        // POST: Restaurant/ReviewOrder/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ReviewOrder([Bind(Include = "")] Review review)
        {
            if (ModelState.IsValid)
            {
                db.reviews.Add(review);
                db.SaveChanges();
                return RedirectToAction("Orders");
            }

            //ViewBag.CId = new SelectList(db.contact, "CId", "Name", restaurant.CId);
            return View(review);
        }

        // GET: Restaurant/OrdersHistory
        public ActionResult OrdersHistory()
        {
            Restaurant res = (from r in db.restaurants
                              where r.Name.Equals(User.Identity.Name)
                              select r).FirstOrDefault();
            if (res == null)
            {
                return HttpNotFound();
            }
            IEnumerable<Order> orders = from o in db.orders
                                        where o.RestaurantId == res.Id
                                        where o.Status == Status.DELIVERED
                                        select o;
            return View(orders);
        }

        // GET: Restaurant/Blacklist
        public ActionResult Blacklist()
        {
            Restaurant res = (from r in db.restaurants
                              where r.Name.Equals(User.Identity.Name)
                              select r).FirstOrDefault();
            if (res == null)
            {
                return HttpNotFound();
            }
            IEnumerable<Deliveryman> blacklist = res.BadDeliverymen;
            return View(blacklist);
        }

        // GET: Restaurant/DeliverymanDetails/5
        public ActionResult DeliverymanDetails(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Deliveryman deliveryman = (from dm in db.deliverymen
                                       where dm.Id == id
                                       select dm).FirstOrDefault();
            if (deliveryman == null)
            {
                return HttpNotFound();
            }
            return View(deliveryman);
        }

        // GET: Restaurant/AddToBlacklist/5
        public ActionResult AddToBlacklist(int? id)
        {
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
                res.BadDeliverymen.Add(badGuy);
                db.SaveChanges();
                return RedirectToAction("Orders");
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
