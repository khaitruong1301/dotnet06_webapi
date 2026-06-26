using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace backend_netcore_dotnet06.Middleware
{
 

    // REMEMBER: Add `services.AddTransient<NameMiddleware>();` to Startup.ConfigureServices() method
    public class SubDomainMiddleware : IMiddleware
    {
        // IMiddleware is activated per request, 
        // so scoped services can be injected into the middleware's constructor.
        public SubDomainMiddleware()
        {
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {

            var hostNameClient = context.Request.Host.Host; // Lấy tên miền từ request

            Console.WriteLine($@"Tên miền client: {hostNameClient}");
          

            await next(context);
        }
    }
}
