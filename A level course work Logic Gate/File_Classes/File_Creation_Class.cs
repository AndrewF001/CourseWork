using A_level_course_work_Logic_Gate.SubGate_Classes;
using Microsoft.Win32;
using System;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace A_level_course_work_Logic_Gate.File_Classes
{
    class File_Creation_Class
    {
        MainWindow MainWind { get; }
        public File_Creation_Class(MainWindow _MainWind)
        {
            MainWind = _MainWind;
        }
        public void MenuItem_Load_Click_Method()
        {
            bool CarryOn = true;
            if (MainWind.File_Location != "")
            {
                Save_File();
                MainWind.Reset_Program();
                var result = MessageBox.Show("Your work has been saved", "New File", MessageBoxButton.OK);
            }
            else
            {
                var result = MessageBox.Show("You haven't saved your work, Do you want to delete it?", "New File", MessageBoxButton.OKCancel);
                if (result == MessageBoxResult.OK)
                {
                    MainWind.Reset_Program();
                }
                else
                {
                    CarryOn = false;
                }
            }
            if (CarryOn)
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                if (openFileDialog.ShowDialog() == true)
                {
                    openFileDialog.InitialDirectory = @"C:\Documents";
                    MainWind.File_Location = openFileDialog.FileName;
                    Stream stream = new FileStream(MainWind.File_Location, FileMode.Open, FileAccess.Read);
                    IFormatter formatter = new BinaryFormatter();
                    try
                    {
                        File_Class Loaded_File = (File_Class)formatter.Deserialize(stream);
                        File_Unload(Loaded_File);
                    }
                    catch
                    {
                        MessageBox.Show("The file you tried to open doesn't work with this program", "Failed", MessageBoxButton.OK);
                    }
                }

            }
        }
        /// <summary>
        /// converts the Serializable file class to fit the program classes. Also sets up everything to be ready to used.
        /// </summary>
        /// <param name="Loaded_File"></File taht needs to be unloaded>
        public void File_Unload(File_Class Loaded_File)
        {
            for (int x = 0; x < Loaded_File.Inputs.Count; x++)
            {
                MainWind.Input_Button_List.Add(new Input_Button(Loaded_File.Inputs[x].Input_ID, Loaded_File.Inputs[x].Input_Port, MainWind.Gate_List, MainWind.Sub_Canvas, MainWind));
                MainWind.Input_Button_List.Last().Change_X_Y(Loaded_File.Inputs[x].X, Loaded_File.Inputs[x].Y);
            }
            for (int x = 0; x < Loaded_File.Output.Count; x++)
            {
                MainWind.Output_Circle_List.Add(new Output_Circle(Loaded_File.Output[x].Output_ID, Loaded_File.Output[x].Output_Port, MainWind.Sub_Canvas, MainWind));
                MainWind.Output_Circle_List.Last().Change_X_Y(Loaded_File.Output[x].X, Loaded_File.Output[x].Y);
            }
            for (int x = 0; x < Loaded_File.Lines.Count; x++)
            {
                MainWind.Line_List.Add(new Line_Class(Loaded_File.Lines[x].Output_ID, MainWind.Sub_Canvas, MainWind, Loaded_File.Lines[x].Input_ID, false));
                MainWind.Line_List.Last().Output_Num = Loaded_File.Lines[x].Output_Num;
                MainWind.Line_List.Last().Input_Num = Loaded_File.Lines[x].Input_Num;
                MainWind.Line_List.Last().Line_Lable.Content = Loaded_File.Lines[x].Content_Copy;
                MainWind.Line_List.Last().UI_Line.X1 = Loaded_File.Lines[x].X1;
                MainWind.Line_List.Last().UI_Line.X2 = Loaded_File.Lines[x].X2;
                MainWind.Line_List.Last().UI_Line.Y1 = Loaded_File.Lines[x].Y1;
                MainWind.Line_List.Last().UI_Line.Y2 = Loaded_File.Lines[x].Y2;
                MainWind.Line_List.Last().X1 = Loaded_File.Lines[x].X1;
                MainWind.Line_List.Last().X2 = Loaded_File.Lines[x].X2;
                MainWind.Line_List.Last().Y1 = Loaded_File.Lines[x].Y1;
                MainWind.Line_List.Last().Y2 = Loaded_File.Lines[x].Y2;
                MainWind.Line_List.Last().UI_Line.Stroke = Brushes.Black;
                MainWind.Line_List.Last().Move_Label();
            }
            for (int i = 0; i < Loaded_File.Gates.Count; i++)
            {
                switch (Loaded_File.Gates[i].Type)
                {
                    case (Gate_Type.And):
                        MainWind.Gate_List.Add(new And_Gate_Class(MainWind.Main_Canvas, MainWind.Sub_Canvas.Scale_Factor, MainWind.Output_Circle_List, MainWind.Line_List, MainWind.Input_Button_List));
                        break;
                    case (Gate_Type.Nand):
                        MainWind.Gate_List.Add(new Nand_Gate_Class(MainWind.Main_Canvas, MainWind.Sub_Canvas.Scale_Factor, MainWind.Output_Circle_List, MainWind.Line_List, MainWind.Input_Button_List));
                        break;
                    case (Gate_Type.Not):
                        MainWind.Gate_List.Add(new Not_Gate_Class(MainWind.Main_Canvas, MainWind.Sub_Canvas.Scale_Factor, MainWind.Output_Circle_List, MainWind.Line_List, MainWind.Input_Button_List));
                        break;
                    case (Gate_Type.Or):
                        MainWind.Gate_List.Add(new Or_Gate_Class(MainWind.Main_Canvas, MainWind.Sub_Canvas.Scale_Factor, MainWind.Output_Circle_List, MainWind.Line_List, MainWind.Input_Button_List));
                        break;
                    case (Gate_Type.Nor):
                        MainWind.Gate_List.Add(new Nor_Gate_Class(MainWind.Main_Canvas, MainWind.Sub_Canvas.Scale_Factor, MainWind.Output_Circle_List, MainWind.Line_List, MainWind.Input_Button_List));
                        break;
                    case (Gate_Type.Xor):
                        MainWind.Gate_List.Add(new Xor_Gate_Class(MainWind.Main_Canvas, MainWind.Sub_Canvas.Scale_Factor, MainWind.Output_Circle_List, MainWind.Line_List, MainWind.Input_Button_List));
                        break;
                    case (Gate_Type.Xnor):
                        MainWind.Gate_List.Add(new Xnor_Gate_Class(MainWind.Main_Canvas, MainWind.Sub_Canvas.Scale_Factor, MainWind.Output_Circle_List, MainWind.Line_List, MainWind.Input_Button_List));
                        break;
                    case (Gate_Type.Transformer):
                        MainWind.Gate_List.Add(new Transformer_Class(MainWind.Main_Canvas, MainWind.Sub_Canvas.Scale_Factor, MainWind.Output_Circle_List, MainWind.Line_List, MainWind.Input_Button_List));
                        break;
                    default:
                        MainWind.Gate_List.Add(new And_Gate_Class(MainWind.Main_Canvas, MainWind.Sub_Canvas.Scale_Factor, MainWind.Output_Circle_List, MainWind.Line_List, MainWind.Input_Button_List));
                        break;
                }
                MainWind.Gate_List.Last().Alive = Loaded_File.Gates[i].Alive;
                MainWind.Gate_List.Last().Gate_Bit = Loaded_File.Gates[i]._Gate_Bit;
                for (int x = 0; x < 3; x++)
                {
                    MainWind.Gate_List.Last().Output[x].Output_ID = Loaded_File.Gates[i].Output[x]._output_ID;
                    MainWind.Gate_List.Last().Output[x].Line_ID = Loaded_File.Gates[i].Output[x]._line_ID;
                    MainWind.Gate_List.Last().Output[x].Output_Port = Loaded_File.Gates[i].Output[x]._output_port;
                    MainWind.Gate_List.Last().Output[x].Output_Type = Loaded_File.Gates[i].Output[x]._output_Type;
                }
                for (int x = 0; x < 2; x++)
                {
                    MainWind.Gate_List.Last().Input[x].Input_ID = Loaded_File.Gates[i].Input[x]._Input_ID;
                    MainWind.Gate_List.Last().Input[x].Line_ID = Loaded_File.Gates[i].Input[x]._line_ID;
                    MainWind.Gate_List.Last().Input[x].Input_Type = Loaded_File.Gates[i].Input[x]._Input_Type;
                }
                MainWind.Main_Canvas.Children.Remove(MainWind.Gate_List.Last().Rect);
                MainWind.Sub_Canvas.Children.Add(MainWind.Gate_List.Last().Rect);
                Canvas.SetLeft(MainWind.Gate_List.Last().Rect, Loaded_File.Gates[i].X);
                Canvas.SetTop(MainWind.Gate_List.Last().Rect, Loaded_File.Gates[i].Y);
            }
            for (int x = 0; x < Loaded_File.Inputs.Count; x++)
            {
                MainWind.Input_Button_List.Last().Bit = Loaded_File.Inputs[x]._Bit;
            }
        }

        //Save file setup
        public void MenuItem_SaveAs_Click_Method()
        {
            Save_AS();
        }
        private void Save_AS()
        {           
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            if (saveFileDialog.ShowDialog() == true)
            {
                MainWind.Saved = true;
                saveFileDialog.InitialDirectory = @"C:\Documents";
                MainWind.File_Location = saveFileDialog.FileName;
                Save_File();
            }
        }
        //just maps the current class vairables into the File Classes
        public File_Class File_Creation()
        {
            File_Class Save = new File_Class();
            for (int i = 0; i < MainWind.Gate_List.Count(); i++)
            {
                Save.Gates.Add(new File_Version_Gate(MainWind.Gate_List[i].Type, MainWind.Gate_List[i].Alive, MainWind.Gate_List[i].Input, MainWind.Gate_List[i].Gate_Bit, MainWind.Gate_List[i].Output, Canvas.GetLeft(MainWind.Gate_List[i].Rect), Canvas.GetTop(MainWind.Gate_List[i].Rect)));
            }
            for (int i = 0; i < MainWind.Line_List.Count; i++)
            {
                Save.Lines.Add(new File_Version_Line(Convert.ToString(MainWind.Line_List[i].Line_Lable.Content), MainWind.Line_List[i].Output_ID, MainWind.Line_List[i].Output_Num, MainWind.Line_List[i].Input_ID, MainWind.Line_List[i].Input_Num, MainWind.Line_List[i].X1, MainWind.Line_List[i].X2, MainWind.Line_List[i].Y1, MainWind.Line_List[i].Y2));
            }
            for (int i = 0; i < MainWind.Input_Button_List.Count; i++)
            {
                Save.Inputs.Add(new File_Version_Input(MainWind.Input_Button_List[i].Bit, MainWind.Input_Button_List[i].Input_ID, MainWind.Input_Button_List[i].Input_Port, Canvas.GetLeft(MainWind.Input_Button_List[i]), Canvas.GetTop(MainWind.Input_Button_List[i])));
            }
            for (int i = 0; i < MainWind.Output_Circle_List.Count; i++)
            {
                Save.Output.Add(new File_Version_Output(MainWind.Output_Circle_List[i].Output_ID, MainWind.Output_Circle_List[i].Output_Port, Canvas.GetLeft(MainWind.Output_Circle_List[i].Circle), Canvas.GetTop(MainWind.Output_Circle_List[i].Circle)));
            }
            return Save;
        }

        public void MenuItem_Save_Click_Method()
        {
            Save_File();
        }
        public void Save_File()
        {
            if (MainWind.Saved)
            {
                MainWind.Clean_Up_Method();
                File_Class Save = File_Creation();
                Stream stream = new FileStream(MainWind.File_Location, FileMode.Create, FileAccess.Write);
                IFormatter formatter = new BinaryFormatter();
                formatter.Serialize(stream, Save);
                stream.Close();
            }
            else
            {
                Save_AS();
            }
        }

        public void MenuItem_New_Click_Method()
        {
            MessageBoxResult messageBoxResult = MessageBox.Show("Are you sure you want to remove everything? Nothing will be kept!", "New Window", MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                MainWind.Reset_Program();
            }
        }        
    }
}
