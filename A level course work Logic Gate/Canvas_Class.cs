using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;

namespace A_level_course_work_Logic_Gate
{
    public class Canvas_Class : Canvas
    {
        MainWindow _MainWind { get; set; }
        public Canvas_Class(MainWindow MainWind)
        {
            _MainWind = MainWind;
            Background = Brushes.Gray;
        }



    }
}
