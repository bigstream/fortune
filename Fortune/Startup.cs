using Fortune.Config;
using Fortune.Data;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[assembly: FunctionsStartup(typeof(Fortune.Startup))]

namespace Fortune
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            // _ = builder.Services.AddSingleton<IAccessTokenProvider, AccessTokenProvider>(s => new AccessTokenProvider());
            _ = builder.Services.AddLogging(loggingBuilder =>
            {
                _ = loggingBuilder.AddFilter(level => true);
            });

            _ = builder.Services.AddMemoryCache();
            _ = builder.Services.AddSingleton<ISettings, Settings>();
            _ = builder.Services.AddSingleton<IDBClient, DBClient>();

            var settings = builder.Services.BuildServiceProvider().GetRequiredService<ISettings>();

            var test = settings.GetCosmosDBName();
        }
    }
}
