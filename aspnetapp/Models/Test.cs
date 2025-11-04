using System.ComponentModel.DataAnnotations;
namespace aspnetapp
{
    public class Test
    {
        [Key]
        public int ID {get;set;}                     // 自增ID
        public string OrderID { get; set; }          // 订单编号（主键）

        public bool IsFirst { get; set; }            // 是否为首件

        public string DZ1Value { get; set; }         // 端子1测试值
        public string DZ2Value { get; set; }         // 端子2测试值
        public string DZ3Value { get; set; }         // 端子3测试值

        public DateTimeOffset?  Date { get; set; }           // 日期

        public bool DZ1Check { get; set; }           // 端子1检验结果
        public bool DZ2Check { get; set; }           // 端子2检验结果
        public bool DZ3Check { get; set; }           // 端子3检验结果
    }
}
