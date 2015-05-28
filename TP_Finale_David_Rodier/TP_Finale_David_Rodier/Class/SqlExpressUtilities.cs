using System;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Web.UI.WebControls;
using System.Web.UI;

namespace Labo2.Class
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////
    // SqlExpressWrapper version beta
    ////////////////////////////////////////////////////////////////////////////////////////////////////
    // Cette classe offre une interface conviviale au programmeur utilisateur pour des transactions SQL
    // avec une table d'une base de données SQL Express
    // Note importante:
    // Afin de profiter des toutes les fonctionnalités de cette classe
    // assurez-vous que le premier champ soit ID de type BigInt INDETITY(1,1) dans la structure des 
    // tables visées
    ////////////////////////////////////////////////////////////////////////////////////////////////////
    // Auteur : Nicolas Chourot
    // Départment d'informatique
    // Collège Lionel-Groulx
    ////////////////////////////////////////////////////////////////////////////////////////////////////
    public class SqlExpressWrapper
    {
        // objet de connection
        SqlConnection connection;
        // chaine de connection
        public string connexionString;
        // Objet de lecture issue de la dernière requête SQL
        public SqlDataReader reader;
        // Nom de la table
        public String SQLTableName = "";
        // Liste des valeur des champs lors de la lecture de la requête 
        public List<string> FieldsValues = new List<string>();
        // Liste des noms des champs de la table en cours de lecture
        public List<string> FieldsNames = new List<string>();
        // Liste des types des champs de la table en cours de lecture
        public List<Type> FieldsTypes = new List<Type>();

        // contructeur obligatoire auquel il faut fournir la chaine de connection et l'objet Page
        public SqlExpressWrapper(Object connexionString)
        {
            this.connexionString = connexionString.ToString();
        }

        // Extraire les valeur des champs de l'enregistrement suivant du lecteur Reader
        bool GetfieldsValues()
        {
            bool endOfReader = false;
            // Effacer la liste des valeurs
            FieldsValues.Clear();
            // si il reste des enregistrements à lire
            if (endOfReader = reader.Read())
            {
                // Extraire les valeurs des champs
                for (int f = 0; f < reader.FieldCount; f++)
                {
                    // la fonction Trim permet l'effacement des espaces en trop 
                    // avant et après la valeur proprement dite
                    FieldsValues.Add(SQLHelper.FromSql(reader[f].ToString().Trim()));
                }
            }
            return endOfReader;
        }

        private bool Valid(int index, int count)
        {
            return ((index >= 0) && (index < count));
        }

        // Extraire les noms et types des champs 
        void GetFieldsNameAndType()
        {
            if (reader != null)
            {
                FieldsNames.Clear();
                FieldsTypes.Clear();
                for (int f = 0; f < reader.FieldCount; f++)
                {
                    if (reader.GetName(f) == "Id")
                        FieldsNames.Add("ID");
                    else
                        FieldsNames.Add(reader.GetName(f));
                    FieldsTypes.Add(reader.GetFieldType(f));
                }
            }
        }

        public virtual void GetValues()
        {
            // Doit être surcharché par les classes dérivées
        }

        // Saisir les valeurs du prochain enregistrement du Reader
        public bool Next()
        {
            bool more = NextRecord();
            if (more)
                GetValues();
            else
                EndQuerySQL();
            return more;
        }

        // Passer à l'enregistrement suivant du lecteur de requête
        public bool NextRecord()
        {
            return GetfieldsValues();
        }

        // Exécuter une commande SQL
        public int QuerySQL(string sqlCommand)
        {
            // instancier l'objet de collection
            connection = new SqlConnection(connexionString);
            // bâtir l'objet de requête
            SqlCommand sqlcmd = new SqlCommand(sqlCommand);
            // affecter l'objet de connection à l'objet de requête
            sqlcmd.Connection = connection;
            // ouvrir la connection avec la bd
            connection.Open();
            // éxécuter la requête SQL et récupérer les enregistrements qui en découlent dans l'objet Reader
            reader = sqlcmd.ExecuteReader();
            // Saisir les noms et types des champs de la table impliquée dans la requête
            GetFieldsNameAndType();
            // retourner le nombre d'enregistrements générés
            return reader.RecordsAffected;

        }

        // Conclure la dernière requête
        public void EndQuerySQL()
        {
            // Fermer la connection
            if (connection.State != System.Data.ConnectionState.Closed)
                connection.Close();

        }

        // Extraire tous les enregistrements
        public virtual bool SelectAll(string orderBy = "")
        {
            string sql = "SELECT * FROM " + SQLTableName;
            if (orderBy != "")
                sql += " ORDER BY " + orderBy;
            QuerySQL(sql);
            return reader.HasRows;
        }

        public virtual bool SelectAll_Game()
        {
            string sql = "SELECT ID, COVER, NAME, CREATOR, R.RATING FROM GAME G INNER JOIN RATING R ON G.ID=R.GAME_ID INNER JOIN USER U ON U.ID=R.USER_ID";
            QuerySQL(sql);
            return reader.HasRows;
        }

        // Extraire l'enregistrement d'id ID
        public bool SelectByID(String ID)
        {
            string sql = "SELECT * FROM " + SQLTableName + " WHERE ID = " + ID;
            QuerySQL(sql);
            if (reader.HasRows)
                Next();
            return reader.HasRows;
        }

        public bool SelectByFieldName(String FieldName, object value)
        {
            string SQL = "SELECT * FROM " + SQLTableName + " WHERE " + FieldName + " = ";
            Type type = value.GetType();
            if (SQLHelper.IsNumericType(type))
                SQL += value.ToString().Replace(',', '.');
            else
                if (type == typeof(DateTime))
                    SQL += "'" + SQLHelper.DateSQLFormat((DateTime)value) + "'";
                else
                    SQL += "'" + SQLHelper.PrepareForSql(value.ToString()) + "'";
            QuerySQL(SQL);
            if (reader.HasRows)
                Next();
            return reader.HasRows;
        }
        // Mise à jour de l'enregistrement
        public virtual void Update()
        {
            UpdateRecord();
        }

        // Met à jour de l'enregistrement courant par le biais des valeurs inscrites dans la liste
        // FieldsValues
        public int UpdateRecord()
        {
            String SQL = "UPDATE " + SQLTableName + " ";
            SQL += "SET ";
            int nb_fields = FieldsNames.Count();
            for (int fieldNum = 1; fieldNum < nb_fields; fieldNum++)
            {
                SQL += "[" + FieldsNames[fieldNum] + "] = ";
                if (FieldsTypes[fieldNum] == typeof(DateTime))
                    SQL += "'" + SQLHelper.DateSQLFormat(DateTime.Parse(FieldsValues[fieldNum])) + "'";
                else
                    SQL += "'" + SQLHelper.PrepareForSql(FieldsValues[fieldNum]) + "'";
                if (fieldNum < (nb_fields - 1)) SQL += ", ";
            }
            SQL += " WHERE [" + FieldsNames[0] + "] = " + FieldsValues[0];

            return NonQuerySQL(SQL);
        }

        // Met à jour de l'enregistrement courant par le biais des valeurs inscrites dans la liste
        // FieldsValues fournie en paramètre
        public int UpdateRecord(params object[] FieldsValues)
        {
            String SQL = "UPDATE " + SQLTableName + " ";
            SQL += "SET ";
            int nb_fields = FieldsValues.Length;
            for (int i = 1; i < nb_fields; i++)
            {
                SQL += "[" + FieldsNames[i] + "] = ";
                Type type = FieldsValues[i].GetType();
                if (SQLHelper.IsNumericType(type))
                    SQL += FieldsValues[i].ToString().Replace(',', '.');
                else
                    if (type == typeof(DateTime))
                        SQL += "'" + SQLHelper.DateSQLFormat((DateTime)FieldsValues[i]) + "'";
                    else
                        SQL += "'" + SQLHelper.PrepareForSql(FieldsValues[i].ToString()) + "'";
                if (i < (nb_fields - 1)) SQL += ", ";
            }
            SQL += " WHERE [" + FieldsNames[0] + "] = " + FieldsValues[0];

            return NonQuerySQL(SQL);
        }

        // Effacer l'enregistrement d'id ID
        public void DeleteRecordByID(String ID)
        {
            String sql = "DELETE FROM " + SQLTableName + " WHERE ID = " + ID;
            NonQuerySQL(sql);
        }

        public void DeleteRecordName(String name, String creator)
        {
            String sql = "DELETE FROM " + SQLTableName + " WHERE NAME = '" + name + "' AND CREATOR = '" + creator + "'";
            NonQuerySQL(sql);
        }

        // Insérer un nouvel enregistrement
        public virtual void Insert()
        {
            InsertRecord();
        }

        // insérer un nouvel enregistrement en utilisant les valeurs stockées dans FieldValues
        public void InsertRecord()
        {
            // Petite patch pour s'assurer que les noms des champs et leur type soient initialisés
            SelectAll();
            NextRecord();
            EndQuerySQL();

            string sql = "INSERT INTO " + SQLTableName + "(";
            for (int i = 1; i < FieldsNames.Count; i++)
            {
                sql += FieldsNames[i];
                if (i < FieldsNames.Count - 1)
                    sql += ", ";
                else
                    sql += ") VALUES (";
            }
            for (int i = 0; i < FieldsValues.Count; i++)
            {
                Type type = FieldsValues[i].GetType();
                if (SQLHelper.IsNumericType(type))
                    sql += FieldsValues[i].ToString().Replace(',', '.');
                else
                    if (type == typeof(DateTime))
                        sql += "'" + SQLHelper.DateSQLFormat((DateTime)DateTime.Parse(FieldsValues[i])) + "'";
                    else
                        sql += "'" + SQLHelper.PrepareForSql(FieldsValues[i].ToString()) + "'";

                if (i < FieldsValues.Count - 1)
                    sql += ", ";
                else
                    sql += ")";
            }
            NonQuerySQL(sql);
        }
        // insérer un nouvel enregistrement en utilisant les valeurs stockées dans FieldValues passé en paramètre
        public void InsertRecord(params object[] FieldsValues)
        {
            // Petite patch pour s'assurer que les noms des champs et leur type soient initialisés
            SelectAll();
            NextRecord();
            EndQuerySQL();

            string sql = "INSERT INTO " + SQLTableName + "(";
            for (int i = 1; i < FieldsValues.Length + 1; i++)
            {
                sql += FieldsNames[i];
                if (i < FieldsValues.Length)
                    sql += ", ";
                else
                    sql += ") VALUES (";
            }
            for (int i = 0; i < FieldsValues.Length; i++)
            {
                Type type = FieldsValues[i].GetType();
                if (SQLHelper.IsNumericType(type))
                    sql += FieldsValues[i].ToString().Replace(',', '.');
                else
                    if (type == typeof(DateTime))
                        sql += "'" + SQLHelper.DateSQLFormat((DateTime)FieldsValues[i]) + "'";
                    else
                        sql += "'" + SQLHelper.PrepareForSql(FieldsValues[i].ToString()) + "'";

                if (i < FieldsValues.Length - 1)
                    sql += ", ";
                else
                    sql += ")";
            }
            NonQuerySQL(sql);
        }

        // Éxécuter une requête SQL qui ne génère pas d'enregistrement
        public int NonQuerySQL(string sqlCommand)
        {
            int recordsAffected = QuerySQL(sqlCommand);
            EndQuerySQL();
            return recordsAffected;
        }

        // retourne l'indexe du champs de nom fieldName
        public int IndexOf(string fieldName)
        {
            return FieldsNames.IndexOf(fieldName);
        }

        // Surchage de l'opérateur [] pour pouvoir atteindre un champ par son nom
        // par exemple : tableUsers["UserName"]
        public string this[string fieldName]
        {
            get { return FieldsValues[IndexOf(fieldName)]; }
            set { FieldsValues[IndexOf(fieldName)] = value; }
        }
    }

    public class SQLHelper
    {

        // pour éviter des erreurs de syntaxe dans les requêtes sql
        static public string PrepareForSql(string text)
        {
            return text.Replace("'", "&c&");
        }

        static public string FromSql(string text)
        {
            return text.Replace("&c&", "'");
        }

        static string TwoDigit(int n)
        {
            string s = n.ToString();
            if (n < 10)
                s = "0" + s;
            return s;

        }

        public static string DateSQLFormat(DateTime date)
        {
            return date.Year + "-" + TwoDigit(date.Month) + "-" + TwoDigit(date.Day) + " " + TwoDigit(date.Hour) + ":" + TwoDigit(date.Minute) + ":" + TwoDigit(date.Second) + ".000";
        }

        public static bool IsNumericType(Type type)
        {
            if (type == null)
            {
                return false;
            }

            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Byte:
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.SByte:
                case TypeCode.Single:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                    return true;
                case TypeCode.Object:
                    if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
                    {
                        return IsNumericType(Nullable.GetUnderlyingType(type));
                    }
                    return false;
            }
            return false;
        }
    }
}