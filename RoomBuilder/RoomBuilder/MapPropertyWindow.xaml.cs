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

namespace RoomBuilder
{
    /// <summary>
    /// Interaction logic for MapPropertWindow.xaml
    /// </summary>
    public partial class MapPropertyWindow : Window
    {
        public MapPropertyWindow()
        {
            InitializeComponent();
        }

        private void confirmBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
