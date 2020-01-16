using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;

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
        private bool Sim_Running { get; set; } = false;
        private bool Saved { get; set; } = false;
        private string File_Location { get; set; } = "";
        public bool Link
        { get { return _link; }
            set
            {
                _link = value;
                if (_link)
                {
                    Link_Button.Content = "Link";
                }
                else if (!_link)
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
                switch (value)
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




        private BackgroundWorker _worker = new BackgroundWorker();
        private BackgroundWorker Simulator_Worker = new BackgroundWorker();

        Progress_Bar_Window Progress_Window = new Progress_Bar_Window();

        //code when the program loads up
        public MainWindow()
        {
            InitializeComponent();
            _worker.DoWork += WorkerDoWork;
            _worker.RunWorkerCompleted += WorkerRunWorkerCompleted;

            Simulator_Worker.DoWork += Simulator_Work;
            //Simulator_Worker.RunWorkerCompleted += Simulator_Terminated;

        }




        private void Canvas_Border_Loaded(object sender, RoutedEventArgs e)
        {
            Sub_Canvas = new Canvas_Class(this);
            SetUp_Canvas();
        }
        public void SetUp_Canvas()
        {
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
            if (!Drag && !Link)
            {
                Drag_Mode = Drag_State.Main_Can;
                //Gate_List.Add(new Gate_Class(Convert.ToString((sender as Rectangle).Tag), this, Sub_Canvas.Scale_Factor));
                
                switch((sender as Rectangle).Tag)
                {
                    case "And":
                        Gate_List.Add(new And_Gate_Class(this,Sub_Canvas.Scale_Factor));
                        break;
                    case "Nand":
                        Gate_List.Add(new Nand_Gate_Class(this, Sub_Canvas.Scale_Factor));
                        break;
                    case "Not":
                        Gate_List.Add(new Not_Gate_Class(this, Sub_Canvas.Scale_Factor));
                        break;
                    case "Or":
                        Gate_List.Add(new Or_Gate_Class(this, Sub_Canvas.Scale_Factor));
                        break;
                    case "Nor":
                        Gate_List.Add(new Nor_Gate_Class(this, Sub_Canvas.Scale_Factor));
                        break;
                    case "Xor":
                        Gate_List.Add(new Xor_Gate_Class(this, Sub_Canvas.Scale_Factor));
                        break;
                    case "Xnor":
                        Gate_List.Add(new Xnor_Gate_Class(this, Sub_Canvas.Scale_Factor));
                        break;
                    case "Transformer":
                        Gate_List.Add(new Transformer_Class(this, Sub_Canvas.Scale_Factor));
                        break;
                }

                Drag_Num = Gate_List.Count() - 1;

            }
        }
        //Whole event for dragging objects in the whole window
        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            if (Drag)
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
            if (Drag && !Link)
            {
                Add_Rect_Sub_FIX_BUG();
            }
        }

        public void Add_Rect_Sub_FIX_BUG()
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
                for (int i = 0; i < Input_Button_List.Count; i++)
                {
                    if (Gate_List[Input_Button_List[i].Input_ID].Input[Input_Button_List[i].Input_Port].Input_Type == IO_Type.IO)
                    {
                        Gate_List[Input_Button_List[i].Input_ID].Input[Input_Button_List[i].Input_Port].Input_Type = IO_Type.Null;
                        Sub_Canvas.Children.Remove(Input_Button_List[i]);
                    }
                }
                for (int i = 0; i < Output_Circle_List.Count; i++)
                {
                    if (Gate_List[Output_Circle_List[i].Output_ID].Output[Output_Circle_List[i].Output_Port].Output_Type == IO_Type.IO)
                    {
                        Gate_List[Output_Circle_List[i].Output_ID].Output[Output_Circle_List[i].Output_Port].Output_Type = IO_Type.Null;
                        Sub_Canvas.Children.Remove(Output_Circle_List[i].Circle);
                    }
                }
            }
            else
            {
                Add_IO_Method();
            }
        }

        private void Add_IO_Method()
        {
            IO_Active = true;
            for (int i = 0; i < Input_Button_List.Count; i++)
            {
                int ID = Input_Button_List[i].Input_ID;
                if (Gate_List[ID].Input[Input_Button_List[i].Input_Port].Input_Type == IO_Type.Null && Gate_List[ID].Alive)
                {
                    Gate_List[ID].Input[Input_Button_List[i].Input_Port].Input_Type = IO_Type.IO;
                    Gate_List[ID].Input[Input_Button_List[i].Input_Port].Input_ID = i;
                    Gate_List[ID].Input[Input_Button_List[i].Input_Port].Input_bit = Input_Button_List[i].Bit;
                    Sub_Canvas.Children.Add(Input_Button_List[i]);
                    Input_Button_List[i].Aline_Box(Gate_List[Input_Button_List[i].Input_ID]);
                }

            }
            for (int i = 0; i < Output_Circle_List.Count; i++)
            {
                int ID = Output_Circle_List[i].Output_ID;
                if (Gate_List[ID].Output[Output_Circle_List[i].Output_Port].Output_Type == IO_Type.Null && Gate_List[ID].Alive)
                {
                    Gate_List[ID].Output[Output_Circle_List[i].Output_Port].Output_Type = IO_Type.IO;
                    Gate_List[ID].Output[Output_Circle_List[i].Output_Port].Output_ID = i;
                    Sub_Canvas.Children.Add(Output_Circle_List[i].Circle);
                    Output_Circle_List[i].Aline_Circle(Gate_List[Output_Circle_List[i].Output_ID]);
                }
            }

            for (int i = 0; i < Gate_List.Count; i++)
            {
                if (Gate_List[i].Alive)
                {
                    if (Gate_List[i].Input[0].Input_Type == IO_Type.Null)
                    {
                        Input_Assignment(i, 0);
                    }
                    if (Gate_List[i].Input[1].Input_Type == IO_Type.Null && Gate_List[i].Type != 2 && Gate_List[i].Type != 7)
                    {
                        Input_Assignment(i, 1);
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
                    Gate_List[i].Gate_Output_Calc();
                }
            }
        }

        public void Input_Assignment(int i, int Port)
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
                return new double[] { Canvas.GetLeft(Gate.Rect) + 5, Canvas.GetTop(Gate.Rect) + 38 };
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

        public double[] Link_Output_Aline(Gate_Class Gate, int Output_Num)
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
            return new double[] { -1, -1 };
        }

        private void Delay_Lable_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (Delay_Lable.Text == "")
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
            if(Sim_Running)
            {
                Sim_Running = false;
                Run_Button.Content = "Run";

            }
            else
            {
                Sim_Running = true;
                Run_Button.Content = "Stop";
                Clean_Up_Method();
                Add_IO_Method();
                Simulator_Worker.RunWorkerAsync();
            }
        }


        private async void Simulator_Work(object sender, DoWorkEventArgs e)
        {
            bool Finished = false;

            List<int> Active_Gates = new List<int>();

            // need to make a list of all active Nodes. So at the start I need a loop to find all the active start nodes.
            for (int i = 0; i < Input_Button_List.Count; i++)
            {
                if (Gate_List[Input_Button_List[i].Input_ID].Alive)
                {
                    Active_Gates.Add(Input_Button_List[i].Input_ID);
                }
            }
            for (int i = 0; i < Active_Gates.Count-1; i++)
            {
                bool OverLap = false;
                for (int x = i+1; x < Active_Gates.Count; x++)
                {
                    if(Active_Gates[x]==Active_Gates[i])
                    {
                        OverLap = true;
                    }
                }
                if (OverLap)
                {
                    Active_Gates.RemoveAt(i);
                    i -= 1;
                }
            }
            //make the rect border red, work out output bit, Change output circle or line to the colour it is.
            //work out the new list
            //display the change on the output?
            

            while(!Finished && Sim_Running)
            {
                List<int> Next_Gate = new List<int>();

                for (int i = 0; i < Active_Gates.Count; i++)
                {
                    Gate_List[Active_Gates[i]].Gate_Output_Calc();
                    await Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => Gate_List[Active_Gates[i]].Rect.Stroke = Brushes.Red));
                    for (int x = 0; x < 3; x++)
                    {
                        if(Gate_List[Active_Gates[i]].Output[x].Output_Type==IO_Type.Gate)
                        {
                            await Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => Line_List[Gate_List[Active_Gates[i]].Output[x].Line_ID].UI_Line.Stroke = Brushes.Red));
                            await Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => Line_List[Gate_List[Active_Gates[i]].Output[x].Line_ID].Line_Lable.Foreground = Brushes.Red));
                            Gate_List[Line_List[Gate_List[Active_Gates[i]].Output[x].Line_ID].Input_ID].Input[Line_List[Gate_List[Active_Gates[i]].Output[x].Line_ID].Input_Num].Input_bit = Gate_List[Active_Gates[i]].Gate_Bit;
                            Next_Gate.Add(Line_List[Gate_List[Active_Gates[i]].Output[x].Line_ID].Input_ID);
                        }
                    }
                }
                for (int i = 0; i < Next_Gate.Count - 1; i++)
                {
                    bool OverLap = false;
                    for (int x = i + 1; x < Next_Gate.Count; x++)
                    {
                        if (Next_Gate[x] == Next_Gate[i])
                        {
                            OverLap = true;
                        }
                    }
                    if (OverLap)
                    {
                        Next_Gate.RemoveAt(i);
                        i -= 1;
                    }
                }

                await Task.Delay(Delay_Intervals);

                for (int i = 0; i < Active_Gates.Count; i++)
                {
                    await Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => Gate_List[Active_Gates[i]].Rect.Stroke = Brushes.Black));
                    for (int x = 0; x < 3; x++)
                    {
                        if (Gate_List[Active_Gates[i]].Output[x].Output_Type == IO_Type.Gate)
                        {
                            await Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => Line_List[Gate_List[Active_Gates[i]].Output[x].Line_ID].UI_Line.Stroke = Brushes.Black));
                            await Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => Line_List[Gate_List[Active_Gates[i]].Output[x].Line_ID].Line_Lable.Foreground = Brushes.Black));
                        }
                    }
                }

                Active_Gates = Next_Gate.ToList();
                Next_Gate.RemoveRange(0, Next_Gate.Count);
                if(Active_Gates.Count==0)
                {
                    Finished = true;
                }
                              
            }
            Sim_Running = false;
            await Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background,new Action(() => Run_Button.Content = "Run"));
        }

//        private void Simulator_Terminated(object sender, RunWorkerCompletedEventArgs e)
//        {
//            Sim_Running = false;
//            Application.Current.Dispatcher.BeginInvoke(
//DispatcherPriority.Background,
//new Action(() => Run_Button.Content = "Run"));
//        }



        private void System_Clean_Up(object sender, RoutedEventArgs e)
        {
            Clean_Up_Method();
        }

        public void Clean_Up_Method()
        {
            Progress_Window = new Progress_Bar_Window();
            Progress_Window.Bar.Minimum = 0;
            Progress_Window.Bar.Value = 0;
            Progress_Window.Bar.Maximum = Gate_List.Count();
            _worker.RunWorkerAsync();
            Progress_Window.ShowDialog();
        }

        private void WorkerDoWork(object sender, DoWorkEventArgs e)
        {
            //remove unused input and output
            for (int i = 0; i < Output_Circle_List.Count; i++)
            {
                if (Gate_List[Output_Circle_List[i].Output_ID].Output[Output_Circle_List[i].Output_Port].Output_Type == IO_Type.Gate || Gate_List[Output_Circle_List[i].Output_ID].Alive == false)
                {                    
                    Output_Circle_List.RemoveAt(i);
                    i -= 1;
                }
            }
            for (int i = 0; i < Input_Button_List.Count; i++)
            {
                if(!Gate_List[Input_Button_List[i].Input_ID].Alive)
                {
                    Input_Button_List.RemoveAt(i);
                    i -= 1;
                }
            }
            //for (int i = 0; i < Line_List.Count; i++)
            //{
            //    if (Gate_List[Line_List[i].Input_ID].Input[Line_List[i].Input_Num].Input_Type != IO_Type.Gate || Gate_List[Line_List[i].Input_ID].Alive == false)
            //    {
            //        Line_List.RemoveAt(i);
            //        i -= 1;
            //    }
            //    for (int x = 0; x < Gate_List.Count; x++)
            //    {
            //        for (int y = 0; y < 3; y++)
            //        {
            //            if (Gate_List[x].Output[y].Output_ID > i)
            //            {
            //                Gate_List[x].Output[y].Output_ID -= 1;
            //            }
            //        }
            //        for (int z = 0; z < 2; z++)
            //        {
            //            if (Gate_List[x].Input[z].Input_ID > i)
            //            {
            //                Gate_List[x].Input[z].Input_ID -= 1;
            //            }
            //        }

            //    }
            //}
            //removes dead gates
            for (int i = 0; i < Gate_List.Count; i++)
            {                
                if (!Gate_List[i].Alive)
                {
                    Gate_List.RemoveAt(i);
                    i -= 1;
                    for (int x = 0; x < Gate_List.Count; x++)
                    {
                        ShiftGate(i, Gate_List[x]);
                    }
                }
                Dispatcher.Invoke(() => { Progress_Window.Value = i; });
            }
        }

        private void ShiftGate (int ID, Gate_Class Gate)
        {
            for (int i = 0; i < 2; i++)
            {
                if (Gate.Input[i].Input_ID==ID)
                {
                    Gate.Input[i].Input_ID = 0;
                    Gate.Input[i].Input_Type = IO_Type.Null;
                    Gate.Input[i].Input_bit = false;
                }
                else if(Gate.Input[i].Input_ID > ID)
                {
                    Gate.Input[i].Input_ID -= 1;
                }
            }            
            for (int i = 0; i < 3; i++)
            {
                if (Gate.Output[i].Output_ID == ID)
                {
                    Gate.Output[i].Output_ID = 0;
                    Gate.Output[i].Output_Type = IO_Type.Null;                    
                }
                else if (Gate.Output[i].Output_ID > ID)
                {
                    Gate.Output[i].Output_ID -= 1;
                }
            }
        }

        private void WorkerRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Progress_Window.Close();
        }

        [Serializable]
        public class File_Class
        {
            public List<File_Version_Gate> Gates { get; set; } = new List<File_Version_Gate>();
            public List<File_Version_Line> Lines { get; set; } = new List<File_Version_Line>();
            public List<File_Version_Input> Inputs { get; set; } = new List<File_Version_Input>();
            public List<File_Version_Output> Output { get; set; } = new List<File_Version_Output>();
                 
        }

        private void MenuItem_Load_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                openFileDialog.InitialDirectory = @"C:\Documents";
                File_Location = openFileDialog.FileName;
                Stream stream = new FileStream(File_Location, FileMode.Open, FileAccess.Read);
                IFormatter formatter = new BinaryFormatter();
                File_Class Loaded_File = (File_Class)formatter.Deserialize(stream);
            }
        }

        private void MenuItem_SaveAs_Click(object sender, RoutedEventArgs e)
        {
            Clean_Up_Method();
            File_Class Save = File_Creation();               
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            if (saveFileDialog.ShowDialog() == true)
            {
                saveFileDialog.InitialDirectory = @"C:\Documents";
                File_Location = saveFileDialog.FileName;
                Save_File(Save);
            }
        }
        public File_Class File_Creation()
        {
            File_Class Save = new File_Class();
            for (int i = 0; i < Gate_List.Count(); i++)
            {
                Save.Gates.Add(new File_Version_Gate(Gate_List[i].Type, Gate_List[i].Alive, Gate_List[i].Input, Gate_List[i].Gate_Bit, Gate_List[i].Output));
            }
            for (int i = 0; i < Line_List.Count; i++)
            {
                Save.Lines.Add(new File_Version_Line(Convert.ToString(Line_List[i].Line_Lable.Content), Line_List[i].Output_ID, Line_List[i].Output_Num, Line_List[i].Input_ID, Line_List[i].Input_Num, Line_List[i].X1, Line_List[i].X2, Line_List[i].Y1, Line_List[i].Y2));
            }
            for (int i = 0; i < Input_Button_List.Count; i++)
            {
                Save.Inputs.Add(new File_Version_Input(Input_Button_List[i].Bit, Input_Button_List[i].Input_ID, Input_Button_List[i].Input_Port));
            }
            for (int i = 0; i < Output_Circle_List.Count; i++)
            {
                Save.Output.Add(new File_Version_Output(Output_Circle_List[i].Bit, Output_Circle_List[i].Output_ID, Output_Circle_List[i].Output_Port));
            }
            return Save;
        }

        private void MenuItem_Save_Click(object sender, RoutedEventArgs e)
        {
            File_Class Save = File_Creation();
            Save_File(Save);
        }
        private void Save_File(File_Class Save)
        {
            Stream stream = new FileStream(File_Location, FileMode.Create, FileAccess.Write);
            IFormatter formatter = new BinaryFormatter();
            formatter.Serialize(stream, Save);
            stream.Close();
        }
        [Serializable]
        public class File_Version_Gate
        {
            public File_Version_Gate(int _Type, bool _Alive,Input_Class[] _Input, bool Gate_Bit, Output_Class[] _Output )
            {
                Type = _Type;
                Alive = _Alive;                
                _Gate_Bit = Gate_Bit;
                for (int i = 0; i < 2; i++)
                {
                    Input[i] = new File_Version_GI(_Input[i].Input_bit, _Input[i].Input_ID, _Input[i].Input_Type, _Input[i].Line_ID);
                }
                for (int i = 0; i < 3; i++)
                {
                    Output[i] = new File_Version_GO(_Output[i].Output_ID, _Output[i].Output_Type, _Output[i].Line_ID);
                }
            }

            public int Type { get; set; }
            public bool Alive { get; set; } = true;
            public File_Version_GI[] Input { get; set; } = new File_Version_GI[2];
            public bool _Gate_Bit;          
            public File_Version_GO[] Output { get; set; } = new File_Version_GO[3];
        }
        [Serializable]
        public class File_Version_Line
        {
            public File_Version_Line(string Bit, int O_ID, int O_Num, int I_ID, int I_Num, double _X1, double _X2, double _Y1, double _Y2)
            {
                Content_Copy = Bit;
                Output_ID = O_ID;
                Output_Num = O_Num;
                Input_ID = I_ID;
                Input_Num = I_Num;
                X1 = _X1;
                X2 = _X2;
                Y1 = _Y1;
                Y2 = _Y2;
            }
            public string Content_Copy { get; set; }
            public int Output_ID { get; set; }
            public int Output_Num { get; set; }
            public int Input_ID { get; set; }
            public int Input_Num { get; set; }
            public double X1, Y1, X2, Y2;
        }
        [Serializable]
        public class File_Version_Input
        {
            public File_Version_Input(bool Bit, int I_ID, int I_Port)
            {
                _Bit = Bit;
                Input_ID = I_ID;
                Input_Port = I_Port;
            }
            public bool _Bit = false;
            public int Input_ID { get; }
            public int Input_Port;
        }
        [Serializable]
        public class File_Version_Output
        {
            public File_Version_Output(bool Bit, int O_ID, int O_Port)
            {
                _Bit = Bit;
                Output_ID = O_ID;
                Output_Port = O_Port;
            }
            public bool _Bit = false;
            public int Output_ID { get; }
            public int Output_Port;
        }
        [Serializable]
        public class File_Version_GO
        {
            public File_Version_GO(int O_ID, IO_Type O_Type, int L_I)
            {
                _output_ID = O_ID;
                _output_Type = O_Type;
                _line_ID = L_I;
            }
            private int _output_ID = -1;
            private IO_Type _output_Type = IO_Type.Null;
            private int _line_ID = -1;
        }
        [Serializable]
        public class File_Version_GI
        {
            public File_Version_GI(bool I_B,int I_ID, IO_Type I_Type, int L_I)
            {
                _Input_Bit = I_B;
                _Input_ID = I_ID;
                _Input_Type = I_Type;
                _line_ID = L_I;
            }
            private bool _Input_Bit;
            private int _Input_ID = -1;
            private IO_Type _Input_Type = IO_Type.Null;
            private int _line_ID = -1;
        }

        private void MenuItem_New_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult messageBoxResult = MessageBox.Show("Are you sure you want to remove everything? Nothing will be kept!", "New Window", MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                Gate_List = new List<Gate_Class>();
                Line_List = new List<Line_Class>();
                Input_Button_List = new List<Input_Button>();
                Output_Circle_List = new List<Output_Circle>();
                Drag = false;
                Delay_Intervals = 1;
                Drag_Num = 0;
                _link = false;
                Sim_Running = false;
                Saved = false;
                File_Location = "";
                drag_mode = Drag_State.Null;
                Linking_ID = 0;
                IO_Active = false;
                Sub_Canvas = new Canvas_Class(this);
                SetUp_Canvas();
            }
        }
    }
}
