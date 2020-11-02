using System.IO;
using API.Extensions;
using API.Helpers;
using API.Middlewares;
using AutoMapper;
using Infrastructure.Data;
using Infrastructure.Identity;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using StackExchange.Redis;

namespace API
{
  public class Startup
  {
    private readonly IConfiguration _config;
    public Startup(IConfiguration config)
    {
      this._config = config;
    }

    public void ConfigureServices(IServiceCollection services)
    {
      services.AddAutoMapper(typeof(MappingProfiles));

      services.AddControllers();

      services.AddDbContext<StoreContext>(opt =>
      {
        opt.UseSqlite(_config.GetConnectionString("DefaultConnection"));
      });

      services.AddDbContext<AppIdentityDbContext>(opt =>
      {
        opt.UseSqlite(_config.GetConnectionString("IdentityConnection"));
      });

      services.AddSingleton<IConnectionMultiplexer>(opt =>
      {
        var configuration = ConfigurationOptions.Parse(_config.GetConnectionString("Redis"), true);

        return ConnectionMultiplexer.Connect(configuration);
      });

      services.AddApplicationServices();

      services.AddIdentityServices(_config);

      services.AddSwaggerDocumentation();

      services.AddCors(opt =>
      {
        opt.AddPolicy("CorsPolicy", policy =>
        {
          policy
            .AllowAnyHeader()
            .AllowAnyMethod()
            .WithOrigins("https://localhost:4200");
        });
      });
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
      app.UseMiddleware<ExceptionMiddleware>();

      app.UseStatusCodePagesWithReExecute("/errors/{0}");

      app.UseHttpsRedirection();

      // serves the compiled Angular view layer
      app.UseStaticFiles();

      // serves the static images
      app.UseStaticFiles(new StaticFileOptions
      {
        FileProvider = new PhysicalFileProvider(
          Path.Combine(Directory.GetCurrentDirectory(), "Content")
        ),
        RequestPath = "/content"
      });

      app.UseRouting();

      app.UseCors("CorsPolicy");

      app.UseAuthentication();

      app.UseAuthorization();

      app.UseSwaggerDocumentation();

      app.UseEndpoints(endpoints =>
      {
        endpoints.MapControllers();

        // endpoint for the compiled Angular view layer
        endpoints.MapFallbackToController("Index", "Fallback");
      });
    }
  }
}
