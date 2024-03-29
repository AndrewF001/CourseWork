﻿namespace A_level_course_work_Logic_Gate
{
    public class Output_Class
    {
        private int _output_ID = -1;
        private IO_Type _output_Type = IO_Type.Null;
        private int _line_ID = -1;
        public int Output_Port { get; set; }
        public int Output_ID {
            get {return _output_ID ; }
            set {_output_ID =value; } }

        public IO_Type Output_Type {
            get { return _output_Type; }
            set {
                switch(value)
                {
                    case (IO_Type.Null):
                        _output_ID = -1;
                        _line_ID = -1;
                        break;
                    case (IO_Type.IO):
                        _line_ID = -1;
                        break;                    
                }
               _output_Type = value;
            }
        }
        public int Line_ID {
            get {return _line_ID; }
            set { _line_ID = value; } }

        public Output_Class()
        {
            Output_Type = IO_Type.Null;
        }
    }
}
