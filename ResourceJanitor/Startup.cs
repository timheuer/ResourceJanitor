using System.Net.Http;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Azure.Services.AppAuthentication;
using System.Net.Http.Headers;

[assembly: FunctionsStartup(typeof(ResourceJanitor.Startup))]

namespace ResourceJanitor
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddHttpClient();
        }
    }
}