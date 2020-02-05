using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A_level_course_work_Logic_Gate.File_Classes
{
    [Serializable]
    public class File_Version_Output
    {
        public File_Version_Output(int O_ID, int O_Port, double x, double y)
        {
            Output_ID = O_ID;
            Output_Port = O_Port;
            X = x;
            Y = y;
        }
        public int Output_ID;
        public int Output_Port;
        public double X, Y;

    }
}
