using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace A_level_course_work_Logic_Gate
{
    public interface Gate_Arguments
    {
        Canvas Main_Canvas { get; set; }
        double Scale_Factor { get; set; } 
        Output_Circle Output_Circle_List{get;set;}
        List<Line_Class> Line_List { get; set; }
        List<Input_Button> Input_Button_List { get; set; }
    }
}
