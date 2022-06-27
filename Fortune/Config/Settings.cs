using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fortune.Config
{
    public class Settings : ISettings
    {
        public string GetCosmosDBEndPoint()
        {
            return GetConfigValueByKey("CosmosDBEndPoint");
        }

        public string GetCosmosDBKey()
        {
            return GetConfigValueByKey("CosmosDBKey");
        }

        public string GetCosmosDBName()
        {
            return GetConfigValueByKey("CosmosDBName");
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
