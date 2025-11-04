#nullable disable
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using aspnetapp;

namespace aspnetapp.Controllers
{
    [Route("api/employees")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly AppDbContext _context;

        public EmployeeController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/employees
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Employee>>> GetEmployees()
        {
            return await _context.Employees.ToListAsync();
        }

        // GET: api/employees/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Employee>> GetEmployee(int id)
        {
            var employee = await _context.Employees.FindAsync(id);

            if (employee == null)
            {
                return NotFound();
            }

            return employee;
        }

        // GET: api/employees/{name}/name
        [HttpGet("serialNumber/{name}")]
        public async Task<ActionResult<int>> GetEmployeeNameByName(string name)
        {
            // 查找指定员工的流水号
            var employee = await _context.Employees
                                            .Where(e => e.Name == name)
                                            .FirstOrDefaultAsync();  // 获取单个结果（无结果时返回null）

            if (employee == null)
            {
                return NotFound($"未找到{name}"); // 员工不存在时返回404
            }

            return employee.SerialNumber;
        }

        // PUT: api/employees/name/{name}/increment-serial
        [HttpPut("increment-serial/{name}")]
        public async Task<IActionResult> IncrementSerialByName(string name)
        {
            // 1. 根据姓名查询员工（可能存在多个，用ToListAsync获取所有匹配项）
            var employees = await _context.Employees
                                         .Where(e => e.Name == name) // 按姓名筛选
                                         .ToListAsync();

            // 2. 处理无匹配员工的情况
            if (!employees.Any())
            {
                return NotFound($"未找到姓名为 {name} 的员工");
            }

            // 3. 处理重名情况（若存在多个同名员工，返回错误）
            if (employees.Count > 1)
            {
                return BadRequest($"存在多个姓名为 {name} 的员工，请使用ID操作以避免错误");
            }

            // 4. 获取唯一匹配的员工，执行流水号自增
            var employee = employees.First();
            employee.SerialNumber += 1;

            try
            {
                // 5. 保存修改到数据库
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                // 6. 并发处理：再次验证员工是否仍存在（避免修改过程中被删除）
                if (!_context.Employees.Any(e => e.ID == employee.ID))
                {
                    return NotFound($"姓名为 {name} 的员工已被删除");
                }
                else
                {
                    throw; // 其他并发错误向上抛出
                }
            }

            // 7. 成功返回无内容状态
            return NoContent();
        }
    }

    [Route("api/accounts")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AccountController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/accounts
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Account>>> GetAccounts()
        {
            return await _context.Accounts.ToListAsync();
        }

        // GET: api/accounts/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Account>> GetAccount(int id)
        {
            var account = await _context.Accounts.FindAsync(id);

            if (account == null)
            {
                return NotFound();
            }

            return account;
        }
    }

    
    // 订单Controller
    [Route("api/orders")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly AppDbContext _context;

        public OrderController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/orders
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Order>>> GetOrders()
        {
            return await _context.Orders.ToListAsync();
        }

        // GET: api/orders/5
        [HttpGet("id/{id}")]
        public async Task<ActionResult<Order>> GetOrder(string id)
        {
           var order = await _context.Orders.FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
            {
                return NotFound();
            }

            return order;
        }

        /// <summary>
        /// 根据工人名字获取订单
        /// </summary>
        /// <param name="workerName">工人名称</param>
        /// <returns></returns>
        // GET: api/orders/worker
        [HttpGet("workerName/{workerName}")]
        public async Task<ActionResult<IEnumerable<Order>>> GetOrdersByWorker(string workerName)
        {
            var orders = await _context.Orders.Where(o => o.Worker == workerName).ToListAsync();

            

            return orders;
        }

        /// <summary>
        /// 获取所有进行中的订单
        /// </summary>
        /// <returns></returns>
        // GET: api/orders/ing
        [HttpGet("ing")]
        public async Task<ActionResult<IEnumerable<Order>>> GetOrdersInProgress()
        {
            var orders = await _context.Orders.Where(o => o.State == "进行中").ToListAsync();

            // if (orders == null || orders.Count == 0)
            // {
            //     return NotFound();
            // }

            return orders;
        }

        // POST: api/orders
        [HttpPost]
        public async Task<ActionResult<Order>> PostOrder(Order order)
        {
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetOrder), new { id = order.OrderID }, order);
        }

        /// <summary>
        /// 修改订单的某个字符串字段（例如 State）
        /// </summary>
        /// <param name="id">订单 ID</param>
        /// <param name="newValue">新的字段值</param>
        /// <returns></returns>
        [HttpPatch("{id}/state")]
        public async Task<IActionResult> UpdateOrderState(string id, [FromBody] string newValue)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound("未找到指定的订单。");
            }
        
            order.State = newValue; // 假设 State 是 varchar 类型字段
        
            // 标记该字段已修改
            _context.Entry(order).Property(o => o.State).IsModified = true;
            await _context.SaveChangesAsync();
        
            return NoContent();
        }
    }

    // 物料库的Controller
    [Route("api/materials")]
    [ApiController]
    public class MaterialController : ControllerBase
    {
        private readonly AppDbContext _context;

        public MaterialController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/materials/KYS
        /// <summary>
        /// 获取坑压式物料列表
        /// </summary>
        /// <returns></returns>
        [HttpGet("KYS")]
        public async Task<ActionResult<IEnumerable<KYS>>> GetKYSMaterials()
        {
            return await _context.KYS.ToListAsync();
        }

        // GET: api/materials/MYS
        /// <summary>
        /// 获取模压式物料列表
        /// </summary>
        /// <returns></returns>
        [HttpGet("MYS")]
        public async Task<ActionResult<IEnumerable<MYS>>> GetMYSMaterials()
        {
            return await _context.MYS.ToListAsync();
        }

        // GET: api/materials/DZXH
        /// <summary>
        /// 获取端子型号物料列表
        /// </summary>
        /// <returns></returns>
        [HttpGet("DZXH")]
        public async Task<ActionResult<IEnumerable<DZXH>>> GetDZXHMaterials()
        {
            return await _context.DZXH.ToListAsync();
        }
    }

        // 测试Controller
    [Route("api/test")]
    [ApiController]
    public class TestController : ControllerBase
    {
        private readonly AppDbContext _context;

        public TestController(AppDbContext context)
        {
            _context = context;
        }

        // 获取全部记录
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Test>>> GetAll()
        {
            return await _context.Test.ToListAsync();
        }

        // 根据订单号获取
        [HttpGet("{orderId}")]
        public async Task<ActionResult<List<Test>>> GetById(string orderId)
        {
            var items = await _context.Test
                .Where(t => t.OrderID == orderId)
                .ToListAsync();
        
            // if (items == null || items.Count == 0)
            //     return NotFound();
        
            return Ok(items);
        }


        // 新增记录
        [HttpPost]
        public async Task<ActionResult<Test>> Create(Test item)
        {
            _context.Test.Add(item);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { orderId = item.OrderID }, item);
        }

        // 更新记录
        [HttpPut("{ID}")]
        public async Task<IActionResult> Update(int ID, Test item)
        {
            if (ID != item.ID)
                return BadRequest();

            _context.Entry(item).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // 删除记录
        [HttpDelete("{ID}")]
        public async Task<IActionResult> Delete(int ID)
        {
            var item = await _context.Test.FindAsync(ID);
            if (item == null)
                return NotFound();

            _context.Test.Remove(item);
            await _context.SaveChangesAsync();
            return NoContent();
        }


    }



}
