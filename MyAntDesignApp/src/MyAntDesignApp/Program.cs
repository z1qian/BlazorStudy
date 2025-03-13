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
        opt.Cookie.HttpOnly = true; // ��ֹ JavaScript ����
        opt.Cookie.SameSite = SameSiteMode.Lax; // ���� SameSite ����
        opt.LoginPath = "/login"; // δ��֤�û����ض���·��
        opt.AccessDeniedPath = "/admin/login"; // ��Ȩ�޵��ض���·��
    })
    .AddCookie("ck",
    opt =>
    {
        opt.Cookie.HttpOnly = true; // ��ֹ JavaScript ����
        opt.Cookie.SameSite = SameSiteMode.Lax; // ���� SameSite ����
        opt.LoginPath = "/login2"; // δ��֤�û����ض���·��
        opt.AccessDeniedPath = "/admin/login"; // ��Ȩ�޵��ض���·��
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
    //���� �����������ã������߷������� Ԥ��Ⱦ��Prerendering�� �� ��̬��Ⱦ��Static Rendering�� ʱ��ҲҪ���� client ��Ŀ�е������
    .AddAdditionalAssemblies(typeof(MyAntDesignApp.Client._Imports).Assembly);

app.Run();
