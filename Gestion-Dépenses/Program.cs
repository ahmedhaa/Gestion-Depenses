using Gestion_Dépenses.Data;
using Gestion_Dépenses.Models.UserModel;
using Gestion_Dépenses.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.EnableAnnotations();  // Active les annotations dans Swagger
});

//**configuration base de donnees*****
builder.Services.AddDbContext<DepenseDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Ajoutez Identity 
builder.Services.AddIdentity<User,IdentityRole>()
    .AddEntityFrameworkStores<DepenseDbContext>()
    .AddDefaultTokenProviders();

// Ajouter l'authentification JWT
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
        var key = Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]);
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])),
            RoleClaimType = "http://schemas.microsoft.com/ws/2008/06/identity/claims/role"

        };
    });

//configuration swagger
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Veuillez entrer le jeton JWT sous la forme 'Bearer votre_token",
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "API de Gestion des Dépenses",
        Version = "v1",
        Description = "Une API pour gérer les dépenses.",
    });
});

var app = builder.Build();


//service initializer pour initialiser les roles et les utilisateurs

using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();

    await Initializer.Initialize(scope.ServiceProvider, userManager, roleManager);
}

//gestion des erreurs
app.Use(async (context, next) =>
{
    await next();
    if (context.Response.HasStarted)
    {
        return;
    }

    if (context.Response.StatusCode == StatusCodes.Status401Unauthorized)
{
    context.Response.ContentType = "application/json";
    await context.Response.WriteAsync("{\"message\": \"Non autorisé : vous devez être authentifié pour accéder à cette ressource.\"}");
}
else if (context.Response.StatusCode == StatusCodes.Status403Forbidden)
{
    context.Response.ContentType = "application/json";
    await context.Response.WriteAsync("{\"message\": \"Accès interdit : vous n'avez pas les permissions nécessaires pour accéder à cette ressource.\"}");
}
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Gestion des Dépenses v1"));
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
