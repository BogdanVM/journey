using Journey.Models;
using Microsoft.AspNet.Identity;
using Microsoft.Security.Application;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Journey.Controllers
{
    public class PostController : Controller
    {
        private ApplicationDbContext db = ApplicationDbContext.Create();

        public ActionResult Index()
        {
            var posts = db.Posts.Include("Category").Include("User").Include("Comment")
                                .ToList()
                                .Where(p => p.UserId.Equals(User.Identity.GetUserId()));

            ViewBag.Posts = posts;
            return View();
        }

        [Authorize(Roles = "User,Administrator")]
        public ActionResult Show(int id)
        {
            Post post = db.Posts.Find(id);

            ViewBag.afisareButoane = false;

            if (User.IsInRole("User") || User.IsInRole("Administrator"))
            {
                ViewBag.afisareButoane = true;
            }

            ViewBag.esteAdmin = User.IsInRole("Administrator");
            ViewBag.utilizatorCurent = User.Identity.GetUserId();


            return View(post);

        }

        [Authorize(Roles = "User,Administrator")]
        [ValidateInput(false)]
        public ActionResult New()
        {
            Post post = new Post();

            post.Categories = GetAllCategories();
            post.UserId = User.Identity.GetUserId();
            post.Photo = post.UserId.ToString() + "_" +
                DateTime.Now.ToString("dd.MM.yyyy_hh.mm.ss") + ".jpg";

            return View(post);
        }

        [HttpPost]
        [Authorize(Roles = "User,Administrator")]
        [ValidateInput(false)]
        public ActionResult New(Post post)
        {
            string fileName = post.UserId.ToString() + "_" + 
                DateTime.Now.ToString("dd.MM.yyyy_hh.mm.ss") + ".jpg";
            post.Categories = GetAllCategories();
            post.Photo = fileName;

            try
            {
                if (ModelState.IsValid)
                {
                    HttpPostedFileBase file = post.PhotoFile;

                    if (file != null && file.ContentLength > 0)
                    {
                        string path = Path.Combine(Server.MapPath("/Images/"),
                                               fileName);
                        file.SaveAs(path);
                    }

                    post.Description = Sanitizer.GetSafeHtmlFragment(post.Description);

                    db.Posts.Add(post);
                    db.SaveChanges();
                    TempData["message"] = "Imaginea a fost postata!";
                    return RedirectToAction("Index");
                }
                else
                {
                    return View(post);
                }
            }
            catch (Exception e)
            {
                TempData["message"] = e.Message + "\n" + e.InnerException;
                return View(post);
            }
        }

        [Authorize(Roles = "User,Administrator")]
        public ActionResult Edit(int id)
        {
            Post post = db.Posts.Find(id);
            post.Categories = GetAllCategories();

            if (post.UserId == User.Identity.GetUserId() ||
                User.IsInRole("Administrator"))
            {
                return View(post);
            }
            else
            {
                TempData["message"] = "Nu aveti dreptul sa faceti modificari asupra unei postari care nu va apartine!";
                return RedirectToAction("Index");
            }

        }

        [HttpPut]
        [Authorize(Roles = "User,Administrator")]
        [ValidateInput(false)]
        public ActionResult Edit(int id, Post requestPost)
        {
            requestPost.Categories = GetAllCategories();

            try
            {
                if (ModelState.IsValid)
                {
                    Post post = db.Posts.Find(id);
                    if (post.UserId == User.Identity.GetUserId() ||
                        User.IsInRole("Administrator"))
                    {
                        if (TryUpdateModel(post))
                        {
                            post.Description = Sanitizer.GetSafeHtmlFragment(requestPost.Description);
                            post.Date = requestPost.Date;
                            post.CategoryId = requestPost.CategoryId;
                            db.SaveChanges();
                            TempData["message"] = "Postarea a fost modificata!";
                        }
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        TempData["message"] = "Nu aveti dreptul sa faceti modificari asupra unui articol care nu va apartine!";
                        return RedirectToAction("Index");
                    }

                }
                else
                {
                    TempData["message"] = "Aici sunt";
                    return View(requestPost);
                }

            }
            catch (Exception e)
            {
                TempData["message"] = e.Message;
                return View(requestPost);
            }
        }

        [HttpDelete]
        [Authorize(Roles = "User,Administrator")]
        public ActionResult Delete(int id)
        {
            Post post = db.Posts.Find(id);
            if (post.UserId == User.Identity.GetUserId() ||
                User.IsInRole("Administrator"))
            {
                db.Posts.Remove(post);
                db.SaveChanges();

                string fullPath = Path.Combine(Server.MapPath("/Images/"), post.Photo);

                if (System.IO.File.Exists(fullPath))
                {
                    System.IO.File.Delete(fullPath);
                }

                TempData["message"] = "Postarea a fost stearsa!";
                return RedirectToAction("Index");
            }
            else
            {
                TempData["message"] = "Nu aveti dreptul sa stergeti o postare care nu va apartine!";
                return RedirectToAction("Index");
            }

        }

        [NonAction]
        public IEnumerable<SelectListItem> GetAllCategories()
        {
            // generam o lista goala
            var selectList = new List<SelectListItem>();

            // Extragem toate categoriile din baza de date
            var categories = from cat in db.Categories
                             select cat;

            // iteram prin categorii
            foreach (var category in categories)
            {
                // Adaugam in lista elementele necesare pentru dropdown
                selectList.Add(new SelectListItem
                {
                    Value = category.CategoryId.ToString(),
                    Text = category.CategoryName.ToString()
                });
            }

            // returnam lista de categorii
            return selectList;
        }
    }
}