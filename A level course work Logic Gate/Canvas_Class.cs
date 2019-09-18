using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace A_level_course_work_Logic_Gate
{
    public class Canvas_Class : Canvas
    {
        public Point Old_Pos { get; set; } = new Point();
        public double Scale_Factor { get; set; } = 1;
        public Point Old_Rect { get; set; } = new Point();

        public MainWindow _MainWind { get; set; }

        public TranslateTransform Translate_Action { get; set; }
        public ScaleTransform Scale_Action { get; set; }

        public TransformGroup Transforms_Group { get; set; }


        public bool MovingCanvas { get; set; } = false;

        public Canvas_Class(MainWindow MainWind)
        {
            _MainWind = MainWind;
            Background = Brushes.Gray;

            Translate_Action = new TranslateTransform(0, 0);
            Scale_Action = new ScaleTransform(1, 1, 0, 0);
            Transforms_Group = new TransformGroup();

            Transforms_Group.Children.Add(Translate_Action);
            Transforms_Group.Children.Add(Scale_Action);

            this.RenderTransform = Transforms_Group;
        }
        //Move canvas event
        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (MovingCanvas)
            {
                Translate_Action.X += (e.GetPosition(this).X - Old_Pos.X);
                Translate_Action.Y += (e.GetPosition(this).Y - Old_Pos.Y);
            }
            Old_Pos = e.GetPosition(this);



            //Point Pos = Mouse.GetPosition(_MainWind.Sub_Canvas);
            //if (e.LeftButton == MouseButtonState.Pressed && !_MainWind.Drag)
            //{
            //    double X_Difference = Pos.X - Old_Pos.X;
            //    double Y_Difference = Pos.Y - Old_Pos.Y;
            //    Console.WriteLine(X_Difference + " : " + Y_Difference);
            //    Sub_Canvas_Translation(X_Difference, Y_Difference);
            //}
            //Old_Pos = Pos;

        }

        private void Sub_Canvas_Translation(double X_Difference, double Y_Difference)
        {

            ////TranslateTransform translateTransform = new TranslateTransform(X_Difference/2, Y_Difference/2);
            ////_MainWind.Sub_Canvas.RenderTransform = translateTransform;

            //Point Pos = Mouse.GetPosition(_MainWind.Sub_Canvas);
            //ScaleTransform scaleTransform = new ScaleTransform(Scale_Factor, Scale_Factor, Pos.X + X_Difference, Pos.Y + Y_Difference);
            //_MainWind.Sub_Canvas.RenderTransform = scaleTransform;


            //SetLeft(_MainWind.Sub_Canvas, GetLeft(_MainWind.Sub_Canvas) + X_Difference);
            //SetTop(_MainWind.Sub_Canvas, GetTop(_MainWind.Sub_Canvas) + Y_Difference);
            //Console.WriteLine(GetLeft(_MainWind.Sub_Canvas) + " : " + GetTop(_MainWind.Sub_Canvas));

            //Canvas.SetLeft(_MainWind.BackGround_Rect, Canvas.GetLeft(_MainWind.BackGround_Rect) + X_Difference);
            //Canvas.SetTop(_MainWind.BackGround_Rect, Canvas.GetTop(_MainWind.BackGround_Rect) + Y_Difference);
            //for (int i = 0; i < _MainWind.Gate_List.Count; i++)
            //{                
            //    Rectangle Rect = _MainWind.Gate_List[i].Rect;
            //    Canvas.SetLeft(Rect, Canvas.GetLeft(Rect) + X_Difference);
            //    Canvas.SetTop(Rect, Canvas.GetTop(Rect) + Y_Difference);
            //}
        }
        //zoom event
        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            bool change = false;
            Point Pos = Mouse.GetPosition(_MainWind.Canvas_Border);
            if (e.Delta > 0)
            {
                if (Scale_Factor != 0.0625)
                {
                    Scale_Factor -= 0.0625;
                    change = true;
                }
            }
            else if (e.Delta < 0)
            {
                if (Scale_Factor != 2)
                {
                    Scale_Factor += 0.0625;
                    change = true;
                }
            }

            if (change)
            {                
                Scale_Action.ScaleX = Scale_Factor;
                Scale_Action.ScaleY = Scale_Factor;
                Scale_Action.CenterX = Pos.X;
                Scale_Action.CenterY = Pos.Y;
            }
        }
        //Re-drag pick up
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {   
            int detected = Rect_detection(0, 0, -1);

            //if(!_mainwind.drag) then switch

            if(!_MainWind.Drag&&detected==-1)
            {
                Old_Pos = e.GetPosition(this);
                MovingCanvas = true;
            }

            if (!_MainWind.Drag && !_MainWind.Link)
            {
                if (detected != -1)
                {
                    _MainWind.Drag_Num = detected;
                    _MainWind.Drag_Mode = Drag_State.Sub_Can;
                    Old_Rect = new Point(Convert.ToDouble(Canvas.GetLeft(_MainWind.Gate_List[_MainWind.Drag_Num].Rect)), Convert.ToDouble(Canvas.GetTop(_MainWind.Gate_List[_MainWind.Drag_Num].Rect)));
                }
            }
            else if (!_MainWind.Drag && _MainWind.Link && detected!=-1)
            {                
                _MainWind.Linking_ID = detected;
                _MainWind.Drag_Mode = Drag_State.Link_Mode_Sub;
                _MainWind.Drag_Num = _MainWind.Line_List.Count();
                _MainWind.Line_List.Add(new Line_Class(detected,this));
                _MainWind.Line_List.Last().Track_Mouse();
                int X = Link_Output_Vaildation(detected);
                //if X == -2 then there was no aviable output for the new link to be made(already deleted the last line)
                if (X != -2)
                    Link_Output_Aline(detected, _MainWind.Drag_Num, X);
                
            }
        }
        //Re-drag drop
        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            MovingCanvas = false;
            if (_MainWind.Drag && !_MainWind.Link)
            {
                _MainWind.Drag_Mode = Drag_State.Null;
                if(Rect_detection(_MainWind.Gate_List[_MainWind.Drag_Num].Rect.Width, _MainWind.Gate_List[_MainWind.Drag_Num].Rect.Height, _MainWind.Drag_Num) !=-1)
                {
                    SetLeft(_MainWind.Gate_List[_MainWind.Drag_Num].Rect, Old_Rect.X);
                    SetTop(_MainWind.Gate_List[_MainWind.Drag_Num].Rect,Old_Rect.Y);
                }
            }
            else if(_MainWind.Drag && _MainWind.Link)
            {
                int X = -1;
                int detection = Rect_detection(0, 0, -1);
                _MainWind.Drag_Mode = Drag_State.Null;
                if (detection != -1 && detection != _MainWind.Linking_ID)
                {
                    //location
                    //_MainWind.Line_List[_MainWind.Drag_Num].Track_Mouse();
                    X = Link_Input_Vaildation(detection);
                    //_MainWind.Line_List[_MainWind.Drag_Num].UI_Line.Stroke = Brushes.Black;
                }
                
                if(X!=-1)
                {
                    Link_Input_Aline(detection, X);
                    //Children.Remove(_MainWind.Line_List[_MainWind.Drag_Num].UI_Line);
                    //_MainWind.Line_List.RemoveAt(_MainWind.Drag_Num);
                }
                else
                {
                    Children.Remove(_MainWind.Line_List[_MainWind.Drag_Num].UI_Line);
                    _MainWind.Line_List.RemoveAt(_MainWind.Drag_Num);
                }
            }
        }

        public int Rect_detection(double Width, double Height,int Drag_Num)
        {
            int Detected = -1;
            Point Pos_Sub = Mouse.GetPosition(this);
            for (int i = 0; i < _MainWind.Gate_List.Count(); i++)
            {
                Gate_Class Rect = _MainWind.Gate_List[i];
                double Rect_X = Canvas.GetLeft(_MainWind.Gate_List[i].Rect);
                double Rect_Y = Canvas.GetTop(_MainWind.Gate_List[i].Rect);
                if (Pos_Sub.X+Width > Rect_X && Pos_Sub.X < Rect_X + Rect.Rect.Width && Pos_Sub.Y+Height > Rect_Y && Pos_Sub.Y < Rect_Y + Rect.Rect.Height&&i!=Drag_Num)
                    Detected = i;
            }
            return Detected;
        }

        public int Link_Output_Vaildation(int Clicked)
        {
            int Output = -1;
            Point Pos = Mouse.GetPosition(this);
            if (_MainWind.Gate_List[Clicked].Type == 7)
            {
                //because type 7 has 3 output I made it so that the output you chose is based off the mouse postion
                //so the first if statement are the range check
                if(Pos.Y<GetTop(_MainWind.Gate_List[Clicked].Rect)+29)
                {
                    //it will set it to it's output number
                    Output = 0;
                    //however if it is already taken then it will overright it and make it a fall number
                    if(_MainWind.Gate_List[Clicked].Output_ID[0]!=-1)
                        Output = -2;                 
                }
                else if(Pos.Y> GetTop(_MainWind.Gate_List[Clicked].Rect) + 29 && Pos.Y < GetTop(_MainWind.Gate_List[Clicked].Rect) + 43)
                {
                    Output = 1;
                    if (_MainWind.Gate_List[Clicked].Output_ID[1] != -1)
                         Output = -2;
                }
                else if(Pos.Y > GetTop(_MainWind.Gate_List[Clicked].Rect) + 43)
                {
                    Output = 2;
                    if (_MainWind.Gate_List[Clicked].Output_ID[2] != -1)
                        Output = -2;
                }
                //if the above failed then it will enter this if statement
                if (Output == -2)
                {
                    //this will check each output and try and find a available output                    
                    if(_MainWind.Gate_List[Clicked].Output_ID[0] == -1)
                        Output = 0;
                    else if (_MainWind.Gate_List[Clicked].Output_ID[1] == -1)
                        Output = 1;
                    else if (_MainWind.Gate_List[Clicked].Output_ID[2] == -1)
                        Output = 2;
                }
            }
            else
            {
                if(_MainWind.Gate_List[Clicked].Output_ID[0]!=-1)
                    Output = -2;
            }

            if(Output==-2)
            {
                Children.Remove(_MainWind.Line_List[_MainWind.Drag_Num].UI_Line);
                _MainWind.Line_List.RemoveAt(_MainWind.Drag_Num);
            }
            return Output;
        }

        //click is the ID of the gate that has been clicked on, ID is the ID of the line in the list, output if for the special gate and is used when moving a gate and the line needs to follow it
        public void Link_Output_Aline(int Clicked, int ID,int Output)
        {
            //special gate class with 3 exit
            if(_MainWind.Gate_List[Clicked].Type==7)
            {
                if (Output==0)
                {
                    _MainWind.Line_List[ID].Change_X1_Y1(Canvas.GetLeft(_MainWind.Gate_List[Clicked].Rect) + 75, Canvas.GetTop(_MainWind.Gate_List[Clicked].Rect) + 23.8);
                }
                else if (Output == 1)
                {
                    _MainWind.Line_List[ID].Change_X1_Y1(Canvas.GetLeft(_MainWind.Gate_List[Clicked].Rect) + 75, Canvas.GetTop(_MainWind.Gate_List[Clicked].Rect) + 36);
                }
                else if (Output == 2)
                {
                    _MainWind.Line_List[ID].Change_X1_Y1(Canvas.GetLeft(_MainWind.Gate_List[Clicked].Rect) + 75, Canvas.GetTop(_MainWind.Gate_List[Clicked].Rect) + 51);
                }
            }
            //not gate
            else if(_MainWind.Gate_List[Clicked].Type == 2)
            {
                _MainWind.Line_List[ID].Change_X1_Y1(Canvas.GetLeft(_MainWind.Gate_List[Clicked].Rect) + 109.5, Canvas.GetTop(_MainWind.Gate_List[Clicked].Rect) + 36);
            }
            //every other gate
            else
            {
                _MainWind.Line_List[ID].Change_X1_Y1(Canvas.GetLeft(_MainWind.Gate_List[Clicked].Rect) + 115, Canvas.GetTop(_MainWind.Gate_List[Clicked].Rect) + 35.7);
            }
        }

        public int Link_Input_Vaildation(int Clicked)
        {
            int Output = -1;
            Point Pos = Mouse.GetPosition(this);
            if(_MainWind.Gate_List[Clicked].Type==2|| _MainWind.Gate_List[Clicked].Type == 7)
            {
                if(_MainWind.Gate_List[Clicked].Input[0].Input_ID==-1)
                {
                    Output = 0;
                }
            }
            else
            {
                if(Pos.Y < GetTop(_MainWind.Gate_List[Clicked].Rect) + 37)
                {
                    if(_MainWind.Gate_List[Clicked].Input[0].Input_ID==-1)
                    {
                        Output = 0;
                    }
                    else if(_MainWind.Gate_List[Clicked].Input[1].Input_ID == -1)
                    {
                        Output = 1;
                    }
                }
                else
                {
                    if (_MainWind.Gate_List[Clicked].Input[1].Input_ID == -1)
                    {
                        Output = 1;
                    }
                    else if (_MainWind.Gate_List[Clicked].Input[0].Input_ID == -1)
                    {
                        Output = 0;
                    }
                }
            }
            if(Output==-1)
            {
                Children.Remove(_MainWind.Line_List[_MainWind.Drag_Num].UI_Line);
                _MainWind.Line_List.RemoveAt(_MainWind.Drag_Num);
            }
            return Output;
        }

        public void Link_Input_Aline(int Clicked, int Input_ID)
        {            
            _MainWind.Line_List[_MainWind.Drag_Num].UI_Line.Stroke = Brushes.Black;
            //not input is in the center of the gate compare to the other gates
            if (_MainWind.Gate_List[Clicked].Type==2)
            {
                _MainWind.Line_List[_MainWind.Drag_Num].Change_X2_Y2(GetLeft(_MainWind.Gate_List[Clicked].Rect)+5, GetTop(_MainWind.Gate_List[Clicked].Rect) + 38);
            }
            //this is similar to the not gate but because it's a sqaure not a rectangle it needed to be moved in the X axis more
            else if(_MainWind.Gate_List[Clicked].Type == 7)
            {
                _MainWind.Line_List[_MainWind.Drag_Num].Change_X2_Y2(GetLeft(_MainWind.Gate_List[Clicked].Rect) + 12.5, GetTop(_MainWind.Gate_List[Clicked].Rect) + 38);
            }
            //the rest all follow the setup for the and gate
            else
            {
                if(Input_ID==0)
                {
                    _MainWind.Line_List[_MainWind.Drag_Num].Change_X2_Y2(GetLeft(_MainWind.Gate_List[Clicked].Rect), GetTop(_MainWind.Gate_List[Clicked].Rect) + 15);
                }
                //else if not needed here but Input_ID isn't a secure variable type.
                else if(Input_ID==1)
                {
                    _MainWind.Line_List[_MainWind.Drag_Num].Change_X2_Y2(GetLeft(_MainWind.Gate_List[Clicked].Rect), GetTop(_MainWind.Gate_List[Clicked].Rect) + 62);
                }
            }

        }

    }
}
