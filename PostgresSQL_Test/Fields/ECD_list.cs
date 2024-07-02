namespace Fields
{
    public class ECD_list
    {
        public static string GetSqlCommandCreator
        {
            get
            {
                return $@"CREATE TABLE ""public"".""ECD_list"" (
                    ""InnerID"" varchar(255) NOT NULL, ""Status"" varchar(255), ""DocsSended"" int8,
                    PRIMARY KEY (""InnerID""));";
            }
        }
        public string InnerID { get; set; }
        public string Status { get; set; }
        public Int16 DocsSended { get;set; }
    }
}
