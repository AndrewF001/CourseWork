using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace A_level_course_work_Logic_Gate
{
    public class Output_Circle
    {
        public Ellipse Circle = new Ellipse { Height = 20, Width = 20, Fill = Brushes.Black, Stroke = Brushes.Black, StrokeThickness = 1 };
        private bool _bit = false;

        public int Output_ID { get; }
        public int Output_Port;
        public MainWindow _MainWind { get; set; }
        public Output_Circle(MainWindow MainWind, int ID, int Port_Num)
        {
            _MainWind = MainWind;
            Output_ID = ID;
            Output_Port = Port_Num;
            _MainWind.Sub_Canvas.Children.Add(Circle);
        }


        public bool Bit
        {
            get { return _bit; }
            set
            {
                _bit = value;
                if (value == false)
                    Circle.Fill = Brushes.Black;
                else
                    Circle.Fill = Brushes.White;
            }
        }
        public void Set_Coors(double x, double y)
        {
            Canvas.SetLeft(Circle, x+40);
            Canvas.SetTop(Circle, y+10);
        }

        public Ellipse Get_Ellipse()
        {
            return Circle;
        }

        public void Aline_Circle(Gate_Class Gate)
        {
            double[] hold = _MainWind.Link_Output_Aline(Gate, Output_Port);
            Change_X_Y(hold[0], hold[1]);
        }

        public void Change_X_Y(double x, double y)
        {
            Canvas.SetLeft(Get_Ellipse(), x);
            Canvas.SetTop(Get_Ellipse(), y-10);
        }

    }
}
