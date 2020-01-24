using System.Windows;

namespace A_level_course_work_Logic_Gate
{
    /// <summary>
    /// Interaction logic for Progress_Bar_Window.xaml
    /// </summary>
    public partial class Progress_Bar_Window : Window
    {
        public int Value
        {
            set
            {
                Bar.Value = value;
            }
        }

        public Progress_Bar_Window(int max)
        {
            InitializeComponent();
            Bar.Minimum = 0;
            Bar.Value = 0;
            Bar.Maximum = max;
        }        

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Bar.Maximum = 0;
        }

       

    }
}
