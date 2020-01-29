using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace A_level_course_work_Logic_Gate.SubGate_Classes
{
    public class Nand_Gate_Class : Gate_Class
    {
        public Nand_Gate_Class(Canvas Main_Canvas, double _Scale_Factor, List<Output_Circle> Output_Circle_List, List<Line_Class> Line_List, List<Input_Button> Input_Button_List) : base(Output_Circle_List, Line_List, Input_Button_List)
        {
            Rect = new Rectangle { Height = 75 * _Scale_Factor, Width = 115 * _Scale_Factor, Stroke = Brushes.Black, Fill = Application.Current.Resources["Nand_Gate_L"] as Brush };
            Part_Constructor(Main_Canvas);
            Type = 1;
        }
        public override void Gate_Output_Calc()
        {
            if (Input[0].Input_bit && Input[1].Input_bit == true)
            {
                Gate_Bit = false;
            }
            else
            {
                Gate_Bit = true;
            }
        }
    }
}
