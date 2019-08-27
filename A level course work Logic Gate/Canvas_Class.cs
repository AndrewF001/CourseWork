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
    public class Canvas_Class : Canvas
    {
        public double Old_Pos_X { get; set; } = 0;
        public double Old_Pos_Y { get; set; } = 0;
        public double Scale_Factor { get; set; } = 1;

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
            if (!_MainWind.Drag)
            {
                Rect_detection();
            }
        }
        //Re-drag drop
        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            if (_MainWind.Drag)
            {
                _MainWind.Drag_Mode = Drag_State.Null;
            }
        }


        private void Rect_detection()
        {
            Point Pos_Sub = Mouse.GetPosition(_MainWind.Sub_Canvas);
            for (int i = 0; i < _MainWind.Gate_List.Count(); i++)
            {
                Gate_Class Rect = _MainWind.Gate_List[i];
                double Rect_X = Canvas.GetLeft(_MainWind.Gate_List[i].Rect);
                double Rect_Y = Canvas.GetTop(_MainWind.Gate_List[i].Rect);
                if (Pos_Sub.X > Rect_X && Pos_Sub.X < Rect_X + Rect.Rect.Width && Pos_Sub.Y > Rect_Y && Pos_Sub.Y < Rect_Y + Rect.Rect.Height)
                {
                    _MainWind.Drag = true;
                    _MainWind.Drag_Num = i;
                    _MainWind.Drag_Mode = Drag_State.Sub_Can;
                }
            }
        }
    }
}
