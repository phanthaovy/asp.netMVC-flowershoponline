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
using System.Web.Security;

namespace FlowerShop.Controllers
{
    public class UsersController : Controller
    {
        private FlowerShopOnline db = new FlowerShopOnline();

        [Authorize(Roles = "Admin")]
        // GET: Users
        public ActionResult Index(string searching, int? page, string sortOrder)
        {
            int pageSize = 5;
            int pageNumber = (page ?? 1);
            var users = db.Users.Where((x => x.Username.Contains(searching) || searching == null)).Include(u => u.Role).ToList();
           ViewBag.NameSort = String.IsNullOrEmpty(sortOrder) ? "name" : "";

            switch (sortOrder)
            {
                case "name":
                    users = users.OrderByDescending(x => x.Username).ToList();
                    break;
            }
            return View(users.Where(x=>x.Is_Delete == 0).ToPagedList(pageNumber, pageSize));
        }
        //public ActionResult getName()
        //{

        //}
        [Authorize(Roles = "Admin")]
        public ActionResult Dashboard()

        {
            return View();
            // return View(getName());

        }
        [Authorize(Roles = "Admin")]
        // GET: Users/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }
        [Authorize(Roles = "Admin")]

        public ActionResult Create()
        {
            ViewBag.Role_ID = new SelectList(db.Roles, "ID", "Name");
                return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Username,Password,Fullname,Email,DateOfBirth,Phone,Address,Is_Delete,Role_ID,Avatar")] User user,HttpPostedFileBase file)
        {
            string folderPath = Server.MapPath("~/Images/");
            string fileName = Path.GetFileName(file.FileName);

            //Check whether Directory (Folder) exists.
            if (!Directory.Exists(folderPath))
            {
                //If Directory (Folder) does not exists. Create it.
                Directory.CreateDirectory(folderPath);
            }
            user.Avatar = "/Images/" + fileName;
            file.SaveAs(folderPath + fileName);
            if (ModelState.IsValid)
            {

                db.Users.Add(user);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Role_ID = new SelectList(db.Roles, "ID", "Name", user.Role_ID);
            return View(user);
        
       
        }

        [Authorize(Roles = "Admin")]


        // GET: Users/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            ViewBag.Role_ID = new SelectList(db.Roles, "ID", "Name", user.Role_ID);
            return View(user);
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Username,Password,Fullname,Email,DateOfBirth,Phone,Address,Is_Delete,Role_ID,Avatar")] User user)
        {
            if (ModelState.IsValid)
            {
                db.Entry(user).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Role_ID = new SelectList(db.Roles, "ID", "Name", user.Role_ID);
            return View(user);
        }
        [Authorize(Roles = "Admin")]
        // GET: Users/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            User user = db.Users.Find(id);
            user.Is_Delete = 1;
            db.Entry(user).State = EntityState.Modified;
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
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login", "Accounts");

        }
    }
}
