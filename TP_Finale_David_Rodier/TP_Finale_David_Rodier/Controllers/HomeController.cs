﻿using System;
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
        public ActionResult UpdateGame(String name, String creator, int id)
        {
            Game game = new Game();
            game.Name = name;
            game.Creator = creator;
            game.ID = id;

            String s = Session["LogedUser"].ToString() + Session["LogedType"].ToString();

            return View(game);
        }
        [HttpPost]
        public ActionResult UpdateGame(Game game)
        {
            Game games = new Game(Session["MainDB"]);
            games.SelectAll();
            games.UpdateRecord_Game(game.ID, game.Name, game.Creator);

            return RedirectToAction("Index");
        }
        [HttpGet]
        public ActionResult CreateGame()
        {
            return View();
        }
        public ActionResult Deco()
        {
            Session["LogedUser"] = null;
            Session["LogedType"] = null;
            return RedirectToAction("Index");
        }
        [HttpGet]
        public ActionResult DeleteGame(String name, String creator)
        {
            Game game = new Game();
            game.Name = name;
            game.Creator = creator;
            return View(game);
        }
        [HttpPost]
        public ActionResult DeleteGame(Game game)
        {
            Game games = new Game(Session["MainDB"]);
            games.DeleteRecordName(game.Name, game.Creator);
            return RedirectToAction("Index");
        }
        [HttpPost]
        public ActionResult CreateGame(Game game)
        {
            Game games = new Game(Session["MainDB"]);
            if (ModelState.IsValid)
            {
                if (Request.Files.Count > 0)
                {
                    var file = Request.Files[0];
                    if (file != null && file.ContentLength > 0)
                    {
                        game.Image_Path = Guid.NewGuid().ToString();
                        file.SaveAs(Server.MapPath(@"~\Images\") + game.Image_Path);
                    }
                }
                
                games.InsertRecord(game.Image_Path, game.Name, game.Creator);
                ViewBag.Error = "";

                return RedirectToAction("Index");
            }
            return View();
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
            users.SelectAll();
            if (users.Username_Exist(log.Username))
            {
                if (users.Check_Password(log.Username, log.Password))
                {
                    Session["LogedUser"] = log.Username;
                    Session["LogedType"] = users.Select_Type(log.Username);
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