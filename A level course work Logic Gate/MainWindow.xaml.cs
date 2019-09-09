﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace A_level_course_work_Logic_Gate
{
    public enum Drag_State  : byte { Null,Main_Can,Sub_Can,Link_Mode_Sub}
    

    public partial class MainWindow : Window
    {
        //varaibles that need to be accessed all around the code
        public List<Gate_Class> Gate_List { get; set; }
        public List<Line_Class> Line_List { get; set; }

        //program info
        public bool Drag { get; set; } = false;
        public int Drag_Num { get; set; } = 0;
        public bool Link { get; set; } = false;
        private Drag_State drag_mode = Drag_State.Null;
        public Drag_State Drag_Mode
        { get { return drag_mode; }
            set
            {
                if(value==Drag_State.Null)
                {
                    Drag = false;
                }
                else if(value==Drag_State.Sub_Can || value == Drag_State.Main_Can||value ==Drag_State.Link_Mode_Sub)
                {
                    Drag = true;
                }
                drag_mode = value;
            }
        }
        public int Linking_ID { get; set; } = 0;
        //UI elements that can't be added in the XAML
        public Canvas_Class Sub_Canvas { get; set; }
        public Rectangle BackGround_Rect { get; set; }

        //code when the program loads up
        public MainWindow()
        {            
            InitializeComponent();
            Gate_List = new List<Gate_Class>();
            Line_List = new List<Line_Class>();            
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
            if(Drag&&Drag_Mode==Drag_State.Main_Can)
            {
                Gate_List[Drag_Num].Rect_Move(Mouse.GetPosition(Main_Grid));
            }
            else if(Drag&& Drag_Mode == Drag_State.Sub_Can)
            {
                Gate_List[Drag_Num].Rect_Move(Mouse.GetPosition(Sub_Canvas));
            }
            else if(Drag&& Drag_Mode == Drag_State.Link_Mode_Sub)
            {
                Line_List[Drag_Num].Track_Mouse();
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
                   (Pos_Border.X < 0 || Pos_Border.X > Canvas_Border.ActualWidth || Pos_Border.Y < 0 || Pos_Border.Y > Canvas_Border.ActualHeight) ? false : true;


                if (Sub_Canvas.Rect_detection(Gate_List[Drag_Num].Rect.Width, Gate_List[Drag_Num].Rect.Height, Drag_Num) != -1) check = false;
                


                Main_Canvas.Children.Remove(Gate_List[Drag_Num].Rect);                
                Drag_Mode = Drag_State.Null;
                if (check)
                {
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
                    Link_Button.Content = "Drag";
                }
                else
                {
                    Link = true;
                    Link_Button.Content = "Link";
                }
            }
            Console.WriteLine(Link);
        }
    }
}
