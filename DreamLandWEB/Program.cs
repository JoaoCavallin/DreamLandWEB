using DreamLandWEB.Data;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using Microsoft.AspNetCore.Localization;
using System.Text.Json.Serialization;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

var cultura = new CultureInfo("en-US");
var supportedCultures = new[] { cultura };

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Casa")));

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromDays(7);
        options.SlidingExpiration = true;
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
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
        };
    });
builder.Services.AddAuthorization(options =>
{
options.AddPolicy("RequerAdmin", policy =>
    policy.RequireClaim("Admin", "True"));
});

builder.Services.AddControllersWithViews(options =>
{
    options.ModelBindingMessageProvider.SetValueIsInvalidAccessor(
        value => $"O valor '{value}' é inválido");

    options.ModelBindingMessageProvider.SetValueMustNotBeNullAccessor(
        value => "Campo obrigatório");

    options.ModelBindingMessageProvider.SetAttemptedValueIsInvalidAccessor(
        (value, fieldName) => $"O valor '{value}' é inválido para o campo '{fieldName}'");

    options.ModelBindingMessageProvider.SetMissingKeyOrValueAccessor(
        () => "Campo obrigatório");

    options.ModelBindingMessageProvider.SetUnknownValueIsInvalidAccessor(
        fieldName => $"O valor informado é inválido para o campo '{fieldName}'");

    options.ModelBindingMessageProvider.SetValueMustBeANumberAccessor(
       fieldName => $"O campo {fieldName} deve ser um número válido.");

})
.AddJsonOptions(o => o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

builder.Services.AddCors(options =>
    options.AddPolicy("Dev", policy =>
        policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));

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

app.UseRequestLocalization(new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture(cultura),
    SupportedCultures = supportedCultures,
    SupportedUICultures = supportedCultures
});

app.UseRouting();

app.UseCors("Dev");

app.UseSession();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
