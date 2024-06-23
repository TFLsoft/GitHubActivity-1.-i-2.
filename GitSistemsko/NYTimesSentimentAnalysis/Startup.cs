using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddControllers();  // Dodavanje kontrolera u servisnu kolekciju
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILogger<Startup> logger)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.Use(async (context, next) =>
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            logger.LogInformation($"Handling request: {context.Request.Method} {context.Request.Path}");

            await next.Invoke();

            stopwatch.Stop();
            logger.LogInformation($"Finished handling request in {stopwatch.ElapsedMilliseconds}ms");
        });

        app.UseRouting();

        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();  // Mapiranje kontrolera
        });
    }
}
