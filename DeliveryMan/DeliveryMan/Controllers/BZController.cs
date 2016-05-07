using BizLogic;
using DataLayer;
using DeliveryMan.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace DeliveryMan.Controllers
{
    public class BZController : Controller
    {
        // GET: BZ
        public ActionResult FindOrder()
        {
            return View();
        }

        // POST: BZ
        [HttpPost]
        public  ActionResult FindOrder(FindOrderViewModel model)
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
       

            using (var context = new ApplicationDbContext())
            {
                var q = (from o in context.orders
                         where o.Destination.Contact.Address.City.Equals(model.city)
                         select o
                         );
                if (q != null) {
                    foreach (Order o in q) {
                        Address addr = o.Destination.Contact.Address;
                        string addr2 = addr.ToString();
                        double dis = helper.ComputeDistanceBetweenAandB(add1, addr2);
                        if (helper.selectOrderByDistance(distance, dis)) {
                            orders.Add(o);
                        }
                    }
                }              
            }
            ViewBag.res = orders;
            return View("ViewOrdersByR");
        }




    }
}