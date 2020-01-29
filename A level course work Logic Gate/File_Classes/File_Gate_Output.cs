using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A_level_course_work_Logic_Gate.File_Classes
{
    [Serializable]
    public class File_Version_GO
    {
        public File_Version_GO(int O_ID, IO_Type O_Type, int L_I, int O_P)
        {
            _output_ID = O_ID;
            _output_Type = O_Type;
            _line_ID = L_I;
            _output_port = O_P;
        }
        public int _output_ID = -1;
        public IO_Type _output_Type = IO_Type.Null;
        public int _line_ID = -1;
        public int _output_port;
    }
}
