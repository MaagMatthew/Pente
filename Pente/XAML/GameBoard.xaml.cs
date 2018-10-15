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
        public bool IsFirstPalyer { get; set; }
        public GameBoard()
        {
            InitializeComponent();
            DefineRows();
            DefineColumns();
            AddCanvasToEachSpace();
            IsFirstPalyer = true;
        }

        #region Creating Board
        private void DefineRows()
        {
            for (int i = 0; i < 19; i++)
            {
                gameBoard.RowDefinitions.Add(new RowDefinition());
            }
        }

        private void DefineColumns()
        {
            for (int i = 0; i < 19; i++)
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

                    //if (canvas.Name == "col10row15")
                    //{
                    //    canvas.Background = Brushes.DarkRed;
                    //}

                    gameBoard.Children.Add(c);
                }
            }
        }
        #endregion

        private void LeftButtonPlace(object sender, MouseButtonEventArgs e)
        {
            var selectedCanvas = e.Source as Canvas;

            if (selectedCanvas != null)
            {
                if (selectedCanvas.Children.Count < 1)
                {
                    int column = Grid.GetColumn(selectedCanvas);
                    int row = Grid.GetRow(selectedCanvas);

                    Ellipse shape = new Ellipse()
                    {
                        Height = 20,
                        Width = 20,
                    };

                    if (IsFirstPalyer)
                    {
                        shape.Fill = Brushes.Aqua;
                        IsFirstPalyer = !IsFirstPalyer;
                    }
                    else
                    {
                        shape.Fill = Brushes.Yellow;
                        IsFirstPalyer = !IsFirstPalyer;
                    }
                    double left = shape.Width / 2;
                    double top = shape.Height / 2;
                    shape.Margin = new Thickness(left, top, 0, 0);
                    selectedCanvas.Children.Add(shape);
                }
            }
        }

        private void RightButtonRemove(object sender, MouseButtonEventArgs e)
        {
            var selectedCanvas = e.Source as Canvas;
            var selectedShape = e.Source as Shape;
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

        private void RemoveShapeCanvas(object sender, MouseButtonEventArgs e)
        {
            Shape clicked = e.OriginalSource as Shape;
            if (clicked != null)
            {
            }
        }
    }
}
