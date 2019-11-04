using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A_level_course_work_Logic_Gate
{
    public class Input_Class
    {
        private bool _input_bit = false;
        private int _input_ID = -1;
        private IO_Type _input_Type = IO_Type.Null;
        private int _line_ID = -1;
        public bool Input_bit {
            get { return _input_bit; }
            set { _input_bit = value; }
        }
        public int Input_ID {
            get { return _input_ID; }
            set { _input_ID = value; } }
        public IO_Type Input_Type
        {
            get { return _input_Type; }
            set { 
                switch(value)
                {
                    case (IO_Type.Null):
                        _input_ID = -1;
                        _line_ID = -1;
                        break;
                    case (IO_Type.Gate):
                        _input_bit = false;
                        break;
                    case (IO_Type.IO):
                        _line_ID = -1;
                        _input_bit = false;
                        break;
                }
                _input_Type = value;
            }
        }
        public int Line_ID {
            get { return _line_ID; }
            set { _line_ID = value; } }

        public Input_Class()
        {
            Input_Type = IO_Type.Null;
        }
    }
}
