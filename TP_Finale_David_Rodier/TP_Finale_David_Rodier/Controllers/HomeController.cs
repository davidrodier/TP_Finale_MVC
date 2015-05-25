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

         List<Game> GamesSelect = new List<Game>();

         while (games.Next())
         {
            Game temp = new Game();
            temp.ID = games.ID;
            temp.Name = games.Name;
            temp.Creator = games.Creator;
            temp.Image_Path = games.Image_Path;
            temp.Rating = games.Rating;
            GamesSelect.Add(temp);
         }
         ViewBag.Page = 0;
         return View(GamesSelect);
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
               ViewBag.Error = "Mot de passe eronné";
            }
         }
         else
         {
            ViewBag.Error = "Nom d'usager eronné";
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