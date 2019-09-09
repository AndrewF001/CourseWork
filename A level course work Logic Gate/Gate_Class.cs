using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows;
using System.Windows.Input;

namespace A_level_course_work_Logic_Gate
{
    public class Gate_Class
    {        
        public int ID { get; set; }
        public int Type { get; set; }
        public Rectangle Rect { get; set; }
        public MainWindow _MainWind { get; set; }

        //data storages for the input and output
        public enum Input_Type : byte { Null,Gate,Button}
        public bool Input_1 { get; set; } = false;
        public int Input_1_ID { get; set; } = -1;
        public Input_Type Input_1_Type { get; set; } = Input_Type.Null;
        public bool Input_2 { get; set; } = false;
        public int Input_2_ID { get; set; } = -1;
        public Input_Type Input_2_Type { get; set; } = Input_Type.Null;
        //Gate output Bit
        public bool Gate_Bit { get; set; } = false;
        //if equal -1 then the output is null
        public int Output_1_ID { get; set; } = -1;
        public int Output_2_ID { get; set; } = -1;
        public int Output_3_ID { get; set; } = -1;


        public Gate_Class(string Tag,MainWindow MainWind,double _Scale_Factor)
        {
            _MainWind = MainWind;
            Setup(Tag,_Scale_Factor);
            _MainWind.Main_Canvas.Children.Add(Rect);
            Rect_Move(Mouse.GetPosition(MainWind.Main_Grid));
        }
        //Basically the constructor
        public void Setup(string _Tag,double _Scale_Factor)
        {
            //scale factor
            Rect = new Rectangle { Height = 75*_Scale_Factor, Width = 115*_Scale_Factor, Stroke = Brushes.Black, Fill = Application.Current.Resources[_Tag] as Brush };
            //Calc Tag
            switch(_Tag)
            {
                case ("And_Gate_L"):
                    Type = 0;
                    break;
                case ("Nand_Gate_L"):
                    Type = 1;
                    break;
                case ("Not_Gate_L"):
                    Type = 2;
                    break;
                case ("Or_Gate_L"):
                    Type = 3;
                    break;
                case ("Nor_Gate_L"):
                    Type = 4;
                    break;
                case ("Xor_Gate_L"):
                    Type = 5;
                    break;
                case ("Xnor_Gate_L"):
                    Type = 6;
                    break;
                case ("Transformer"):
                    Type = 7;
                    //scale factor
                    Rect.Width = 85;
                    break;
            }
        }

        //change Rectangle location.
        public void Rect_Move(Point Pos)
        {
            Canvas.SetLeft(Rect, Pos.X);
            Canvas.SetTop(Rect, Pos.Y);
        }

    }
}
