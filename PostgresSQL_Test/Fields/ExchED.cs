namespace Fields
{
    public class ExchED
    {
        /// <summary>
        /// Get string command to Create table into Database
        /// Default table => "ExchED"
        /// </summary>
        public static string GetSqlCommandCreator
        {
            get
            {
                return @"CREATE TABLE ""public"".""ExchED"" (""InnerID"" varchar NOT NULL,
                ""MessageType"" varchar(255), ""EnvelopeID"" varchar, ""CompanySet_key_id"" int4,
                ""DocumentID"" varchar, ""DocName"" varchar(255), ""DocNum"" varchar(255),
                ""DocCode"" varchar(255), ""ArchFileName"" varchar(255), PRIMARY KEY(""InnerID""));";
            }
        }

        /// <summary>
        /// Get string command to Insert into Database
        /// Table name => "ExchED" 
        /// </summary>
        /// INSERT INTO "public"."ExchED"
        /// ("InnerID", "MessageType", "EnvelopeID", "CompanySet_key_id", "DocumentID", "DocName", "DocNum", "DocCode", "ArchFileName")
        /// VALUES ('InnerID_1', 'MessageType_1', 'EnvelopeID_1', 1, 'DocumentID_1', 'DocName', 'DocNum', 'DocCode', 'ArchFileName_1');
        public string GetSqlCommandInsert
        {
            get
            {
                return $@"INSERT INTO ""public"".""ExchED""
                    (""InnerID"", ""MessageType"", ""EnvelopeID"", ""CompanySet_key_id"",
                    ""DocumentID"", ""DocName"", ""DocNum"", ""DocCode"", ""ArchFileName"")
                    VALUES ('{InnerID}', '{MessageType}', '{EnvelopeID}', {CompanySet_key_id}, '{DocumentID}',
                    '{DocName}', '{DocNum}', '{DocCode}', '{ArchFileName}');";
            }
        }

        /// <summary>
        /// Return table name
        /// </summary>
        public static string GetTableName { get { return "ExchED"; } }

        public string InnerID { get;set; }
        public string MessageType { get;set; }
        public string EnvelopeID { get;set; }
        public Int16 CompanySet_key_id { get;set; }
        public string DocumentID { get;set; }
        public string DocName { get;set; }
        public string DocNum { get; set; }
        public string DocCode { get;set; }
        public string ArchFileName { get;set; }

    }
}
