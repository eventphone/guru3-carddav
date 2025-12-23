using eventphone.guru3.carddav.DAL;
using eventphone.guru3.carddav.DAV;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NWebDav.Server;
using NWebDav.Server.Stores;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<Guru3Context>(x => x.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddNWebDav(opts =>
{
    opts.Handlers["OPTIONS"] = typeof(CarddavOptionsHandler);
    opts.Handlers["REPORT"] = typeof(CarddavReportHandler);
    opts.Handlers["PROPFIND"] = typeof(WellKnownHandler);
})
.AddScoped<IStore, Guru3Store>()
.AddScoped<CarddavOptionsHandler>()
.AddScoped<CarddavReportHandler>()
.AddScoped<WellKnownHandler>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.All
});
app.UseNWebDav();
app.Run();