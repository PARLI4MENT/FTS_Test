using System;

namespace Fields
{
    public class ECD_list
    {
        /// <summary>
        /// Return table name (string)
        /// </summary>
        public static string GetTableName { get { return "ECD_list"; } }

        public string InnerID { get; set; }
        public string Status { get; set; }
        public Int16 DocsSended { get; set; }

        /// <summary>
        /// Get string command to Create table into Database
        /// Default table => "ECD_list"
        /// </summary>
        public static string GetSqlCommandCreator
        {
            get
            {
                return @"CREATE TABLE ""public"".""ECD_list"" (
                    ""InnerID"" varchar(255) NOT NULL, ""Status"" varchar(255), 
                    ""DocsSended"" int8 DEFAULT 0, PRIMARY KEY (""InnerID""));";
            }
        }

        /// <summary>
        /// Get string command to Insert into Database
        /// Table name => "ECD_list" 
        /// </summary>
        public string GetSqlCommandInsert
        {
            get
            {
                return $@"INSERT INTO ""public"".""ECD_list"" (""InnerID"", ""Status"", ""DocsSended"")
                    VALUES ('{InnerID}', '{Status}', {DocsSended})";
            }
        }


        //public string GetSqlCommandUpdate
        //{
        //    get
        //    {
        //        /// UPDATE films SET kind = 'Dramatic' WHERE kind = 'Drama';
        //        return $@"UPDATE ""public"".""ECD_list"" SET Status = 'Отправка в архив',
        //            DocsSended = DocsSended +1 WHERE InnerID = {}";
        //    }
        //}


    }
}