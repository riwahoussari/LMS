using LMS.Application.Interfaces;
using LMS.Application.Mappings;
using LMS.Application.Services;
using LMS.Domain.Entities;
using LMS.Domain.Interfaces;
using LMS.Infrastructure;
using LMS.Infrastructure.Persistence;
using LMS.Infrastructure.Repositories;
using LMS.Infrastructure.Seeding;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

DotNetEnv.Env.Load();

var builder = WebApplication.CreateBuilder(args);

////// Add services to the container.

// Connect Database
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
);

// Identity setup (we’ll configure in next step)
builder.Services.AddIdentity<AppUser, IdentityRole>(options =>
    {
        options.User.RequireUniqueEmail = true;
    })
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

// JWT setup
var key = Encoding.ASCII.GetBytes(builder.Configuration["Jwt:Key"]!);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(key)
    };
});

// AutoMapper library setup
builder.Services.AddAutoMapper(typeof(MappingProfile), typeof(Program), typeof(AuthService));

// Seeding
builder.Services.AddScoped<RoleSeeder>();
builder.Services.AddScoped<AdminSeeder>();

// Services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserService, UserService>();

// Repositories
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
//builder.Services.AddScoped<IRepository<object>, Repository<object>>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IStudentProfileRepository, StudentProfileRepository>();
builder.Services.AddScoped<ITutorProfileRepository, TutorProfileRepository>();
builder.Services.AddScoped<IEnrollmentRepository, EnrollmentRepository>();


// Controllers
builder.Services.AddControllers();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "LMS API", Version = "v1" });

    // JWT Bearer config for Swagger
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: 'Bearer {token}'",
        Name = "Authorization",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey
    });
    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[]{}
        }
    });
});


////// build app

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "LMS API v1"));
}

// seeding
using (var scope = app.Services.CreateScope())
{
    var roleSeeder = scope.ServiceProvider.GetRequiredService<RoleSeeder>();
    await roleSeeder.SeedRolesAsync();

    var adminSeeder = scope.ServiceProvider.GetRequiredService<AdminSeeder>();
    await adminSeeder.SeedAdminAsync();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
