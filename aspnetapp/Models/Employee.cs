namespace aspnetapp
{
    public class Employee
    {
        public int ID { get; set; }
        public string Name { get; set; } = null!;
        public int Jurisdiction { get; set; } // 权限
    }
}