using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows;
using System.Collections.Generic;
using System;
using System.Windows.Threading;
using System.Windows.Input;

namespace A_level_course_work_Logic_Gate
{    
    public abstract class Gate_Class
    {
        public Gate_Type Type { get; set; }
        public Rectangle Rect { get; set; }
        public bool Alive { get; set; } = true;

        //data storages for the input and output

        public Input_Class[] Input = new Input_Class[] { new Input_Class(), new Input_Class() };
        List<Output_Circle> _Output_Circle_List { get; set; }
        List<Line_Class> _Line_List { get; set; }
        List<Input_Button> _Input_Button_List { get; set; }


        /// <summary>
        /// depending on what type of output it is it will update and change the corrosonding values.
        /// </summary>
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

        protected void Part_Constructor(Canvas Main_Canvas)
        {
            Main_Canvas.Children.Add(Rect);
            Point Pos = Mouse.GetPosition(Main_Canvas);
            Rect_Move(Pos);
        }
        //overridable method
        public abstract void Gate_Output_Calc();

        //change Rectangle location.
        public void Rect_Move(Point Pos)
        {
            Canvas.SetLeft(Rect, Pos.X);
            Canvas.SetTop(Rect, Pos.Y);
        }

        public void Border_Change()
        {
            if (Rect.Stroke == Brushes.Red)
            {
                Rect.Stroke = Brushes.Black;
            }
            else if (Rect.Stroke == Brushes.Black)
            {
                Rect.Stroke = Brushes.Red;
            }
        }
        /// <summary>
        /// One method that can be called to update everything linked to the gate.
        /// Most Ideal world would to have an event handler when this gate class is moved in the canvas for this
        /// method to fire.
        /// </summary>
        public void Move_IO()
        {
            for (int i = 0; i < 3; i++)
            {
                if(Output[i].Output_Type == IO_Type.Gate)
                {
                    _Line_List[Output[i].Line_ID].Link_Output_Align_Line(this);
                }
                else if(Output[i].Output_Type == IO_Type.IO)
                {
                    _Output_Circle_List[Output[i].Output_ID].Align_Circle(this);
                }
            }
            for (int i = 0; i < 2; i++)
            {
                if (Input[i].Input_Type==IO_Type.Gate)
                {
                    _Line_List[Input[i].Line_ID].Link_Input_Align_Line(this);
                }
                else if(Input[i].Input_Type == IO_Type.IO)
                {
                    _Input_Button_List[Input[i].Input_ID].Align_Box(this);
                }
            }
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
