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
            Game games = new Game(Session["MainDB"]);
            games.SelectAll();

            return View(games);
        }
        [HttpGet]
        public ActionResult Connexion()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Connexion(Login log)
        {
            User users = new User(Session["MainDB"]);
            if (users.Username_Exist(log.Username))
            {
                if (users.Check_Password(log.Username, log.Password))
                {
                    Session["LogedUser"] = log.Username;
                    ViewBag.Error = "";
                    return RedirectToAction("Index");
                }
                else
                {
                    ViewBag.Error = "Mauvais mot de passe";
                }
            }
            else
            {
                ViewBag.Error = "Mauvais nom d'usager";
            }
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