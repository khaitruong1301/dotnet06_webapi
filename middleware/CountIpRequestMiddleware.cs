using System;
using System.Threading.Tasks;
using backend_netcore_dotnet06.Models.DBUser;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace backend_netcore_dotnet06.Middleware
{



    // REMEMBER: Add `services.AddTransient<CountIpRequestMiddleware>();` to Startup.ConfigureServices() method
    public class CountIpRequestMiddleware : IMiddleware
    {
        private readonly UserDBContext _userDBContext;

        // IMiddleware is activated per request, 
        // so scoped services can be injected into the middleware's constructor.
        public CountIpRequestMiddleware(UserDBContext userDBContext)
        {

            _userDBContext = userDBContext;
        }


        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {

            Console.WriteLine($@"Đã đi qua middleware ipcount");

            var ipAddress = context.Connection.RemoteIpAddress?.ToString();


            if (!string.IsNullOrEmpty(ipAddress))
            {
                IpCount? ipRequest = await _userDBContext.IpCounts.SingleOrDefaultAsync(item => item.Ip == ipAddress && item.DateRequest == DateTime.Today);
                if (ipRequest == null)
                {
                    ipRequest = new IpCount
                    {
                        Ip = ipAddress,
                        Count = 1,
                        DateRequest = DateTime.Today
                    };
                    _userDBContext.IpCounts.Add(ipRequest);
                }
                else
                {
                    ipRequest.Count++;
                }
                await _userDBContext.SaveChangesAsync();

            }

            await next(context);

           

        }
    }
}
