using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using FlowerShop.Models;
using System.IO;
using PagedList;
using System.Data.Entity.Validation;

namespace FlowerShop.Controllers
{
    public class ProductsController : Controller
    {
        private FlowerShopOnline db = new FlowerShopOnline();
        [Authorize(Roles = "Admin")]
        // GET: Products
        public ActionResult Index(string searching, int? page, string sortOrder)
        {

            int pageSize = 5;
            int pageNumber = (page ?? 1);
            ViewBag.NameSort = String.IsNullOrEmpty(sortOrder) ? "name" : "";
            var products = db.Products.Where(x => x.Name.Contains(searching) || searching == null).Include(p => p.Category).Include(p => p.Comment).Include(p => p.Photos).Include(p => p.Quantities).ToList(); ;

            switch (sortOrder)
            {
                case "name":
                    products = products.OrderByDescending(x => x.Name).ToList();
                    break;
            }

            return View(products.ToPagedList(pageNumber, pageSize));
        }
        [Authorize(Roles = "Admin")]
        // GET: Products/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }
        [Authorize(Roles = "Admin")]
        // GET: Products/Create
        public ActionResult Create()
        {
            ViewBag.Category_ID = new SelectList(db.Categories, "ID", "Name");
            ViewBag.ID = new SelectList(db.Photos, "ID", "PhotoName");

            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Name,Description,Is_Delete,Category_ID")] Product product, Photo photoModel, HttpPostedFileBase file, Quantity quantity, int origin, DateTime product_date, decimal origin_price, decimal promotion_price, Price price)
        {

            string folderPath = Server.MapPath("~/Images/");
            string fileName = Path.GetFileName(file.FileName);
            //Check whether Directory (Folder) exists.
            if (!Directory.Exists(folderPath))
            {
                //If Directory (Folder) does not exists. Create it.
                Directory.CreateDirectory(folderPath);
            }

            photoModel.PhotoName = "/Images/" + fileName;
            file.SaveAs(folderPath + fileName);

            if (ModelState.IsValid)
            {
                price.Origin_Price = origin_price;
                price.Promotion_Price = promotion_price;
                price.Final_Price = origin_price - (origin_price * (promotion_price / 100));
                quantity.Product_Date = product_date;
                quantity.Origin = origin;
                quantity.Remain = origin;
                product.Photos = new List<Photo>();
                product.Quantities = new List<Quantity>();
                product.Prices = new List<Price>();
                product.Photos.Add(photoModel);
                product.Quantities.Add(quantity);
                product.Prices.Add(price);
                db.Products.Add(product);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.photoName = db.Photos.Select(x => x.PhotoName);
            ViewBag.Category_ID = new SelectList(db.Categories, "ID", "Name", product.Category_ID);
            ViewBag.ID = new SelectList(db.Photos, "ID", "PhotoName", product.ID);
            return View(product);
        }
        [Authorize(Roles = "Admin")]
        // GET: Products/Edit/5
        public ActionResult Edit(int? id)
        {

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            ViewBag.Category_ID = new SelectList(db.Categories, "ID", "Name", product.Category_ID);
            ViewBag.ID = new SelectList(db.Photos, "ID", "PhotoName");
            return View(product);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Name,Description,Is_Delete,Category_ID")]  Product product, Photo photoModel, HttpPostedFileBase file, int id)
        {
            
            if (ModelState.IsValid)
            {
                string folderPath = Server.MapPath("~/Images/");
                string fileName = Path.GetFileName(file.FileName);
                //Check whether Directory (Folder) exists.
                if (!Directory.Exists(folderPath))
                {
                    //If Directory (Folder) does not exists. Create it.
                    Directory.CreateDirectory(folderPath);
                }

                photoModel.PhotoName = "/Images/" + fileName;
                file.SaveAs(folderPath + fileName);
                photoModel.Product_ID = product.ID;
                db.Entry(photoModel).State = EntityState.Modified;
                db.Entry(product).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Category_ID = new SelectList(db.Categories, "ID", "Name", product.Category_ID);
            ViewBag.ID = new SelectList(db.Photos, "ID", "PhotoName");
            return View(product);

        }
        [Authorize(Roles = "Admin")]
        // GET: Products/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Product product = db.Products.Find(id);
            db.Products.Remove(product);
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
