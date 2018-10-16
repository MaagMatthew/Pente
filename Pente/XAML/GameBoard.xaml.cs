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
using System.Windows.Shapes;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Pente.XAML
{
    /// <summary>
    /// Interaction logic for GameBoard.xaml
    /// </summary>
    public partial class GameBoard : UserControl
    {
        private bool IsFirstPlayer { get; set; }
        private string Player1Name, Player2Name;
        private bool IsCPUPlaying;

        public GameBoard(int squareSize, string Player1, string Player2,bool isCPUPlaying)
        {
            InitializeComponent();
            BuildBoard(squareSize);
            AddCanvasToEachSpace();
            Player1Name = Player1;
            Player2Name = Player2;
            IsCPUPlaying = isCPUPlaying;
            IsFirstPlayer = true;
        }

        private void BuildBoard(int squareSize)
        {
            DefineColumns(squareSize);
            DefineRows(squareSize);
        }
        private void DefineRows(int rows)
        {
            for (int i = 0; i < rows; i++)
            {
                gameBoard.RowDefinitions.Add(new RowDefinition());
            }
        }

        private void DefineColumns(int columns)
        {
            for (int i = 0; i < columns; i++)
            {
                gameBoard.ColumnDefinitions.Add(new ColumnDefinition());
            }
        }
        private void AddCanvasToEachSpace()
        {
            int columns = gameBoard.ColumnDefinitions.Count();
            int rows = gameBoard.RowDefinitions.Count();

            for (int i = 0; i < columns; i++)
            {
                for (int j = 0; j < rows; j++)
                {
                    Border b = new Border();
                    Grid.SetColumn(b, i);
                    Grid.SetRow(b, j);
                    Grid.SetColumnSpan(b, 1);
                    Grid.SetRowSpan(b, 1);
                    b.BorderBrush = Brushes.Brown;
                    b.BorderThickness = new Thickness(1);

                    Canvas c = new Canvas();
                    Grid.SetColumn(c, i);
                    Grid.SetRow(c, j);
                    Grid.SetColumnSpan(c, 1);
                    Grid.SetRowSpan(c, 1);
                    c.Name = "col" + i + "row" + j;

                    c.Background = Brushes.DarkOrange;

                    //Each Canvas has a Name like "col" "10" "row" "15" (Example Below, Uncomment to see Example on Run) 
                    //if (canvas.Name == "col10row15")
                    //{
                    //    canvas.Background = Brushes.DarkRed;
                    //}

                    gameBoard.Children.Add(c);
                }
            }
        }

        private void LeftButtonPlace(object sender, MouseButtonEventArgs e)
        {
            var selectedCanvas = e.Source as Canvas;

            //Did player Click on a canvas
            //if so create a stone
            //color the stone
            //set margins so its close to the center of canvas
            //place the stone on canvas
            //and switch turns

            if (selectedCanvas != null)
            {
                if (selectedCanvas.Children.Count < 1)
                {

                    Ellipse shape = CreateShape();
                    ColorStone(shape);
                    PlaceStone(shape);
                    selectedCanvas.Children.Add(shape);
                    SwitchTurn();
                }
            }
        }

        private Ellipse CreateShape()
        {
            Ellipse shape = new Ellipse()
            {
                Height = this.ActualHeight / 30,
                Width = this.ActualWidth / 30,
            };
            return shape;
        }

        private void ColorStone(Shape shape)
        {

            if (IsFirstPlayer)
            {
                shape.Fill = Brushes.Aqua;
            }
            else
            {
                shape.Fill = Brushes.Yellow;

            }
        }
        private void PlaceStone(Shape shape)
        {
            double left = shape.Width / 10;
            double top = shape.Height / 10;
            shape.Margin = new Thickness(left, top, 0, 0);
        }

        private void SwitchTurn()
        {
            IsFirstPlayer = !IsFirstPlayer;
        }

        private void RightButtonRemove(object sender, MouseButtonEventArgs e)
        {
            var selectedCanvas = e.Source as Canvas;
            var selectedShape = e.Source as Shape;

            //Did I click on a canvas
            //if i  did remove all children
            // if not did i click on a shaped
            //if i did remove it from canvas

            if (selectedCanvas != null)
            {
                selectedCanvas.Children.Clear();
            }

            else if (selectedShape != null)
            {
                var parentOf = selectedShape.Parent as Canvas;
                if (parentOf != null)
                {
                    parentOf.Children.Remove(selectedShape);
                }
            }
        }
    }
}
