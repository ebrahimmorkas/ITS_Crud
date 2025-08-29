using ITSAssignment.Web;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("Mumineen")));
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();
app.UseSession();

// ?? Middleware to enforce login
app.Use(async (context, next) =>
{
    var path = context.Request.Path.Value?.ToLower();

    // Allow login/logout/static files
    if (path!.StartsWith("/auth/login") || path.StartsWith("/auth/logout") || path.StartsWith("/css") || path.StartsWith("/js") || path.StartsWith("/lib"))
    {
        await next();
        return;
    }

    var its = context.Session.GetString("Its");
    var role = context.Session.GetString("Role");

    if (string.IsNullOrEmpty(its))
    {
        // Not logged in ? force to login
        context.Response.Redirect("/Auth/Login");
        return;
    }

    // Agar already logged in aur root ya /home jaa raha hai ? redirect based on role
    if (path == "/" || path.StartsWith("/home"))
    {
        if (role == "admin")
        {
            context.Response.Redirect("/Admin/AddMumineen");
            return;
        }
        else if (role == "user")
        {
            context.Response.Redirect("/Mumineen/Add");
            return;
        }
    }

    await next();
});

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
