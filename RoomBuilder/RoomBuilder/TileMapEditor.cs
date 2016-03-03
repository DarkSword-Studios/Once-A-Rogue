using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Imaging;
using System.Windows.Controls;
using System.Windows.Media;
using System.IO;

namespace RoomBuilder
{
    class TileMapEditor
    {
        //Creating a main window
        private MainWindow editorWindow;

        //Creating a tile map that will be used to place images
        private int[,] tileMap;

        //Height and Width for the tile map
        private int height;
        private int width;

        //Bool to see if the user has changed anything since last save
        public bool hasChanged { get; set; }

        //Height and width for the tiles
        private int tileWidth = 64;
        private int tileHeight = 64;

        //Creating a bitmap source for the tile set
        private BitmapSource tileSet;

        //Creating a list that will store the images in the tileset
        private List<CroppedBitmap> images = new List<CroppedBitmap>();

        //Parameterized constructor
        public TileMapEditor(MainWindow window, int width, int height, int tileWidth, int tileHeight, BitmapSource bitmapSrc)
        {
            editorWindow = window;
            tileSet = bitmapSrc;
            SetProperties(width, height, tileWidth, tileHeight);

            //Content has changed
            hasChanged = true;

            Initialize();
        }

        //Parameterized Constructor
        public TileMapEditor(MainWindow mainWindow, BitmapImage bitmapImg)
        {
            editorWindow = mainWindow;
            tileSet = bitmapImg;
        }

        //Method used to set the grid and tile variables to whatever the user passes in
        internal void SetProperties(int width, int height, int tileWidth, int tileHeight)
        {
            //If the tilemap has not been initialized yet
            if (tileMap == null)
            {
                //Initialize the map with the width and height that the user specifies
                tileMap = new int[width, height];

                //Change the height and width of the map to the values passed in
                this.width = width;
                this.height = height;

                //Set the tile height and width to the values passed in
                this.tileWidth = tileWidth;
                this.tileHeight = tileHeight;

                //Set the rows and colums of the map based on the width and height
                for (int col = 0; col < height; col++)
                {
                    for (int row = 0; row < width; row++)
                    {
                        //Fill the array with -1's to show there is not currently an image being stored and instantiate each value
                        tileMap[row, col] = -1;
                    }
                }
            }

            //If there is already a tilemap
            else
            {
                //Create a new map
                int[,] newTileMap = new int[width, height];

                //Set the colums and rows of the new map with the passed in values
                for (int col = 0; col < height; col++)
                {
                    for (int row = 0; row < width; row++)
                    {
                        //Fill the array with -1's to show there is not currently an image being stored and instantiate each value
                        newTileMap[row, col] = -1;
                    }
                }

                //Set the colums and rows of the old map to the values of the new map
                for (int col = 0; col < height; col++)
                {
                    for (int row = 0; row < width; row++)
                    {
                        //Preserving tiles that can fit from the old map.
                        newTileMap[row, col] = tileMap[row, col];
                    }
                }

                //Setting the variables to the newly passed in values
                this.tileWidth = tileWidth;
                this.tileHeight = tileHeight;

                this.width = width;
                this.height = height;

                //Setting the tile Map to be the newly created map
                tileMap = newTileMap;
            }

            Initialize();
        }

        private void Initialize()
        {
            //Clear the current list of images
            images.Clear();

            //Cropping images to the specified height and width from an image
            for (int y = 0; y < tileSet.PixelHeight; y += tileHeight)
            {
                for (int x = 0; x < tileSet.PixelWidth; x += tileWidth)
                {
                    try
                    {
                        //Adding each image to the image list until the entire image is cropped
                        CroppedBitmap cb = new CroppedBitmap(tileSet, new System.Windows.Int32Rect(x, y, tileWidth, tileHeight));
                        images.Add(cb);
                    }

                    catch
                    {

                    }
                }
            }

            editorWindow.SetImages(images);
            editorWindow.SetCanvasProperties(width * tileWidth, height * tileHeight, tileWidth, tileHeight);
            editorWindow.SetCanvasContent(tileMap, width, height, tileWidth, tileHeight);
        }

        internal void Draw(System.Windows.Point point, int p)
        {
            //Getting the point clicked upon
            int x = (int)point.X / tileWidth;
            int y = (int)point.Y / tileHeight;

            //If the x and y values are within the height and width of the canvas and the spot being clicked
            //on doesn't equal the tile we are trying to draw already
            if(x >= 0 && x < width && y >= 0 && y < height && tileMap[x,y] != p)
            {
                //Set the tilemap value at the position at the point of the cursor equal to the value passed in
                tileMap[x, y] = p;

                hasChanged = true;

                editorWindow.SetCanvasContentDrawOver(x, y, tileWidth, tileHeight, p);
            }
        }

        //Method to repaint the canvas
        internal void Repaint()
        {
            //Restoring the canvas
            editorWindow.SetCanvasContent(tileMap, width, height, tileWidth, tileHeight);
        }

        //Method to save the values in the grid in a text file
        internal void Save(string filePath)
        {
            //Formatting the string
            StringBuilder sb = new StringBuilder();

            //Going through the grid and writing each value followed by a comma
            for(int col = 0; col < height; col++)
            {
                for (int row = 0; row < width; row++)
                {
                    sb.Append(tileMap[row, col] + ",");
                }

                sb.Remove(sb.Length - 1, 1).AppendLine("");
            }

            //Writing the string to a file
            File.WriteAllText(filePath, sb.ToString());
        }

        //Method to load the tile map
        internal void LoadMap(string filePath)
        {
            //Creating a string array to store the lines in the file
            string[] lines = File.ReadAllLines(filePath);

            //Setting the height to be equal to the length of the lines array
            height = lines.Length;

            //Setting the width to the number of entries per line
            width = lines[0].Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Length;

            //Set the tileMap to be of the size read from the file
            tileMap = new int[width, height];

            //Creating two variables to store the values in the lines array
            int x = 0;
            int y = 0;

            //For each line in lines
            foreach(var line in lines)
            {
                //Start at the beginning when the line switches
                x = 0;

                //Then, split up the line and set the value in the tile map to the value
                foreach(var num in line.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    tileMap[x, y] = int.Parse(num);

                    //Move forward one position
                    x = x + 1;
                }

                //Move down a row
                y = y + 1;

                //Initialize and repaint the canvas
                Initialize();
                Repaint();
            }
        }

        //Setting the tileset
        private void SetTileset(BitmapImage image)
        {
            tileSet = image;
            Initialize();
        }
    }
}
