using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Collections.Generic;

namespace A_level_course_work_Logic_Gate
{
    public class Canvas_Class : Canvas
    {        
        //variabls for the class
        public Point Old_Pos { get; set; } = new Point();
        public double Scale_Factor { get; set; } = 1;
        public Point Old_Rect { get; set; } = new Point();

        public TranslateTransform Translate_Action { get; set; }
        public ScaleTransform Scale_Action { get; set; }

        public TransformGroup Transforms_Group { get; set; }

        public bool MovingCanvas { get; set; } = false;
        public Border _Canvas_Border { get; set; }

        public Canvas_Variables variables;
        private MainWindow _MainWind { get; }        

       /// <summary>
       /// This intilaises the canvas class
       /// </summary>
       /// <param name="Variables">This contains the values needed for X</param>
        public Canvas_Class( Canvas_Variables Variables, ref Border Canvas_Border, MainWindow MainWind)
        {
            variables = Variables;
            _Canvas_Border = Canvas_Border;
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
            if (hold != variables.Last_ID_For_Rect && hold !=-1)
            {
                variables.Last_ID_For_Rect = hold;
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
            Point Pos = Mouse.GetPosition(_Canvas_Border);
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

            if(!variables.Drag &&detected==-1)
            {
                Old_Pos = e.GetPosition(this);
                MovingCanvas = true;
            }

            if (!variables.Drag && !variables.Link)
            {
                if (detected != -1)
                {
                    variables.Drag_Num = detected;
                    variables.Drag_Mode = Drag_State.Sub_Can;
                    Old_Rect = new Point(Convert.ToDouble(Canvas.GetLeft(variables.Gate_List[variables.Drag_Num].Rect)), Convert.ToDouble(Canvas.GetTop(variables.Gate_List[variables.Drag_Num].Rect)));
                }
            }
            else if (!variables.Drag && variables.Link && detected!=-1)
            {
                variables.Linking_ID = detected;
                variables.Drag_Num = variables.Line_List.Count();
                variables.Line_List.Add(new Line_Class(detected,this,_MainWind,detected));
                //if X == -2 then there was no aviable output for the new link to be made(already deleted the last line)
                if (variables.Line_List.Last().Output_Num != -2)
                {
                    variables.Drag_Mode = Drag_State.Link_Mode_Sub;
                    variables.Gate_List[detected].Output[variables.Line_List.Last().Output_Num].Line_ID = variables.Drag_Num;
                    variables.Line_List[variables.Drag_Num].Link_Output_Aline_Line(variables.Gate_List[detected]);
                }
                else
                {
                    variables.Line_List[variables.Drag_Num].Remove_Class();
                }

            }
        }
        //Re-drag drop
        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            MovingCanvas = false;
            if(variables.Drag && !variables.Link && variables.Drag_Mode == Drag_State.Main_Can)
            {
                _MainWind.Add_Rect_Sub_FIX_BUG();
            }

            if (variables.Drag && !variables.Link)
            {
                variables.Drag_Mode = Drag_State.Null;
                if(Rect_detection(variables.Gate_List[variables.Drag_Num].Rect.Width, variables.Gate_List[variables.Drag_Num].Rect.Height, variables.Drag_Num) !=-1)
                {
                    variables.Gate_List[variables.Drag_Num].Rect_Move(Old_Rect);
                    variables.Gate_List[variables.Drag_Num].Move_IO();
                }
            }
            else if(variables.Drag && variables.Link)
            {
                int X = -1;
                int detection = Rect_detection(0, 0, -1);
                variables.Drag_Mode = Drag_State.Null;
                if (detection != -1 && detection != variables.Linking_ID)
                {
                    X = Link_Input_Vaildation(detection);
                }

                if (X!=-1)
                {

                    variables.Line_List[variables.Drag_Num].Input_ID = detection;
                    variables.Line_List[variables.Drag_Num].Input_Num = X;
                    variables.Line_List[variables.Drag_Num].Link_Input_Aline_Line(variables.Gate_List[detection]);
                    variables.Gate_List[detection].Input[X].Input_Type = IO_Type.Gate;
                    variables.Gate_List[detection].Input[X].Input_ID = variables.Linking_ID;
                    variables.Gate_List[detection].Input[X].Line_ID = variables.Drag_Num;
                    variables.Gate_List[variables.Linking_ID].Output[variables.Line_List.Last().Output_Num].Output_ID = detection;
                    variables.Gate_List[variables.Linking_ID].Output[variables.Line_List.Last().Output_Num].Output_Type = IO_Type.Gate;
                }
                else if(X==-1)
                {
                    variables.Line_List[variables.Drag_Num].Remove_Class();
                }
            }
        }

        public int Rect_detection(double Width, double Height,int Drag_Num)
        {
            int Detected = -1;
            Point Pos_Sub = Mouse.GetPosition(this);
            for (int i = 0; i < variables.Gate_List.Count(); i++)
            {
                Gate_Class Rect = variables.Gate_List[i];
                double Rect_X = GetLeft(variables.Gate_List[i].Rect);
                double Rect_Y = GetTop(variables.Gate_List[i].Rect);
                if (Pos_Sub.X+Width > Rect_X && Pos_Sub.X < Rect_X + Rect.Rect.Width && Pos_Sub.Y+Height > Rect_Y && Pos_Sub.Y < Rect_Y + Rect.Rect.Height&&i!=Drag_Num && Rect.Alive)
                    Detected = i;
            }
            return Detected;
        }

        public int Link_Output_Vaildation(int Clicked)
        {
            int Output_pos = Output_Slot(Clicked);
            if (variables.Gate_List[Clicked].Type == 7)
            {
                if (variables.Gate_List[Clicked].Output[Output_pos].Output_ID != -1)
                {
                     Output_pos = -2;
                }
                //if the above failed then it will enter this if statement
                if (Output_pos == -2)
                {
                    //this will check each output and try and find a available output  
                    for (int i = 0; i < 3; i++)
                    {
                        if (variables.Gate_List[Clicked].Output[i].Output_ID == -1)
                        {
                            Output_pos = i;
                        }
                    }
                }
            }
            else
            {
                if (variables.Gate_List[Clicked].Output[0].Output_ID != -1)
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
            if (variables.Gate_List[Clicked].Type == 7)
            {
                if (Pos.Y < GetTop(variables.Gate_List[Clicked].Rect) + 29)
                    Output = 0;                    
                else if (Pos.Y > GetTop(variables.Gate_List[Clicked].Rect) + 29 && Pos.Y < GetTop(variables.Gate_List[Clicked].Rect) + 43)
                    Output = 1;
                else if (Pos.Y > GetTop(variables.Gate_List[Clicked].Rect) + 43)
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
            if(variables.Gate_List[Clicked].Type==2|| variables.Gate_List[Clicked].Type == 7)
            {
                if(variables.Gate_List[Clicked].Input[0].Input_ID!=-1)
                {
                    Output = -1;
                }
            }
            else
            {
                if (variables.Gate_List[Clicked].Input[Output].Input_ID != -1)
                {
                    Output = -1;
                }

                if (Output == -1)
                {
                    if(variables.Gate_List[Clicked].Input[0].Input_ID == -1)
                    {
                        Output = 0;
                    }
                    else if (variables.Gate_List[Clicked].Input[1].Input_ID == -1)
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
            if (variables.Gate_List[Clicked].Type == 2 || variables.Gate_List[Clicked].Type == 7)
            {
                    Input_Slot = 0;
            }
            else
            {
                if (Pos.Y < GetTop(variables.Gate_List[Clicked].Rect) + 37)
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
            if(!variables.Drag)
            {
                //sees if you acutally right click on something
                int detection = Rect_detection(0, 0, -1);
                if (detection != -1)
                { //see which mode of the program you're in because each one will have a different action
                    if(!variables.Link)
                    {    //completely romoves the gate.
                        variables.Gate_List[detection].Alive = false;
                        Children.Remove(variables.Gate_List[detection].Rect);
                        for (int i = 0; i < 3; i++)  //this is for each output of the gate after it's been removed
                        {
                            if(variables.Gate_List[detection].Output[i].Output_Type == IO_Type.Gate)
                            { //removes the line "connecting" the 2 gates.
                                variables.Line_List[variables.Gate_List[detection].Output[i].Line_ID].Remove_UI();
                                for (int x = 0; x < 2; x++) // this is to determin which input the gate is connected to
                                {
                                    if (variables.Gate_List[variables.Gate_List[detection].Output[i].Output_ID].Input[x].Input_ID == detection && variables.Gate_List[variables.Gate_List[detection].Output[i].Output_ID].Input[x].Input_Type == IO_Type.Gate)
                                    { //changes the state of the gate it's connected to too null so that it can accept another input.
                                        variables.Gate_List[variables.Gate_List[detection].Output[i].Output_ID].Input[x].Input_Type = IO_Type.Null;
                                    }
                                }
                                variables.Gate_List[detection].Output[i].Output_Type = IO_Type.Null;
                            }
                            else if(variables.Gate_List[detection].Output[i].Output_Type == IO_Type.IO)
                            {
                                Children.Remove(variables.Output_Circle_List[variables.Gate_List[detection].Output[i].Output_ID].Circle);                                
                                //remove the ellipses from the canvas in the output_Circle list with the ID in the 
                            }
                        }
                        for (int i = 0; i < 2; i++) //does the same but for input
                        {
                            if (variables.Gate_List[detection].Input[i].Input_Type == IO_Type.Gate)
                            {
                                variables.Line_List[variables.Gate_List[detection].Input[i].Line_ID].Remove_UI();
                                for (int x = 0; x < 3; x++)
                                {
                                    if (variables.Gate_List[variables.Gate_List[detection].Input[i].Input_ID].Output[x].Output_ID == detection)
                                    {
                                        variables.Gate_List[variables.Gate_List[detection].Input[i].Input_ID].Output[x].Output_Type = IO_Type.Null;
                                    }
                                }
                            }
                            else if (variables.Gate_List[detection].Input[i].Input_Type == IO_Type.IO)
                            {
                                Children.Remove(variables.Input_Button_List[variables.Gate_List[detection].Input[i].Input_ID]);
                            }
                        }
                    }
                    else if(variables.Link) //if in linked mode then it should act as tho you're doing the oppersite of adding a connection.
                    {
                        int Output_Num = Output_Slot(detection);
                        if (variables.Gate_List[detection].Output[Output_Num].Output_ID != -1 && variables.Gate_List[detection].Output[Output_Num].Output_Type==IO_Type.Gate)
                        {
                            for (int i = 0; i < 2; i++)
                            {
                                if(variables.Gate_List[variables.Gate_List[detection].Output[Output_Num].Output_ID].Input[i].Input_ID==detection)
                                {
                                    variables.Gate_List[variables.Gate_List[detection].Output[Output_Num].Output_ID].Input[i].Input_Type = IO_Type.Null;
                                }
                            }
                            variables.Line_List[variables.Gate_List[detection].Output[Output_Num].Line_ID].Remove_UI();
                            variables.Gate_List[detection].Output[Output_Num].Output_Type = IO_Type.Null;
                        }
                    }

                }
            }
        }
    }
}
