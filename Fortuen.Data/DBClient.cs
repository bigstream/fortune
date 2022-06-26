using System;
using System.Threading.Tasks;
using System.Configuration;
using System.Collections.Generic;
using System.Net;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Caching.Memory;

namespace Fortune.Data
{
    public class DBClient
    {
        private const string DbVersionPartitionKey = "00c0bc7b-8278-4dd7-b3e2-76c75803f267";

        private const string SysUser = "sys";

        private readonly Container _fortuneContainer;

        private readonly CosmosClient _cosmosClient;

        private readonly Database _database;

        private readonly ISettings _settings

        private readonly IMemoryCache _memoryCache;

        public DBClient(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;

            var
        }
    }
}