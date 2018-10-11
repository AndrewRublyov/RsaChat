using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using RsaChat.Hubs;

namespace RsaChat
{
  public class Startup
  {
    public void ConfigureServices(IServiceCollection services)
    {
      services.AddSignalR(o => o.EnableDetailedErrors = true);
      services.AddCors();
    }
 
    public void Configure(IApplicationBuilder app)
    {
      app.UseDefaultFiles();
      app.UseCors(builder => builder
        .AllowAnyOrigin()
        .AllowAnyMethod()
        .AllowAnyHeader()
        .AllowCredentials());
      
      app.UseSignalR(routes =>
      {
        routes.MapHub<ChatHub>("/chat");
      });
    }
  }
}