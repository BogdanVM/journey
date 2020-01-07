using Journey.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Journey.Controllers
{
    public class HomeController : Controller
    {
        private ApplicationDbContext db = ApplicationDbContext.Create();
        public ActionResult Index()
        {
            if (Request.IsAuthenticated)
            {
                return RedirectToAction("Index", "Post");
            }

            var posts = (from post in db.Posts select post);

            ViewBag.Posts = posts.OrderByDescending(p => p.Date);

            return View();
        }
    }
}