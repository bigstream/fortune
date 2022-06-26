using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fortune.Config
{
    internal class Settings : ISettings
    {
        public string GetCosmosDBEndPoint()
        {
            return GetConfigValueByKey("CosmosDB:EndPoint");
        }

        public string GetCosmosDBKey()
        {
            return GetConfigValueByKey("CosmosDB:Key");
        }

        public string GetCosmosDBName()
        {
            return GetConfigValueByKey("CosmosDB:DBName");
        }

        public bool IsDevelopment()
        {
            return "Development".Equals(Environment.GetEnvironmentVariable("AZURE_FUNCTIONS_ENVIRONMENT"), StringComparison.OrdinalIgnoreCase);
        }

        public string GetHostName()
        {
            return GetConfigValueByKey("WEBSITE_HOSTNAME");
        }

        private static string GetConfigValueByKey(string key)
        {
            string value = Environment.GetEnvironmentVariable(key);

            if (string.IsNullOrEmpty(value))
            {
                throw new Exception($"Configuration ´{key}´ is missing.");
            }

            return value;
        }

    }
}
