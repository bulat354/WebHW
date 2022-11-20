using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HtmlEngine.Models
{
    public class Test1
    {
        public Test2 test2 { get; set; } = new Test2() { result = "OK TEST1" };
        public Test2[] tests { get; set; } = new[]
        {
                new Test2() { result = "Ok" },
                new Test2() { result = "very good" }
            };
        public string[] results { get; set; } = new[]
        {
                "omg",
                "its very cool"
            };
        public bool cond = true;
        public int number = 7;
    }

    public class Test2
    {
        public string result { get; set; }

        public string Test() => "ok";
    }
}
