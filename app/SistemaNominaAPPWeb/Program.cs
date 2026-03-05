using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SistemaNominaAPPWeb.Data;
using SistemaNominaAPPWeb.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Mvc.Authorization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews(options =>
{
    options.Filters.Add(new AuthorizeFilter());
});

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<SistemaNominaAPPWeb.Services.IPayrollService, SistemaNominaAPPWeb.Services.PayrollService>();

builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

// Optional JWT Config for API side
var jwtKey = builder.Configuration["Jwt:Key"] ?? "superSecretKey_whichNeedToBeReplacedLater_32CharsMin";
var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "SistemaNominaAPPWeb";

builder.Services.AddAuthentication()
    .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtIssuer,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });

// Add Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Sistema Nomina API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Ingresa tu token JWT Bearer"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            new string[] { }
        }
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Sistema Nomina API v1");
});

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

    string[] roles = { "Administrador", "RRHH" };

    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
            await roleManager.CreateAsync(new IdentityRole(role));
    }

    string email = "admin@sistema.com";
    string password = "Admin123!";

    if (await userManager.FindByEmailAsync(email) == null)
    {
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        
        var adminEmp = new Employee 
        { 
            CI = "00000000000",
            BirthDate = "1990-01-01",
            FirstName = "Admin", 
            LastName = "System", 
            Gender = "M",
            HireDate = "2024-01-01",
            Correo = email,
            IsActive = true 
        };
        db.Employees.Add(adminEmp);
        await db.SaveChangesAsync();

        var user = new ApplicationUser
        {
            UserName = email,
            Email = email,
            EmpNo = adminEmp.EmpNo
        };

        var resultAdmin = await userManager.CreateAsync(user, password);

        if (resultAdmin.Succeeded)
        {
            await userManager.AddToRoleAsync(user, "Administrador");
        }
        else
        {
            foreach (var error in resultAdmin.Errors)
            {
                Console.WriteLine($"Error creando Admin: {error.Description}");
            }
        }
    }

    string emailRrhh = "rrhh@sistema.com";
    string passwordRrhh = "RRHH123!";

    if (await userManager.FindByEmailAsync(emailRrhh) == null)
    {
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var rrhhEmp = new Employee 
        { 
            CI = "11111111111",
            BirthDate = "1990-01-01",
            FirstName = "Recursos", 
            LastName = "Humanos", 
            Gender = "F",
            HireDate = "2024-01-01",
            Correo = emailRrhh,
            IsActive = true 
        };
        db.Employees.Add(rrhhEmp);
        await db.SaveChangesAsync();

        var userRrhh = new ApplicationUser
        {
            UserName = emailRrhh,
            Email = emailRrhh,
            EmpNo = rrhhEmp.EmpNo
        };

        var resultRrhh = await userManager.CreateAsync(userRrhh, passwordRrhh);

        if (!resultRrhh.Succeeded)
        {
            foreach (var error in resultRrhh.Errors)
            {
                Console.WriteLine("ERROR CREANDO RRHH: " + error.Description);
            }
        }
        else
        {
            var roleResult = await userManager.AddToRoleAsync(userRrhh, "RRHH");

            if (!roleResult.Succeeded)
            {
                foreach (var error in roleResult.Errors)
                {
                    Console.WriteLine("ERROR ASIGNANDO ROL RRHH: " + error.Description);
                }
            }
        }
    }
}

app.Run();

