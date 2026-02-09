using E_Insurance_App.Helpers;
using E_Insurance_App.Repositories;
using E_Insurance_App.Services.Implementations;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSingleton<DbHelper>();
builder.Services.AddScoped<UserRepository>();
builder.Services.AddScoped<BankRepository>();
builder.Services.AddScoped<PolicyRepository>();
builder.Services.AddScoped<PaymentRepository>();
builder.Services.AddScoped<CommissionRepository>();
builder.Services.AddScoped<PremiumService>();
builder.Services.AddScoped<PaymentService>();
builder.Services.AddScoped<CommissionService>();

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
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

app.Run();
