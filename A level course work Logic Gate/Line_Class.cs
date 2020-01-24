using System.Collections.Generic;
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
        public Label Line_Lable { get; set; } = new Label { Content = "0", Width = 16, Height = 29, Foreground = Brushes.Black };
        public int Output_ID { get; set; } = -1;
        public int Output_Num { get; set; } = -1; //this should only change if the gate type is the multiple output(type 7)
        public int Input_ID { get; set; } = -1;
        public int Input_Num { get; set; } = -1;
        public double X1,Y1,  X2,  Y2;
        public List<Line_Class> _Line_List { get; set; }

        public Canvas_Class _Sub_Canvas { get; set; }
        private MainWindow _MainWind { get; }
        public Line_Class(int _Output_ID, Canvas_Class Sub_Canvas, MainWindow MainWind,int _Input_ID,bool New_Class)
        {
            _Sub_Canvas = Sub_Canvas;
            _MainWind = MainWind;
            _Sub_Canvas.Children.Add(UI_Line);
            _Sub_Canvas.Children.Add(Line_Lable);
            Output_ID = _Output_ID;
            _Line_List = MainWind.Line_List;
            Track_Mouse();
            Input_ID = _Input_ID;            
            if(New_Class)
            {
                Output_Num = _Sub_Canvas.Link_Output_Vaildation(_Input_ID);
            }
        }

        public void Track_Mouse()
        {
            Point Pos = Mouse.GetPosition(_Sub_Canvas);
            UI_Line.X2 = Pos.X;
            UI_Line.Y2 = Pos.Y;
            X2 = Pos.X;
            Y2 = Pos.Y;
            Move_Label();
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

        public void Move_Label()
        {
            double X = (X2 - X1)/2 - 5+X1;
            double Y = (Y2 - Y1)/2 - 23+Y1;
            Canvas.SetLeft(Line_Lable, X);
            Canvas.SetTop(Line_Lable, Y);
        }

        //change this so that the values are generic and then just have it so that the X and Y coords are changed directly and don't need the method to do it.(A lot of work :(
        public void Link_Input_Aline_Line(Gate_Class Gate)
        {
            UI_Line.Stroke = Brushes.Black;
            double[] hold = _MainWind.Link_Input_Aline(Gate, Input_Num);
            Change_X2_Y2(hold[0], hold[1]);
            X2 = hold[0];
            Y2 = hold[1];
            Move_Label();
        }

        public void Link_Output_Aline_Line(Gate_Class Gate)
        {
            double[] hold = _MainWind.Link_Output_Aline(Gate, Output_Num);
            Change_X1_Y1(hold[0], hold[1]);
            X1 = hold[0];
            Y1 = hold[1];
            Move_Label();
        }

        public void Remove_UI()
        {
            _Sub_Canvas.Children.Remove(UI_Line);
            _Sub_Canvas.Children.Remove(Line_Lable);
        }

        public void Remove_Class()
        {
            Remove_UI();
            _Line_List.Remove(this);
        }
    }
}
