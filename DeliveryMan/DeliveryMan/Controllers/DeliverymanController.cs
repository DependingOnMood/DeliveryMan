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
using BizLogic;

namespace DeliveryMan.Controllers
{
    public class DeliverymanController : Controller
    {
        public int GetRole()
        {
            var q = (
            from c in db.contacts
            where c.Email.Equals(User.Identity.Name)
            select c).FirstOrDefault();

            return (int)q.Role;
            //return 0;

        }

        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: BZ
        public ActionResult FindOrder()
        {
            if (User.Identity.IsAuthenticated)
            {
                ViewBag.UserType = GetRole();
            }
            return View();
        }
        
        // POST: BZ
        [HttpPost]
        public ActionResult FindOrder(FindOrderViewModel model)
        {
            if (User.Identity.IsAuthenticated)
            {
                ViewBag.UserType = GetRole();
            }

            //   String location = model;
            List<Order> orders = new List<Order>();
            String add1 = model.line1 + " " + model.line2 + " " + model.city + " " + model.state + " " + model.zipCode;
            FindOrderLogic helper = new FindOrderLogic();

            double distance = 1;
            if (model.distance != 0)
            {
                distance = model.distance;
            }


                var q = (from o in db.orders
                         where o.Contact.City.Equals(model.city)
                         where o.Status == Status.WAITING
                         orderby o.DeliveryFee descending
                         select o
                         );
                if (q != null)
                {
                    foreach (Order o in q.ToList())
                    {
                        Contact c = o.Contact;
                        String addr2 = c.AddressLine1 + " " + c.AddressLine2 + " " + c.City + " " + c.State + " " + c.ZipCode;
                        double dis = helper.ComputeDistanceBetweenAandB(add1, addr2);
                        if (helper.selectOrderByDistance(distance, dis))
                        {
                            orders.Add(o);
                        }
                    }
                }
            
            ViewBag.res = orders;
            return View("FindOrderResults");
        }

        // GET: 
        public ActionResult AcceptOrder(int? id )
        {
            if (User.Identity.IsAuthenticated)
            {
                ViewBag.UserType = GetRole();
            }

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var res = (from o in db.orders
                         where o.Id == id
                         where o.Status == Status.WAITING
                              select o).FirstOrDefault();
            if (res == null)
            {
                return HttpNotFound();
            }
            ViewBag.ifSuccessed = 0;
            return View(res);
        }

        // POST: 
        [HttpPost]
        public ActionResult PickUpOrder(Order o)
        {
            if (User.Identity.IsAuthenticated)
            {
                ViewBag.UserType = GetRole();
            }

            if (o.Id == 0) {
                return HttpNotFound();
                    }
            int id = o.Id;
            var res = (from o1 in db.orders
                       where o1.Id == id
                       select o1).FirstOrDefault();
            res.Status = Status.PENDING;
            res.PickUpTime = DateTime.Now;

            var user = (from u in db.deliverymen
                        where u.Contact.Email.Equals(User.Identity.Name)
                        select u).FirstOrDefault();

            res.DeliverymanId = user.Id;
            res.Deliveryman = user;
            db.SaveChanges();
            ViewBag.ifSuccessed = 1;
            return View();
        }
        
        // GET: Deliveryman
        public ActionResult Orders()
        {
            if (User.Identity.IsAuthenticated)
            {
                ViewBag.UserType = GetRole();
            }

            var d = (from u in db.deliverymen
                              where u.Contact.Email.Equals(User.Identity.Name)
                              select u).FirstOrDefault();
            if (d == null)
            {
                return HttpNotFound();
            }
            IEnumerable<Order> wo = from o in db.orders
                                    where o.DeliverymanId == d.Id
                                    where o.Status == Status.WAITING
                                    orderby o.PlacedTime descending
                                    select o;
            IEnumerable<Order> po = from o in db.orders
                                    where o.DeliverymanId == d.Id
                                    where o.Status == Status.PENDING
                                    orderby o.PlacedTime descending
                                    select o;
            IEnumerable<Order> io = from o in db.orders
                                    where o.DeliverymanId == d.Id
                                    where o.Status == Status.INPROGRESS
                                    orderby o.PlacedTime descending
                                    select o;
            DeliverymanOrdersViewModel rovm = new DeliverymanOrdersViewModel()
            {
                WaitingOrders = wo,
                PendingOrders = po,
                InProgressOrders = io,
            };
            return View(rovm);
        }

        // GET: Deliveryman/OrderDetails/5
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
            Order order = (from o in db.orders
                           where o.Id == id
                           where o.Deliveryman.Contact.Email.Equals(User.Identity.Name)
                           select o).FirstOrDefault();

            if (order == null)
            {
                return HttpNotFound();
            }
            return View(order);
        }

        // GET: Deliveryman/MyOrders
        public ActionResult MyOrders()
        {
            if (User.Identity.IsAuthenticated)
            {
                ViewBag.UserType = GetRole();
            }

            Deliveryman deli = (from dm in db.deliverymen
                                where dm.Contact.Email.Equals(User.Identity.Name)
                                select dm).FirstOrDefault();
            if (deli == null)
            {
                return HttpNotFound();
            }
            IEnumerable<Order> orders = from o in db.orders
                                        where o.DeliverymanId == deli.Id
                                        select o;
            return View(orders);
        }

        // POST: Deliveryman/CancelPickup/5
        public ActionResult CancelPickup(int id)
        {
            if (User.Identity.IsAuthenticated)
            {
                ViewBag.UserType = GetRole();
            }

            Deliveryman deliveryman = (from dm in db.deliverymen
                                       where dm.Contact.Email.Equals(User.Identity.Name)
                                       select dm).FirstOrDefault();
            if (deliveryman == null)
            {
                return HttpNotFound();
            }
            Order order = (from o in db.orders
                           where o.Id == id
                           where o.Status == Status.PENDING
                           where o.DeliverymanId == deliveryman.Id
                           select o).FirstOrDefault();
            if (order == null)
            {
                return HttpNotFound();
            }
            //order.Status = Status.WAITING;
            //order.DeliverymanId = 0;
            //db.SaveChanges();
            return RedirectToAction("Orders");

        }

        // POST: Deliveryman/CompleteOrder/5
        public ActionResult CompleteOrder(int id)
        {
            if (User.Identity.IsAuthenticated)
            {
                ViewBag.UserType = GetRole();
            }

            Deliveryman deliveryman = (from dm in db.deliverymen
                                       where dm.Contact.Email.Equals(User.Identity.Name)
                                       select dm).FirstOrDefault();
            if (deliveryman == null)
            {
                return HttpNotFound();
            }
            Order order = (from o in db.orders
                           where o.Id == id
                           where o.Status == Status.INPROGRESS
                           where o.DeliverymanId == deliveryman.Id
                           select o).FirstOrDefault();
            if (order == null)
            {
                return HttpNotFound();
            }
            order.Status = Status.DELIVERED;
            order.DeliveredTime = DateTime.Now;
            db.SaveChanges();
            return RedirectToAction("Orders");
        }

        //Get
        public ActionResult Direction()
        {
            ViewBag.source = "110 riverdrive south 07310";
            ViewBag.desti = "55 riverdrive south 07310";
            return View();
        }






        protected override void Dispose(bool disposing)
        {
            if (User.Identity.IsAuthenticated)
            {
                ViewBag.UserType = GetRole();
            }

            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
