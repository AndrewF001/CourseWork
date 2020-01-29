using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A_level_course_work_Logic_Gate.File_Classes
{
    [Serializable]
    public class File_Version_Line
    {
        public File_Version_Line(string Bit, int O_ID, int O_Num, int I_ID, int I_Num, double _X1, double _X2, double _Y1, double _Y2)
        {
            Content_Copy = Bit;
            Output_ID = O_ID;
            Output_Num = O_Num;
            Input_ID = I_ID;
            Input_Num = I_Num;
            X1 = _X1;
            X2 = _X2;
            Y1 = _Y1;
            Y2 = _Y2;
        }
        public string Content_Copy { get; set; }
        public int Output_ID { get; set; }
        public int Output_Num { get; set; }
        public int Input_ID { get; set; }
        public int Input_Num { get; set; }
        public double X1, Y1, X2, Y2;
    }
}
