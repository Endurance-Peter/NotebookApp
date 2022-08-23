using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Notebook.Api.Authorizations;
using Notebook.Infrastructure.Configurations;
using Notebook.Infrastructure.UnitOfWorks;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
// Add services to the container.
var configuration = builder.Configuration;
var services= builder.Services;
services.AddDbContext<ApplicationContext>(opt => opt.UseSqlite(configuration.GetConnectionString("DefaultDb")));
services.Configure<JwtSecrets>(configuration.GetSection("JwtSecrets"));
services.AddScoped<IUnitOfWork, UnitOfWork>();

var key = Encoding.ASCII.GetBytes(configuration["JwtSecrets:Secrets"]);
var tokenValidationParameters = new TokenValidationParameters
{
    IssuerSigningKey = new SymmetricSecurityKey(key),
    ValidateIssuer = false,
    ValidateAudience = false,
    ValidateIssuerSigningKey = true,
    ValidateLifetime = false,
    RequireExpirationTime = false
};

services.AddSingleton<TokenValidationParameters>();
// add authentication
services.AddAuthentication(option =>
{
    option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    option.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.SaveToken = true;
    options.TokenValidationParameters = tokenValidationParameters;
});

services.AddDefaultIdentity<IdentityUser>(opt=>opt.SignIn.RequireConfirmedAccount=true).AddEntityFrameworkStores<ApplicationContext>();
services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
services.AddEndpointsApiExplorer();
services.AddSwaggerGen();

var app = builder.Build(); // Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
