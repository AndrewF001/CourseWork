﻿using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows;
using System.Windows.Input;
using System.Collections.Generic;
using System;

namespace A_level_course_work_Logic_Gate
{
    public class Gate_Class
    {
        //public int ID { get; set; }
        public int Type { get; set; }
        public Rectangle Rect { get; set; }
        public bool Alive { get; set; } = true;
        public MainWindow _MainWind { get; set; }

        //data storages for the input and output

        public Input_Class[] Input { get; set; } = new Input_Class[] { new Input_Class(), new Input_Class() }; 

        //Gate output Bit
        public bool Gate_Bit { get; set; } = false;

        public Output_Class[] Output { get; set; } = new Output_Class[] { new Output_Class(), new Output_Class(), new Output_Class() };



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


        public void Move_IO()
        {
            for (int i = 0; i < 3; i++)
            {
                if(Output[i].Output_Type == IO_Type.Gate)
                {
                    _MainWind.Line_List[Output[i].Line_ID].Link_Output_Aline_Line(this);
                }
                else if(Output[i].Output_Type == IO_Type.IO)
                {
                    _MainWind.Output_Circle_List[Output[i].Output_ID].Aline_Circle(this);
                }
            }
            for (int i = 0; i < 2; i++)
            {
                if (Input[i].Input_Type==IO_Type.Gate)
                {
                    _MainWind.Line_List[Input[i].Line_ID].Link_Input_Aline_Line(this);
                }
                else if(Input[i].Input_Type == IO_Type.IO)
                {
                    _MainWind.Input_Button_List[Input[i].Input_ID].Aline_Box(this);
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
    }
}