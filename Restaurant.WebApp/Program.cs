using Microsoft.AspNetCore.Authentication.Cookies;
using Restaurant.Plugins.DataStore.SQL;
using Restaurant.UseCases;
using Restaurant.UseCases.PluginInterfaces;
using Restaurant.WebApp.Components;
using Restaurant.WebApp.State;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// 1. Cookie Authentication (y chang eShop mẫu)
builder.Services.AddAuthentication("Restaurant.CookieAuth")
    .AddCookie("Restaurant.CookieAuth", options =>
    {
        options.Cookie.Name = "Restaurant.CookieAuth";
        options.LoginPath = "/login";
        options.LogoutPath = "/logout";
        options.AccessDeniedPath = "/access-denied";
    });
builder.Services.AddAuthorization();
builder.Services.AddCascadingAuthenticationState();

// 2. Connection String
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? "Data Source=LAPTOP-7JU85UG3\\SQLEXPRESS;Initial Catalog=RestaurantDB;Integrated Security=True;TrustServerCertificate=True";

// 3. DI Repositories (Dapper)
builder.Services.AddTransient<IMenuItemRepository>(sp => new MenuItemRepository(connectionString));
builder.Services.AddTransient<IDiningTableRepository>(sp => new DiningTableRepository(connectionString));
builder.Services.AddTransient<IReservationRepository>(sp => new ReservationRepository(connectionString));
builder.Services.AddTransient<ITableOrderRepository>(sp => new TableOrderRepository(connectionString));

// 4. DI Use Cases
builder.Services.AddTransient<ViewMenuItemsUseCase>();
builder.Services.AddTransient<CreateReservationUseCase>();
builder.Services.AddTransient<PlaceTableOrderUseCase>();
builder.Services.AddTransient<CloseTableOrderUseCase>();
builder.Services.AddTransient<ExportBillUseCase>();

// 5. Scoped State (ShoppingCart equivalent)
builder.Services.AddScoped<ScopedOrderState>();

var app = builder.Build();

// Add the table service timer column to existing databases without requiring a rebuild.
await new DiningTableRepository(connectionString).EnsureTableTimingSchemaAsync();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

// Login endpoint (y chang eShop mẫu)
app.MapPost("/login", async (HttpContext context,
    [Microsoft.AspNetCore.Mvc.FromForm] string username,
    [Microsoft.AspNetCore.Mvc.FromForm] string password) =>
{
    if (username == "admin" && password == "admin")
    {
        var claims = new System.Collections.Generic.List<System.Security.Claims.Claim>
        {
            new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Name, username),
            new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Role, "Admin")
        };
        var identity = new System.Security.Claims.ClaimsIdentity(claims, "Restaurant.CookieAuth");
        var principal = new System.Security.Claims.ClaimsPrincipal(identity);
        await Microsoft.AspNetCore.Authentication.AuthenticationHttpContextExtensions.SignInAsync(context, "Restaurant.CookieAuth", principal);
        return Microsoft.AspNetCore.Http.Results.Redirect("/admin/tables");
    }
    return Microsoft.AspNetCore.Http.Results.Redirect("/login?error=InvalidCredentials");
});

// Logout endpoint
app.MapGet("/logout", async (HttpContext context) =>
{
    await Microsoft.AspNetCore.Authentication.AuthenticationHttpContextExtensions.SignOutAsync(context, "Restaurant.CookieAuth");
    return Microsoft.AspNetCore.Http.Results.Redirect("/");
});

app.Run();
