using Microsoft.EntityFrameworkCore;
using PlatformService.Models;
using Serilog;
using ILogger = Serilog.ILogger;

namespace PlatformService.Data
{
  public static class PrepDb
  {
    public static void PrepPopulation(ILogger logger, IApplicationBuilder app, bool IsProduction)
    {
      using (var serviceScope = app.ApplicationServices.CreateScope())
      {
        SeedData(logger, serviceScope.ServiceProvider.GetService<AppDbContext>(), IsProduction);
      }
    }

    private static void SeedData(ILogger logger, AppDbContext context, bool IsProduction)
    {
      if(IsProduction)
      {
        logger.Information("--> Attempting to apply migrations...");
        try
        {
          context.Database.Migrate();
        }
        catch (Exception ex)
        {
          logger.Warning("--> Could not run migrations: {errorMessage}", ex.Message);
        }
      }

      if(!context.Platforms.Any())
      {
        logger.Information("--> Seeding Data...");

        context.Platforms.AddRange(
          new Platform() {Name="Dot Net", Publisher="Microsoft", Cost="Free"},
          new Platform() {Name="SQL Server Express", Publisher="Microsoft", Cost="Free"},
          new Platform() {Name="Kubernetes", Publisher="Cloud Native Computing Foundation", Cost="Free"}
        );

        context.SaveChanges();
      }
      else
      {
        Console.WriteLine("--> We already have data.");
      }
    }
  }
}