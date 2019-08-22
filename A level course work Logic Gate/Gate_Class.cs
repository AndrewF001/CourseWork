using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        public Gate_Class(string Tag,MainWindow MainWind)
        {
            _MainWind = MainWind;
            Setup(Tag);
            _MainWind.Main_Canvas.Children.Add(Rect);
            Rect_Move(Mouse.GetPosition(MainWind.Main_Grid));
        }

        public void Setup(string _Tag)
        {
            //scale factor
            Rect = new Rectangle { Height = 75, Width = 115, Stroke = Brushes.Black, Fill = Application.Current.Resources[_Tag] as Brush };
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


        public void Rect_Move(Point Pos)
        {
            Canvas.SetLeft(Rect, Pos.X);
            Canvas.SetTop(Rect, Pos.Y);
        }


    }
}
