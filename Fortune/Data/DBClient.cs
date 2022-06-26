using System;
using System.Threading.Tasks;
using System.Configuration;
using System.Collections.Generic;
using System.Net;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Caching.Memory;
using Fortune.Config;
using Microsoft.Azure.Cosmos.Fluent;
using Fortune.Models;
using Fortune.Extensions;

namespace Fortune.Data
{
    public class DBClient : IDBClient
    {
        private const string DbVersionPartitionKey = "00c0bc7b-8278-4dd7-b3e2-76c75803f267";

        private const string SysUser = "sys";

        private readonly Container _fortuneContainer;

        private readonly CosmosClient _cosmosClient;

        private readonly Database _database;

        private readonly ISettings _settings;

        private readonly IMemoryCache _memoryCache;

        public DBClient(ISettings settings, IMemoryCache memoryCache)
        {
            _settings = settings;
            _memoryCache = memoryCache;

            var cosmosDBEndpoint = settings.GetCosmosDBEndPoint();
            var cosmosDBKey = settings.GetCosmosDBKey();

            var cosmosClientBuilder = new CosmosClientBuilder(cosmosDBEndpoint, cosmosDBKey);

            if (_settings.IsDevelopment())
            {
                cosmosClientBuilder = cosmosClientBuilder.WithConnectionModeGateway();
            }
            else
            {
                cosmosClientBuilder = cosmosClientBuilder.WithConnectionModeDirect();
            }

            _cosmosClient = cosmosClientBuilder.Build();

            var cosmosDbName = _settings.GetCosmosDBName();

            _database = _cosmosClient.CreateDatabaseIfNotExistsAsync(cosmosDbName).Result;

            // Clear TEST containers
            if (cosmosDbName.Contains("_TEST"))
            {
                var iterator = _database.GetContainerQueryIterator<ContainerProperties>();
                var containerProps = iterator.ReadNextAsync().Result;

                foreach (var property in containerProps)
                {
                    _ = _database.GetContainer(property.Id).DeleteContainerAsync().GetAwaiter().GetResult();
                }
            }

            _fortuneContainer = _database.CreateContainerIfNotExistsAsync(nameof(FortuneTemplate), "/id").Result;
        }

        public Container FortuneContainer => _fortuneContainer;

        public Database Database => _database;

        public async Task<FortuneTemplate> GetTemplateById(Guid templateId)
        {
            FortuneTemplate template = null;
            var cacheKey = $"{nameof(GetTemplateById)}:{templateId}";

            ItemResponse<FortuneTemplate> cacheEntry = null;
            try
            {
                if (_memoryCache.TryGetValue(cacheKey, out cacheEntry))
                {
                    var response = await _fortuneContainer.ReadItemAsync<FortuneTemplate>(templateId.ToString(), new PartitionKey(templateId.ToString()), new ItemRequestOptions() { IfNoneMatchEtag = cacheEntry.ETag });

                    if (response.StatusCode == HttpStatusCode.NotModified)
                    {
                        return cacheEntry.Resource.DeepClone();
                    }
                    else
                    {
                        template = response.Resource;
                    }

                    cacheEntry = response;
                }
                else
                {
                    var response = await _fortuneContainer.ReadItemAsync<FortuneTemplate>(templateId.ToString(), new PartitionKey(templateId.ToString()));
                    cacheEntry = response;
                    template = response.Resource;
                }

                _ = _memoryCache.Set(cacheKey, cacheEntry);

                return template.DeepClone();
            }
            catch (CosmosException e)
            {
                if (e.StatusCode == HttpStatusCode.NotFound)
                {
                    return null;
                }
                else if (e.StatusCode == HttpStatusCode.NotModified)
                {
                    return cacheEntry.Resource.DeepClone();
                }
                else
                {
                    throw e;
                }
            }
        }
    }
}