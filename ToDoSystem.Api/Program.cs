using MedicalManagementSystem.Api.Middleware;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using System.Text.Json.Serialization;
using ToDoSystem.Application;
using ToDoSystem.Application.Helper;
using ToDoSystem.Domain.Entities.Identity;
using ToDoSystem.Infrastructure;
using ToDoSystem.Infrastructure.Database;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<JWT>(builder.Configuration.GetSection("JWT"));

// Add services to the container.

builder.Services.AddControllers();

#region Cors 
var Cors = "CorsPolicy";
builder.Services.AddCors(options =>
{
    options.AddPolicy(Cors,
        builder => builder.AllowAnyOrigin()
        .AllowAnyMethod()
        .AllowAnyHeader());
});
#endregion

#region Identity
builder.Services.AddIdentity<User, IdentityRole<int>>(o =>
{
    o.Password.RequiredLength = 8;
    o.Password.RequireDigit = false;
    o.Password.RequireLowercase = false;
    o.Password.RequireUppercase = false;
    o.Password.RequireNonAlphanumeric = false;

    o.SignIn.RequireConfirmedAccount = false;
    o.SignIn.RequireConfirmedPhoneNumber = false;
    o.SignIn.RequireConfirmedEmail = false;
}).AddEntityFrameworkStores<AppDbContext>();
#endregion

#region Swagger Authorize configuration
builder.Services.AddSwaggerGen(c =>
{
    //c.SwaggerDoc("v1", new OpenApiInfo { Title = "’MissionSystemApi", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type=ReferenceType.SecurityScheme,
                    Id="Bearer"
                }
            },
            Array.Empty<string>()
        }
    });

});
#endregion

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

builder.Services.AddInfrastructureDependencies().AddApplicationDependencies();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

#region ConnectionString
builder.Services.AddDbContext<AppDbContext>(opt => opt.UseSqlServer(builder.Configuration.GetConnectionString("Default")));
#endregion

#region JWT Bearer Authorization 
// add authentication services 
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(o =>
{
    o.RequireHttpsMetadata = false;
    o.SaveToken = false;
    o.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidIssuer = builder.Configuration["JWT:Issuer"],
        ValidAudience = builder.Configuration["JWT:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Key"]!)),
        ClockSkew = TimeSpan.Zero
    };
});
#endregion

#region Ignore Null Value
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    options.JsonSerializerOptions.WriteIndented = true;
});
#endregion


var app = builder.Build();

#region Seeding Admin & Roles
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<int>>>();
    var roles = new[] { "User" };
    //Create 
    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            IdentityRole<int> roleRole = new(role);
            await roleManager.CreateAsync(roleRole);
        }
    }
}
#endregion



// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseMiddleware<ErrorHandlerMiddleware>();

app.UseHttpsRedirection();

app.UseCors(Cors);

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
