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

        //Command for closing the window
        private void CloseCommand(object sender, ExecutedRoutedEventArgs e)
        {
            if(editor == null || !editor.hasChanged || Save())
            {
                Close();
            }
        }

        //Command for opening a file
        private void OpenCommand(object sender, ExecutedRoutedEventArgs e)
        {
            //Getting the file name
            string file = OpenFile(".txt");

            //If the file name isn't empty
            if(file != "")
            {
                //If the editor is there and has not changed
                if (editor != null && editor.hasChanged)
                {
                    //If the user hasn't saved
                    if(!Save())
                    {
                        //Don't open
                        return;
                    }
                }

                //If the editor is not there
                else if(editor == null)
                {
                    //Open a new editor with a loaded png file
                    editor = new TileMapEditor(this, new BitmapImage(new Uri(OpenFile(".png"))));
                }

                //Load the file
                editor.LoadMap(file);
            }
        }

        //Command for creating a new map
        private void NewCommand(object sender, ExecutedRoutedEventArgs e)
        {
            //Making sure the user saved their work
            if(editor != null && editor.hasChanged)
            {
                if(!Save())
                {
                    return;
                }
            }

            //Creating a new map
            editor = NewMap();
        }

        //New map constructor
        private TileMapEditor NewMap()
        {
            //Catching any exceptions
            try
            {
                //creating a new property window
                MapPropertyWindow mpw = new MapPropertyWindow();
                
                //Showing the dialog box
                mpw.ShowDialog();

                //Setting a bitmap image to be the opened png file
                BitmapImage image = new BitmapImage(new Uri(OpenFile(".png")));

                //Setting the default zoom values
                zoom = 1;
                offsetX = 0;
                offsetY = 0;

                //Returning the values inputted into the map property window
                return new TileMapEditor(this, int.Parse(mpw.widthBox.Text), int.Parse(mpw.heightBox.Text), int.Parse(mpw.tileWidthBox.Text), int.Parse(mpw.tileHeightBox.Text), image);
            }

            //if anything goes wrong
            catch
            {
                //Report that something went wrong with a null value
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

        //Method to set the images inside the list view
        public void SetImages(List<CroppedBitmap> images)
        {

            //Taking all the images in the list and adding them
            listView1.Items.Clear();
            foreach(var image in images)
            {
                listView1.Items.Add(new Image() { Source = image });
            }

            //Setting the width of the listview
            listView1.Width = images[0].Width * 2 + 32;
        }

        //Method to handle the canvas properties
        public void SetCanvasProperties(int width, int height, int tileWidth, int tileHeight)
        {
            //Clearing the previous elements
            canvas1.Children.Clear();

            //Adding new rectangles to act as a grid
            canvas1.Children.Add(new Rectangle() { Width = width, Height = height, Stroke = Brushes.LightGray, Fill = Brushes.LightGray });

            //Creating a grid by filling the canvas with rectangles of the tileheight and width until the grid is full
            for (int y = 0; y < height; y += tileHeight)
            {
                for(int x = 0; x < width; x += tileWidth)
                {
                    //Adding the rectangles of the user specified size to the canvas
                    canvas1.Children.Add(new Rectangle() { Width = tileWidth, Height = tileHeight, Margin = new Thickness(x, y, 0, 0), Stroke = Brushes.Gray, StrokeThickness = 1 });
                }
            }
        }

        //Checks to see if a mouse button is clicked
        private void PressDownMouse(object sender, MouseEventArgs e)
        {
            //Checking to see if the editor window exists
            if (editor != null)
            {
                //If the editor is there check the button pressed

                //Left and right button presses will draw or erase respectively
                if (e.LeftButton == MouseButtonState.Pressed || e.RightButton == MouseButtonState.Pressed)
                {
                    MoveMouse(sender, e);
                }

                //If the middle button is pressed, store the mouse position
                else if (e.MiddleButton == MouseButtonState.Pressed)
                {
                    previousPosition = e.GetPosition(this);
                }
            }

        }

        //Method for mouse controls
        private void MoveMouse(object sender, MouseEventArgs e)
        {
            //If the editor is there
            if(editor != null)
            {

                //If the left button is pressed
                if(e.LeftButton == MouseButtonState.Pressed)
                {
                    //Draw the selected image in the listview at the selected location on the canvas
                    editor.Draw(e.GetPosition(canvas1), listView1.SelectedIndex);
                }

                //If the right button is pressed
                else if(e.RightButton == MouseButtonState.Pressed)
                {
                    //Draw a blank rectangle at the mouse position
                    editor.Draw(e.GetPosition(canvas1), -1);
                }

                //If the middle button is pressed
                else if(e.MiddleButton == MouseButtonState.Pressed)
                {
                    //Store the current position
                    var position = e.GetPosition(this);

                    //Find the difference between the previous position
                    var diff = position - previousPosition;

                    //Calculate the offsets
                    offsetX = offsetX + diff.X;
                    offsetY = offsetY + diff.Y;

                    //Zooming the canvas
                    MoveZoom();
                }

                //Setting the previous position
                previousPosition = e.GetPosition(this);
            }
        }

        //Method to add the images to the canvas
        internal void SetCanvasContent(int[,] map, int width, int height, int tileWidth, int tileHeight)
        {
            //If the editor is there
            if(editor != null)
            {
                //Clearing the canvas
                canvas2.Children.Clear();

                //Go through the grid
                for(int col = 0; col < height; col++)
                {
                    for(int row = 0; row < width; row++)
                    {
                        //Set the index to the current grid spot
                        int index = map[row, col];

                        //Checking the value
                        if (index != -1 && index < listView1.Items.Count)
                        {
                            //If the value is not -1 and is less than the total number of images the user loaded in

                            //Get the image being used
                            Image i = (Image)listView1.Items[index];
                            Image toDraw = new Image();

                            //Setting the image to the user defined properties
                            toDraw.Source = i.Source;
                            toDraw.Width = tileWidth;
                            toDraw.Height = tileHeight;

                            //Setting the margin to draw
                            toDraw.Margin = new Thickness(row * tileWidth, col * tileHeight, 0, 0);

                            //Adding the image to the canvas
                            canvas2.Children.Add(toDraw);
                        }
                    }
                }
            }
        }

        //Method to draw over a canvas space
        internal void SetCanvasContentDrawOver(int x, int y, int tileWidth, int tileHeight, int index)
        {
            //If editor is present
            if(editor != null)
            {
                //If the space isn't a blank rectangle
                if(index != -1)
                {
                    //Get the image to draw
                    Image i = (Image)listView1.Items[index];
                    Image toDraw = new Image();

                    //Adjust the properties of the image
                    toDraw.Source = i.Source;
                    toDraw.Width = tileWidth;
                    toDraw.Height = tileHeight;
                    toDraw.Margin = new Thickness(x * tileWidth, y * tileHeight, 0, 0);

                    //Draw the image
                    canvas2.Children.Add(toDraw);
                }

                //If the index is a blank rectangle
                else
                {
                    //Draw a blank rectangle
                    canvas2.Children.Add(new Rectangle() { Width = tileWidth, Height = tileHeight, Margin = new Thickness(x * tileWidth, y * tileHeight, 0, 0), Stroke = Brushes.Gray, Fill = Brushes.LightGray, StrokeThickness = 1 });
                }
            }
        }

        //Method to test if the left mouse button is up
        private void canvas2_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if(editor != null)
            {
                //Repaint the editor
                editor.Repaint();
            }
        }

        //If the mouse wheel is rolled
        private void canvas1_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (editor != null)
            {
                //If rolled forward
                if(e.Delta > 0)
                {
                    //Zoom by 25%
                    zoom = zoom * 1.25;
                }

                //If rolled backwards
                else if(e.Delta < 0)
                {
                    //Zoom out 25%
                    zoom = zoom / 1.25;
                }

                //Zoom by the value
                MoveZoom();
            }
        }

        //Method to zoom on the canvas
        private void MoveZoom()
        {
            //If the editor is there
            if(editor != null)
            {
                //Create a way to transform
                TransformGroup tg = new TransformGroup();

                //Add ways to scale and translate the canvas
                tg.Children.Add(new ScaleTransform(zoom, zoom));
                tg.Children.Add(new TranslateTransform(offsetX, offsetY));

                //Allow the canvases to use the scale and translate
                canvas1.RenderTransform = tg;
                canvas2.RenderTransform = tg;
            }
        }
    }
}