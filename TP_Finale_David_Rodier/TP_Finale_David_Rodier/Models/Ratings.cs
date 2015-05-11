using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TP_Finale_David_Rodier.Models
{
   public class Ratings :Labo2.Class.SqlExpressWrapper
   {
      public int User_ID { get; set; }
      public int Game_ID { get; set; }
      public int Rating { get; set; }

              public Ratings(Object connexionString)
            : base(connexionString)
        {
            SQLTableName = "RATING";
        }

        public Ratings()
            : base("")
        {
        }
        public override void GetValues()
        {
            Game_ID = int.Parse(this["GAME_ID"]);
            User_ID = int.Parse(this["USER_ID"]);
            Rating = int.Parse(this["RATING"]);
        }
        public override void Insert()
        {
            InsertRecord(User_ID, Game_ID, Rating);
        }
        public override void Update()
        {
            UpdateRecord(User_ID, Game_ID, Rating);
        }

   }
}