﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using DataLayer;
using BizLogic;
using DeliveryMan.Models;
using System.Drawing;
using System.Drawing.Imaging;
using System.Web;

namespace DeliveryMan.Controllers
{
    [Authorize]
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
                if (GetRole() != 1)
                {
                    throw new Exception("NotARestaurant");
                }
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
            if (User.Identity.IsAuthenticated)
            {
                ViewBag.UserType = GetRole();
                if (GetRole() != 1)
                {
                    throw new Exception("NotARestaurant");
                }
            }
            if (res.Balance.CompareTo(0.00M) < 0)
            {
                ModelState.AddModelError("OrderFee", "Your balance is less than 0.");
                return View("CreateOrder", model);
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
                String latAndLong = "";
                try
                {
                    latAndLong = helper.getLatandLngByAddr(totalAddress);
                }
                catch (Exception e)
                {
                    ModelState.AddModelError("Addressline1", "Please input a valid address!");
                    return View("CreateOrder", model);
                }

                contact.Latitude = Decimal.Parse(latAndLong.Split(' ')[0]);
                contact.Longitude = Decimal.Parse(latAndLong.Split(' ')[1]);
                db.contacts.Add(contact);
            }
            else
            {
                if (contact.Role == Role.RESTAUTANT) {
                    ModelState.AddModelError("PhoneNumber","This phone number belongs to a restaurant.");
                    return View("CreateOrder", model);
                }

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
                    String latAndLong = "";
                    try
                    {
                        latAndLong = helper.getLatandLngByAddr(totalAddress);
                    }
                    catch (Exception e)
                    {
                        ModelState.AddModelError("Addressline1", "Pleas input a valid address!");
                        return View("CreateOrder", model);
                    }
                
                    contact.Latitude = Decimal.Parse(latAndLong.Split(' ')[0]);
                    contact.Longitude = Decimal.Parse(latAndLong.Split(' ')[1]);
                }
            }

            db.SaveChanges();

            helper = new GoogleMapHelper();
            string loc1 = res.Contact.getFullAddress();
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
                if (GetRole() != 1)
                {
                    throw new Exception("NotARestaurant");
                }
            }

            Restaurant res = (from r in db.restaurants
                              where r.Contact.Email.Equals(User.Identity.Name)
                              select r).FirstOrDefault();

            if (res == null)
            {
                throw new Exception("Error");
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
                Balance = res.Balance,
            };

            return View(rovm);
        }

        // GET: Restaurant/PickUpOrder/5
        public ActionResult PickUpOrder(int? id)
        {
            if (User.Identity.IsAuthenticated)
            {
                ViewBag.UserType = GetRole();
                if (GetRole() != 1)
                {
                    throw new Exception("NotARestaurant");
                }
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
                if (GetRole() != 1)
                {
                    throw new Exception("NotARestaurant");
                }
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
                throw new Exception("Error");
            }

            Order order = (from o in db.orders
                           where o.RestaurantId == res.Id
                           where o.Id == id
                           select o).FirstOrDefault();

            if (order == null)
            {
                throw new Exception("Error");
            }

            return View(order);
        }

        // GET: Restaurant/EditOrder/5
        public ActionResult EditOrder(int? id)
        {
            if (User.Identity.IsAuthenticated)
            {
                ViewBag.UserType = GetRole();
                if (GetRole() != 1)
                {
                    throw new Exception("NotARestaurant");
                }
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
                throw new Exception("Error");
            }

            Order order = (from o in db.orders
                           where o.RestaurantId == res.Id
                           where o.Id == id
                           select o).FirstOrDefault();

            RestaurantEditOrderViewModel model = new RestaurantEditOrderViewModel()
            {
                OrderId = order.Id,
                Note = order.Note,
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
        public ActionResult EditOrder([Bind(Include = "OrderId, Note, AddressLine1, AddressLine2, City, State, ZipCode")]
        RestaurantEditOrderViewModel model)
        {

            if (User.Identity.IsAuthenticated)
            {
                ViewBag.UserType = GetRole();
                if (GetRole() != 1)
                {
                    throw new Exception("NotARestaurant");
                }
            }

            Restaurant res = (from r in db.restaurants
                              where r.Contact.Email.Equals(User.Identity.Name)
                              select r).FirstOrDefault();

            if (res == null)
            {
                throw new Exception("Error");
            }

            Order order = (from o in db.orders
                           where o.RestaurantId == res.Id
                           where o.Id == model.OrderId
                           select o).FirstOrDefault();

            order.Note = model.Note;
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
            if (id == null)
            {
                throw new Exception("Error");
            }

            if (User.Identity.IsAuthenticated)
            {
                ViewBag.UserType = GetRole();

                if (GetRole() != 1)
                {
                    throw new Exception("NotARestaurant");
                }
            }

            Restaurant res = (from r in db.restaurants
                              where r.Contact.Email.Equals(User.Identity.Name)
                              select r).FirstOrDefault();

            if (res == null)
            {
                throw new Exception("Error");
            }

            Order order = (from o in db.orders
                           where o.RestaurantId == res.Id
                           where o.Id == id
                           select o).FirstOrDefault();

            CancelOrderViewModel cancelOrderVM = new CancelOrderViewModel()
            {
                OrderId = order.Id,
                OrderName = order.Note,
                OrderStatus = order.Status,
                ETA = order.ETA,
                PlacedTime = order.PlacedTime,
                PickUpTime = order.PickUpTime,
                DeliveryFee = order.DeliveryFee,
                CancellationFee = Decimal.Parse(order.cancellationFee().ToString("F")),
            };

            return View(cancelOrderVM);
        }

        // POST: Restaurant/CancelOrder/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CancelOrder([Bind(Include = "OrderId, OrderName, OrderStatus, ETA, PlacedTime, PickUpTime, DeliveryFee, CancellationFee")]
            CancelOrderViewModel model)
        {
            if (User.Identity.IsAuthenticated)
            {
                ViewBag.UserType = GetRole();
                if (GetRole() != 1)
                {
                    throw new Exception("NotARestaurant");
                }
            }

            Restaurant res = (from r in db.restaurants
                              where r.Contact.Email.Equals(User.Identity.Name)
                              select r).FirstOrDefault();

            if (res == null)
            {
                throw new Exception("Error");
            }

            Order order = (from o in db.orders
                           where o.RestaurantId == res.Id
                           where o.Id == model.OrderId
                           select o).FirstOrDefault();

            if (order == null)
            {
                throw new Exception("Error");
            }

            if (order.Status == Status.WAITING)
            {
                db.orders.Remove(order);
                db.SaveChanges();
                return RedirectToAction("Orders");
            }
            else if (order.Status == Status.PENDING || order.Status == Status.INPROGRESS)
            {
                if ((order.Restaurant.Balance - model.CancellationFee).CompareTo(0M) < 0)
                {
                    ModelState.AddModelError("CancellationFee", "You have insufficient balance to cancel it!");
                    return View("CancelOrder", model);
                }
                else
                {
                    order.Restaurant.Balance -= model.CancellationFee;
                    order.Deliveryman.Balance += model.CancellationFee;
                    db.orders.Remove(order);
                    db.SaveChanges();

                    return RedirectToAction("Orders");
                }
            }
            else
            {
                ModelState.AddModelError("OrderStatus", "A Delivered Order can not be cancelled");

                return View("CancelOrder", model);
            }

        }

        // GET: Restaurant/ReviewOrder/5
        public ActionResult ReviewOrder(int? id)
        {
            if (User.Identity.IsAuthenticated)
            {
                ViewBag.UserType = GetRole();
                if (GetRole() != 1)
                {
                    throw new Exception("NotARestaurant");
                }
            }

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Order orderDetails = (from o in db.orders
                                  where o.Restaurant.Contact.Email == User.Identity.Name
                                  where o.Id == id
                                  select o).FirstOrDefault();

            ReviewOrderViewModel reviewOrderVM = new ReviewOrderViewModel()
            {
                OrderId = orderDetails.Id,
                DeliverymanName = orderDetails.Deliveryman.getName(),
                PlacedTime = orderDetails.PlacedTime,
                PickUpTime = orderDetails.PickUpTime,
                DeliveredTime = orderDetails.DeliveredTime,
                IconUrl = orderDetails.Deliveryman.IconImageUrl,
            };

            var blacklist = (from b in db.blacklists
                             where b.Restaurant.Contact.Email == User.Identity.Name
                             where b.DeliverymanId == orderDetails.DeliverymanId
                             select b).FirstOrDefault();

            if (blacklist == null)
            {
                reviewOrderVM.Blacklist = false;
            }
            else
            {
                reviewOrderVM.Blacklist = true;
            }

            return View(reviewOrderVM);
        }

        // POST: Restaurant/ReviewOrder/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ReviewOrder(ReviewOrderViewModel model)
        {
            if (User.Identity.IsAuthenticated)
            {
                ViewBag.UserType = GetRole();
                if (GetRole() != 1)
                {
                    throw new Exception("NotARestaurant");
                }
            }

            if (ModelState.IsValid)
            { // check model state
              //   if (model.DeliveredTime != null)
              //  { // only allow delivered order to be reviewed
                Order order = (from o in db.orders
                               where o.Restaurant.Contact.Email.Equals(User.Identity.Name)
                               where o.Id == model.OrderId
                               select o).FirstOrDefault();

                Review newReview = new Review()
                {
                    OrderId = order.Id,
                    Order = order,
                    ReviewText = model.ReviewText,
                    Rating = model.Rating,
                };

                // calculate cumulative rating
                Deliveryman deliveryman = db.deliverymen.Find(order.DeliverymanId);

                decimal rating = ReviewRating.calculateRating(deliveryman, newReview.Rating);

                // update deliveryman table
                deliveryman.Rating = rating;
                deliveryman.TotalStarsEarned += newReview.Rating;
                deliveryman.TotalDeliveryCount += 1;
                deliveryman.Ranking = newReview.calculateScore();

                db.reviews.Add(newReview);

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

                if (GetRole() != 1)
                {
                    throw new Exception("NotARestaurant");
                }
            }

            Restaurant res = (from r in db.restaurants
                              where r.Contact.Email.Equals(User.Identity.Name)
                              select r).FirstOrDefault();

            if (res == null)
            {
                throw new Exception("Error");
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

        // GET: Restaurant/AddToBlacklist
        // add a deliveryman to blacklist
        public ActionResult AddToBlacklist(int? id)
        {
            if (User.Identity.IsAuthenticated)
            {
                ViewBag.UserType = GetRole();
                if (GetRole() != 1)
                {
                    throw new Exception("NotARestaurant");
                }
            }

            var orderDetails = (from o in db.orders
                                where o.Restaurant.Contact.Email == User.Identity.Name
                                where o.Id == id
                                select o).FirstOrDefault();

            int dmanID = (int)orderDetails.DeliverymanId;

            Deliveryman deliveryman = db.deliverymen.Find(dmanID);
            Restaurant restaurant = db.restaurants.Find(orderDetails.RestaurantId);

            BlackListViewModel blackListVM = new BlackListViewModel();

            blackListVM.OrderId = id;
            blackListVM.DeliverymanName = deliveryman.FirstName + " " + deliveryman.LastName;
            blackListVM.Rating = deliveryman.Rating;
            blackListVM.RestaurantId = orderDetails.RestaurantId;
            blackListVM.DeliverymanId = orderDetails.DeliverymanId;

            // calculate average delivery time
            IEnumerable<Order> deliverymanOrders = (from o in db.orders
                                                    where o.DeliverymanId == dmanID
                                                    where o.Status == Status.DELIVERED
                                                    select o);

            int count = deliverymanOrders.Count();
            int total = 0;
            double deliveryTime = 0.0, totalTime = 0.0;

            for (int i = 0; i < count; i++)
            {
                DateTime pickupTime = (DateTime)deliverymanOrders.Skip(i).First().PickUpTime;
                DateTime deliveredTime = (DateTime)deliverymanOrders.Skip(i).First().DeliveredTime;

                deliveryTime = deliveredTime.Subtract(pickupTime).TotalMinutes;

                total++;
                totalTime += deliveryTime;
            }

            int averagetime = (int)DeliverymanTime.averageTime(total, totalTime);

            blackListVM.AverageDeliveryTime = averagetime.ToString() + " minutes";
            blackListVM.TotalOrders = total;

            db.SaveChanges();

            return View("AddToBlacklist", blackListVM);
        }

        // POST: Restaurant/AddToBlacklist
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddToBlacklist(BlackListViewModel model, int? id)
        {
            if (User.Identity.IsAuthenticated)
            {
                ViewBag.UserType = GetRole();
                if (GetRole() != 1)
                {
                    throw new Exception("NotARestaurant");
                }
            }

            var orderDetails = (from o in db.orders
                                where o.Restaurant.Contact.Email == User.Identity.Name
                                where o.Id == id
                                select o).FirstOrDefault();

            // add to blacklist
            Blacklist newBlacklist = new Blacklist();

            newBlacklist.DeliverymanId = (int)orderDetails.DeliverymanId;
            newBlacklist.RestaurantId = orderDetails.RestaurantId;

            db.blacklists.Add(newBlacklist);

            db.SaveChanges();

            IEnumerable<Blacklist> blacklists = (from b in db.blacklists
                                                 where b.RestaurantId == orderDetails.RestaurantId
                                                 select b);

            int count = blacklists.Count();

            List<BlackListViewModel> blacklistVMs =
                new List<BlackListViewModel>(new BlackListViewModel[count]);

            for (int i = 0; i < count; i++)
            {
                Blacklist blacklist = blacklists.Skip(i).First();
                Deliveryman deliveryman = db.deliverymen.Find(blacklist.DeliverymanId);

                blacklistVMs[i] = new BlackListViewModel();

                blacklistVMs[i].DeliverymanName = deliveryman.FirstName + " " + deliveryman.LastName;
                blacklistVMs[i].Rating = deliveryman.Rating;

                blacklistVMs[i].DeliverymanId = deliveryman.Id;
            }

            ViewBag.orderId = id;

            TempData["orderId"] = id;

            return View("BlacklistView", blacklistVMs);
        }

        // GET: Restaurant/BlacklistView
        // display all deliverymen thats in your blacklist
        public ActionResult BlacklistView()
        {
            if (User.Identity.IsAuthenticated)
            {
                ViewBag.UserType = GetRole();
                if (GetRole() != 1)
                {
                    throw new Exception("NotARestaurant");
                }
            }
            IEnumerable<Blacklist> blacklists = (from b in db.blacklists
                                                 where b.Restaurant.Contact.Email == User.Identity.Name
                                                 select b);

            int count = blacklists.Count();

            List<BlackListViewModel> blacklistVMs =
                new List<BlackListViewModel>(new BlackListViewModel[count]);

            for (int i = 0; i < count; i++)
            {
                Blacklist blacklist = blacklists.Skip(i).First();
                Deliveryman deliveryman = db.deliverymen.Find(blacklist.DeliverymanId);

                blacklistVMs[i] = new BlackListViewModel();

                blacklistVMs[i].DeliverymanName = deliveryman.FirstName + " " + deliveryman.LastName;
                blacklistVMs[i].Rating = deliveryman.Rating;

                blacklistVMs[i].DeliverymanId = deliveryman.Id;
            }

            return View("BlacklistView", blacklistVMs);
        }

        // GET: Restaurant/DeleteFromBlacklist
        // remove deliveryman from your blacklist
        public ActionResult DeleteFromBlacklist(int? id)
        {
            if (User.Identity.IsAuthenticated)
            {
                ViewBag.UserType = GetRole();
                if (GetRole() != 1)
                {
                    throw new Exception("NotARestaurant");
                }
            }

            // remove deliveryman from blacklist
            Blacklist reverseBlacklist = (from b in db.blacklists
                                          where b.Restaurant.Contact.Email == User.Identity.Name
                                          where b.DeliverymanId == id
                                          select b).FirstOrDefault();

            db.blacklists.Remove(reverseBlacklist);

            db.SaveChanges();

            // return new blacklist view
            IEnumerable<Blacklist> blacklists = (from b in db.blacklists
                                                 where b.Restaurant.Contact.Email == User.Identity.Name
                                                 select b);

            int count = blacklists.Count();

            List<BlackListViewModel> blacklistVMs =
                new List<BlackListViewModel>(new BlackListViewModel[count]);

            for (int i = 0; i < count; i++)
            {
                Blacklist blacklist = blacklists.Skip(i).First();
                Deliveryman deliveryman = db.deliverymen.Find(blacklist.DeliverymanId);

                blacklistVMs[i] = new BlackListViewModel();

                blacklistVMs[i].DeliverymanName = deliveryman.FirstName + " " + deliveryman.LastName;
                blacklistVMs[i].Rating = deliveryman.Rating;
            }

            return RedirectToAction("BlacklistView");
        }

        // GET: Restaurant/DeliverymanDetails/5
        public ActionResult DeliverymanDetails(int? id)
        {
            if (User.Identity.IsAuthenticated)
            {
                ViewBag.UserType = GetRole();
                if (GetRole() != 1)
                {
                    throw new Exception("NotARestaurant");
                }
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
                throw new Exception("Error");
            }

            Order order = (from o in db.orders
                           where o.Id == id
                           where o.RestaurantId == res.Id
                           select o).FirstOrDefault();

            if (order == null)
            {
                throw new Exception("Error");
            }

            return View(order.Deliveryman);
        }

        // GET: Restaurant/AddBalance
        public ActionResult AddBalance()
        {
            if (User.Identity.IsAuthenticated)
            {
                ViewBag.UserType = GetRole();
                if (GetRole() != 1)
                {
                    throw new Exception("NotARestaurant");
                }
            }
            Restaurant res = (from r in db.restaurants
                              where r.Contact.Email.Equals(User.Identity.Name)
                              select r).FirstOrDefault();

            if (res == null)
            {
                throw new Exception("Error");
            }

            RestaurantAddBalanceViewModel model = new RestaurantAddBalanceViewModel()
            {
                RestaurantId = res.Id,
                Balance = res.Balance,
            };

            return View(model);
        }

        // POST: Restaurant/AddBalance
        [HttpPost]
        public ActionResult AddBalance([Bind(Include = "RestaurantId, Balance")] RestaurantAddBalanceViewModel model)
        {
            if (User.Identity.IsAuthenticated)
            {
                ViewBag.UserType = GetRole();
                if (GetRole() != 1)
                {
                    throw new Exception("NotARestaurant");
                }
            }
            Restaurant res = (from r in db.restaurants
                              where model.RestaurantId == r.Id
                              select r).FirstOrDefault();

            if (res == null)
            {
                throw new Exception("Error");
            }

            res.Balance = model.Balance;

            db.SaveChanges();

            return RedirectToAction("Orders");
        }

        // GET: Restaurant/ChangeRestaurantUserInfo
        public ActionResult ChangeRestaurantUserInfo()
        {
            if (User.Identity.IsAuthenticated)
            {
                ViewBag.UserType = GetRole();
                if (GetRole() != 1)
                {
                    throw new Exception("NotARestaurant");
                }
            }
            Restaurant res = (from r in db.restaurants
                              where r.Contact.Email == User.Identity.Name
                              select r).FirstOrDefault();

            if (res == null)
            {
                throw new Exception("Error");
            }

            ChangeRestaurantInfoModel model = new ChangeRestaurantInfoModel()
            {
                Name = res.Name,

                AddressLine1 = res.Contact.AddressLine1,
                AddressLine2 = res.Contact.AddressLine2,
                City = res.Contact.City,
                State = res.Contact.State,
                ZipCode = res.Contact.ZipCode,


            };

            return View(model);
        }

        // POST: Restaurant/ChangeRestaurantUserInfo
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangeRestaurantUserInfo([Bind(Include = "Name, PhoneNumber, AddressLine1, AddressLine2, City, State, ZipCode, file")]
            ChangeRestaurantInfoModel model, HttpPostedFileBase file)
        {
            if (User.Identity.IsAuthenticated)
            {
                ViewBag.UserType = GetRole();
                if (GetRole() != 1)
                {
                    throw new Exception("NotARestaurant");
                }
            }

            Restaurant res = (from r in db.restaurants
                              where r.Contact.Email.Equals(User.Identity.Name)
                              select r).FirstOrDefault();
            if (res == null)
            {
                throw new Exception("Error");
            }
            String fileUrl = "";
            if (model.file != null)
            {
                fileUrl = HttpContext.Server.MapPath("~/Content/UserIcon/")
                                                    + User.Identity.Name + ".png";


                Bitmap b = (Bitmap)Bitmap.FromStream(file.InputStream);
                if (System.IO.File.Exists(fileUrl))
                {
                    System.IO.File.Delete(fileUrl);
                }
                fileUrl = HttpContext.Server.MapPath("~/Content/UserIcon/")
                                                    + User.Identity.Name + "new" + ".png";
                b.Save(fileUrl, ImageFormat.Png);

            }



            res.Name = model.Name;

            res.Contact.AddressLine1 = model.AddressLine1;
            res.Contact.AddressLine2 = model.AddressLine2;
            res.Contact.City = model.City;
            res.Contact.State = model.State;
            res.Contact.ZipCode = model.ZipCode;
           res.IconImageUrl = User.Identity.Name + "new" + ".png";
            db.SaveChanges();
            return RedirectToAction("Index", "Manage");
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
