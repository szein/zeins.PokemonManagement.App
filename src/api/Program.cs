using api.Services;
using core.Inerfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog();

builder.Host.UseSerilog((context, services, configuration) => configuration
    .ReadFrom.Configuration(context.Configuration)
    .ReadFrom.Services(services));

builder.Services.AddOpenApi();

var connectionString = builder.Configuration.GetConnectionString($"{builder.Configuration["DefaultConnectionName"]}")
                      ?? "Data Source=DB-not-from-appsettings.db";

builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlite(connectionString));
builder.Services.AddScoped(typeof(IRepository<>), typeof(SqlRepository<>));
builder.Services.AddScoped<ICollectionRepository, CollectionRepository>();

//Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
.AddJwtBearer(options =>
{
  options.Authority = "https://zeins.ciamlogin.com/b1a86dd4-0adc-4a99-a182-920e55a2a363/v2.0";
  options.Audience = "fbd6924a-387a-4673-aa63-693d198ee022";
});

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IUserContext, UserContext>();
builder.Services.AddScoped<ICollectionService, CollectionService>();
builder.Services.AddMemoryCache();

builder.Services.AddHttpClient<IPokemonService, PokemonService>(client =>
{
  client.BaseAddress = new Uri("https://pokeapi.co/api/v2/");
});

//Services and Controllers
builder.Services.AddControllers();

//TODO: try to manage CORS from Entra Id App regestration
builder.Services.AddCors(options =>
{
  options.AddPolicy("AllowedCORS",
      policy => policy.WithOrigins(builder.Configuration.GetSection("AllowedOrigin").Get<string[]>() ?? [])
                      .AllowAnyHeader()
                      .AllowAnyMethod());
});

var app = builder.Build();

//Seed
await using (var scope = app.Services.CreateAsyncScope())
{
  var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

  await dbContext.Database.OpenConnectionAsync();

  await dbContext.Database.MigrateAsync();

  if (!await dbContext.Collections.AnyAsync())
  {
    await SeedDatabaseAsync(dbContext);
    await dbContext.SaveChangesAsync();
  }
  await dbContext.Database.CloseConnectionAsync();
}

//TODO: move the seeding to better place
async Task SeedDatabaseAsync(AppDbContext dbContext)
{

}

app.UseSerilogRequestLogging();
// Configure the HTTP request pipeline.
if (!app.Environment.IsProduction())
{
  Console.WriteLine(app.Environment.EnvironmentName);
  app.MapOpenApi();
}
else
{
  app.UseHttpsRedirection();
}

app.UseCors("AllowedCORS");

app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<UserContextMiddleware>();

app.MapControllers();

app.Run();
