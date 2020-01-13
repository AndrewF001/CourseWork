using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

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

            RenderTransform = Transforms_Group;
        }
        //Move canvas event
        protected override void OnMouseMove(MouseEventArgs e)
        {
            int hold = Rect_detection(0, 0, -1);
            if (hold != _MainWind.Last_ID_For_Rect && hold !=-1)
            {
                _MainWind.Last_ID_For_Rect = hold;
                _MainWind.Gate_List[hold].Output_Rect_Status(hold);
            }


            if (MovingCanvas)
            {
                Translate_Action.X += (e.GetPosition(this).X - Old_Pos.X);
                Translate_Action.Y += (e.GetPosition(this).Y - Old_Pos.Y);
            }
            Old_Pos = e.GetPosition(this);
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
                _MainWind.Drag_Num = _MainWind.Line_List.Count();
                _MainWind.Line_List.Add(new Line_Class(detected,_MainWind));
                _MainWind.Line_List.Last().Track_Mouse();
                _MainWind.Line_List.Last().Input_ID=detected;
                _MainWind.Line_List.Last().Output_Num = Link_Output_Vaildation(detected);
                //if X == -2 then there was no aviable output for the new link to be made(already deleted the last line)
                if (_MainWind.Line_List.Last().Output_Num != -2)
                {
                    _MainWind.Drag_Mode = Drag_State.Link_Mode_Sub;
                    _MainWind.Gate_List[detected].Output[_MainWind.Line_List.Last().Output_Num].Line_ID = _MainWind.Drag_Num;
                    _MainWind.Line_List[_MainWind.Drag_Num].Link_Output_Aline_Line(_MainWind.Gate_List[detected]);
                }
                else
                {
                    Children.Remove(_MainWind.Line_List[_MainWind.Drag_Num].UI_Line);
                    _MainWind.Line_List.RemoveAt(_MainWind.Drag_Num);
                }

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
                    _MainWind.Gate_List[_MainWind.Drag_Num].Move_IO();
                }
            }
            else if(_MainWind.Drag && _MainWind.Link)
            {
                int X = -1;
                int detection = Rect_detection(0, 0, -1);
                _MainWind.Drag_Mode = Drag_State.Null;
                if (detection != -1 && detection != _MainWind.Linking_ID)
                {
                    X = Link_Input_Vaildation(detection);
                }

                if (X!=-1)
                {

                    _MainWind.Line_List[_MainWind.Drag_Num].Input_ID = detection;
                    _MainWind.Line_List[_MainWind.Drag_Num].Input_Num = X;
                    _MainWind.Line_List[_MainWind.Drag_Num].Link_Input_Aline_Line(_MainWind.Gate_List[detection]);
                    _MainWind.Gate_List[detection].Input[X].Input_Type = IO_Type.Gate;
                    _MainWind.Gate_List[detection].Input[X].Input_ID = _MainWind.Linking_ID;
                    _MainWind.Gate_List[detection].Input[X].Line_ID = _MainWind.Drag_Num;
                    _MainWind.Gate_List[_MainWind.Linking_ID].Output[_MainWind.Line_List.Last().Output_Num].Output_ID = detection;
                    _MainWind.Gate_List[_MainWind.Linking_ID].Output[_MainWind.Line_List.Last().Output_Num].Output_Type = IO_Type.Gate;
                }
                else if(X==-1)
                {
                    Children.Remove(_MainWind.Line_List[_MainWind.Drag_Num].UI_Line);
                    Children.Remove(_MainWind.Line_List[_MainWind.Drag_Num].Content);
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
                if (Pos_Sub.X+Width > Rect_X && Pos_Sub.X < Rect_X + Rect.Rect.Width && Pos_Sub.Y+Height > Rect_Y && Pos_Sub.Y < Rect_Y + Rect.Rect.Height&&i!=Drag_Num && Rect.Alive)
                    Detected = i;
            }
            return Detected;
        }

        public int Link_Output_Vaildation(int Clicked)
        {
            int Output_pos = Output_Slot(Clicked);
            if (_MainWind.Gate_List[Clicked].Type == 7)
            {
                if (_MainWind.Gate_List[Clicked].Output[Output_pos].Output_ID != -1)
                {
                     Output_pos = -2;
                }
                //if the above failed then it will enter this if statement
                if (Output_pos == -2)
                {
                    //this will check each output and try and find a available output                    
                    if(_MainWind.Gate_List[Clicked].Output[0].Output_ID == -1)
                        Output_pos = 0;
                    else if (_MainWind.Gate_List[Clicked].Output[1].Output_ID == -1)
                        Output_pos = 1;
                    else if (_MainWind.Gate_List[Clicked].Output[2].Output_ID == -1)
                        Output_pos = 2;
                }
            }
            else
            {
                if (_MainWind.Gate_List[Clicked].Output[0].Output_ID != -1)
                {
                    Output_pos = -2;
                }
            }

            return Output_pos;
        }

        public int Output_Slot(int Clicked)
        {
            int Output = 0;
            Point Pos = Mouse.GetPosition(this);
            if (_MainWind.Gate_List[Clicked].Type == 7)
            {
                if (Pos.Y < GetTop(_MainWind.Gate_List[Clicked].Rect) + 29)
                    Output = 0;                    
                else if (Pos.Y > GetTop(_MainWind.Gate_List[Clicked].Rect) + 29 && Pos.Y < GetTop(_MainWind.Gate_List[Clicked].Rect) + 43)
                    Output = 1;
                else if (Pos.Y > GetTop(_MainWind.Gate_List[Clicked].Rect) + 43)
                    Output = 2;
            }
            else
            {
                Output = 0;
            }
            return Output;
        }

        public int Link_Input_Vaildation(int Clicked)
        {
            int Output = Input_Slot(Clicked);
            Point Pos = Mouse.GetPosition(this);
            if(_MainWind.Gate_List[Clicked].Type==2|| _MainWind.Gate_List[Clicked].Type == 7)
            {
                if(_MainWind.Gate_List[Clicked].Input[0].Input_ID!=-1)
                {
                    Output = -1;
                }
            }
            else
            {
                if (_MainWind.Gate_List[Clicked].Input[Output].Input_ID != -1)
                {
                    Output = -1;
                }

                if (Output == -1)
                {
                    if(_MainWind.Gate_List[Clicked].Input[0].Input_ID == -1)
                    {
                        Output = 0;
                    }
                    else if (_MainWind.Gate_List[Clicked].Input[1].Input_ID == -1)
                    {
                        Output = 1;
                    }

                }
            }
            
            return Output;
        }
        
        public int Input_Slot(int Clicked)
        {
            int Input_Slot = -1;
            Point Pos = Mouse.GetPosition(this);
            if (_MainWind.Gate_List[Clicked].Type == 2 || _MainWind.Gate_List[Clicked].Type == 7)
            {
                    Input_Slot = 0;
            }
            else
            {
                if (Pos.Y < GetTop(_MainWind.Gate_List[Clicked].Rect) + 37)
                {
                    Input_Slot = 0;                    
                }
                else
                {
                    Input_Slot = 1;
                }
            }

            return Input_Slot;
        }

        protected override void OnMouseRightButtonUp(MouseButtonEventArgs e)
        {
            //checks you're not dragging
            if(!_MainWind.Drag)
            {
                //sees if you acutally right click on something
                int detection = Rect_detection(0, 0, -1);
                if (detection!=-1)
                { //see which mode of the program you're in because each one will have a different action
                    if(!_MainWind.Link)
                    {    //completely romoves the gate.
                        _MainWind.Gate_List[detection].Alive = false;
                        Children.Remove(_MainWind.Gate_List[detection].Rect);
                        for (int i = 0; i < 3; i++)  //this is for each output of the gate after it's been removed
                        {
                            if(_MainWind.Gate_List[detection].Output[i].Output_Type == IO_Type.Gate)
                            { //removes the line "connecting" the 2 gates.
                                _MainWind.Sub_Canvas.Children.Remove(_MainWind.Line_List[_MainWind.Gate_List[detection].Output[i].Line_ID].UI_Line);
                                _MainWind.Sub_Canvas.Children.Remove(_MainWind.Line_List[_MainWind.Gate_List[detection].Output[i].Line_ID].Content);
                                for (int x = 0; x < 2; x++) // this is to determin which input the gate is connected to
                                {
                                    if (_MainWind.Gate_List[_MainWind.Gate_List[detection].Output[i].Output_ID].Input[x].Input_ID == detection && _MainWind.Gate_List[_MainWind.Gate_List[detection].Output[i].Output_ID].Input[x].Input_Type == IO_Type.Gate)
                                    { //changes the state of the gate it's connected to too null so that it can accept another input.
                                        _MainWind.Gate_List[_MainWind.Gate_List[detection].Output[i].Output_ID].Input[x].Input_Type = IO_Type.Null;
                                    }
                                }
                            }
                            else if(_MainWind.Gate_List[detection].Output[i].Output_Type == IO_Type.IO)
                            {
                                //remove the ellipses from the canvas in the output_Circle list with the ID in the 
                            }
                        }
                        for (int i = 0; i < 2; i++) //does the same but for input
                        {
                            if (_MainWind.Gate_List[detection].Input[i].Input_Type == IO_Type.Gate)
                            {
                                _MainWind.Sub_Canvas.Children.Remove(_MainWind.Line_List[_MainWind.Gate_List[detection].Input[i].Line_ID].UI_Line);
                                _MainWind.Sub_Canvas.Children.Remove(_MainWind.Line_List[_MainWind.Gate_List[detection].Input[i].Line_ID].Content);
                                for (int x = 0; x < 3; x++)
                                {
                                    if (_MainWind.Gate_List[_MainWind.Gate_List[detection].Input[i].Input_ID].Output[x].Output_ID == detection)
                                    {
                                        _MainWind.Gate_List[_MainWind.Gate_List[detection].Input[i].Input_ID].Output[x].Output_Type = IO_Type.Null;
                                    }
                                }
                            }
                            else if (_MainWind.Gate_List[detection].Input[i].Input_Type == IO_Type.IO)
                            {
                                //remove the button from the canvas in the input_button list with the ID in the 
                            }
                        }
                    }
                    else if(_MainWind.Link) //if in linked mode then it should act as tho you're doing the oppersite of adding a connection.
                    {
                        int Output_Num = Output_Slot(detection);
                        if (_MainWind.Gate_List[detection].Output[Output_Num].Output_ID != -1)
                        {
                            for (int i = 0; i < 2; i++)
                            {
                                if(_MainWind.Gate_List[_MainWind.Gate_List[detection].Output[Output_Num].Output_ID].Input[i].Input_ID==detection)
                                {
                                    _MainWind.Gate_List[_MainWind.Gate_List[detection].Output[Output_Num].Output_ID].Input[i].Input_Type = IO_Type.Null;
                                }
                            }
                            _MainWind.Sub_Canvas.Children.Remove(_MainWind.Line_List[_MainWind.Gate_List[detection].Output[Output_Num].Line_ID].UI_Line);
                            _MainWind.Sub_Canvas.Children.Remove(_MainWind.Line_List[_MainWind.Gate_List[detection].Output[Output_Num].Line_ID].Content);
                            _MainWind.Gate_List[detection].Output[Output_Num].Output_Type = IO_Type.Null;
                        }
                    }

                }
            }
        }
    }
}
