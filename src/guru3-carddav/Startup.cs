using System;
using eventphone.guru3.carddav.DAL;
using eventphone.guru3.carddav.DAV;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NWebDav.Server;
using NWebDav.Server.AspNetCore;
using NWebDav.Server.Stores;

namespace eventphone.guru3.carddav
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<Guru3Context>(x => x.UseMySql(Configuration.GetConnectionString("DefaultConnection"))
                .ConfigureWarnings(warnings => warnings.Throw(RelationalEventId.QueryClientEvaluationWarning)));

            services.AddSingleton<IRequestHandlerFactory, CarddavRequestHandlerFactory>();
            services.AddScoped<IStore, Guru3Store>();
            services.AddScoped<IWebDavDispatcher, WebDavDispatcher>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.Run(async (context) =>
            {
                // Create the proper HTTP context
                var httpContext = new AspNetCoreContext(context);

                var webDavDispatcher = context.RequestServices.GetRequiredService<IWebDavDispatcher>();

                // Dispatch request
                await webDavDispatcher.DispatchRequestAsync(httpContext, context.RequestAborted);
            });
        }
    }
}
