using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TP_Finale_David_Rodier.Models
{
   public class Login
   {
      public String Type { get; set; }
      public int ID { get; set; }
      [Required]
      public string Username { get; set; }
      [Required]
      [DataType(DataType.Password)]
      public string Password { get; set; }
   }
}