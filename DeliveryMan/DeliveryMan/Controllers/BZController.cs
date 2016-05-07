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


            ViewBag.Find = 0;
    
            return View();
        }

        // POST: BZ
        [HttpPost]
        public  ActionResult FindOrder(FindOrderViewModel model)
        {
         //   String location = model;
            

            ViewBag.Find = 1;


            return View();
        }




    }
}