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

namespace Pente.XAML
{
    /// <summary>
    /// Interaction logic for GameBoard.xaml
    /// </summary>
    public partial class GameBoard : UserControl
    {
        public GameBoard()
        {
            InitializeComponent();
            DefineRows();
            DefineColumns();
            AddCanvasToEachSpace();
        }


        private void DefineRows()
        {
            for (int i = 0; i < 21; i++)
            {
                gameBoard.RowDefinitions.Add(new RowDefinition());
            }
        }

        private void DefineColumns()
        {
            for (int i = 0; i < 21; i++)
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
                for(int j = 0; j < rows; j++)
                {
                    Canvas canvas = new Canvas();
                    Grid.SetColumn(canvas, i);
                    Grid.SetRow(canvas, j);
                    Grid.SetColumnSpan(canvas, 1);
                    Grid.SetRowSpan(canvas, 1);
                    canvas.Name = "col" + i + "row" + j;

                    //Border border = new Border();
                    //border.BorderThickness = new Thickness(1);
                    //border.BorderBrush = Brushes.Brown;

                    //canvas.Children.Add(border);

                    if (i == 20 || i == 0 || j == 0 || j == 20)
                    {
                        canvas.Background = Brushes.DarkSeaGreen;
                    }
                    else
                    {
                        canvas.Background = Brushes.DarkOrange;
                    }

                    //if (canvas.Name == "col10row15")
                    //{
                    //    canvas.Background = Brushes.DarkRed;
                    //}

                    gameBoard.Children.Add(canvas);
                }
            }
        }
    }
}
