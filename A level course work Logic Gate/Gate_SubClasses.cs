using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace A_level_course_work_Logic_Gate
{
    public class And_Gate_Class : Gate_Class
    {
        public And_Gate_Class(Canvas Main_Canvas, double _Scale_Factor, List<Output_Circle> Output_Circle_List, List<Line_Class> Line_List, List<Input_Button> Input_Button_List) : base(Output_Circle_List,Line_List,Input_Button_List)
        {
            Rect = new Rectangle { Height = 75 * _Scale_Factor, Width = 115 * _Scale_Factor, Stroke = Brushes.Black, Fill = Application.Current.Resources["And_Gate_L"] as Brush };
            Main_Canvas.Children.Add(Rect);
            Point Pos = Mouse.GetPosition(Main_Canvas);
            Rect_Move(Pos);
            Type = 0;
        }
        public override void Gate_Output_Calc()
        {
            if (Input[0].Input_bit == true && Input[1].Input_bit == true)
            {
                Gate_Bit = true;
            }
            else
            {
                Gate_Bit = false;
            }
        }
        
    }
    public class Nand_Gate_Class : Gate_Class
    {
        public Nand_Gate_Class(Canvas Main_Canvas, double _Scale_Factor, List<Output_Circle> Output_Circle_List, List<Line_Class> Line_List, List<Input_Button> Input_Button_List) : base(Output_Circle_List, Line_List, Input_Button_List)
        {
            Rect = new Rectangle { Height = 75 * _Scale_Factor, Width = 115 * _Scale_Factor, Stroke = Brushes.Black, Fill = Application.Current.Resources["Nand_Gate_L"] as Brush };
            Main_Canvas.Children.Add(Rect);
            Point Pos = Mouse.GetPosition(Main_Canvas);
            Rect_Move(Pos);
            Type = 1;
        }
        public override void Gate_Output_Calc()
        {
            if (Input[0].Input_bit == true && Input[1].Input_bit == true)
            {
                Gate_Bit = false;
            }
            else
            {
                Gate_Bit = true;
            }
        }
    }

    
    public class Not_Gate_Class : Gate_Class
    {
        public Not_Gate_Class(Canvas Main_Canvas, double _Scale_Factor, List<Output_Circle> Output_Circle_List, List<Line_Class> Line_List, List<Input_Button> Input_Button_List) : base(Output_Circle_List, Line_List, Input_Button_List)
        {
            Rect = new Rectangle { Height = 75 * _Scale_Factor, Width = 115 * _Scale_Factor, Stroke = Brushes.Black, Fill = Application.Current.Resources["Not_Gate_L"] as Brush };
            Main_Canvas.Children.Add(Rect);
            Point Pos = Mouse.GetPosition(Main_Canvas);
            Rect_Move(Pos);
            Type = 2;
        }
        public override void Gate_Output_Calc()
        {
            if (Input[0].Input_bit == true)
            {
                Gate_Bit = false;
            }
            else
            {
                Gate_Bit = true;
            }
        }
    }
    public class Or_Gate_Class : Gate_Class
    {
        public Or_Gate_Class(Canvas Main_Canvas, double _Scale_Factor, List<Output_Circle> Output_Circle_List, List<Line_Class> Line_List, List<Input_Button> Input_Button_List) : base(Output_Circle_List, Line_List, Input_Button_List)
        {
            Rect = new Rectangle { Height = 75 * _Scale_Factor, Width = 115 * _Scale_Factor, Stroke = Brushes.Black, Fill = Application.Current.Resources["Or_Gate_L"] as Brush };
            Main_Canvas.Children.Add(Rect);
            Point Pos = Mouse.GetPosition(Main_Canvas);
            Rect_Move(Pos);
            Type = 3;
        }
        public override void Gate_Output_Calc()
        {
            if (Input[0].Input_bit == true || Input[1].Input_bit == true)
            {
                Gate_Bit = true;
            }
            else
            {
                Gate_Bit = false;
            }
        }
    }
    public class Xor_Gate_Class : Gate_Class
    {
        public Xor_Gate_Class(Canvas Main_Canvas, double _Scale_Factor, List<Output_Circle> Output_Circle_List, List<Line_Class> Line_List, List<Input_Button> Input_Button_List) : base(Output_Circle_List, Line_List, Input_Button_List)
        {
            Rect = new Rectangle { Height = 75 * _Scale_Factor, Width = 115 * _Scale_Factor, Stroke = Brushes.Black, Fill = Application.Current.Resources["Xor_Gate_L"] as Brush };
            Main_Canvas.Children.Add(Rect);
            Point Pos = Mouse.GetPosition(Main_Canvas);
            Rect_Move(Pos);
            Type = 5;
        }
        public override void Gate_Output_Calc()
        {
            if ((Input[0].Input_bit == true && Input[1].Input_bit == true) || (Input[0].Input_bit == false && Input[1].Input_bit == false))
            {
                Gate_Bit = false;
            }
            else
            {
                Gate_Bit = true;
            }
        }    
    }
    public class Nor_Gate_Class : Gate_Class
    {
        public Nor_Gate_Class(Canvas Main_Canvas, double _Scale_Factor, List<Output_Circle> Output_Circle_List, List<Line_Class> Line_List, List<Input_Button> Input_Button_List) : base(Output_Circle_List, Line_List, Input_Button_List)
        {
            Rect = new Rectangle { Height = 75 * _Scale_Factor, Width = 115 * _Scale_Factor, Stroke = Brushes.Black, Fill = Application.Current.Resources["Nor_Gate_L"] as Brush };
            Main_Canvas.Children.Add(Rect);
            Point Pos = Mouse.GetPosition(Main_Canvas);
            Rect_Move(Pos);
            Type = 4;
        }
        public override void Gate_Output_Calc()
        {
            if (Input[0].Input_bit == true || Input[1].Input_bit == true)
            {
                Gate_Bit = false;
            }
            else
            {
                Gate_Bit = true;
            }
        }
    }
    public class Xnor_Gate_Class : Gate_Class
    {
        public Xnor_Gate_Class(Canvas Main_Canvas, double _Scale_Factor, List<Output_Circle> Output_Circle_List, List<Line_Class> Line_List, List<Input_Button> Input_Button_List) : base(Output_Circle_List, Line_List, Input_Button_List)
        {
            Rect = new Rectangle { Height = 75 * _Scale_Factor, Width = 115 * _Scale_Factor, Stroke = Brushes.Black, Fill = Application.Current.Resources["Xnor_Gate_L"] as Brush };
            Main_Canvas.Children.Add(Rect);
            Point Pos = Mouse.GetPosition(Main_Canvas);
            Rect_Move(Pos);
            Type = 6;
        }
        public override void Gate_Output_Calc()
        {
            if ((Input[0].Input_bit == true && Input[1].Input_bit == true) || (Input[0].Input_bit == false && Input[1].Input_bit == false))
            {
                Gate_Bit = true;
            }
            else
            {
                Gate_Bit = false;
            }
        }
    }
    public class Transformer_Class : Gate_Class
    {
        public Transformer_Class(Canvas Main_Canvas, double _Scale_Factor, List<Output_Circle> Output_Circle_List, List<Line_Class> Line_List, List<Input_Button> Input_Button_List) : base(Output_Circle_List, Line_List, Input_Button_List)
        {
            Rect = new Rectangle { Height = 75 * _Scale_Factor, Width = 85 * _Scale_Factor, Stroke = Brushes.Black, Fill = Application.Current.Resources["Transformer"] as Brush };
            Main_Canvas.Children.Add(Rect);
            Point Pos = Mouse.GetPosition(Main_Canvas);
            Rect_Move(Pos);
            Type = 7;
        }
        public override void Gate_Output_Calc()
        {
            if (Input[0].Input_bit == true)
            {
                Gate_Bit = true;
            }
            else
            {
                Gate_Bit = false;
            }
        }
    }

}
