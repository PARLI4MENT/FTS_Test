using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XMLSigner.SQL
{
    public interface ISql
    {
        void ExecuteToDB(string[] args);
    }
}
