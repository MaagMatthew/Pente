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
            //gameBoard.ShowGridLines = true;
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

                    if (i == 20 || i == 0 || j == 0 || j == 20)
                    {
                        c.Background = Brushes.DarkSeaGreen;
                    }
                    else
                    {
                        c.Background = Brushes.DarkOrange;
                    }

                    //Each Canvas has a Name like "col" "10" "row" "15" (Example Below, Uncomment to see Example on Run) 
                    //if (canvas.Name == "col10row15")
                    //{
                    //    canvas.Background = Brushes.DarkRed;
                    //}

                    gameBoard.Children.Add(c);

                    if (i < 20 && i > 0 && j > 0 && j < 20)
                    {
                        gameBoard.Children.Add(b);
                    }
                }
            }
        }
    }
}
