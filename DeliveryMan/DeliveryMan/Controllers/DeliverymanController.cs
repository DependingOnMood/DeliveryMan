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


        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: BZ
        public ActionResult FindOrder()
        {
            return View();
        }

        // POST: BZ
        [HttpPost]
        public ActionResult FindOrder(FindOrderViewModel model)
        {
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
                         from c in db.contacts
                         where o.ContactId.Equals(c.PhoneNumber)
                         where c.Email.Equals(User.Identity.Name)
                         select o
                         );
                if (q != null)
                {
                    foreach (Order o in q)
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
            return View("ViewOrdersByR");
        }








        // GET: Deliveryman
        public ActionResult Orders()
        {
            return View();
        }

        // GET: Deliveryman/OrderDetails/5
        public ActionResult OrderDetails(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = (from o in db.orders
                           where o.Id == id
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
