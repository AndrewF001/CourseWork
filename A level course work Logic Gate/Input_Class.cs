﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A_level_course_work_Logic_Gate
{
    public class Input_Class
    {
        public bool Input_bit { get; set; } = false;
        public int Input_ID { get; set; } = -1;
        public IO_Type Input_Type { get; set; } = IO_Type.Null;
        public int Line_ID { get; set; } = -1;
    }
}
