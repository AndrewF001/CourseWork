using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace A_level_course_work_Logic_Gate
{
    public class Canvas_Class : Canvas
    {
        public double Old_Pos_X { get; set; } = 0;
        public double Old_Pos_Y { get; set; } = 0;
        public double Scale_Factor { get; set; } = 1;
        public double Old_Rect_X { get; set; } = 0;
        public double Old_Rect_Y { get; set; } = 0;

        public MainWindow _MainWind { get; set; }
        public Canvas_Class(MainWindow MainWind)
        {
            _MainWind = MainWind;
            Background = Brushes.Gray;
        }
        //Move canvas event
        protected override void OnMouseMove(MouseEventArgs e)
        {
            Point Pos = Mouse.GetPosition(_MainWind.Sub_Canvas);
            if (e.LeftButton == MouseButtonState.Pressed && !_MainWind.Drag)
            {
                double X_Difference = Pos.X - Old_Pos_X;
                double Y_Difference = Pos.Y - Old_Pos_Y;
                Sub_Canvas_Translation(X_Difference, Y_Difference);
            }
            Old_Pos_X = Pos.X;
            Old_Pos_Y = Pos.Y;
        }
        
        private void Sub_Canvas_Translation(double X_Difference, double Y_Difference)
        {
            Canvas.SetLeft(_MainWind.BackGround_Rect, Canvas.GetLeft(_MainWind.BackGround_Rect) + X_Difference);
            Canvas.SetTop(_MainWind.BackGround_Rect, Canvas.GetTop(_MainWind.BackGround_Rect) + Y_Difference);
            for (int i = 0; i < _MainWind.Gate_List.Count; i++)
            {                
                Rectangle Rect = _MainWind.Gate_List[i].Rect;
                Canvas.SetLeft(Rect, Canvas.GetLeft(Rect) + X_Difference);
                Canvas.SetTop(Rect, Canvas.GetTop(Rect) + Y_Difference);
            }
        }
        //zoom event
        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            bool change = false;
            Point Pos = Mouse.GetPosition(_MainWind.Canvas_Border);
            if (e.Delta > 0)
            {
                if (Scale_Factor != 0.0625)
                {
                    Scale_Factor -= 0.0625;
                    change = true;
                }
            }
            else if (e.Delta < 0)
            {
                if (Scale_Factor != 2)
                {
                    Scale_Factor += 0.0625;
                    change = true;
                }
            }

            if (change)
            {
                ScaleTransform scaleTransform = new ScaleTransform(Scale_Factor, Scale_Factor, Pos.X, Pos.Y);
                _MainWind.Sub_Canvas.RenderTransform = scaleTransform;
            }
        }
        //Re-drag pick up
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            int detected = Rect_detection(0, 0, -1);
            if (!_MainWind.Drag && !_MainWind.Link)
            {
                if (detected != -1)
                {                    
                    _MainWind.Drag_Num = detected;
                    _MainWind.Drag_Mode = Drag_State.Sub_Can;
                    Old_Rect_X = Canvas.GetLeft(_MainWind.Gate_List[_MainWind.Drag_Num].Rect);
                    Old_Rect_Y = Canvas.GetTop(_MainWind.Gate_List[_MainWind.Drag_Num].Rect);
                }
            }
            else if (!_MainWind.Drag && _MainWind.Link && detected!=-1)
            {
                _MainWind.Linking_ID = detected;
                _MainWind.Drag_Mode = Drag_State.Link_Mode_Sub;
                _MainWind.Drag_Num = _MainWind.Line_List.Count();
                _MainWind.Line_List.Add(new Line_Class(detected,_MainWind.Sub_Canvas));
                Link_Output_Aline(detected);
                //location
            }
        }
        //Re-drag drop
        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            if (_MainWind.Drag && !_MainWind.Link)
            {
                _MainWind.Drag_Mode = Drag_State.Null;
                if(Rect_detection(_MainWind.Gate_List[_MainWind.Drag_Num].Rect.Width, _MainWind.Gate_List[_MainWind.Drag_Num].Rect.Height, _MainWind.Drag_Num) !=-1)
                {
                    Canvas.SetLeft(_MainWind.Gate_List[_MainWind.Drag_Num].Rect, Old_Rect_X);
                    Canvas.SetTop(_MainWind.Gate_List[_MainWind.Drag_Num].Rect,Old_Rect_Y);
                }
            }
            else if(_MainWind.Drag && _MainWind.Link)
            {
                int detection = Rect_detection(0, 0, -1);
                _MainWind.Drag_Mode = Drag_State.Null;
                if(detection!=-1&& detection != _MainWind.Linking_ID)
                {
                    //location
                    Link_Input_Aline();
                    _MainWind.Line_List[_MainWind.Drag_Num].Track_Mouse();

                    _MainWind.Line_List[_MainWind.Drag_Num].UI_Line.Stroke = Brushes.Black;
                }
                else
                {
                    _MainWind.Sub_Canvas.Children.Remove(_MainWind.Line_List[_MainWind.Drag_Num].UI_Line);
                    _MainWind.Line_List.RemoveAt(_MainWind.Drag_Num);
                }
            }
        }

        public int Rect_detection(double Width, double Height,int Drag_Num)
        {
            int Detected = -1;
            Point Pos_Sub = Mouse.GetPosition(_MainWind.Sub_Canvas);
            for (int i = 0; i < _MainWind.Gate_List.Count(); i++)
            {
                Gate_Class Rect = _MainWind.Gate_List[i];
                double Rect_X = Canvas.GetLeft(_MainWind.Gate_List[i].Rect);
                double Rect_Y = Canvas.GetTop(_MainWind.Gate_List[i].Rect);
                if (Pos_Sub.X+Width > Rect_X && Pos_Sub.X < Rect_X + Rect.Rect.Width && Pos_Sub.Y+Height > Rect_Y && Pos_Sub.Y < Rect_Y + Rect.Rect.Height&&i!=Drag_Num)
                {
                    Detected = i;
                }
            }
            return Detected;
        }
        public void Link_Output_Aline(int Clicked)
        {
            // make the X1 and Y1 a function;
            if(_MainWind.Gate_List[Clicked].Type==7)
            {
                if (_MainWind.Gate_List[Clicked].Output_1_ID == -1)
                {
                    _MainWind.Line_List.Last().UI_Line.X1 = Canvas.GetLeft(_MainWind.Gate_List[Clicked].Rect) + 75;
                    _MainWind.Line_List.Last().UI_Line.Y1 = Canvas.GetTop(_MainWind.Gate_List[Clicked].Rect) + 23.8;
                }
                else if (_MainWind.Gate_List[Clicked].Output_2_ID == -1)
                {
                    _MainWind.Line_List.Last().UI_Line.X1 = Canvas.GetLeft(_MainWind.Gate_List[Clicked].Rect) + 85;
                    _MainWind.Line_List.Last().UI_Line.Y1 = Canvas.GetTop(_MainWind.Gate_List[Clicked].Rect) + 36;
                }
                else if (_MainWind.Gate_List[Clicked].Output_3_ID == -1)
                {
                    _MainWind.Line_List.Last().UI_Line.X1 = Canvas.GetLeft(_MainWind.Gate_List[Clicked].Rect) + 85;
                    _MainWind.Line_List.Last().UI_Line.Y1 = Canvas.GetTop(_MainWind.Gate_List[Clicked].Rect) + 45;
                }
            }
            else if(_MainWind.Gate_List[Clicked].Type == 2)
            {
                _MainWind.Line_List.Last().UI_Line.X1 = Canvas.GetLeft(_MainWind.Gate_List[Clicked].Rect) + 109.5;
                _MainWind.Line_List.Last().UI_Line.Y1 = Canvas.GetTop(_MainWind.Gate_List[Clicked].Rect) + 36;
            }
            else
            {
                _MainWind.Line_List.Last().UI_Line.X1 = Canvas.GetLeft(_MainWind.Gate_List[Clicked].Rect) + 115;
                _MainWind.Line_List.Last().UI_Line.Y1 = Canvas.GetTop(_MainWind.Gate_List[Clicked].Rect) + 35.7;
            }
        }

        public void Link_Input_Aline()
        {

        }

    }
}
