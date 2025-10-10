using aspnetapp;

var builder = WebApplication.CreateBuilder(args);

// 添加控制器（Web API）和 Razor 页面支持
builder.Services.AddControllers();
builder.Services.AddRazorPages();

// 注册数据库上下文
builder.Services.AddDbContext<AppDbContext>();

var app = builder.Build();

// 配置 HTTP 请求管道
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}

app.UseStaticFiles();
app.UseRouting();

app.UseAuthorization();

// 映射控制器和页面
app.MapControllers();
app.MapRazorPages();

app.Run();
