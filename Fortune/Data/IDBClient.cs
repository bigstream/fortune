using Microsoft.Azure.Cosmos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fortune.Data
{
    public interface IDBClient
    {
        Database Database { get; }

        Container FortuneContainer { get; }
    }
}
