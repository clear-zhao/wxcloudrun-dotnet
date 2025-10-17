using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace aspnetapp.Services
{
    public class DailyJobService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;

        public DailyJobService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                // 计算下次零点时间
                var now = DateTime.Now;
                var nextMidnight = now.Date.AddDays(1);
                var delay = nextMidnight - now;

                await Task.Delay(delay, stoppingToken); // 等到零点

                try
                {
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                        // TODO: 这里写你要做的事，比如清理旧记录
                        await DoDailyWork(db);
                    }
                }
                catch (Exception ex)
                {
                    // 可写日志
                    Console.WriteLine($"定时任务执行出错: {ex.Message}");
                }
            }
        }

        private async Task DoDailyWork(AppDbContext db)
        {
            // 将所有员工的流水号重置为1
            var employees = await db.Employees.ToListAsync();
            foreach (var emp in employees)
            {
                emp.SerialNumber = 1;
            }
            await db.SaveChangesAsync();

            Console.WriteLine("已重置所有员工的流水号为1");
        }
    }
}
