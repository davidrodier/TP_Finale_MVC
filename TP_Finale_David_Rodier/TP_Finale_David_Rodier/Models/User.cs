using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TP_Finale_David_Rodier.Models
{
    public class User : Labo2.Class.SqlExpressWrapper
    {
        public int ID { get; set; }
        [Required]
        [StringLength(50, MinimumLength = 3, ErrorMessage="Votre nom d'usager doit être entre 3 et 50 caractères")]
        public string Username { get; set; }
        [Required]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Votre nom d'usager doit être entre 3 et 50 caractères")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Required]
        [Display(Name="Password Confirmation")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage="La confirmation du mot de passe n'est pas identique au mot de passe")]
        public string Password_Validation {get;set;}

        public User(Object connexionString)
            : base(connexionString)
        {
            SQLTableName = "USERS";
        }

        public User()
            : base("")
        {
        }
        public override void GetValues()
        {
            ID = int.Parse(this["ID"]);
            Username = this["USERNAME"];
            Password = this["PASSWORD"];
        }
        public override void Insert()
        {
            InsertRecord(Username, Password);
        }
        public bool Username_Exist(String username)
        {
            QuerySQL("SELECT * FROM " + SQLTableName + " WHERE USERNAME = '" + username + "'");
            //if (reader.HasRows) 
            //    GetValues(); 
            return reader.HasRows;
        }
        public bool Check_Password(String username, String password)
        {
            QuerySQL("SELECT * FROM " + SQLTableName + " WHERE USERNAME = '" + username + "' AND PASSWORD='" + password + "'");
            return reader.HasRows;
        }
        public override void Update()
        {
            UpdateRecord(ID, Username, Password);
        }
    }

}