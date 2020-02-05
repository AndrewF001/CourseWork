using System.Windows;

namespace A_level_course_work_Logic_Gate
{
    /// <summary>
    /// Interaction logic for Progress_Bar_Window.xaml
    /// </summary>
    public partial class Progress_Bar_Window : Window
    {
        public Progress_Bar_Window(int max)
        {
            InitializeComponent();
            Bar.Minimum = 0;
            Bar.Value = 0;
            Bar.Maximum = max;
        }        
    }
}
