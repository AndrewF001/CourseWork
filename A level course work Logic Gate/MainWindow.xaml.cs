using A_level_course_work_Logic_Gate.File_Classes;
using A_level_course_work_Logic_Gate.SubGate_Classes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
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
    public enum Detection_State { Null, Detected }

    public partial class MainWindow : Window, Canvas_Variables
    {
        //varaibles that need to be accessed all around the code
        public List<Gate_Class> Gate_List { get; set; } = new List<Gate_Class>();
        public List<Line_Class> Line_List { get; set; } = new List<Line_Class>();
        //make these custom classes
        public List<Input_Button> Input_Button_List { get; set; } = new List<Input_Button>();
        public List<Output_Circle> Output_Circle_List { get; set; } = new List<Output_Circle>();
        private File_Creation_Class File_Worker { get; set; }

        //program info
        public bool Drag { get; set; } = false;
        public int Delay_Intervals { get; set; } = 1;
        public int Drag_Num { get; set; } = 0;
        private bool _link = false;
        public bool Sim_Running { get; set; } = false;
        public bool Saved { get; set; } = false;
        public string File_Location { get; set; } = "";
        public bool Link
        {
            get { return _link; }
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
        {
            get { return drag_mode; }
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

        //Threads that do the work simulatnously with the UI element
        private BackgroundWorker _worker = new BackgroundWorker();
        private BackgroundWorker Simulator_Worker = new BackgroundWorker();

        Progress_Bar_Window Progress_Window = new Progress_Bar_Window(0);

        public MainWindow()
        {            
            InitializeComponent();
            _worker.DoWork += WorkerDoWork;
            _worker.RunWorkerCompleted += WorkerRunWorkerCompleted;

            Simulator_Worker.DoWork += Simulator_Work;
            File_Worker = new File_Creation_Class(this);
        }

        //Need the UI to load before adding the canvas as it's added in the CS instead of the XAML       
        private void Canvas_Border_Loaded(object sender, RoutedEventArgs e)
        {
            Sub_Canvas = new Canvas_Class(this, ref Canvas_Border, this);
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
        /// <New Rect spawner>
        /// When you press the rect box on the left hand side the sender(The rect) will hold it's tag for which type
        /// of gate it is. Then Adds that rectangle to the list
        /// </summary>
        private void Rect_Button_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!Drag && !Link)
            {
                Drag_Mode = Drag_State.Main_Can;

                switch ((sender as Rectangle).Tag)
                {
                    case "And":
                        Gate_List.Add(new And_Gate_Class(Main_Canvas, Sub_Canvas.Scale_Factor, Output_Circle_List, Line_List, Input_Button_List));
                        break;
                    case "Nand":
                        Gate_List.Add(new Nand_Gate_Class(Main_Canvas, Sub_Canvas.Scale_Factor, Output_Circle_List, Line_List, Input_Button_List));
                        break;
                    case "Not":
                        Gate_List.Add(new Not_Gate_Class(Main_Canvas, Sub_Canvas.Scale_Factor, Output_Circle_List, Line_List, Input_Button_List));
                        break;
                    case "Or":
                        Gate_List.Add(new Or_Gate_Class(Main_Canvas, Sub_Canvas.Scale_Factor, Output_Circle_List, Line_List, Input_Button_List));
                        break;
                    case "Nor":
                        Gate_List.Add(new Nor_Gate_Class(Main_Canvas, Sub_Canvas.Scale_Factor, Output_Circle_List, Line_List, Input_Button_List));
                        break;
                    case "Xor":
                        Gate_List.Add(new Xor_Gate_Class(Main_Canvas, Sub_Canvas.Scale_Factor, Output_Circle_List, Line_List, Input_Button_List));
                        break;
                    case "Xnor":
                        Gate_List.Add(new Xnor_Gate_Class(Main_Canvas, Sub_Canvas.Scale_Factor, Output_Circle_List, Line_List, Input_Button_List));
                        break;
                    case "Transformer":
                        Gate_List.Add(new Transformer_Class(Main_Canvas, Sub_Canvas.Scale_Factor, Output_Circle_List, Line_List, Input_Button_List));
                        break;
                }

                Drag_Num = Gate_List.Count() - 1;

            }
        }
        /// <Mouse move event>
        ///  So depending on what Drag state the program is in it will respond with the right method.
        /// </summary>

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
        /// <rectangle added to subcanvas>
        /// first check is to make sure it's inside the subcanvas/background_rect border.
        /// then added it to the subcanvas and set the variables.
        /// </summary>
        public void Add_Rect_Sub_FIX_BUG()
        {
            Point Pos_Rect = Mouse.GetPosition(BackGround_Rect);
            Point Pos_Border = Mouse.GetPosition(Canvas_Border);
            Point Pos_Sub = Mouse.GetPosition(Sub_Canvas);

            (Detection_State State, int ThrowAway) = Sub_Canvas.Rect_detection(Gate_List[Drag_Num].Rect.Width, Gate_List[Drag_Num].Rect.Height, Drag_Num);

            //checks if it's in side the border and if it's inside the area that is allowed in the border.
            bool check = (Pos_Rect.X < 0 || Pos_Rect.X > 4000 || Pos_Rect.Y < 0 || Pos_Rect.Y > 4000) ||
               (Pos_Border.X < 0 || Pos_Border.X > Canvas_Border.ActualWidth || Pos_Border.Y < 0 || Pos_Border.Y > Canvas_Border.ActualHeight)
               || (State == Detection_State.Detected) ? false : true;

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
                Link = !Link;
            }
        }
        /// <this is for the button that adds and remove the input buttons>
        /// It's just a switch if statement as each time it's pressed it will go between the 2 if statements
        /// The first if statement removes the current IO buttons from the canvas.
        /// </summary>
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
                        Output_Circle_List[i].Remove_UI();
                    }
                }
            }
            else
            {
                Add_IO_Method();
            }
        }


        /// <Adding IO buttons>
        /// This is it's own method as when you load a file this part of the method needs to be called to add the gates.
        /// the first part is just checking already existing gates in the input_button_List and output_Circle_List
        /// and adding them to the canvas.
        /// The second part is just find out which gate on the screen still doesn't have any IO and adds it.
        /// </summary>
        private void Add_IO_Method()
        {
            IO_Active = true;
            Add_Existing_IO();

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

        private void Add_Existing_IO()
        {
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
                    Output_Circle_List[i].Add_UI();
                    Output_Circle_List[i].Aline_Circle(Gate_List[Output_Circle_List[i].Output_ID]);
                }
            }
        }
        /// <summary>
        /// Adds a new input_Button to input_Button_List and sets the value of the gate to fit the new values it should have.
        /// </summary>
        /// <param name="i"></just the position in the list, Gate Num>
        /// <param name="Port"></which input slot it needs to be allocated it>
        public void Input_Assignment(int i, int Port)
        {
            Input_Button_List.Add(new Input_Button(i, Port, Gate_List, Sub_Canvas, this));
            //can't be in constructor because UI_Elemtent isn't loaded until the constructor finished.
            Input_Button_List.Last().Aline_Box(Gate_List[i]);
            Gate_List[i].Input[Port].Input_Type = IO_Type.IO;
            Gate_List[i].Input[Port].Input_ID = Input_Button_List.Count - 1;
        }
        //Same as input(Above)
        public void Output_Assignment(int i, int Port)
        {
            Output_Circle_List.Add(new Output_Circle(i, Port, Sub_Canvas, this));
            Output_Circle_List.Last().Aline_Circle(Gate_List[i]);
            Gate_List[i].Output[Port].Output_Type = IO_Type.IO;
            Gate_List[i].Output[Port].Output_ID = Output_Circle_List.Count - 1;
        }


        /// <summary>
        /// just returns the alinement value + the gate it's attached to coords.
        /// </summary>
        /// <param name="Gate"></just gives easy access to work with the gate it's being linked to>
        /// <param name="Input_Num"></it's just the input port number.>
        public double[] Link_Input_Aline(Gate_Class Gate, int Input_Num)
        {
            //not gate, input is in the center of the gate compare to the other gates whic has 2 on the side
            if (Gate.Type == 2)
            {
                return new double[] { Canvas.GetLeft(Gate.Rect) + 5, Canvas.GetTop(Gate.Rect) + 38 };
            }
            //this is similar to the not gate but because it's a sqaure not a rectangle it needed to be moved in the X axis more
            else if (Gate.Type == 7)
            {
                return new double[] { Canvas.GetLeft(Gate.Rect) + 12.5, Canvas.GetTop(Gate.Rect) + 38 };
            }
            //the rest all follow the same setup as the and gate
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
        //Same as input
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
        /// <summary>
        /// Just make sure that the input is a valid input, wont allow any string otherwise it will just go back to
        /// the previous acceptable input.
        /// If left blank it will just equal 0.
        /// </summary>
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
            if (Sim_Running)
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

        /// <summary>
        /// This is the work that the simulator is on.
        /// first part is just finding all the starting points.(input_Buttons)
        /// it then removes any dupucating starting points.
        /// enter while loop where the condition can be broken inside or outside the loop.
        /// just goes through each of the active gates, and calculate the new output whilst changing it's border colour.
        /// adds the next gate to next_Gate list. At the end it 
        /// </summary>
        private async void Simulator_Work(object sender, DoWorkEventArgs e)
        {
            bool Finished = false;

            List<int> Active_Gates = Set_Up_Start_Sim();

            while (!Finished && Sim_Running)
            {
                List<int> Next_Gate = new List<int>();

                for (int i = 0; i < Active_Gates.Count; i++)
                {
                    Gate_List[Active_Gates[i]].Gate_Output_Calc();
                    await Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => Gate_List[Active_Gates[i]].Rect.Stroke = Brushes.Red));
                    for (int x = 0; x < 3; x++)
                    {
                        if (Gate_List[Active_Gates[i]].Output[x].Output_Type == IO_Type.Gate)
                        {
                            await Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => Line_List[Gate_List[Active_Gates[i]].Output[x].Line_ID].Change_UI_Red()));
                            Gate_List[Line_List[Gate_List[Active_Gates[i]].Output[x].Line_ID].Input_ID].Input[Line_List[Gate_List[Active_Gates[i]].Output[x].Line_ID].Input_Num].Input_bit = Gate_List[Active_Gates[i]].Gate_Bit;
                            Next_Gate.Add(Line_List[Gate_List[Active_Gates[i]].Output[x].Line_ID].Input_ID);
                        }
                    }
                }

                Next_Gate = Remove_Duplicats(Next_Gate);

                await Task.Delay(Delay_Intervals);

                Set_Input_Back(Active_Gates);

                Active_Gates = Next_Gate.ToList();
                Next_Gate.RemoveRange(0, Next_Gate.Count);
                if (Active_Gates.Count == 0)
                {
                    Finished = true;
                }
            }
            Sim_Running = false;
            await Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => Run_Button.Content = "Run"));
        }

        private List<int> Set_Up_Start_Sim()
        {
            List<int> Active_Gates = new List<int>();
            for (int i = 0; i < Input_Button_List.Count; i++)
            {
                if (Gate_List[Input_Button_List[i].Input_ID].Alive)
                {
                    Active_Gates.Add(Input_Button_List[i].Input_ID);
                }
            }
            for (int i = 0; i < Active_Gates.Count - 1; i++)
            {
                bool OverLap = false;
                for (int x = i + 1; x < Active_Gates.Count; x++)
                {
                    if (Active_Gates[x] == Active_Gates[i])
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
            return Active_Gates;
        }

        private List<int> Remove_Duplicats(List<int> Next_Gate)
        {
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
            return Next_Gate;
        }
        private async void Set_Input_Back(List<int> Active_Gates)
        {
            for (int i = 0; i < Active_Gates.Count; i++)
            {
                await Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => Gate_List[Active_Gates[i]].Rect.Stroke = Brushes.Black));
                for (int x = 0; x < 3; x++)
                {
                    if (Gate_List[Active_Gates[i]].Output[x].Output_Type == IO_Type.Gate)
                    {
                        await Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => Line_List[Gate_List[Active_Gates[i]].Output[x].Line_ID].Change_UI_Black()));
                    }
                }
            }
        }
        private void System_Clean_Up(object sender, RoutedEventArgs e)
        {
            Clean_Up_Method();
        }
        /// <summary>
        /// Creates a new window UI that hold the progress bar.
        /// Starts the background worker to clean up the system.
        /// The MainWindow is holded untill the system clean up is complete.
        /// </summary>
        public void Clean_Up_Method()
        {
            Progress_Window = new Progress_Bar_Window(Gate_List.Count());
            _worker.RunWorkerAsync();
            Progress_Window.ShowDialog();
        }
        /// <summary>
        /// goes through all the list in the program and removes unused objects and resorts the lists to work with the change.
        /// </summary>
        private void WorkerDoWork(object sender, DoWorkEventArgs e)
        {
            //remove unused input and output
            Remove_Unsed_Output();
            Remove_Unsed_Input();
            Remove_Unsed_Line();
            Remove_Unsed_Gate();
        }

        private void Remove_Unsed_Output()
        {
            for (int i = 0; i < Output_Circle_List.Count; i++)
            {
                if (Gate_List[Output_Circle_List[i].Output_ID].Output[Output_Circle_List[i].Output_Port].Output_Type == IO_Type.Gate || Gate_List[Output_Circle_List[i].Output_ID].Alive == false)
                {
                    for (int x = 0; x < Gate_List.Count; x++)
                    {
                        if (Gate_List[x].Output[Output_Circle_List[i].Output_Port].Output_ID > i && Gate_List[x].Output[Output_Circle_List[i].Output_Port].Output_Type == IO_Type.IO)
                        {
                            Gate_List[x].Output[Output_Circle_List[i].Output_Port].Output_ID -= 1;
                        }
                    }
                    Output_Circle_List.RemoveAt(i);
                    i -= 1;
                }
            }
        }
        private void Remove_Unsed_Input()
        {
            for (int i = 0; i < Input_Button_List.Count; i++)
            {
                if (!Gate_List[Input_Button_List[i].Input_ID].Alive)
                {
                    for (int x = 0; x < Gate_List.Count; x++)
                    {
                        if (Gate_List[x].Input[Input_Button_List[i].Input_Port].Input_ID > i && Gate_List[x].Input[Input_Button_List[i].Input_Port].Input_Type == IO_Type.IO)
                        {
                            Gate_List[x].Input[Input_Button_List[i].Input_Port].Input_ID -= 1;
                        }
                    }
                    Input_Button_List.RemoveAt(i);
                    i -= 1;
                }
            }
        }
        private void Remove_Unsed_Line()
        {
            for (int i = 0; i < Line_List.Count; i++)
            {
                if (Gate_List[Line_List[i].Input_ID].Input[Line_List[i].Input_Num].Input_Type != IO_Type.Gate || Gate_List[Line_List[i].Input_ID].Alive == false)
                {
                    for (int x = 0; x < Gate_List.Count; x++)
                    {
                        for (int y = 0; y < 3; y++)
                        {
                            if (Gate_List[x].Output[y].Line_ID > i)
                            {
                                Gate_List[x].Output[y].Line_ID -= 1;
                            }
                        }
                        for (int z = 0; z < 2; z++)
                        {
                            if (Gate_List[x].Input[z].Line_ID > i)
                            {
                                Gate_List[x].Input[z].Line_ID -= 1;
                            }
                        }
                    }
                    Line_List.RemoveAt(i);
                    i -= 1;
                }
            }
        }

        private void Remove_Unsed_Gate()
        {
            for (int i = 0; i < Gate_List.Count; i++)
            {
                if(!Gate_List[i].Alive)
                {
                    for (int x = 0; x < Gate_List.Count; x++)
                    {
                        for (int z = 0; z < 3; z++)
                        {
                            if (Gate_List[x].Output[z].Output_Type==IO_Type.Gate && Gate_List[x].Output[z].Output_ID>i)
                            {
                                Gate_List[x].Output[z].Output_ID -= 1;
                            }
                        }
                        for (int z = 0; z < 2; z++)
                        {
                            if (Gate_List[x].Input[z].Input_Type == IO_Type.Gate && Gate_List[x].Input[z].Input_ID > i)
                            {
                                Gate_List[x].Input[z].Input_ID -= 1;
                            }
                        }                        
                    }
                    for (int z = 0; z < Input_Button_List.Count; z++)
                    {
                        if(Input_Button_List[z].Input_ID>i)
                        {
                            Input_Button_List[z].Input_ID -= 1;
                        }
                    }
                    for (int z = 0; z < Output_Circle_List.Count; z++)
                    {
                        if(Output_Circle_List[z].Output_ID >i)
                        {
                            Output_Circle_List[z].Output_ID -= 1;
                        }
                    }
                    for (int z = 0; z < Line_List.Count; z++)
                    {
                        if(Line_List[z].Output_ID>i)
                        {
                            Line_List[z].Output_ID -= 1;
                        }
                        if(Line_List[z].Input_ID>i)
                        {
                            Line_List[z].Input_ID -= 1;
                        }
                    }
                    Gate_List.RemoveAt(i);
                    i -= 1;
                }
            }
        }


        //private void Remove_Unsed_Gate()
        //{
        //    for (int i = 0; i < Gate_List.Count; i++)
        //    {
        //        if (!Gate_List[i].Alive)
        //        {
        //            Gate_List.RemoveAt(i);
        //            i -= 1;
        //            for (int x = 0; x < Gate_List.Count; x++)
        //            {
        //                ShiftGate(i, Gate_List[x]);
        //            }
        //        }
        //        Dispatcher.Invoke(() => { Progress_Window.Value = i; });
        //    }
        //}
        //private void ShiftGate(int ID, Gate_Class Gate)
        //{
        //    for (int i = 0; i < 2; i++)
        //    {
        //        if (Gate.Input[i].Input_ID == ID)
        //        {
        //            Gate.Input[i].Input_ID = 0;
        //            Gate.Input[i].Input_Type = IO_Type.Null;
        //            Gate.Input[i].Input_bit = false;
        //        }
        //        else if (Gate.Input[i].Input_ID > ID)
        //        {
        //            Gate.Input[i].Input_ID -= 1;
        //        }
        //    }
        //    for (int i = 0; i < 3; i++)
        //    {
        //        if (Gate.Output[i].Output_ID == ID)
        //        {
        //            Gate.Output[i].Output_ID = 0;
        //            Gate.Output[i].Output_Type = IO_Type.Null;
        //        }
        //        else if (Gate.Output[i].Output_ID > ID)
        //        {
        //            Gate.Output[i].Output_ID -= 1;
        //        }
        //    }
        //}
        //Resuesmes the mainwindow.
        private void WorkerRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Progress_Window.Close();
        }
        private void MenuItem_Load_Click(object sender, RoutedEventArgs e)
        {
            File_Worker.MenuItem_Load_Click_Method();
        }
        private void MenuItem_SaveAs_Click(object sender, RoutedEventArgs e)
        {
            File_Worker.MenuItem_SaveAs_Click_Method();
        }

        private void MenuItem_Save_Click(object sender, RoutedEventArgs e)
        {
            File_Worker.MenuItem_Save_Click_Method();
        }
        private void MenuItem_New_Click(object sender, RoutedEventArgs e)
        {
            File_Worker.MenuItem_New_Click_Method();
        }
        public void Reset_Program()
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
            Sub_Canvas = new Canvas_Class(this, ref Canvas_Border, this);
            SetUp_Canvas();

        }

        private void MenuItem_Exit_Click(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }
    }
}
