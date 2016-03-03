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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;

namespace RoomBuilder
{
   public partial class MainWindow : Window
    {
        //Creating an instance of an editor
        private TileMapEditor editor;

        //The distance the mouse has moved from the origin
        private double offsetX;
        private double offsetY;

        //Varible for zooming
        private double zoom = 1;

        //Previous position of the mouse
        private Point previousPosition;

        public MainWindow()
        {
            //Adding commands to the buttons that can be pressed
            CommandBindings.Add(new CommandBinding(ApplicationCommands.New, new ExecutedRoutedEventHandler(NewCommand)));
            CommandBindings.Add(new CommandBinding(ApplicationCommands.Open, new ExecutedRoutedEventHandler(OpenCommand)));
            CommandBindings.Add(new CommandBinding(ApplicationCommands.Save, new ExecutedRoutedEventHandler(SaveCommand)));
            CommandBindings.Add(new CommandBinding(ApplicationCommands.Close, new ExecutedRoutedEventHandler(CloseCommand)));
            CommandBindings.Add(new CommandBinding(ApplicationCommands.Properties, new ExecutedRoutedEventHandler(PropertiesCommand)));
            CommandBindings.Add(new CommandBinding(ApplicationCommands.Replace, new ExecutedRoutedEventHandler(TilesetCommand)));

            InitializeComponent();
        }

        //Event Handler for the save button
        private void SaveCommand(object sender, ExecutedRoutedEventArgs e)
        {
            //If there is is a TileMapEditor initialized
            if (editor != null)
            {
                Save();
            }
        }

        private bool Save()
        {
            //Opening the Microsoft box for saving files
            Microsoft.Win32.SaveFileDialog sdlg = new Microsoft.Win32.SaveFileDialog();

            //Setting the default extension
            sdlg.DefaultExt = ".txt";

            //Setting the filter
            sdlg.Filter = "Roombuilder|*.txt";

            //Seeing if the box can be shown
            bool? result = sdlg.ShowDialog();

            //If the box can be shown
            if(result == true)
            {
                //Save the file
                editor.Save(sdlg.FileName);

                //Change to nothing has changed
                editor.hasChanged = false;

                //return that the file saved
                return true;
            }

            //If nothing saved, return false
            return false;

        }

        //Event Handler for loading in a tile set image
        private void TilesetCommand(object sender, ExecutedRoutedEventArgs e)
        {
            if(editor != null)
            {
                //Getting the image that will be used as the tileset
                BitmapImage image = new BitmapImage(new Uri(OpenFile(".png")));
            }
        }

        private void PropertiesCommand(object sender, ExecutedRoutedEventArgs e)
        {
            if(editor != null)
            {
                //Creating a property window box
                MapPropertyWindow mpw = new MapPropertyWindow();

                //Displaying the box
                mpw.ShowDialog();

                //Setting the properties to those that are specified in the box
                editor.SetProperties(int.Parse(mpw.widthBox.Text), int.Parse(mpw.heightBox.Text), int.Parse(mpw.tileWidthBox.Text), int.Parse(mpw.tileHeightBox.Text));
            }
        }

        private void CloseCommand(object sender, ExecutedRoutedEventArgs e)
        {
            if(editor == null || !editor.hasChanged || Save())
            {
                Close();
            }
        }

        private void OpenCommand(object sender, ExecutedRoutedEventArgs e)
        {
            string file = OpenFile(".txt");

            if(file != "")
            {
                if (editor != null && editor.hasChanged)
                {
                    if(!Save())
                    {
                        return;
                    }
                }

                else if(editor == null)
                {
                    editor = new TileMapEditor(this, new BitmapImage(new Uri(OpenFile(".png"))));
                }

                editor.LoadMap(file);
            }
        }

        private void NewCommand(object sender, ExecutedRoutedEventArgs e)
        {
            if(editor != null && editor.hasChanged)
            {
                if(!Save())
                {
                    return;
                }
            }

            editor = NewMap();
        }

        private TileMapEditor NewMap()
        {
            try
            {
                MapPropertyWindow mpw = new MapPropertyWindow();
                mpw.ShowDialog();

                BitmapImage image = new BitmapImage(new Uri(OpenFile(".png")));

                zoom = 1;
                offsetX = 0;
                offsetY = 0;

                return new TileMapEditor(this, int.Parse(mpw.widthBox.Text), int.Parse(mpw.heightBox.Text), int.Parse(mpw.tileWidthBox.Text), int.Parse(mpw.tileHeightBox.Text), image);
            }

            catch
            {
                return null;
            }
        }

        //Method used to open a file
        public string OpenFile(string fileType)
        {
            //Opening the Windows Box to select a file
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

            //Setting the default file extension
            dlg.DefaultExt = fileType;

            //Filtering the files by the extension
            dlg.Filter = fileType + "|*" + fileType;

            //Showing the dialog box
            Nullable<bool> result = dlg.ShowDialog();

            //If the dialog box opens
            if(result == true)
            {
                //Open the selected file
                return dlg.FileName;
            }

            //Return an empty string if nothing else is returned
            return "";
        }

        public void SetImages(List<CroppedBitmap> images)
        {
            listView1.Items.Clear();
            foreach(var image in images)
            {
                listView1.Items.Add(new Image() { Source = image });
            }
            listView1.Width = images[0].Width * 2 + 32;
        }

        public void SetCanvasProperties(int width, int height, int tileWidth, int tileHeight)
        {
            canvas1.Children.Clear();
            canvas1.Children.Add(new Rectangle() { Width = width, Height = height, Stroke = Brushes.LightGray, Fill = Brushes.LightGray });
            for(int x = 0; x < height; x += tileHeight)
            {
                for(int y = 0; y < width; y += tileWidth)
                {
                    canvas1.Children.Add(new Rectangle() { Width = tileWidth, Height = tileHeight, Margin = new Thickness(y, x, 0, 0), Stroke = Brushes.Gray, StrokeThickness = 1 });
                }
            }
        }

        private void PressDownMouse(object sender, MouseEventArgs e)
        {
            if (editor != null)
            {
                if (e.LeftButton == MouseButtonState.Pressed || e.RightButton == MouseButtonState.Pressed)
                {
                    MoveMouse(sender, e);
                }
                else if (e.MiddleButton == MouseButtonState.Pressed)
                {
                    previousPosition = e.GetPosition(this);
                }
            }

        }

        private void MoveMouse(object sender, MouseEventArgs e)
        {
            if(editor != null)
            {
                if(e.LeftButton == MouseButtonState.Pressed)
                {
                    editor.Draw(e.GetPosition(canvas1), listView1.SelectedIndex);
                }

                else if(e.RightButton == MouseButtonState.Pressed)
                {
                    editor.Draw(e.GetPosition(canvas1), -1);
                }

                else if(e.MiddleButton == MouseButtonState.Pressed)
                {
                    var position = e.GetPosition(this);
                    var diff = position - previousPosition;
                    offsetX = offsetX + diff.X;
                    offsetY = offsetY + diff.Y;

                    MoveZoom();
                }

                previousPosition = e.GetPosition(this);
            }
        }

        internal void SetCanvasContent(int[,] map, int width, int height, int tileWidth, int tileHeight)
        {
            if(editor != null)
            {
                canvas2.Children.Clear();
                for(int col = 0; col < height; col++)
                {
                    for(int row = 0; row < width; row++)
                    {
                        int index = map[row, col];
                        if (index != -1 && index < listView1.Items.Count)
                        {
                            Image i = (Image)listView1.Items[index];
                            Image toDraw = new Image();
                            toDraw.Source = i.Source;
                            toDraw.Width = tileWidth;
                            toDraw.Height = tileHeight;
                            toDraw.Margin = new Thickness(row * tileWidth, col * tileHeight, 0, 0);
                            canvas2.Children.Add(toDraw);
                        }
                    }
                }
            }
        }

        internal void SetCanvasContentDrawOver(int x, int y, int tileWidth, int tileHeight, int index)
        {
            if(editor != null)
            {
                if(index != -1)
                {
                    Image i = (Image)listView1.Items[index];
                    Image toDraw = new Image();
                    toDraw.Source = i.Source;
                    toDraw.Width = tileWidth;
                    toDraw.Height = tileHeight;
                    toDraw.Margin = new Thickness(x * tileWidth, y * tileHeight, 0, 0);
                    canvas2.Children.Add(toDraw);
                }
                else
                {
                    canvas2.Children.Add(new Rectangle() { Width = tileWidth, Height = tileHeight, Margin = new Thickness(x * tileWidth, y * tileHeight, 0, 0), Stroke = Brushes.Gray, Fill = Brushes.LightGray, StrokeThickness = 1 });
                }
            }
        }

        private void canvas2_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if(editor != null)
            {
                editor.Repaint();
            }
        }

        private void canvas1_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (editor != null)
            {
                if(e.Delta > 0)
                {
                    zoom = zoom * 1.25;
                }

                else if(e.Delta < 0)
                {
                    zoom = zoom / 1.25;
                }

                MoveZoom();
            }
        }

        private void MoveZoom()
        {
            if(editor != null)
            {
                TransformGroup tg = new TransformGroup();
                tg.Children.Add(new ScaleTransform(zoom, zoom));
                tg.Children.Add(new TranslateTransform(offsetX, offsetY));

                canvas1.RenderTransform = tg;
                canvas2.RenderTransform = tg;
            }
        }
    }
}