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
using System.Drawing;
using System.Drawing.Imaging;

namespace DeliveryMan.Controllers
{
    [Authorize]
    public class DeliverymanController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public int GetRole()
        {
            var q = (
            from c in db.contacts
            where c.Email.Equals(User.Identity.Name)
            select c).FirstOrDefault();
            return (int)q.Role;
        }

        // GET: Deliveryman/FindOrder
        public ActionResult FindOrder()
        {
            if (User.Identity.IsAuthenticated)
            {     
                ViewBag.UserType = GetRole();
                if (GetRole() != 0)
                {
                    throw new Exception("NotADeliveryman");
                }
            }
            return View();
        }

        // POST: Deliveryman/FindOrder
        [HttpPost]
        public ActionResult FindOrder(FindOrderViewModel model)
        {
            if (User.Identity.IsAuthenticated)
            {
                ViewBag.UserType = GetRole();
                if (GetRole() != 0)
                {
                    throw new Exception("NotADeliveryman");
                }      
            }
            List<Order> orders = new List<Order>();
            String add1 = "";
            if (model.latlng == null)
            {
                add1 = model.line1 + " " + model.line2 + " " + model.city + " " + model.state + " " + model.zipCode;
            }
            else {
                GoogleMapHelper map = new GoogleMapHelper();

                try
                {
                    add1 = map.getAddrByLatandLng(model.latlng);
                }
                catch (Exception e)
                {

                    ModelState.AddModelError("line1", "Pleas input a valid address!");
                    return View("FindOrder",model);
                }
            }


            FindOrderLogic helper = new FindOrderLogic();

            double distance = 1;
            if (model.distance != 0)
            {
                distance = model.distance;
            }

            var q = (from o in db.orders
                     where o.Status == Status.WAITING
                     orderby o.DeliveryFee descending
                     select o
                     );

            if (q != null)
            {
                foreach (Order o in q.ToList())
                {
                    Contact c = o.Restaurant.Contact;
                    String addr2 = c.getFullAddress();
                    double dis = 0;

                    try
                    {
                        dis = helper.ComputeDistanceBetweenAandB(add1, addr2);
                    }
                    catch (Exception e)
                    {

                        ModelState.AddModelError("line1", "Pleas input a valid address!");
                        return View("FindOrder", model);
                    }

                    // check to see if deliveryman is in any blacklist
                    var deliveryman = (from d in db.deliverymen
                                       where d.Contact.Email == User.Identity.Name
                                       select d).FirstOrDefault();

                    var blacklist = (from b in db.blacklists
                                     where b.RestaurantId == o.RestaurantId
                                     where b.DeliverymanId == deliveryman.Id
                                     select b).FirstOrDefault();

                    if (helper.selectOrderByDistance(distance, dis) && blacklist == null)
                    {
                        orders.Add(o);
                    }
                }
            }

            ViewBag.res = orders;
            return View("FindOrderResults");
        }

        // GET: 
        public ActionResult AcceptOrder(int? id)
        {
            if (User.Identity.IsAuthenticated)
            {
                ViewBag.UserType = GetRole();
                if (GetRole() != 0)
                {
                    throw new Exception("NotADeliveryman");
                }
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
                throw new Exception("Error");
            }
            ViewBag.ifSuccessed = 0;

            return View(res);
        }

        // POST: 
        [HttpPost]
        public ActionResult AcceptOrder(Order o)
        {
            if (User.Identity.IsAuthenticated)
            {
                ViewBag.UserType = GetRole();
                if (GetRole() != 0)
                {
                    throw new Exception("NotADeliveryman");
                }
            }

            if (o.Id == 0)
            {
                throw new Exception("Error");
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

            return RedirectToAction("MyOrders");
        }

        // GET: Deliveryman/OrderDetails/5
        public ActionResult OrderDetails(int? id)
        {
            if (User.Identity.IsAuthenticated)
            {
                ViewBag.UserType = GetRole();
                if (GetRole() != 0)
                {
                    throw new Exception("NotADeliveryman");
                }
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
                throw new Exception("Error");
            }

            return View(order);
        }

        // GET: Deliveryman/MyOrders
        public ActionResult MyOrders()
        {
            if (User.Identity.IsAuthenticated)
            {
                ViewBag.UserType = GetRole();
                if (GetRole() != 0)
                {
                    throw new Exception("NotADeliveryman");
                }
            }
            Deliveryman deli = (from dm in db.deliverymen
                                where dm.Contact.Email.Equals(User.Identity.Name)
                                select dm).FirstOrDefault();
            if (deli == null)
            {
                throw new Exception("Error");
            }
            IEnumerable<Order> pendingO = from o in db.orders
                                          where o.DeliverymanId == deli.Id
                                          where o.Status == Status.PENDING
                                          orderby o.PlacedTime descending
                                          select o;
            IEnumerable<Order> inProgressO = from o in db.orders
                                             where o.DeliverymanId == deli.Id
                                             where o.Status == Status.INPROGRESS
                                             orderby o.PickUpTime descending
                                             select o;
            IEnumerable<Order> deliveredO = from o in db.orders
                                            where o.DeliverymanId == deli.Id
                                            where o.Status == Status.DELIVERED
                                            orderby o.DeliveredTime descending
                                            select o;
            DeliverymanMyOrdersViewModel model = new DeliverymanMyOrdersViewModel()
            {
                Balance = deli.Balance,
                pendingOrders = pendingO,
                inProgressOrders = inProgressO,
                deliveredOrders = deliveredO,
            };
            return View(model);
        }

        // GET: Deliveryman/CancelPickup/5
        public ActionResult CancelPickup(int? id)
        {
            if (User.Identity.IsAuthenticated)
            {
                ViewBag.UserType = GetRole();
                if (GetRole() != 0)
                {
                    throw new Exception("NotADeliveryman");
                }
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Deliveryman deliveryman = (from dm in db.deliverymen
                                       where dm.Contact.Email.Equals(User.Identity.Name)
                                       select dm).FirstOrDefault();
            if (deliveryman == null)
            {
                throw new Exception("Error");
            }

            Order order = (from o in db.orders
                           where o.Id == id
                           where o.Status == Status.PENDING
                           where o.DeliverymanId == deliveryman.Id
                           select o).FirstOrDefault();
            if (order == null)
            {
                throw new Exception("Error");
            }

            DeliverymanCancelPickupViewModel model = new DeliverymanCancelPickupViewModel()
            {
                OrderId = order.Id,
                OrderNote = order.Note,
                OrderStatus = order.Status,
                RestaurantName = order.Restaurant.Name,
                ETA = order.ETA,
                PlacedTime = order.PlacedTime,
                PickUpTime = order.PickUpTime,
                DeliveryFee = order.DeliveryFee,
                CancellationFee = order.cancellationFee(),
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CancelPickup(DeliverymanCancelPickupViewModel model)
        {
            if (User.Identity.IsAuthenticated)
            {
                ViewBag.UserType = GetRole();
                if (GetRole() != 0)
                {
                    throw new Exception("NotADeliveryman");
                }
            }
            Order order = db.orders.Find(model.OrderId);
            if ((order.Deliveryman.Balance - model.CancellationFee).CompareTo(0M) < 0)
            {
                //ViewBag.Message = "You have insufficient balance to cancel!";
                ModelState.AddModelError("CancellationFee", "You have insufficient balance to cancel!");
                return View("CancelPickup", model);
            }
            else
            {
                order.Deliveryman.Balance -= model.CancellationFee;
                order.Status = Status.WAITING;
                order.DeliverymanId = null;
                order.Deliveryman = null;
                db.SaveChanges();
                return RedirectToAction("MyOrders");
            }
        }

        // GET: Deliveryman/CompleteOrder/5
        public ActionResult CompleteOrder(int? id)
        {
            if (User.Identity.IsAuthenticated)
            {
                ViewBag.UserType = GetRole();
                if (GetRole() != 0)
                {
                    throw new Exception("NotADeliveryman");
                }
            }
            Deliveryman deliveryman = (from dm in db.deliverymen
                                       where dm.Contact.Email.Equals(User.Identity.Name)
                                       select dm).FirstOrDefault();

            if (deliveryman == null)
            {
                throw new Exception("Error");
            }

            Order order = (from o in db.orders
                           where o.Id == id
                           where o.Status == Status.INPROGRESS
                           where o.DeliverymanId == deliveryman.Id
                           select o).FirstOrDefault();

            if (order == null)
            {
                throw new Exception("Error");
            }

            order.Status = Status.DELIVERED;
            order.DeliveredTime = DateTime.Now;
            order.Deliveryman.Balance += order.DeliveryFee;
            order.Restaurant.Balance -= order.DeliveryFee;

            db.SaveChanges();

            return RedirectToAction("MyOrders");
        }

        //Get
        public ActionResult Direction(String s1, String s2)
        {
            //ViewBag.source = "110 riverdrive south 07310";
            //ViewBag.desti = "55 riverdrive south 07310";

            if (User.Identity.IsAuthenticated)
            {
                ViewBag.UserType = GetRole();
                if (GetRole() != 0)
                {
                    throw new Exception("NotADeliveryman");
                }
            }
            ViewBag.source = s1;
            ViewBag.desti = s2;
            return View();
        }

        protected override void Dispose(bool disposing)
        {
            if (User.Identity.IsAuthenticated)
            {
                ViewBag.UserType = GetRole();
                if (GetRole() != 0)
                {
                    throw new Exception("NotADeliveryman");
                }
            }

            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        // GET: Deliveryman/ChangeDeliverymantUserInfo
        public ActionResult ChangeDeliverymanUserInfo()
        {
            if (User.Identity.IsAuthenticated)
            {
                ViewBag.UserType = GetRole();
                if (GetRole() != 0)
                {
                    throw new Exception("NotADeliveryman");
                }
            }
            Deliveryman del = (from d in db.deliverymen
                               where d.Contact.Email.Equals(User.Identity.Name)
                               select d).FirstOrDefault();
            if (del == null)
            {
                throw new Exception("Error");
            }

            ChangeDeliverymanInfoModel model = new ChangeDeliverymanInfoModel()
            {
                FirstName = del.FirstName,
                LastName = del.LastName,
                AddressLine1 = del.Contact.AddressLine1,
                AddressLine2 = del.Contact.AddressLine2,
                City = del.Contact.City,
                State = del.Contact.State,
                ZipCode = del.Contact.ZipCode,
            };
            return View(model);
        }

        // POST: Deliveryman/ChangeDeliverymanUserInfo
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangeDeliverymanUserInfo([Bind(Include = "FirstName, LastName, AddressLine1, AddressLine2, City, State, ZipCode, file")]
            ChangeDeliverymanInfoModel model)
        {
            if (User.Identity.IsAuthenticated)
            {
                ViewBag.UserType = GetRole();
                if (GetRole() != 0)
                {
                    throw new Exception("NotADeliveryman");
                }
            }

            Deliveryman del = (from d in db.deliverymen
                               where d.Contact.Email.Equals(User.Identity.Name)
                               select d).FirstOrDefault();

            if (del == null)
            {
                throw new Exception("Error");
            }



            del.FirstName = model.FirstName;
            del.LastName = model.LastName;

            del.Contact.AddressLine1 = model.AddressLine1;
            del.Contact.AddressLine2 = model.AddressLine2;
            del.Contact.City = model.City;
            del.Contact.State = model.State;
            del.Contact.ZipCode = model.ZipCode;
            db.SaveChanges();

            return RedirectToAction("Index", "Manage");
        }

    }
}
