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

        // GET: Restaurant/CreateOrder
        public ActionResult CreateOrder()
        {
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
                              from c in db.contacts
                              where r.ContactId.Equals(c.PhoneNumber)
                              select r).FirstOrDefault();
        
            if (res == null)
            {
                return HttpNotFound();
            }
            Contact contact = (from c in db.contacts
                               where c.PhoneNumber.Equals(model.PhoneNumber)
                               select c).FirstOrDefault();
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
                String totalAddress = model.AddressLine1 + " " + model.AddressLine2 + " " + model.City + " " + model.State + " " + model.ZipCode;
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
                    String totalAddress = model.AddressLine1 + " " + model.AddressLine2 + " " + model.City + " " + model.State + " " + model.ZipCode;
                    helper = new GoogleMapHelper();
                    String latAndLong = helper.getLatandLngByAddr(totalAddress);
                    contact.Latitude = Decimal.Parse(latAndLong.Split(' ')[0]);
                    contact.Longitude = Decimal.Parse(latAndLong.Split(' ')[1]);
                }
            }
            db.SaveChanges();
            Order order = new Order()
            {
                RestaurantId = res.Id,
                Status = Status.WAITING,
                Note = model.Note,
                PlacedTime = DateTime.Now,
                ContactId = contact.PhoneNumber,
                //calc ETA
                //calc DeliveryFee
            };
            helper = new GoogleMapHelper();
            string loc1 = res.getAddress();
            string loc2 = order.getAddress();
            double distance = helper.computeDistanceByLocation(loc1, loc2);
            
            db.orders.Add(order);
            db.SaveChanges();
            return RedirectToAction("Orders");
        }

        // GET: Restaurant
        public ActionResult Orders()
        {
            Restaurant res = (from r in db.restaurants
                              from c in db.contacts
                              where r.ContactId.Equals(c.PhoneNumber)
                              select r).FirstOrDefault();
            if (res == null)
            {
                return HttpNotFound();
            }
            IEnumerable<Order> wo = from o in db.orders
                                    from r in db.restaurants
                                    where o.RestaurantId == r.Id
                                    where o.Status == Status.WAITING
                                    orderby o.PlacedTime descending
                                    select o;
            IEnumerable<Order> po = from o in db.orders
                                    from r in db.restaurants
                                    where o.RestaurantId == r.Id
                                    where o.Status == Status.PENDING
                                    orderby o.PlacedTime descending
                                    select o;
            IEnumerable<Order> io = from o in db.orders
                                    from r in db.restaurants
                                    where o.RestaurantId == r.Id
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
                              from c in db.contacts
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
                              where r.Contact.Email.Equals(User.Identity.Name)
                              select r).FirstOrDefault();
            if (res == null)
            {
                return HttpNotFound();
            }
            Order order = (from o in db.orders
                           where o.RestaurantId == res.Id
                           where o.Id == id
                           where o.Status != Status.DELIVERED
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
                              where r.Contact.Email.Equals(User.Identity.Name)
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
            } else
            {
                return HttpNotFound();
            }
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
                if (badGuy == null)
                {
                    return HttpNotFound();
                } else
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
