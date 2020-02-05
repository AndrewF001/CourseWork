using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace A_level_course_work_Logic_Gate
{
    public class Output_Circle
    {
        public Ellipse Circle = new Ellipse { Height = 20, Width = 20, Fill = Brushes.White, Stroke = Brushes.Black, StrokeThickness = 1 };
        public Label Output_Lab = new Label { Content = "0", Width = 16, Height = 29, Foreground = Brushes.Black };
        
        public int Output_ID { get; set; }
        public int Output_Port;
        private MainWindow _MainWind { get; }
        public Output_Circle(int ID, int Port_Num, Canvas_Class Sub_Canvas,MainWindow MainWind)
        {
            Output_ID = ID;
            Output_Port = Port_Num;
            Sub_Canvas.Children.Add(Circle);
            Sub_Canvas.Children.Add(Output_Lab);
            _MainWind = MainWind;
        }


        public bool Bit
        {
            set
            {
                if (value == false)
                {
                    Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => Circle_0()));
                }
                else
                {
                    Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => Circle_1()));
                }
            }
        }
        private void Circle_0()
        {
            Circle.Fill = Brushes.White;
            Output_Lab.Foreground = Brushes.Black;
            Output_Lab.Content = "0";

        }

        private void Circle_1()
        {
            Circle.Fill = Brushes.Black;
            Output_Lab.Foreground = Brushes.White;
            Output_Lab.Content = "1";
        }

        public void Add_UI()
        {
            _MainWind.Sub_Canvas.Children.Add(Circle);
            _MainWind.Sub_Canvas.Children.Add(Output_Lab);
        }
        public void Remove_UI()
        {
            _MainWind.Sub_Canvas.Children.Remove(Circle);
            _MainWind.Sub_Canvas.Children.Remove(Output_Lab);
        }
        public void Aline_Circle(Gate_Class Gate)
        {
            double[] hold = _MainWind.Link_Output_Aline(Gate, Output_Port);
            Change_X_Y(hold[0], hold[1]);
        }

        public void Change_X_Y(double x, double y)
        {
            Canvas.SetLeft(Circle, x);
            Canvas.SetTop(Circle, y-10);
            Canvas.SetLeft(Output_Lab, x+2);
            Canvas.SetTop(Output_Lab, y-13);
        }

    }
}
