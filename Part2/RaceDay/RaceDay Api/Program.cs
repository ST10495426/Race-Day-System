using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;
using RaceDay_Api.Data;
using RaceDay_Api.Services;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// ============================================
// DATABASE CONFIGURATION
// ============================================
// Register the DbContext with the connection string from appsettings.json
builder.Services.AddDbContext<RaceDayDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// ============================================
// SESSION CONFIGURATION
// ============================================
// Session stores user ID and role on the server, keyed by a cookie
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);   // Session expires after 30 minutes of inactivity
    options.Cookie.HttpOnly = true;                   // JS cannot access cookie (security)
    options.Cookie.IsEssential = true;
    options.Cookie.SameSite = SameSiteMode.Lax;
});

// ============================================
// CORS (for Part 3 MVC to call this API)
// ============================================
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowMvc", policy =>
    {
        policy.WithOrigins("https://localhost:7000")  // Will update in Part 3
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();                     // Required for cookies
    });
});

// Register custom services for dependency injection
builder.Services.AddScoped<PasswordHasher>();

// Add services to the container and swagger.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    // Read XML comments from the generated file
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        options.IncludeXmlComments(xmlPath);
    }
});

var app = builder.Build();

// Configure the HTTP request pipeline.
// MIDDLEWARE PIPELINE
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowMvc");

app.UseSession();   // MUST come before UseAuthorization

app.UseAuthorization();

app.MapControllers();

app.Run();

