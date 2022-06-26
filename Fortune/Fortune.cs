using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Fortune.Data;
using Fortune.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;

namespace Fortune
{
    public class Fortune
    {
        private readonly ILogger<Fortune> Log;
        private IDBClient DBClient { get; set; }
        private IMemoryCache MemoryCache { get; set; }

        public Fortune(ILogger<Fortune> log, IDBClient dBClient, IMemoryCache memoryCache)
        {
            Log = log;
            DBClient = dBClient;
            MemoryCache = memoryCache;
        }

        [FunctionName("ping")]
        [OpenApiOperation(operationId: "Run", tags: new[] { "ping" })]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(string), Description = "The OK response")]
        public IActionResult Ping([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req)
        {
            Log.LogInformation("Ping!");

            string responseMessage = "Hello World!";

            return new OkObjectResult(responseMessage);
        }


        [FunctionName(nameof(ReadTemplate))]
        [OpenApiOperation(operationId: "read", tags: new[] { "Template" })]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiParameter(name: "id", In = ParameterLocation.Query, Required = true, Type = typeof(Guid), Description = "The **Id** parameter: f.E.: b1251ce6-d4b5-40ce-b84e-9e84edf13962")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(FortuneTemplate), Description = "The template")]
        public async Task<IActionResult> ReadTemplate(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "template")] HttpRequest req)
        {
            Log.LogInformation("Get Template.");

            try
            {
                string id = req.Query["id"];
                Log.LogInformation($"TemplateID: {id}");

                // Get Template
                var template = await DBClient.GetTemplateById(new Guid(id));

                if (template == null)
                {
                    return new NotFoundResult();
                }

                string responseMessage = JsonConvert.SerializeObject(template);

                return new OkObjectResult(template);
            }
            catch (Exception e)
            {
                var errorMessage = $"Failed to {nameof(ReadTemplate)}.";
                Log.LogError(errorMessage, e);
                return new BadRequestObjectResult(errorMessage);
            }
        }

        [FunctionName(nameof(CreateTemplate))]
        [OpenApiOperation(operationId: "create", tags: new[] { "Template" })]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiRequestBody(contentType: "application/json", bodyType: typeof(FortuneTemplate), Description = "The Template - IDs will be created automatically.", Required = true)]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.Created, contentType: "application/json", bodyType: typeof(FortuneTemplate), Description = "The Created response")]
        public async Task<IActionResult> CreateTemplate(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "template")] HttpRequest req)
        {
            Log.LogInformation("Create Template.");

            try
            {
                var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                var template = JsonConvert.DeserializeObject<FortuneTemplate>(requestBody);

                // Set IDs
                template.Id = Guid.NewGuid();
                template.Items.ForEach(x => x.Id = Guid.NewGuid());

                // Save Template
                try
                {
                    ItemResponse<FortuneTemplate> item = await DBClient.FortuneContainer.CreateItemAsync(template, new PartitionKey(template.Id.ToString()));
                    var response = item.Resource;
                    return new CreatedResult($"{nameof(response)}/{response.Id}", response);
                }
                catch (CosmosException e)
                {
                    if (e.StatusCode == HttpStatusCode.Conflict)
                    {
                        var errorMessage = $"Failed to create {nameof(FortuneTemplate)}. It already exists.";
                        Log.LogError(e, errorMessage);
                        return new ConflictObjectResult(errorMessage);
                    }
                    else
                    {
                        var errorMessage = $"Failed to create {nameof(FortuneTemplate)}.";
                        Log.LogError(e, errorMessage);
                        return new BadRequestResult();
                    }
                }
            }
            catch (Exception e)
            {
                var errorMessage = $"Failed to {nameof(ReadTemplate)}.";
                Log.LogError(errorMessage, e);
                return new BadRequestObjectResult(errorMessage);
            }
        }
    }
}

