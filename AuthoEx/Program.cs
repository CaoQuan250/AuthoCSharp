using AuthoEx.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddControllersWithViews();

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("View", policy =>
        policy.RequireAuthenticatedUser()
              .RequireAssertion(context =>
                  context.User.Identity?.Name == "admin@mvc.web" ||
                  context.User.Identity?.Name == "add@mvc.web" ||
                  context.User.Identity?.Name == "view@mvc.net"));

    options.AddPolicy("Add", policy =>
        policy.RequireAuthenticatedUser()
              .RequireAssertion(context =>
                  context.User.Identity?.Name == "admin@mvc.web" ||
                  context.User.Identity?.Name == "add@mvc.web"));

    options.AddPolicy("Edit&Delete", policy =>
        policy.RequireAuthenticatedUser()
              .RequireAssertion(context =>
                  context.User.Identity?.Name == "admin@mvc.web"));
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
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

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();
