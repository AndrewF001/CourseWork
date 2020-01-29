using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A_level_course_work_Logic_Gate.File_Classes
{
    [Serializable]
    public class File_Version_GI
    {
        public File_Version_GI(bool I_B, int I_ID, IO_Type I_Type, int L_I)
        {
            _Input_Bit = I_B;
            _Input_ID = I_ID;
            _Input_Type = I_Type;
            _line_ID = L_I;
        }
        public bool _Input_Bit;
        public int _Input_ID = -1;
        public IO_Type _Input_Type = IO_Type.Null;
        public int _line_ID = -1;
    }
}
