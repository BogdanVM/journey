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
    public class CommentController : Controller
    {
        private ApplicationDbContext db = ApplicationDbContext.Create();

        [Authorize(Roles = "User,Administrator")]
        [ValidateInput(false)]
        [HttpPost]
        public ActionResult New(int id, string content)
        {
            Post post = db.Posts.Find(id);

            try
            {
                if (ModelState.IsValid)
                {
                    Comment comment = new Comment();
                    comment.Content = Sanitizer.GetSafeHtmlFragment(content);
                    comment.Date = DateTime.Now;
                    comment.PostId = id;
                    comment.UserId = User.Identity.GetUserId();

                    db.Comments.Add(comment);
                    db.SaveChanges();
                    TempData["message"] = "Comentariul a fost postat!";

                    return RedirectToAction("Show", "Post", new { id = id });   
                }
                else
                {
                    return View("Show", "Post", post);
                }
            }
            catch (Exception e)
            {
                TempData["message"] = e.Message + "\n" + e.InnerException;
                return View("Show", "Post", post);
            }
        }

        [Authorize(Roles = "User,Administrator")]
        public ActionResult Edit(int id)
        {
            Comment comment = db.Comments.Find(id);

            if (comment.UserId == User.Identity.GetUserId() ||
                User.IsInRole("Administrator"))
            {
                return View(comment);
            }
            else
            {
                TempData["message"] = "Nu aveti dreptul sa faceti modificari asupra unui comentariu care nu va apartine!";
                return RedirectToAction("Show", "Post", new { id = comment.PostId });
            }

        }

        [HttpPut]
        [Authorize(Roles = "User,Administrator")]
        [ValidateInput(false)]
        public ActionResult Edit(int id, string content)
        {
            Comment comment = db.Comments.Find(id);

            try
            {
                if (ModelState.IsValid)
                {
                    if (comment.UserId == User.Identity.GetUserId() ||
                        User.IsInRole("Administrator"))
                    {
                        if (TryUpdateModel(comment))
                        {
                            comment.Content = Sanitizer.GetSafeHtmlFragment(content);
                            db.SaveChanges();
                            TempData["message"] = "Comentariul a fost modificat!";
                        }
                        return RedirectToAction("Show", "Post", new { id = comment.PostId });
                    }
                    else
                    {
                        TempData["message"] = "Nu aveti dreptul sa faceti modificari asupra unui comentariu care nu va apartine!";
                        return RedirectToAction("Show", "Post", new { id = comment.PostId });
                    }

                }
                else
                {
                    TempData["message"] = "Aici sunt";
                    return View(comment);
                }

            }
            catch (Exception e)
            {
                TempData["message"] = e.Message;
                return View(comment);
            }
        }

        [HttpDelete]
        [Authorize(Roles = "User,Administrator")]
        public ActionResult Delete(int id)
        {
            Comment comment = db.Comments.Find(id);

            if (comment.UserId == User.Identity.GetUserId() ||
                User.IsInRole("Administrator"))
            {
                db.Comments.Remove(comment);
                db.SaveChanges();

                TempData["message"] = "Comentariul a fost sters!";
                return RedirectToAction("Show", "Post", new { id = comment.PostId });
            }
            else
            {
                TempData["message"] = "Nu aveti dreptul sa stergeti un comentariu care nu va apartine!";
                return RedirectToAction("Show", "Post", new { id = comment.PostId });
            }

        }
    }
}