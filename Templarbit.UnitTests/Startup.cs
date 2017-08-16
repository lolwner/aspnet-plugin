using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Templarbit.Core;

namespace Templarbit.UnitTests
{
    public class Startup
    {
        public string TemplarbitApiToken { get; set; }
        public string TemplarbitPropertyId { get; set; }
        public string TemplarbitApiUrl { get; set; } = "";
        public ITemplarbitLogger Logger { get; set; }
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole();

            app.UseTemplarbit(new TemplarbitMiddlewareModel()
            {
                TemplarbitApiToken = TemplarbitApiToken,
                TemplarbitPropertyId = TemplarbitPropertyId,
                TemplarbitApiUrl = TemplarbitApiUrl
            }, Logger);
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.Run(async (context) =>
            {
                await context.Response.WriteAsync("Hello World!");
            });
        }
    }
}
