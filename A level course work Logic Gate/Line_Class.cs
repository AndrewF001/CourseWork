using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace A_level_course_work_Logic_Gate
{
    public class Line_Class
    {
        public Line UI_Line { get; set; } = new Line { StrokeThickness = 4, Stroke = Brushes.Red };
        public int Output_ID { get; set; } 
        public int Output_Num { get; set; } = 1; //this should only change if the gate type is the multiple output(type 7)
        public int Input_ID { get; set; } = -1;
        public int Input_Num { get; set; } = -1;
        
        public Canvas_Class _Sub_Canvas { get; set; } 
        public Line_Class(int _Output_ID, Canvas_Class Sub_Canvas)
        {
            _Sub_Canvas = Sub_Canvas;
            _Sub_Canvas.Children.Add(UI_Line);                
            Output_ID = _Output_ID;
        }

        public void Track_Mouse()
        {
            Point Pos = Mouse.GetPosition(_Sub_Canvas);
            UI_Line.X2 = Pos.X;
            UI_Line.Y2 = Pos.Y;
        }

        public void Change_X1_Y1(double X, double Y)
        {
            UI_Line.X1 = X;
            UI_Line.Y1 = Y;
        }
        public void Change_X2_Y2(double X, double Y)
        {
            UI_Line.X2 = X;
            UI_Line.Y2 = Y;
        }
    }
}
