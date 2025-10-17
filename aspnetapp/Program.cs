using aspnetapp;
using aspnetapp.Services;

var builder = WebApplication.CreateBuilder(args);

// ✅ 必须显式添加控制器支持
builder.Services.AddControllers();

// ✅ 你的 Razor 页面也要保留
builder.Services.AddRazorPages();

// ✅ 注册数据库上下文（例如使用 Pomelo）
builder.Services.AddDbContext<AppDbContext>();

// ✅ 在这里注册后台定时任务服务
builder.Services.AddHostedService<DailyJobService>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}

app.UseStaticFiles();
app.UseRouting();

app.UseAuthorization();

// ✅ 映射控制器（Web API）
app.MapControllers();

// ✅ 映射 Razor 页面
app.MapRazorPages();

app.Run();
