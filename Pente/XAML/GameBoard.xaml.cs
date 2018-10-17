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
using System.ComponentModel;

namespace Pente.XAML
{
    /// <summary>
    /// Interaction logic for GameBoard.xaml
    /// </summary>
    public partial class GameBoard : UserControl, INotifyPropertyChanged
    {
        private bool IsFirstPlayer;
        private string Player1Name, Player2Name;
        private bool IsCPUPlaying;

        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChange(string info)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(info));
        }

        public string CurrentPlayerName {
            get { return CurrentPlayerName; }
            set { CurrentPlayerName = value; NotifyPropertyChange("Player"); }
        }

        public GameBoard(int squareSize, string Player1, string Player2, bool isCPUPlaying)
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
                    Border border = CreateCanvasBorder(i, j);
                    Canvas canvas = CreateCanvas(i,j, border);

                    gameBoard.Children.Add(border);
                }
            }
        }

        //Create Canvas
        private Canvas CreateCanvas(int row,int column,Border border)
        {

            Canvas canvas = new Canvas();

            Grid.SetColumn(canvas, column);
            Grid.SetRow(canvas, row);


            canvas.Background = Brushes.Orange;

            border.Child = canvas;

            canvas.Name = $"R{row}C{column}";

            return canvas;
        }

        //Create Border
        private Border CreateCanvasBorder(int row, int column)
        {
            Border border = new Border();
            Grid.SetColumn(border,column);
            Grid.SetRow(border,row);
            border.BorderBrush = Brushes.LimeGreen;
            border.BorderThickness = new Thickness(1);
            return border;
        }

        //Did player Click on a canvas
        //if so create a stone
        //color the stone
        //set margins so its close to the center of canvas
        //place the stone on canvas
        //and switch turns
        private void LeftButtonPlace(object sender, MouseButtonEventArgs e)
        {
            var selectedCanvas = e.Source as Canvas;


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
            double rowCount = gameBoard.RowDefinitions.Count;
            double columnCount = gameBoard.ColumnDefinitions.Count;

            Ellipse shape = new Ellipse()
            {
                Height = this.ActualHeight / (rowCount * 1.5),
                Width = this.ActualWidth / (columnCount * 1.5),
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
            double left = shape.Width/3;
            double top = shape.Height/3;
            shape.Margin = new Thickness(left, top, 0, 0);
        }

        public void SwitchTurn()
        {
            IsFirstPlayer = !IsFirstPlayer;
            if (CurrentPlayerName == Player1Name)
            {
                CurrentPlayerName = Player2Name;
            }
            else
            {
                CurrentPlayerName = Player1Name;
            }
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
