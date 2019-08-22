using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace A_level_course_work_Logic_Gate
{

    public partial class MainWindow : Window
    {
        //varaibles that need to be accessed all around the code
        public List<Gate_Class> Gate_List { get; set; }

        //program info
        public bool Drag { get; set; } = false;
        public int Drag_Num { get; set; } = 0;

        //code when the program laods up
        public MainWindow()
        {            
            InitializeComponent();
            Gate_List = new List<Gate_Class>();
            
        }

        private void Canvas_Border_Loaded(object sender, RoutedEventArgs e)
        {
            Canvas_Class Main_Canvas = new Canvas_Class(this);
            Canvas_Border.Child = Main_Canvas;

            Rectangle BackGround_Rect = new Rectangle { Height = 4000, Width = 4000, Fill = Brushes.White };
            Main_Canvas.Children.Add(BackGround_Rect);
            Canvas.SetLeft(BackGround_Rect, -1000);
            Canvas.SetLeft(BackGround_Rect, -1000);
        }

        //eventhandler that aren't linked to the canvas
        private void Rect_Button_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if(!Drag)
            {
                Drag = true;
                Gate_List.Add(new Gate_Class(Convert.ToString((sender as Rectangle).Tag),this));
                Drag_Num = Gate_List.Count()-1;
            }
        }

        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            if(Drag)
            {
                Gate_List[Drag_Num].Rect_Move(Mouse.GetPosition(Main_Grid));
            }
        }
    }
}
