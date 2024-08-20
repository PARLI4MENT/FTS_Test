using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XMLSigner.SQL
{
    interface ISql
    {
        string Server { get; set; }
        string Port { get; set; }
        string Uid { get; set; }
        string Password { get; set; }
        string Database { get; set; }


        void ExecuteToDB(string[] args);
        void ExecuteToDB(string[] args, int Company_key_id);
    }
}
