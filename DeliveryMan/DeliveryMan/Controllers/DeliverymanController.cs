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
    public class DeliverymanController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Deliveryman
        public ActionResult Index()
        {
            var deliveryman = db.deliveryman.Include(d => d.Contact);
            return View(deliveryman.ToList());
        }

        // GET: Deliveryman/OrderDetails/5
        public ActionResult OrderDetails(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Deliveryman deliveryman = db.deliveryman.Find(id);
            if (deliveryman == null)
            {
                return HttpNotFound();
            }
            return View(deliveryman);
        }

        // GET: Deliveryman/Create
        public ActionResult Create()
        {
            ViewBag.CId = new SelectList(db.contact, "CId", "Name");
            return View();
        }

        // POST: Deliveryman/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,CId,IconImageUrl,TotalDeliveryCount,TotalStarsEarned,Rating,Ranking,Balance")] Deliveryman deliveryman)
        {
            if (ModelState.IsValid)
            {
                db.deliveryman.Add(deliveryman);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CId = new SelectList(db.contact, "CId", "Name", deliveryman.CId);
            return View(deliveryman);
        }

        // GET: Deliveryman/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Deliveryman deliveryman = db.deliveryman.Find(id);
            if (deliveryman == null)
            {
                return HttpNotFound();
            }
            ViewBag.CId = new SelectList(db.contact, "CId", "Name", deliveryman.CId);
            return View(deliveryman);
        }

        // POST: Deliveryman/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,CId,IconImageUrl,TotalDeliveryCount,TotalStarsEarned,Rating,Ranking,Balance")] Deliveryman deliveryman)
        {
            if (ModelState.IsValid)
            {
                db.Entry(deliveryman).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CId = new SelectList(db.contact, "CId", "Name", deliveryman.CId);
            return View(deliveryman);
        }

        // GET: Deliveryman/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Deliveryman deliveryman = db.deliveryman.Find(id);
            if (deliveryman == null)
            {
                return HttpNotFound();
            }
            return View(deliveryman);
        }

        // POST: Deliveryman/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Deliveryman deliveryman = db.deliveryman.Find(id);
            db.deliveryman.Remove(deliveryman);
            db.SaveChanges();
            return RedirectToAction("Index");
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
