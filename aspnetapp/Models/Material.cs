using System.ComponentModel.DataAnnotations;

namespace aspnetapp
{
    public class KYS
    {
        [Key]
        public double mm { get; set; }
        public double standard { get; set; }
        public double firststandard { get; set; }
    }


    public class MYS
    {
        [Key]
        public double mm { get; set; }
        public double standard { get; set; }
        public double firststandard { get; set; }
    }


    public class DZXH
    {
        public string describ { get; set; }
        public string method { get; set; }
    }
}
