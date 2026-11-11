using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using MyPortfolio.Data;
using MyPortfolio.Models.Entities;
using MyPortfolio.Services;
using System.Net;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
DatabaseConfiguration.AddDatabase(builder.Services, builder.Configuration);
builder.Services.AddScoped<HomeService>();
builder.Services.AddScoped<AccountService>();
builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
builder.Services.AddScoped<AccountCookieEvents>();
builder.Services.AddScoped<SiteContentService>();
builder.Services.AddScoped<ProjectService>();
builder.Services.AddScoped<ServiceOfferingService>();
builder.Services.AddScoped<TeamMemberService>();
builder.Services.AddScoped<SocialLinkService>();
builder.Services.AddScoped<ProfileService>();
builder.Services.AddScoped<ImageUploadService>();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/admin/login";
        options.AccessDeniedPath = "/Account/AccessDenied";
        options.Cookie.Name = "DecodeStudio.PublicAdmin.Auth";
        options.Cookie.HttpOnly = true;
        options.Cookie.SameSite = SameSiteMode.Lax;
        options.Cookie.SecurePolicy = builder.Environment.IsDevelopment()
            ? CookieSecurePolicy.SameAsRequest
            : CookieSecurePolicy.Always;
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
        options.EventsType = typeof(AccountCookieEvents);
    });

builder.Services.AddAuthorization();

var knownProxy = builder.Configuration["ReverseProxy:KnownProxy"];
if (!string.IsNullOrWhiteSpace(knownProxy))
{
    var proxyAddress = IPAddress.Parse(knownProxy);
    builder.Services.Configure<ForwardedHeadersOptions>(options =>
    {
        options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
        options.KnownProxies.Add(proxyAddress);
    });
}

var app = builder.Build();

if (!string.IsNullOrWhiteSpace(knownProxy))
{
    app.UseForwardedHeaders();
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
// Uploaded profile photos are created after the build-time asset manifest.
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets().AllowAnonymous();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
