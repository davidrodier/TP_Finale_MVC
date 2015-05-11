using System;
using System.Collections.Generic;
using TP_Finale_David_Rodier.Models;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;


namespace TP_Finale_David_Rodier.Models
{
   public class Game : Labo2.Class.SqlExpressWrapper
   {
      public int ID { get; set; }
      [Required]
      public string Name { get; set; }
      [Required]
      public string Creator { get; set; }
      [Required]
      [Display(Name = "Image Path")]
      public string Image_Path { get; set; }
      public int Rating { get; set; }

      public Game(Object connexionString)
         : base(connexionString)
      {
         SQLTableName = "GAME";
      }

      public Game()
         : base("")
      {
      }
      public override void GetValues()
      {
         ID = int.Parse(this["ID"]);
         Name = this["NAME"];
         Creator = this["CREATOR"];
         Image_Path = this["COVER"];
      }
      public override void Insert()
      {
         InsertRecord(Image_Path, Name, Creator);
      }
      //public bool Username_Exist(String username)
      //{
      //    QuerySQL("SELECT * FROM " + SQLTableName + " WHERE USERNAME = '" + username + "'");
      //    //if (reader.HasRows) 
      //    //    GetValues(); 
      //    return reader.HasRows;
      //}
      public override void Update()
      {
         UpdateRecord(ID, Image_Path, Rating, Name, Creator);
      }
   }
}