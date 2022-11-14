using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HtmlEngine.Models
{
    public class Professor
    {
        public string Result { get; set; }
        public Student Student { get; set; }
    }

    public class Student
    {
        public string Name { get; set; }
    }
}
