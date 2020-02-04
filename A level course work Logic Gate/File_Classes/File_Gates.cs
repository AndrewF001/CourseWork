using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A_level_course_work_Logic_Gate.File_Classes
{
    [Serializable]
    public class File_Version_Gate
    {
        public File_Version_Gate(Gate_Type _Type, bool _Alive, Input_Class[] _Input, bool Gate_Bit, Output_Class[] _Output, double x, double y)
        {
            Type = _Type;
            Alive = _Alive;
            _Gate_Bit = Gate_Bit;
            X = x;
            Y = y;
            for (int i = 0; i < 2; i++)
            {
                Input[i] = new File_Version_GI(_Input[i].Input_bit, _Input[i].Input_ID, _Input[i].Input_Type, _Input[i].Line_ID);
            }
            for (int i = 0; i < 3; i++)
            {
                Output[i] = new File_Version_GO(_Output[i].Output_ID, _Output[i].Output_Type, _Output[i].Line_ID, _Output[i].Output_Port);
            }
        }

        public Gate_Type Type { get; set; }
        public bool Alive { get; set; } = true;
        public File_Version_GI[] Input { get; set; } = new File_Version_GI[2];
        public bool _Gate_Bit;
        public File_Version_GO[] Output { get; set; } = new File_Version_GO[3];
        public double X, Y;
    }
}
