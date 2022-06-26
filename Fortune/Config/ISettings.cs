using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fortune.Config
{
    public interface ISettings
    {
        string GetCosmosDBEndPoint();

        string GetCosmosDBKey();

        string GetCosmosDBName();

        bool IsDevelopment();
        string GetHostName();
    }
}
