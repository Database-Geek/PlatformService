using Microsoft.EntityFrameworkCore;
using PlatformService.Data;
using PlatformService.SyncDataServices.Http;
using Serilog;
using ILogger = Serilog.ILogger;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
Serilog.ILogger logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();
builder.Logging.AddSerilog(logger);
builder.Services.AddSingleton(logger);


builder.Services.AddDbContext<AppDbContext>(opt => opt.UseInMemoryDatabase("InMem"));
builder.Services.AddScoped<IPlatformRepo, PlatformRepo>();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddHttpClient<ICommandDataClient, HttpCommandDataClient>();

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
  app.UseSwagger();
  app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

PrepDb.PrepPopulation(app);

app.Run();
