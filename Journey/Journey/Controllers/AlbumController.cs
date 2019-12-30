using Journey.Models;
using Microsoft.AspNet.Identity;
using Microsoft.Security.Application;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Journey.Controllers
{
    public class AlbumController : Controller
    {
        private ApplicationDbContext db = ApplicationDbContext.Create();
        public ActionResult Index()
        {
            var albums = db.Albums.Include("Post").Include("User")
                            .ToList()
                            .Where(p => p.UserId.Equals(User.Identity.GetUserId()));

            ViewBag.Albums = albums;
            return View();
        }

        [Authorize(Roles = "User,Administrator")]
        public ActionResult Show(int id)
        {
            Album album = db.Albums.Find(id);

            ViewBag.afisareButoane = false;

            if (User.IsInRole("User") || User.IsInRole("Administrator"))
            {
                ViewBag.afisareButoane = true;
            }

            ViewBag.esteAdmin = User.IsInRole("Administrator");
            ViewBag.utilizatorCurent = User.Identity.GetUserId();

            ViewBag.Posts = album.Post.ToList();
            return View(album);

        }

        [Authorize(Roles = "User,Administrator")]
        [ValidateInput(false)]
        public ActionResult New()
        {
            Album album = new Album();
            album.UserId = User.Identity.GetUserId();
            return View(album);
        }

        [HttpPost]
        [Authorize(Roles = "User,Administrator")]
        [ValidateInput(false)]
        public ActionResult New(Album album)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    album.Name = Sanitizer.GetSafeHtmlFragment(album.Name);

                    db.Albums.Add(album);
                    db.SaveChanges();
                    TempData["message"] = "Albumul a fost adaugat!";
                    return RedirectToAction("Index");
                }
                else
                {
                    return View(album);
                }
            }

            catch (Exception e)
            {
                TempData["message"] = e.Message + "\n" + e.InnerException;
                return View(album);
            }
        }

        [Authorize(Roles = "User,Administrator")]
        public ActionResult Edit(int id)
        {
            Album album = db.Albums.Find(id);

            if (album.UserId == User.Identity.GetUserId() ||
                User.IsInRole("Administrator"))
            {
                return View(album);
            }
            else
            {
                TempData["message"] = "Nu aveti dreptul sa faceti modificari asupra unui album care nu va apartine!";
                return RedirectToAction("Index");
            }

        }

        [HttpPut]
        [Authorize(Roles = "User,Administrator")]
        [ValidateInput(false)]
        public ActionResult Edit(int id, string name)
        {
            Album album = db.Albums.Find(id);

            try
            {
                if (ModelState.IsValid)
                {
                    if (album.UserId == User.Identity.GetUserId() ||
                        User.IsInRole("Administrator"))
                    {
                        if (TryUpdateModel(album))
                        {
                            album.Name = Sanitizer.GetSafeHtmlFragment(name);
                            db.SaveChanges();
                            TempData["message"] = "Albumul a fost modificat!";
                        }
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        TempData["message"] = "Nu aveti dreptul sa faceti modificari asupra unui comentariu care nu va apartine!";
                        return RedirectToAction("Index");
                    }

                }
                else
                {
                    TempData["message"] = "Aici sunt";
                    return View(album);
                }

            }
            catch (Exception e)
            {
                TempData["message"] = e.Message;
                return View(album);
            }
        }
    }
}