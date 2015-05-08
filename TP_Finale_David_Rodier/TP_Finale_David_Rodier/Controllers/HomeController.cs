using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TP_Finale_David_Rodier.Models;

namespace TP_Finale_David_Rodier.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public ActionResult Inscription()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Inscription(User newUser)
        {
            User users = new User(Session["MainDB"]);
            if (ModelState.IsValid)
            {
                if (!users.Username_Exist(newUser.Username))
                {
                    users.InsertRecord(newUser.Username, newUser.Password);
                    ViewBag.Error = "";

                    return RedirectToAction("Index");
                }
                else
                {
                    ViewBag.Error = "Nom d'usager existant";
                }
            }
            return View();
        }
    }
}