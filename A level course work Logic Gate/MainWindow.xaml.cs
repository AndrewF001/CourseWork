using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace A_level_course_work_Logic_Gate
{
    public enum Drag_State  { Null,Main_Can,Sub_Can,Link_Mode_Sub}
    public enum IO_Type  { Null, Gate, IO }
     
    public partial class MainWindow : Window
    {
        //delete this varaible after testing!
        public int Last_ID_For_Rect { get; set; } = -1;
        
        //varaibles that need to be accessed all around the code
        public List<Gate_Class> Gate_List { get; set; } = new List<Gate_Class>();
        public List<Line_Class> Line_List { get; set; } = new List<Line_Class>();
        //make these custom classes
        public List<Input_Button> Input_Button_List { get; set; } = new List<Input_Button>();
        public List<Output_Circle> Output_Circle_List { get; set; } = new List<Output_Circle>();
        //program info
        public bool Drag { get; set; } = false;
        public int Delay_Intervals { get; set; } = 1;
        public int Drag_Num { get; set; } = 0;
        private bool _link = false;
        public bool Link
        { get { return _link; }
            set
            {
                _link = value;
                if(_link)
                {
                    Link_Button.Content = "Link";
                }
                else if(!_link)
                {
                    Link_Button.Content = "Drag";
                }
            }
        }

        private Drag_State drag_mode = Drag_State.Null;
        public Drag_State Drag_Mode
        { get { return drag_mode; }
            set
            {
                switch(value)
                {
                    case (Drag_State.Null):
                        Drag = false;
                        break;
                    default:
                        Drag = true;
                        break;
                }
                
                drag_mode = value;
            }
        }
        public int Linking_ID { get; set; } = 0;
        public bool IO_Active { get; set; } = false;
        //UI elements that can't be added in the XAML
        public Canvas_Class Sub_Canvas { get; set; }
        public Rectangle BackGround_Rect { get; set; }

        //code when the program loads up
        public MainWindow()
        {            
            InitializeComponent();         
        }

        private void Canvas_Border_Loaded(object sender, RoutedEventArgs e)
        {            
            Sub_Canvas = new Canvas_Class(this);
            Canvas_Border.Child = Sub_Canvas;

            BackGround_Rect = new Rectangle { Height = 4000, Width = 4000, Fill = Brushes.White };
            Sub_Canvas.Children.Add(BackGround_Rect);
            Canvas.SetLeft(BackGround_Rect, -1000);
            Canvas.SetTop(BackGround_Rect, -1000);
        }

        //eventhandler that aren't linked to the canvas
        //gate button event
        private void Rect_Button_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if(!Drag&&!Link)
            {
                Drag_Mode = Drag_State.Main_Can;
                Gate_List.Add(new Gate_Class(Convert.ToString((sender as Rectangle).Tag),this,Sub_Canvas.Scale_Factor));
                Drag_Num = Gate_List.Count()-1;
            }
        }
        //Whole event for dragging objects in the whole window
        private void Window_MouseMove(object sender, MouseEventArgs e)
        {

            if(Drag)
            {
                switch (drag_mode)
                {
                    case Drag_State.Main_Can:
                        Gate_List[Drag_Num].Rect_Move(Mouse.GetPosition(Main_Grid));
                        break;
                    case Drag_State.Sub_Can:
                        Gate_List[Drag_Num].Rect_Move(Mouse.GetPosition(Sub_Canvas));
                        Gate_List[Drag_Num].Move_IO();
                        break;
                    case Drag_State.Link_Mode_Sub:
                        Line_List[Drag_Num].Track_Mouse();
                        break;
                }
            }
        }
        //event for cancling dragging in whole window
        private void Main_Canvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {      
            if (Drag&&!Link)
            {
                Point Pos_Rect = Mouse.GetPosition(BackGround_Rect);
                Point Pos_Border = Mouse.GetPosition(Canvas_Border);
                Point Pos_Sub = Mouse.GetPosition(Sub_Canvas);

                //checks if it's in side the border and if it's inside the area that is allowed in the border.
                bool check = (Pos_Rect.X < 0 || Pos_Rect.X > 4000 || Pos_Rect.Y < 0 || Pos_Rect.Y > 4000) ||
                   (Pos_Border.X < 0 || Pos_Border.X > Canvas_Border.ActualWidth || Pos_Border.Y < 0 || Pos_Border.Y > Canvas_Border.ActualHeight)
                   || (Sub_Canvas.Rect_detection(Gate_List[Drag_Num].Rect.Width, Gate_List[Drag_Num].Rect.Height, Drag_Num) != -1) ? false : true;


                //if (Sub_Canvas.Rect_detection(Gate_List[Drag_Num].Rect.Width, Gate_List[Drag_Num].Rect.Height, Drag_Num) != -1) check = false;
                


                Main_Canvas.Children.Remove(Gate_List[Drag_Num].Rect);                
                Drag_Mode = Drag_State.Null;                
                if (check)
                {
                    Gate_List[Drag_Num].Rect.Width = Gate_List[Drag_Num].Rect.Width / Sub_Canvas.Scale_Factor;
                    Gate_List[Drag_Num].Rect.Height = Gate_List[Drag_Num].Rect.Height / Sub_Canvas.Scale_Factor;
                    Sub_Canvas.Children.Add(Gate_List[Drag_Num].Rect);
                    Gate_List[Drag_Num].Rect_Move(Pos_Sub);
                }
                else
                {
                    Gate_List.RemoveAt(Drag_Num);
                }

            }
        }
        //Flip between the 2 states of the program
        private void Link_Button_Click(object sender, RoutedEventArgs e)
        {
            if (!Drag)
            {
                if (Link)
                {
                    Link = false;
                }
                else
                {
                    Link = true;
                }
            }
        }

        private void Input_Output_Button_Click(object sender, RoutedEventArgs e)
        {
            if (IO_Active)
            {
                IO_Active = false;
            }
            else
            {
                IO_Active = true;
                for (int i = 0; i < Gate_List.Count; i++)
                {
                    if (Gate_List[i].Alive)
                    {
                        if (Gate_List[i].Input[0].Input_Type == IO_Type.Null)
                        {
                            Input_Assignment(i,0);
                        }
                        if (Gate_List[i].Input[1].Input_Type == IO_Type.Null && Gate_List[i].Type != 2 && Gate_List[i].Type != 7)
                        {
                            Input_Assignment(i,1);
                        }


                        if (Gate_List[i].Output[0].Output_Type == IO_Type.Null)
                        {
                            Output_Assignment(i, 0);
                        }
                        if (Gate_List[i].Output[1].Output_Type == IO_Type.Null && Gate_List[i].Type == 7)
                        {
                            Output_Assignment(i, 1);
                        }
                        if (Gate_List[i].Output[2].Output_Type == IO_Type.Null && Gate_List[i].Type == 7)
                        {
                            Output_Assignment(i, 2);
                        }
                    }
                }
            }
        }

        public void Input_Assignment(int i,int Port)
        {
            Input_Button_List.Add(new Input_Button(this, i, Port));
            Input_Button_List.Last().Aline_Box(Gate_List[i]);
            Gate_List[i].Input[Port].Input_Type = IO_Type.IO;
            Gate_List[i].Input[Port].Input_ID = Input_Button_List.Count - 1;
        }

        public void Output_Assignment(int i, int Port)
        {
            Output_Circle_List.Add(new Output_Circle(this, i, Port));
            Output_Circle_List.Last().Aline_Circle(Gate_List[i]);
            Gate_List[i].Output[Port].Output_Type = IO_Type.IO;
            Gate_List[i].Output[Port].Output_ID = Output_Circle_List.Count - 1;
        }




        public double[] Link_Input_Aline(Gate_Class Gate, int Input_Num)
        {
            //UI_Line.Stroke = Brushes.Black;
            //not input is in the center of the gate compare to the other gates
            if (Gate.Type == 2)
            {
                return new double[] {Canvas.GetLeft(Gate.Rect) + 5, Canvas.GetTop(Gate.Rect) + 38 };
            }
            //this is similar to the not gate but because it's a sqaure not a rectangle it needed to be moved in the X axis more
            else if (Gate.Type == 7)
            {
                return new double[] { Canvas.GetLeft(Gate.Rect) + 12.5, Canvas.GetTop(Gate.Rect) + 38 };
            }
            //the rest all follow the setup for the and gate
            else
            {
                if (Input_Num == 0)
                {
                    return new double[] { Canvas.GetLeft(Gate.Rect), Canvas.GetTop(Gate.Rect) + 15 };
                }
                //else if not needed here but Input_ID isn't a secure variable type.
                else if (Input_Num == 1)
                {
                    return new double[] { Canvas.GetLeft(Gate.Rect), Canvas.GetTop(Gate.Rect) + 62 };
                }
            }
            return new double[] { -1, -1 };

        }

        public double [] Link_Output_Aline(Gate_Class Gate,int Output_Num)
        {
            //special gate class with 3 exit
            if (Gate.Type == 7)
            {
                if (Output_Num == 0)
                {
                    return new double[] { Canvas.GetLeft(Gate.Rect) + 75, Canvas.GetTop(Gate.Rect) + 23.8 };
                }
                else if (Output_Num == 1)
                {
                    return new double[] { Canvas.GetLeft(Gate.Rect) + 75, Canvas.GetTop(Gate.Rect) + 36 };
                }
                else if (Output_Num == 2)
                {
                    return new double[] { Canvas.GetLeft(Gate.Rect) + 75, Canvas.GetTop(Gate.Rect) + 51 };
                }
            }
            //not gate
            else if (Gate.Type == 2)
            {
                return new double[] { Canvas.GetLeft(Gate.Rect) + 109.5, Canvas.GetTop(Gate.Rect) + 36 };
            }
            //every other gate
            else
            {
                return new double[] { Canvas.GetLeft(Gate.Rect) + 115, Canvas.GetTop(Gate.Rect) + 35.7 };
            }
            return new double[] { -1,-1};
        }

        private void Delay_Lable_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if(Delay_Lable.Text=="")
                {
                    Delay_Intervals = 0;
                    Delay_Lable.Text = "0";
                }
                Delay_Intervals = Convert.ToInt32(Delay_Lable.Text);
            }
            catch
            {
                Delay_Lable.Text = Convert.ToString(Delay_Intervals);
            }
        }



        /// <summary>
        ///  This is the main part of the logical part of the program. It's also the simulator that runs with
        ///  multithreading.
        /// </summary>



        private void Run_Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void System_Clean_Up(object sender, RoutedEventArgs e)
        {
            int i=0;
            Progress_Bar_Window New_Window = new Progress_Bar_Window(9999, i);
            Thread Test_Thread = new Thread(() => test_method(New_Window,this));
            Test_Thread.Start();
            Canvas.SetLeft(New_Window, Width / 2);
            Canvas.SetTop(New_Window, Height / 2);
            New_Window.ShowDialog();
            

        }
        
        private void test_method(Progress_Bar_Window _New_Window,MainWindow _MainWind)
        {
            List<Gate_Class> _Gate_List = _MainWind.Gate_List;
            for (int i = 0; i < 10000; i++)
            {




                _New_Window.Dispatcher.Invoke(() =>
                {
                    _New_Window.Bar.Value = i;
                });
            }
            _New_Window.Dispatcher.Invoke(() =>
            {
                _New_Window.Close();
            });
            _MainWind.Dispatcher.Invoke(() =>
            {
                _MainWind.Gate_List = _Gate_List;
            });
        }
    }
}
