using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
//Configure Dependency Injection
builder.Services.AddSingleton<Performance_Board_System.DBContext.DapperContext>();
builder.Services.AddScoped<Performance_Board_System.Repository.Interfaces.IUserRepository, Performance_Board_System.Repository.Implementations.UserRepository>();
builder.Services.AddScoped<Performance_Board_System.Repository.Interfaces.IDepartmentRepository, Performance_Board_System.Repository.Implementations.DepartmentRepository>();
builder.Services.AddScoped<Performance_Board_System.Repository.Interfaces.IDesignationRepository, Performance_Board_System.Repository.Implementations.DesignationRepository>();
builder.Services.AddScoped<Performance_Board_System.Repository.Interfaces.IRoleRepository, Performance_Board_System.Repository.Implementations.RoleRepository>();
builder.Services.AddScoped<Performance_Board_System.Repository.Interfaces.IRatingRepository, Performance_Board_System.Repository.Implementations.RatingRepository>();
builder.Services.AddScoped<Performance_Board_System.Repository.Interfaces.IAttendanceStatusRepository, Performance_Board_System.Repository.Implementations.AttendanceStatusRepository>();

//Added for session
builder.Services.AddSession();
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/login";
        options.Events = new CookieAuthenticationEvents
        {
            OnRedirectToLogin = context =>
            {
                // Force redirect to /login without ReturnUrl
                context.Response.Redirect("/login");
                return Task.CompletedTask;
            }
        };
    });


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseSession();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();


app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/", context =>
{
    context.Response.Redirect("/signup");
    return Task.CompletedTask;
});

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=SignUp}/{id?}");

app.Run();
