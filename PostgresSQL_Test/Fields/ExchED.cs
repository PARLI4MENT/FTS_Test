namespace Fields
{
    public class ExchED
    {
        public static string GetSqlCommandCreator
        {
            get
            {
                return @$"CREATE TABLE ""public"".""ExchED"" (""InnerID"" varchar NOT NULL,
                ""MessageType"" varchar(255), ""EnvelopeID"" varchar, ""CompanySet_key_id"" int4,
                ""DocumentID"" varchar, ""DocName"" varchar(255), ""DocNum"" varchar(255),
                ""DocCode"" varchar(255), ""ArchFileName"" varchar(255), PRIMARY KEY(""InnerID""));";
            }
        }
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
