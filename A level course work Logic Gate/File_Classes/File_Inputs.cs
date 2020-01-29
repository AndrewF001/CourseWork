using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A_level_course_work_Logic_Gate.File_Classes
{
    [Serializable]
    public class File_Version_Input
    {
        public File_Version_Input(bool Bit, int I_ID, int I_Port, double x, double y)
        {
            _Bit = Bit;
            Input_ID = I_ID;
            Input_Port = I_Port;
            X = x;
            Y = y;
        }
        public bool _Bit = false;
        public int Input_ID { get; }
        public int Input_Port;
        public double X, Y;
    }
}
