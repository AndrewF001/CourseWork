using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows;
using System.Windows.Input;
using System.Collections.Generic;
using System;
using System.Windows.Threading;

namespace A_level_course_work_Logic_Gate
{    
    public abstract class Gate_Class
    {
        public int Type { get; set; }
        public Rectangle Rect { get; set; }
        public bool Alive { get; set; } = true;
        //protected MainWindow _MainWind { get; set; }

        //data storages for the input and output

        public Input_Class[] Input = new Input_Class[] { new Input_Class(), new Input_Class() };
        List<Output_Circle> _Output_Circle_List { get; set; }
        List<Line_Class> _Line_List { get; set; }
        List<Input_Button> _Input_Button_List { get; set; }


        //Gate output Bit
        private bool _Gate_Bit;
        public  bool Gate_Bit {
            get
            { return _Gate_Bit; }
             set 
            {
                _Gate_Bit = value;
                for (int i = 0; i < 3; i++)
                {
                    if(Output[i].Output_Type==IO_Type.IO)
                    {
                        if (_Gate_Bit == true)
                        {
                            _Output_Circle_List[Output[i].Output_ID].Bit = true;
                        }
                        else
                        {
                            _Output_Circle_List[Output[i].Output_ID].Bit = false;
                        }
                    }
                    else if(Output[i].Output_Type == IO_Type.Gate)
                    {
                        if(_Gate_Bit==true)
                        {
                            Set_Label_1(i);
                        }
                        else
                        {
                            Set_Label_0(i); 
                        }
                    }
                }
            } 
        }

        public Output_Class[] Output { get; set; } = new Output_Class[] { new Output_Class(), new Output_Class(), new Output_Class() };



        public Gate_Class(List<Output_Circle> Output_Circle_List,List<Line_Class> Line_List,List<Input_Button> Input_Button_List)
        {
            _Output_Circle_List = Output_Circle_List;
            _Line_List = Line_List;
            _Input_Button_List = Input_Button_List;
            Gate_Output_Calc();
        }

        public abstract void Gate_Output_Calc();


        //Basically the constructor
        //public void Setup(string _Tag,double _Scale_Factor)
        //{
        //    //scale factor
        //    Rect = new Rectangle { Height = 75*_Scale_Factor, Width = 115*_Scale_Factor, Stroke = Brushes.Black, Fill = Application.Current.Resources[_Tag] as Brush };
        //    //Calc Tag
        //    switch(_Tag)
        //    {
        //        case ("And_Gate_L"):
        //            Type = 0;
        //            break;
        //        case ("Nand_Gate_L"):
        //            Type = 1;
        //            break;
        //        case ("Not_Gate_L"):
        //            Type = 2;
        //            break;
        //        case ("Or_Gate_L"):
        //            Type = 3;
        //            break;
        //        case ("Nor_Gate_L"):
        //            Type = 4;
        //            break;
        //        case ("Xor_Gate_L"):
        //            Type = 5;
        //            break;
        //        case ("Xnor_Gate_L"):
        //            Type = 6;
        //            break;
        //        case ("Transformer"):
        //            Type = 7;
        //            //scale factor
        //            Rect.Width = 85 * _Scale_Factor ;
        //            break;
        //    }
        //}

        //change Rectangle location.
        public void Rect_Move(Point Pos)
        {
            Canvas.SetLeft(Rect, Pos.X);
            Canvas.SetTop(Rect, Pos.Y);
        }

        public void Border_Change()
        {
            if (Rect.Stroke == Brushes.Red)
                Rect.Stroke = Brushes.Black;
            else if(Rect.Stroke == Brushes.Black)
                Rect.Stroke = Brushes.Red;
        }

        public void Move_IO()
        {
            for (int i = 0; i < 3; i++)
            {
                if(Output[i].Output_Type == IO_Type.Gate)
                {
                    _Line_List[Output[i].Line_ID].Link_Output_Aline_Line(this);
                }
                else if(Output[i].Output_Type == IO_Type.IO)
                {
                    _Output_Circle_List[Output[i].Output_ID].Aline_Circle(this);
                }
            }
            for (int i = 0; i < 2; i++)
            {
                if (Input[i].Input_Type==IO_Type.Gate)
                {
                    _Line_List[Input[i].Line_ID].Link_Input_Aline_Line(this);
                }
                else if(Input[i].Input_Type == IO_Type.IO)
                {
                    _Input_Button_List[Input[i].Input_ID].Aline_Box(this);
                }
            }
        }

        //delete
        public void Output_Rect_Status(int ID)
        {
            Console.WriteLine("Rect ID : {0}\nType : {1}\nAlive : {2}\nGate Bit : {3}", ID,Type,Alive,Gate_Bit);
            Console.WriteLine("\n       Input 0");
            Input[0].Output_Status();
            Console.WriteLine("\n       Input 1");
            Input[1].Output_Status();
            Console.WriteLine("\n       Output 0");
            Output[0].Output_Status();
            Console.WriteLine("\n       Output 1");
            Output[1].Output_Status();
            Console.WriteLine("\n       Output 2");
            Output[2].Output_Status();
            Console.WriteLine("\n\n\n\n");
        }

        public async void Set_Label_0(int i)
        {
            await Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => _Line_List[Output[i].Line_ID].Line_Lable.Content = "0"));
        }
        public async void Set_Label_1(int i)
        {
            await Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => _Line_List[Output[i].Line_ID].Line_Lable.Content = "1"));
        }
    }
}
