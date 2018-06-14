using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using FlowerShop.Models;

namespace FlowerShop.Controllers
{
    public class QuantitiesController : Controller
    {
        private FlowerShopOnline db = new FlowerShopOnline();

        // GET: Quantities
        public ActionResult Index()
        {
            var quantities = db.Quantities.Include(q => q.Product);
            return View(quantities.ToList());
        }

        // GET: Quantities/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Quantity quantity = db.Quantities.Find(id);
            if (quantity == null)
            {
                return HttpNotFound();
            }
            return View(quantity);
        }

        // GET: Quantities/Create
        public ActionResult Create()
        {
            ViewBag.Product_ID = new SelectList(db.Products, "ID", "Name");
            return View();
        }

        // POST: Quantities/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Product_ID,Origin,Remain,Product_Date")] Quantity quantity)
        {
            if (ModelState.IsValid)
            {
                db.Quantities.Add(quantity);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Product_ID = new SelectList(db.Products, "ID", "Name", quantity.Product_ID);
            return View(quantity);
        }

        // GET: Quantities/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Quantity quantity = db.Quantities.Find(id);
            if (quantity == null)
            {
                return HttpNotFound();
            }
            ViewBag.Product_ID = new SelectList(db.Products, "ID", "Name", quantity.Product_ID);
            return View(quantity);
        }

        // POST: Quantities/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Product_ID,Origin,Remain,Product_Date")] Quantity quantity)
        {
            if (ModelState.IsValid)
            {
                db.Entry(quantity).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Product_ID = new SelectList(db.Products, "ID", "Name", quantity.Product_ID);
            return View(quantity);
        }

        // GET: Quantities/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Quantity quantity = db.Quantities.Find(id);
            if (quantity == null)
            {
                return HttpNotFound();
            }
            return View(quantity);
        }

        // POST: Quantities/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Quantity quantity = db.Quantities.Find(id);
            db.Quantities.Remove(quantity);
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
