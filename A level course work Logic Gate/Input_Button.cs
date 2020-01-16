using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;

namespace A_level_course_work_Logic_Gate
{
    public class Input_Button : Button
    {
        private bool _Bit = false;
        public bool Bit { get { return _Bit; }
            set
            {
                _Bit = value;
                _MainWind.Gate_List[Input_ID].Gate_Output_Calc();
            }
        
        }
        public int Input_ID { get;}
        public int Input_Port;
        public MainWindow _MainWind { get; set; }
        public Input_Button(MainWindow MainWind,int ID, int Port_Num)
        {
            _MainWind = MainWind;
            Input_ID = ID;
            Input_Port = Port_Num;
            _MainWind.Sub_Canvas.Children.Add(this);
            Background = Brushes.White;
            Content = 0;
            Foreground = Brushes.Black;
            Height = 20;
            Width = 20;

        }
        //make this bit depend. So when the bit variable changes so does everything else.
        protected override void OnClick()
        {
            if (_Bit)
            {
                Background = Brushes.White;
                _MainWind.Gate_List[Input_ID].Input[Input_Port].Input_bit = false;
                Bit = false;
                Content = 0;
                Foreground = Brushes.Black;
            }
            else
            {                
                Background = Brushes.Black;
                _MainWind.Gate_List[Input_ID].Input[Input_Port].Input_bit = true;
                Bit = true;
                Content = 1;
                Foreground = Brushes.White;
            }
        }

        public void Aline_Box(Gate_Class Gate)
        {
            double[] hold = _MainWind.Link_Input_Aline(Gate, Input_Port);
            Change_X_Y(hold[0], hold[1]);
        }

        public void Change_X_Y(double x, double y)
        {
            Canvas.SetLeft(this, x-20);
            Canvas.SetTop(this, y-10);
        }
    }
}
