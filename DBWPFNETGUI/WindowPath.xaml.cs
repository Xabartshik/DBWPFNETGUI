using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace DBWPFNETGUI
{
    /// <summary>
    /// Логика взаимодействия для WindowPath.xaml
    /// </summary>
    public partial class WindowPath : Window
    {
        public string chosen_path { get; set; }
        public WindowPath()
        {
            InitializeComponent();
        }

        private void ButtonEnter_Click(object sender, RoutedEventArgs e)
        {
            chosen_path = TBStringChoice.Text;
            this.Close();
        }
    }
}
