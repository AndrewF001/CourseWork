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
        public int Output_ID { get; set; } = -1;
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



        public void Link_Input_Aline(Gate_Class Gate)
        {
            UI_Line.Stroke = Brushes.Black;
            //not input is in the center of the gate compare to the other gates
            if (Gate.Type == 2)
            {
                Change_X2_Y2(Canvas.GetLeft(Gate.Rect) + 5, Canvas.GetTop(Gate.Rect) + 38);
            }
            //this is similar to the not gate but because it's a sqaure not a rectangle it needed to be moved in the X axis more
            else if (Gate.Type == 7)
            {
                Change_X2_Y2(Canvas.GetLeft(Gate.Rect) + 12.5, Canvas.GetTop(Gate.Rect) + 38);
            }
            //the rest all follow the setup for the and gate
            else
            {
                if (Input_Num == 0)
                {
                    Change_X2_Y2(Canvas.GetLeft(Gate.Rect), Canvas.GetTop(Gate.Rect) + 15);
                }
                //else if not needed here but Input_ID isn't a secure variable type.
                else if (Input_Num == 1)
                {
                    Change_X2_Y2(Canvas.GetLeft(Gate.Rect), Canvas.GetTop(Gate.Rect) + 62);
                }
            }

        }

        public void Link_Output_Aline(Gate_Class Gate)
        {
            //special gate class with 3 exit
            if (Gate.Type == 7)
            {
                if (Output_Num == 0)
                {
                    Change_X1_Y1(Canvas.GetLeft(Gate.Rect) + 75, Canvas.GetTop(Gate.Rect) + 23.8);
                }
                else if (Output_Num == 1)
                {
                    Change_X1_Y1(Canvas.GetLeft(Gate.Rect) + 75, Canvas.GetTop(Gate.Rect) + 36);
                }
                else if (Output_Num == 2)
                {
                    Change_X1_Y1(Canvas.GetLeft(Gate.Rect) + 75, Canvas.GetTop(Gate.Rect) + 51);
                }
            }
            //not gate
            else if (Gate.Type == 2)
            {
                Change_X1_Y1(Canvas.GetLeft(Gate.Rect) + 109.5, Canvas.GetTop(Gate.Rect) + 36);
            }
            //every other gate
            else
            {
                Change_X1_Y1(Canvas.GetLeft(Gate.Rect) + 115, Canvas.GetTop(Gate.Rect) + 35.7);
            }
        }

    }
}
