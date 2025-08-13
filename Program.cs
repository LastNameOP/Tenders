var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddScoped<TenderService>();
builder.Services.AddHttpClient<TenderClientService>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/TenderHome/Error");
    app.UseHsts();
}

app.UseMiddleware<BasicAuthMiddleware>();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=TenderHome}/{action=Index}/{id?}");

app.Run();
