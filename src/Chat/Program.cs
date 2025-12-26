using System.Text;

using ChatApp.Data;
using ChatApp.Hubs;
using ChatApp.Services;
using ChatApp.Services.Internal;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Db
builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseSqlite(builder.Configuration.GetConnectionString("Default")));

// Auth (JWT)
builder.Services.AddScoped<TokenService>();
builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new()
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)
            )
        };
        // Habilita token no SignalR via QueryString
        options.Events = new Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var accessToken = context.Request.Query["access_token"];
                var path = context.HttpContext.Request.Path;
                if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hubs/chat"))
                    context.Token = accessToken;
                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddAuthorization();
builder.Services.AddSignalR();
builder.Services.AddControllers();
builder.Services.AddCors(opt =>
{
    opt.AddPolicy("AllowFrontend", p =>
        p.AllowAnyHeader().AllowAnyMethod()
         .AllowCredentials()
         .WithOrigins("http://localhost:4200"));
});


var app = builder.Build();

#region Executa as migrações e força o cache das persistencias

using (var scope = app.Services.CreateScope())
{
    var migrationDtc = new MigrationAcao();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<MigrationAcao>>();

    var options = scope.ServiceProvider.GetRequiredService<DbContextOptions<AppDbContext>>();
    using (var dbContext = new AppDbContext(options))
    {
        var isLiberado = !app.Environment.IsDevelopment();
        if (!isLiberado && app.Environment.IsDevelopment())
        {
            isLiberado = app.Environment.IsDevelopment();
        }
        if (isLiberado)
        {
            _ = migrationDtc.ExecutarMigracaoDataCempro(dbContext, logger);
        }
    }
}

#endregion

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    //app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseCors("AllowFrontend");
app.UseAuthentication();
app.UseAuthorization();

// servir uploads
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(Path.Combine(app.Environment.ContentRootPath, "uploads")),
    RequestPath = "/uploads"
});


app.MapControllers();
app.MapHub<ChatHub>("/hubs/chat");

app.Run();
