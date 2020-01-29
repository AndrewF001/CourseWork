using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A_level_course_work_Logic_Gate.File_Classes
{
    [Serializable]
    public class File_Class
    {
        public List<File_Version_Gate> Gates { get; set; } = new List<File_Version_Gate>();
        public List<File_Version_Line> Lines { get; set; } = new List<File_Version_Line>();
        public List<File_Version_Input> Inputs { get; set; } = new List<File_Version_Input>();
        public List<File_Version_Output> Output { get; set; } = new List<File_Version_Output>();
    }
}
