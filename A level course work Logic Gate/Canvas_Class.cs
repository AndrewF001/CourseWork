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
        private int Last_Rect { get; set; } = -1;

/// <summary>
///  The constructor just sets the value for the canvas and adds the transformation set up.
/// </summary>
/// <param name="Variables"></This a more secure way of accessing and transfering varaibles in a bidirectional way>
/// <param name="Canvas_Border"></Needed as a reference point for mouse postion>
/// <param name="MainWind"></Needed to call methods in the class>
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
        /// <Mouse move>
        /// This is just for if the canvas is being moved around
        /// </summary>
        protected override void OnMouseMove(MouseEventArgs e)
        {
            Detection_State State;
            int ID;
            (State, ID) = Rect_detection(0, 0, -1);
            if(State == Detection_State.Detected)
            {
                if (Last_Rect != -1)
                {
                    variables.Gate_List[Last_Rect].Rect.Stroke = Brushes.Black;
                }
                variables.Gate_List[ID].Rect.Stroke = Brushes.LightGreen;
                Last_Rect = ID;
            }
            else if(State == Detection_State.Detected && Last_Rect!=-1)
            {
                variables.Gate_List[Last_Rect].Rect.Stroke = Brushes.Black;
                Last_Rect = -1;
            }
            
            if (MovingCanvas)
            {
                Translate_Action.X += (e.GetPosition(this).X - Old_Pos.X);
                Translate_Action.Y += (e.GetPosition(this).Y - Old_Pos.Y);
            }
            Old_Pos = e.GetPosition(this);
        }
        /// <summary>
        /// Controls the zoom in and out for the canvas.
        /// </summary>                
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
        /// <summary>
        /// 1. Checks and start the canvas drag
        /// 2.checks to find a gate and start dragging that.
        /// 3.checks to find a gate and check mode and add new link line for the gate
        /// </summary>
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {   
            (Detection_State State, int detected) = Rect_detection(0, 0, -1);

            if(!variables.Drag &&State == Detection_State.Null)
            {
                Old_Pos = e.GetPosition(this);
                MovingCanvas = true;
            }

            if (!variables.Drag && !variables.Link)
            {
                if (State == Detection_State.Detected)
                {
                    variables.Drag_Num = detected;
                    variables.Drag_Mode = Drag_State.Sub_Can;
                    Old_Rect = new Point(Convert.ToDouble(Canvas.GetLeft(variables.Gate_List[variables.Drag_Num].Rect)), Convert.ToDouble(Canvas.GetTop(variables.Gate_List[variables.Drag_Num].Rect)));
                }
            }
            else if (!variables.Drag && variables.Link && State == Detection_State.Detected)
            {
                Line_M1_Down( detected);
            }
        }
        private void Line_M1_Down(int detected)
        {
            variables.Linking_ID = detected;
            variables.Drag_Num = variables.Line_List.Count();
            variables.Line_List.Add(new Line_Class(detected, this, _MainWind, detected, true));
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
        /// <summary>
        /// depending on what is happening it will be the end the event.
        /// </summary>
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
                (Detection_State State, int Null) = Rect_detection(variables.Gate_List[variables.Drag_Num].Rect.Width, variables.Gate_List[variables.Drag_Num].Rect.Height, variables.Drag_Num);
                if (State == Detection_State.Detected)
                {
                    variables.Gate_List[variables.Drag_Num].Rect_Move(Old_Rect);
                    variables.Gate_List[variables.Drag_Num].Move_IO();
                }
            }
            else if(variables.Drag && variables.Link)
            {
                Line_M1_Up();
            }
        }
        private void Line_M1_Up()
        {
            int X = -1;
            (Detection_State State, int detection) = Rect_detection(0, 0, -1);
            variables.Drag_Mode = Drag_State.Null;
            if (State == Detection_State.Detected && detection != variables.Linking_ID)
            {
                X = Link_Input_Vaildation(detection);
            }

            if (X != -1)
            {
                //This a block of code that is only done one time and isn't assocaited with each other so making a method wouldn't make a lot of sense but it's a lot of just nothing.
                variables.Line_List[variables.Drag_Num].Input_ID = detection;
                variables.Line_List[variables.Drag_Num].Input_Num = X;
                variables.Line_List[variables.Drag_Num].Link_Input_Aline_Line(variables.Gate_List[detection]);
                variables.Gate_List[detection].Input[X].Input_Type = IO_Type.Gate;
                variables.Gate_List[detection].Input[X].Input_ID = variables.Linking_ID;
                variables.Gate_List[detection].Input[X].Line_ID = variables.Drag_Num;
                variables.Gate_List[variables.Linking_ID].Output[variables.Line_List.Last().Output_Num].Output_ID = detection;
                variables.Gate_List[variables.Linking_ID].Output[variables.Line_List.Last().Output_Num].Output_Type = IO_Type.Gate;
            }
            else if (X == -1)
            {
                variables.Line_List[variables.Drag_Num].Remove_Class();
            }
        }
        /// <summary>
        /// Goes through the whole gate list and finds out if you clicked in the range of a gate.
        /// </summary>        
        public (Detection_State,  int) Rect_detection(double Width, double Height,int Drag_Num)
        {
            Detection_State State = Detection_State.Null;
            int Detected = -1;
            Point Pos_Sub = Mouse.GetPosition(this);
            for (int i = 0; i < variables.Gate_List.Count(); i++)
            {
                Gate_Class Rect = variables.Gate_List[i];
                double Rect_X = GetLeft(variables.Gate_List[i].Rect);
                double Rect_Y = GetTop(variables.Gate_List[i].Rect);
                if (Pos_Sub.X + Width > Rect_X && Pos_Sub.X < Rect_X + Rect.Rect.Width && Pos_Sub.Y + Height > Rect_Y && Pos_Sub.Y < Rect_Y + Rect.Rect.Height && i != Drag_Num && Rect.Alive)
                {
                    Detected = i;
                    State = Detection_State.Detected;
                }
            }
            return (State, Detected);
        }
        /// <summary>
        /// Output_Slot works out the output slot that the user clicked on.
        /// The rest of the method works out if that slot is taken, If so it will try and find the next aviable slot.
        /// Otherwise it is deleted.
        /// </summary>
        /// <param name="Clicked"></the gate number that was clicked on>
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
        /// <summary>
        ///  Just works out the boundaries for the gate types and what slot it should be.
        /// </summary>
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
        /// <summary>
        /// same as the output method but for input(2 methods above, Link_Output_Vaildation)
        /// </summary>
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
        /// <summary>
        /// same as the output method but for input(2 methods above, Output_Slot)
        /// </summary>
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
        /// <summary>
        /// Depending on what mode the program is in it will delete the UI element from the canvas.
        /// It will also remove any connections and input/outputs that that gate might of had.
        /// </summary>
        protected override void OnMouseRightButtonUp(MouseButtonEventArgs e)
        {
            //checks you're not dragging
            if(!variables.Drag)
            {
                //sees if you acutally right click on something
                (Detection_State State, int detection) = Rect_detection(0, 0, -1);
                if (State == Detection_State.Detected)
                { //see which mode of the program you're in because each one will have a different action
                    if(!variables.Link)
                    {    //completely romoves the gate.
                        Remove_Gate(detection);
                    }
                    else if(variables.Link) //if in linked mode then it should act as tho you're doing the oppersite of adding a connection.
                    {
                        Remove_Line(detection);
                    }

                }
            }
        }
        private void Remove_Gate(int detection)
        {
            variables.Gate_List[detection].Alive = false;
            Children.Remove(variables.Gate_List[detection].Rect);
            for (int i = 0; i < 3; i++)  //this is for each output of the gate after it's been removed
            {
                if (variables.Gate_List[detection].Output[i].Output_Type == IO_Type.Gate)
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
                else if (variables.Gate_List[detection].Output[i].Output_Type == IO_Type.IO)
                {
                    variables.Output_Circle_List[variables.Gate_List[detection].Output[i].Output_ID].Remove_UI();
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

        private void Remove_Line(int detection)
        {
            int Output_Num = Output_Slot(detection);
            if (variables.Gate_List[detection].Output[Output_Num].Output_ID != -1 && variables.Gate_List[detection].Output[Output_Num].Output_Type == IO_Type.Gate)
            {
                for (int i = 0; i < 2; i++)
                {
                    if (variables.Gate_List[variables.Gate_List[detection].Output[Output_Num].Output_ID].Input[i].Input_ID == detection)
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
