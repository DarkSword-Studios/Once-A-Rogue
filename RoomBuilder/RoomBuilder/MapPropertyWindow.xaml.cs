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
    //Ian Moon
    //3/3/2016
    //This class creates a window where the user can modify the height and width of the canvas, as well as the height and width of the tiles.
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
