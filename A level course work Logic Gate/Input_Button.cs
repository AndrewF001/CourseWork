using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media;

namespace A_level_course_work_Logic_Gate
{
    public class Input_Button : Button 
    {
        private bool _Bit = false;
        /// <summary>
        /// Changes the feature of the button for what input bit it is automatically.
        /// </summary>
        public bool Bit { get { return _Bit; }
            set
            {
                _Bit = value;
                if(!value)
                {
                    Content = 0;
                    Foreground = Brushes.Black;
                    Background = Brushes.White;
                    _Gate_List[Input_ID].Input[Input_Port].Input_bit = false;
                }
                else
                {
                    Background = Brushes.Black;
                    _Gate_List[Input_ID].Input[Input_Port].Input_bit = true;
                    Content = 1;
                    Foreground = Brushes.White;
                }
                _Gate_List[Input_ID].Gate_Output_Calc();
            }
        
        }
        public int Input_ID { get;}
        public int Input_Port;
        public List<Gate_Class> _Gate_List = new List<Gate_Class>();
        private MainWindow _MainWind { get; set; }
        public Input_Button(int ID, int Port_Num, List<Gate_Class> Gate_List, Canvas_Class Sub_Canvas, MainWindow MainWind)
        {            
            _MainWind = MainWind;
            _Gate_List = Gate_List;
            Input_ID = ID;
            Input_Port = Port_Num;
            Sub_Canvas.Children.Add(this);
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
                Bit = false;
            }
            else
            {                
                Bit = true;
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
