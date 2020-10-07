// Default URL for triggering event grid function in the local environment.
// http://localhost:7071/runtime/webhooks/EventGrid?functionName={functionname}
using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Azure.EventGrid.Models;
using Microsoft.Azure.WebJobs.Extensions.EventGrid;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace ResourceJanitor
{
    public static class EventGridTrigger
    {
        [FunctionName("gridTrigger")]
        public static async Task Run([EventGridTrigger]EventGridEvent eventGridEvent, [DurableClient] IDurableClient client, ILogger log, ExecutionContext context)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(context.FunctionAppDirectory)
                .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

            log.LogInformation($"Received notification of create for resource group {eventGridEvent.Subject}");

            string resourceGroupName = eventGridEvent.Subject.ParseResourceGroupName();

            // check to make sure it isn't an RG with special consideration
            var reservedList = config["RESERVED_PREFIXES"].Split(';');
            foreach (var term in reservedList)
            {
                if (resourceGroupName.ToLowerInvariant().StartsWith(term.ToString()))
                {
                    log.LogInformation($"Ignoring resource group {resourceGroupName} for reserved characters");
                    return;
                }
            }

            await client.SignalEntityAsync(
                new EntityId(nameof(AzureResource), eventGridEvent.Id),
                nameof(AzureResource.CreateResource),
                eventGridEvent.Subject
            );
        }
    }
}
