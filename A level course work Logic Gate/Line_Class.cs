﻿using System.Windows;
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
        public MainWindow _MainWind { get; set; }

        public Canvas_Class _Sub_Canvas { get; set; }
        public Line_Class(int _Output_ID, MainWindow MainWind)
        {
            _MainWind = MainWind;
            _Sub_Canvas = _MainWind.Sub_Canvas;
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


        //change this so that the values are generic and then just have it so that the X and Y coords are changed directly and don't need the method to do it.(A lot of work :(
        public void Link_Input_Aline_Line(Gate_Class Gate)
        {
            UI_Line.Stroke = Brushes.Black;
            double[] hold = _MainWind.Link_Input_Aline(Gate, Input_Num);
            Change_X2_Y2(hold[0], hold[1]);
        }

        public void Link_Output_Aline_Line(Gate_Class Gate)
        {
            double[] hold = _MainWind.Link_Output_Aline(Gate, Output_Num);
            Change_X1_Y1(hold[0], hold[1]);
        }
    }
}
