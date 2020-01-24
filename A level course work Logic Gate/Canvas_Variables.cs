using System.Collections.Generic;

namespace A_level_course_work_Logic_Gate
{
    /// <summary>
    /// All the variables that mainwindow and canvas class overlap with.
    /// </summary>
    public interface Canvas_Variables
    {
        List<Gate_Class> Gate_List { get; set; } 
        List<Line_Class> Line_List { get; set; }        
        List<Input_Button> Input_Button_List { get; set; } 
        List<Output_Circle> Output_Circle_List { get; set; } 
        bool Drag { get; set; }
        bool Link { get; set; }
        int Drag_Num { get; set; }
        Drag_State Drag_Mode { get; set; }
        int Linking_ID { get; set; }
    }
}
