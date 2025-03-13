using AntDesign.ProLayout;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Options;
using MyAntDesignApp.Client.Pages;
using MyAntDesignApp.Components;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();

builder.Services.AddHttpContextAccessor();

builder.Services.AddAntDesign();

builder.Services.AddScoped(sp =>
{
    var httpContext = sp.GetRequiredService<IHttpContextAccessor>().HttpContext;
    if (httpContext != null)
    {
        return new HttpClient
        {
            BaseAddress = new Uri(httpContext.Request.Scheme + "://" + httpContext.Request.Host)
        };
    }
    return new HttpClient();
});

MyAntDesignApp.Client.Program.AddClientServices(builder.Services);

builder.Services.Configure<ProSettings>(builder.Configuration.GetSection("ProSettings"));

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(opt =>
    {
        opt.Cookie.HttpOnly = true; // 防止 JavaScript 访问
        opt.Cookie.SameSite = SameSiteMode.Lax; // 设置 SameSite 策略
        opt.LoginPath = "/login"; // 未认证用户的重定向路径
        opt.AccessDeniedPath = "/admin/login"; // 无权限的重定向路径
    })
    .AddCookie("ck",
    opt =>
    {
        opt.Cookie.HttpOnly = true; // 防止 JavaScript 访问
        opt.Cookie.SameSite = SameSiteMode.Lax; // 设置 SameSite 策略
        opt.LoginPath = "/login2"; // 未认证用户的重定向路径
        opt.AccessDeniedPath = "/admin/login"; // 无权限的重定向路径
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseAuthentication();
app.UseAuthorization();

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();
app.MapStaticAssets();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    //这是 服务器端配置，它告诉服务器在 预渲染（Prerendering） 和 静态渲染（Static Rendering） 时，也要解析 client 项目中的组件。
    .AddAdditionalAssemblies(typeof(MyAntDesignApp.Client._Imports).Assembly);

app.Run();
