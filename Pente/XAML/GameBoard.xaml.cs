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
        #region Properties
        public bool _HasWinner { get; private set; }
        public bool HasWinner
        {
            get { return this._HasWinner; }
            set
            {
                _HasWinner = value;
                NotifyGameEnded("Winner");
                return;
            }
        }
        private bool IsCPUPlaying;
        private bool IsFirstPlayer;
        private string Player1Name, Player2Name;
        public string CurrentPlayerName
        {
            get { return this._CurrentPlayer; }
            set
            {
                _CurrentPlayer = value;
                NotifyPropertyChange("Current");
                return;
            }
        }
        public string _CurrentPlayer { get; private set; }
        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyGameEnded(string info)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(info));
        }
        public void NotifyPropertyChange(string info)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(info));
        }
        #endregion
        #region Initialization
        public GameBoard(int squareSize, string Player1, string Player2, bool isCPUPlaying)
        {
            InitializeComponent();
            BuildBoard(squareSize);
            AddCanvasToEachSpace();
            Player1Name = Player1;
            Player2Name = Player2;
            CurrentPlayerName = Player1;
            IsCPUPlaying = isCPUPlaying;
            IsFirstPlayer = true;
        }

        //Create Grid Based on User Input
        private void BuildBoard(int squareSize)
        {
            DefineColumns(squareSize);
            DefineRows(squareSize);
        }
        //Add Rows
        private void DefineRows(int rows)
        {
            for (int i = 0; i < rows; i++)
            {
                gameBoard.RowDefinitions.Add(new RowDefinition());
            }
        }

        //Add Columns
        private void DefineColumns(int columns)
        {
            for (int i = 0; i < columns; i++)
            {
                gameBoard.ColumnDefinitions.Add(new ColumnDefinition());
            }
        }

        //Add canvas to Each Row and column
        private void AddCanvasToEachSpace()
        {
            int columns = gameBoard.ColumnDefinitions.Count();
            int rows = gameBoard.RowDefinitions.Count();

            for (int i = 0; i < columns; i++)
            {
                for (int j = 0; j < rows; j++)
                {
                    Border border = CreateCanvasBorder(i, j);
                    Canvas canvas = CreateCanvas(i, j, border);

                    gameBoard.Children.Add(border);
                }
            }
        }

        //Create Canvas
        private Canvas CreateCanvas(int row, int column, Border border)
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
            Grid.SetColumn(border, column);
            Grid.SetRow(border, row);
            border.BorderBrush = Brushes.Black;
            border.BorderThickness = new Thickness(1);
            return border;
        }
        #endregion
        #region Stone Creation

        //User Mouse Event if left mouse button is clicked or screen touched 
        private void LeftButtonPlace(object sender, MouseButtonEventArgs e)
        {
            //Make sure game is not over
            //Did player Click on a canvas
            //if so create a stone
            //color the stone
            //set margins so its close to the center of canvas
            //place the stone on canvas
            //Check for Win Conditions or Removal
            //and switch turns
            if (!HasWinner)
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
                        CheckWin(selectedCanvas);
                        if (IsCPUPlaying)
                        {
                            SwitchTurn();
                            TakeCPUTurn();
                            MessageBox.Show("CPU Has Taken Turn");
                        }
                        SwitchTurn();
                    }
                }
            }
        }
        public void TakeCPUTurn()
        {
            Random r = new Random();
            bool IsValidSpot = false;
            int selectedColumn;
            int selectedRow;
            do
            {
                selectedColumn = r.Next(0, gameBoard.ColumnDefinitions.Count);
                selectedRow = r.Next(0, gameBoard.RowDefinitions.Count);
                Canvas canvas = GetCanvas(selectedColumn, selectedRow);
                if (canvas.Children.Count < 1)
                {
                    Ellipse shape = CreateShape();
                    ColorStone(shape);
                    PlaceStone(shape);
                    canvas.Children.Add(shape);
                    CheckWin(canvas);
                    IsValidSpot = true;
                }
            } while (!IsValidSpot);
        }

        //Craetes Ellipse
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

        //Colors Stone Depending on Turn
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

        //Places Stone in the middle of the Canvas
        private void PlaceStone(Shape shape)
        {
            double left = shape.Width / 4.5;
            double top = shape.Height / 4.5;
            shape.Margin = new Thickness(left, top, 0, 0);
        }

        #endregion
        #region Taking Turn(Main,Helper,Check)
        #region Main
        //Check for capture and wins
        public void CheckWin(Canvas currentCanvas)
        {
            //Setup

            //Var to chech how many boxes we are away from the starting point
            int positionsAway = 0;

            //What color is Considered Friendly
            SolidColorBrush friendlyColor = GetFriendlyColor();

            //Counters to setup win/capture
            int friendlyCounter = 0, enemyCounter = 0;

            //List of enemies positions(row,column) may end up being removed 
            List<int> enemyPositions = new List<int>();

            //Get Row and Column of the starting point
            int currentRow = Grid.GetRow(currentCanvas);
            int currentColumn = Grid.GetColumn(currentCanvas);

            CheckUpLeft(currentRow, currentColumn, positionsAway, friendlyCounter, enemyCounter, enemyPositions, friendlyColor);
            CheckDownLeft(currentRow, currentColumn, positionsAway, friendlyCounter, enemyCounter, enemyPositions, friendlyColor);
            CheckUpRight(currentRow, currentColumn, positionsAway, friendlyCounter, enemyCounter, enemyPositions, friendlyColor);
            CheckDownRight(currentRow, currentColumn, positionsAway, friendlyCounter, enemyCounter, enemyPositions, friendlyColor);
            CheckLeft(currentRow, currentColumn, positionsAway, friendlyCounter, enemyCounter, enemyPositions, friendlyColor);
            CheckRight(currentRow, currentColumn, positionsAway, friendlyCounter, enemyCounter, enemyPositions, friendlyColor);
            CheckUp(currentRow, currentColumn, positionsAway, friendlyCounter, enemyCounter, enemyPositions, friendlyColor);
            CheckDown(currentRow, currentColumn, positionsAway, friendlyCounter, enemyCounter, enemyPositions, friendlyColor);
        }

        //Switch names and boolean
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

        #endregion
        #region Helper Methods
        //Remove stones from these positions (Row,Column)
        private void CaptureStones(List<int> positions)
        {
            if (positions.Count == 4)
            {
                for (int i = 0; i < 4; i += 2)
                {
                    Canvas canvas = GetCanvas(positions[i + 1], positions[i]);
                    canvas.Children.Clear();
                }
            }
        }

        //Check if a canvas has a stone of a certain color
        private bool HasSameColorStone(SolidColorBrush friendlyColor, Canvas neighbor)
        {
            Ellipse neighborEllipse = GetStone(neighbor);
            if (neighborEllipse.Fill == friendlyColor)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        //Gets a stone From a certain Canvas
        private Ellipse GetStone(Canvas canvas)
        {
            foreach (Ellipse ellipse in canvas.Children)
            {
                return ellipse;
            }
            return new Ellipse();
        }

        //Returns a color depending on turn
        private SolidColorBrush GetFriendlyColor()
        {
            if (IsFirstPlayer)
            {
                return Brushes.Aqua;
            }
            else
            {
                return Brushes.Yellow;
            }
        }

        //Makes sure these points arent the same
        private bool IsStartingPoint(int rowLeft, int columnLeft, int rowRight, int columnRight)
        {
            if (rowLeft != rowRight && columnLeft != columnRight)
            {
                return false;
            }
            else if (rowLeft == rowRight && columnLeft != columnRight)
            {
                return false;
            }
            else if (rowLeft != rowRight && columnLeft == columnRight)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        //Gets a canvas from a certain Position
        public Canvas GetCanvas(int column, int row)
        {
            foreach (Border item in gameBoard.Children)
            {
                if (Grid.GetRow(item) == row && Grid.GetColumn(item) == column)
                {
                    return (Canvas)item.Child;
                }
            }
            return null;
        }

        //Return if this position is within bounds
        private bool IsValidPosition(int number)
        {
            int max = gameBoard.ColumnDefinitions.Count;
            if (number >= 0 && number < max)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        #endregion
        #region Check Methods
        //Go through Up a row and Left a column searching for an stone of the same color, enemy stone, or space
        public void CheckUpLeft(int startRow, int startColumn, int positionsAway, int friendlyCounter, int enemyCounter, List<int> enemyPositions, SolidColorBrush friendlyColor)
        {

            int columnOffset = startColumn - 1;
            int rowOffset = startRow - 1;
            positionsAway = 0;
            enemyPositions.Clear();
            friendlyCounter = 0;
            enemyCounter = 0;

            while (positionsAway < 5)
            {
                positionsAway++;

                //Makes sure that the spot we are looking in is within the borders
                if (IsValidPosition(columnOffset) && IsValidPosition(rowOffset))
                {
                    //Get the canvas we are looking At
                    Canvas neighborCanvas = GetCanvas(columnOffset, rowOffset);

                    //If there is no canvas continue to the next neighbor
                    //If the canvas has the same color stone add to friendly count
                    ////If there was an two enemies between start point and the fourth *In Palatine Voice* kill'em...kill'em now....do it
                    ////Change State to winner if there is 5 in a row
                    //if the canvas has the enemy color stone add to enemy count and add positsions

                    if (neighborCanvas == null)
                    {
                        break;
                    }
                    else if (HasSameColorStone(friendlyColor, neighborCanvas))
                    {
                        friendlyCounter++;
                        if (positionsAway == 3 && enemyCounter == 2 && friendlyCounter == 1)
                        {
                            CaptureStones(enemyPositions);
                        }
                        if (friendlyCounter == 4)
                        {
                            HasWinner = true;
                        }
                    }
                    else if (neighborCanvas.Children.Count != 0)
                    {
                        enemyCounter++;
                        enemyPositions.Add(rowOffset);
                        enemyPositions.Add(columnOffset);
                    }
                    columnOffset--;
                    rowOffset--;
                }
                else
                {
                    positionsAway = 6;
                }

            }
        }

        //Go through Up a row and Right a column searching for an stone of the same color, enemy stone, or space
        public void CheckUpRight(int startRow, int startColumn, int positionsAway, int friendlyCounter, int enemyCounter, List<int> enemyPositions, SolidColorBrush friendlyColor)
        {

            int columnOffset = startColumn + 1;
            int rowOffset = startRow - 1;
            positionsAway = 0;
            enemyPositions.Clear();
            friendlyCounter = 0;
            enemyCounter = 0;

            while (positionsAway < 5)
            {
                positionsAway++;

                //Makes sure that the spot we are looking in is within the borders
                if (IsValidPosition(columnOffset) && IsValidPosition(rowOffset))
                {
                    //Get the canvas we are looking At
                    Canvas neighborCanvas = GetCanvas(columnOffset, rowOffset);

                    //If its null continue to the next neighbor
                    //If the canvas has the same color stone add to friendly count
                    ////If there was an two enemies between start point and the fourth *In Palatine Voice* kill'em...kill'em now....do it
                    //if the canvas has the enemy color stone add to enemy count and add positsions

                    if (neighborCanvas == null)
                    {
                        break;
                    }
                    else if (HasSameColorStone(friendlyColor, neighborCanvas))
                    {
                        friendlyCounter++;
                        if (positionsAway == 3 && enemyCounter == 2 && friendlyCounter == 1)
                        {
                            CaptureStones(enemyPositions);
                        }
                        if (friendlyCounter == 4)
                        {
                            HasWinner = true;
                        }
                    }
                    else if (neighborCanvas.Children.Count != 0)
                    {
                        enemyCounter++;
                        enemyPositions.Add(rowOffset);
                        enemyPositions.Add(columnOffset);
                    }
                    columnOffset++;
                    rowOffset--;
                }
                else
                {
                    positionsAway = 6;
                }

            }
        }

        //Go through down a row and left a column searching for an stone of the same color, enemy stone, or space
        public void CheckDownLeft(int startRow, int startColumn, int positionsAway, int friendlyCounter, int enemyCounter, List<int> enemyPositions, SolidColorBrush friendlyColor)
        {

            int columnOffset = startColumn - 1;
            int rowOffset = startRow + 1;
            positionsAway = 0;
            enemyPositions.Clear();
            friendlyCounter = 0;
            enemyCounter = 0;

            while (positionsAway < 5)
            {
                positionsAway++;

                //Makes sure that the spot we are looking in is within the borders
                if (IsValidPosition(columnOffset) && IsValidPosition(rowOffset))
                {
                    //Get the canvas we are looking At
                    Canvas neighborCanvas = GetCanvas(columnOffset, rowOffset);

                    //If its null continue to the next neighbor
                    //If the canvas has the same color stone add to friendly count
                    ////If there was an two enemies between start point and the fourth *In Palatine Voice* kill'em...kill'em now....do it
                    //if the canvas has the enemy color stone add to enemy count and add positsions

                    if (neighborCanvas == null)
                    {
                        break;
                    }
                    else if (HasSameColorStone(friendlyColor, neighborCanvas))
                    {
                        friendlyCounter++;
                        if (positionsAway == 3 && enemyCounter == 2 && friendlyCounter == 1)
                        {
                            CaptureStones(enemyPositions);
                        }
                        if (friendlyCounter == 4)
                        {
                            HasWinner = true;
                        }
                    }
                    else if (neighborCanvas.Children.Count != 0)
                    {
                        enemyCounter++;
                        enemyPositions.Add(rowOffset);
                        enemyPositions.Add(columnOffset);
                    }
                    columnOffset--;
                    rowOffset++;
                }
                else
                {
                    positionsAway = 6;
                }

            }
        }

        //Go through Updown a row and right a column searching for an stone of the same color, enemy stone, or space
        public void CheckDownRight(int startRow, int startColumn, int positionsAway, int friendlyCounter, int enemyCounter, List<int> enemyPositions, SolidColorBrush friendlyColor)
        {

            int columnOffset = startColumn + 1;
            int rowOffset = startRow + 1;
            positionsAway = 0;
            enemyPositions.Clear();
            friendlyCounter = 0;
            enemyCounter = 0;

            while (positionsAway < 5)
            {
                positionsAway++;

                //Makes sure that the spot we are looking in is within the borders
                if (IsValidPosition(columnOffset) && IsValidPosition(rowOffset))
                {
                    //Get the canvas we are looking At
                    Canvas neighborCanvas = GetCanvas(columnOffset, rowOffset);

                    //If its null continue to the next neighbor
                    //If the canvas has the same color stone add to friendly count
                    ////If there was an two enemies between start point and the fourth *In Palatine Voice* kill'em...kill'em now....do it
                    //if the canvas has the enemy color stone add to enemy count and add positsions

                    if (neighborCanvas == null)
                    {
                        break;
                    }
                    else if (HasSameColorStone(friendlyColor, neighborCanvas))
                    {
                        friendlyCounter++;
                        if (positionsAway == 3 && enemyCounter == 2 && friendlyCounter == 1)
                        {
                            CaptureStones(enemyPositions);
                        }
                        if (friendlyCounter == 4)
                        {
                            HasWinner = true;
                        }
                    }
                    else if (neighborCanvas.Children.Count != 0)
                    {
                        enemyCounter++;
                        enemyPositions.Add(rowOffset);
                        enemyPositions.Add(columnOffset);
                    }
                    columnOffset++;
                    rowOffset++;
                }
                else
                {
                    positionsAway = 6;
                }

            }
        }

        //Go through directly left searching for an stone of the same color, enemy stone, or space
        public void CheckLeft(int startRow, int startColumn, int positionsAway, int friendlyCounter, int enemyCounter, List<int> enemyPositions, SolidColorBrush friendlyColor)
        {

            int columnOffset = startColumn - 1;
            int rowOffset = startRow;
            positionsAway = 0;
            enemyPositions.Clear();
            friendlyCounter = 0;
            enemyCounter = 0;

            while (positionsAway < 5)
            {
                positionsAway++;

                //Makes sure that the spot we are looking in is within the borders
                if (IsValidPosition(columnOffset) && IsValidPosition(rowOffset))
                {
                    //Get the canvas we are looking At
                    Canvas neighborCanvas = GetCanvas(columnOffset, rowOffset);

                    //If its null continue to the next neighbor
                    //If the canvas has the same color stone add to friendly count
                    ////If there was an two enemies between start point and the fourth *In Palatine Voice* kill'em...kill'em now....do it
                    //if the canvas has the enemy color stone add to enemy count and add positsions

                    if (neighborCanvas == null)
                    {
                        break;
                    }
                    else if (HasSameColorStone(friendlyColor, neighborCanvas))
                    {
                        friendlyCounter++;
                        if (positionsAway == 3 && enemyCounter == 2 && friendlyCounter == 1)
                        {
                            CaptureStones(enemyPositions);
                        }
                        if (friendlyCounter == 4)
                        {
                            HasWinner = true;
                        }
                    }
                    else if (neighborCanvas.Children.Count != 0)
                    {
                        enemyCounter++;
                        enemyPositions.Add(rowOffset);
                        enemyPositions.Add(columnOffset);
                    }
                    columnOffset--;
                }
                else
                {
                    positionsAway = 6;
                }

            }
        }

        //Go through directly right searching for an stone of the same color, enemy stone, or space
        public void CheckRight(int startRow, int startColumn, int positionsAway, int friendlyCounter, int enemyCounter, List<int> enemyPositions, SolidColorBrush friendlyColor)
        {

            int columnOffset = startColumn + 1;
            int rowOffset = startRow;
            positionsAway = 0;
            enemyPositions.Clear();
            friendlyCounter = 0;
            enemyCounter = 0;

            while (positionsAway < 5)
            {
                positionsAway++;

                //Makes sure that the spot we are looking in is within the borders
                if (IsValidPosition(columnOffset) && IsValidPosition(rowOffset))
                {
                    //Get the canvas we are looking At
                    Canvas neighborCanvas = GetCanvas(columnOffset, rowOffset);

                    //If its null continue to the next neighbor
                    //If the canvas has the same color stone add to friendly count
                    ////If there was an two enemies between start point and the fourth *In Palatine Voice* kill'em...kill'em now....do it
                    //if the canvas has the enemy color stone add to enemy count and add positsions

                    if (neighborCanvas == null)
                    {
                        break;
                    }
                    else if (HasSameColorStone(friendlyColor, neighborCanvas))
                    {
                        friendlyCounter++;
                        if (positionsAway == 3 && enemyCounter == 2 && friendlyCounter == 1)
                        {
                            CaptureStones(enemyPositions);
                        }
                        if (friendlyCounter == 4)
                        {
                            HasWinner = true;
                        }
                    }
                    else if (neighborCanvas.Children.Count != 0)
                    {
                        enemyCounter++;
                        enemyPositions.Add(rowOffset);
                        enemyPositions.Add(columnOffset);
                    }
                    columnOffset++;
                }
                else
                {
                    positionsAway = 6;
                }

            }
        }

        //Go through directly up searching for an stone of the same color, enemy stone, or space
        public void CheckUp(int startRow, int startColumn, int positionsAway, int friendlyCounter, int enemyCounter, List<int> enemyPositions, SolidColorBrush friendlyColor)
        {

            int columnOffset = startColumn;
            int rowOffset = startRow - 1;
            positionsAway = 0;
            enemyPositions.Clear();
            friendlyCounter = 0;
            enemyCounter = 0;

            while (positionsAway < 5)
            {
                positionsAway++;

                //Makes sure that the spot we are looking in is within the borders
                if (IsValidPosition(columnOffset) && IsValidPosition(rowOffset))
                {
                    //Get the canvas we are looking At
                    Canvas neighborCanvas = GetCanvas(columnOffset, rowOffset);

                    //If its null continue to the next neighbor
                    //If the canvas has the same color stone add to friendly count
                    ////If there was an two enemies between start point and the fourth *In Palatine Voice* kill'em...kill'em now....do it
                    //if the canvas has the enemy color stone add to enemy count and add positsions

                    if (neighborCanvas == null)
                    {
                        break;
                    }
                    else if (HasSameColorStone(friendlyColor, neighborCanvas))
                    {
                        friendlyCounter++;
                        if (positionsAway == 3 && enemyCounter == 2 && friendlyCounter == 1)
                        {
                            CaptureStones(enemyPositions);
                        }
                        if (friendlyCounter == 4)
                        {
                            HasWinner = true;
                        }
                    }
                    else if (neighborCanvas.Children.Count != 0)
                    {
                        enemyCounter++;
                        enemyPositions.Add(rowOffset);
                        enemyPositions.Add(columnOffset);
                    }
                    rowOffset--;
                }
                else
                {
                    positionsAway = 6;
                }

            }
        }

        //Go through directly down searching for an stone of the same color, enemy stone, or space
        public void CheckDown(int startRow, int startColumn, int positionsAway, int friendlyCounter, int enemyCounter, List<int> enemyPositions, SolidColorBrush friendlyColor)
        {

            int columnOffset = startColumn;
            int rowOffset = startRow + 1;
            positionsAway = 0;
            enemyPositions.Clear();
            friendlyCounter = 0;
            enemyCounter = 0;

            while (positionsAway < 5)
            {
                positionsAway++;

                //Makes sure that the spot we are looking in is within the borders
                if (IsValidPosition(columnOffset) && IsValidPosition(rowOffset))
                {
                    //Get the canvas we are looking At
                    Canvas neighborCanvas = GetCanvas(columnOffset, rowOffset);

                    //If its null continue to the next neighbor
                    //If the canvas has the same color stone add to friendly count
                    ////If there was an two enemies between start point and the fourth *In Palatine Voice* kill'em...kill'em now....do it
                    //if the canvas has the enemy color stone add to enemy count and add positsions

                    if (neighborCanvas == null)
                    {
                        break;
                    }
                    else if (HasSameColorStone(friendlyColor, neighborCanvas))
                    {
                        friendlyCounter++;
                        if (positionsAway == 3 && enemyCounter == 2 && friendlyCounter == 1)
                        {
                            CaptureStones(enemyPositions);
                        }
                        if (friendlyCounter == 4)
                        {
                            HasWinner = true;
                        }
                    }
                    else if (neighborCanvas.Children.Count != 0)
                    {
                        enemyCounter++;
                        enemyPositions.Add(rowOffset);
                        enemyPositions.Add(columnOffset);
                    }
                    rowOffset++;
                }
                else
                {
                    positionsAway = 6;
                }

            }
        }
        #endregion

        #endregion
    }
}
