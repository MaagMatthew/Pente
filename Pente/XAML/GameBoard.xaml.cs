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
        private bool IsFirstMove;
        int p1CaptureCount;
        int p2CaptureCount;
        int MAX_BOARD_SIZE;
        int pieceCount;
        public bool _HasWinner { get; private set; }
        public bool HasTria { get; private set; }
        public bool HasTessera { get; private set; }
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
            get { return this._CurrentPlayerName; }
            set
            {
                _CurrentPlayerName = value;
                NotifyPropertyChange("Current");
                return;
            }
        }
        public string _CurrentPlayerName { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;
        
        //Notify Game Has ended
        public void NotifyGameEnded(string info)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(info));
        }

        //Notify turn has Changed
        public void NotifyPropertyChange(string info)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(info));
        }
        #endregion
        #region Initialization
        //Creates board
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
            IsFirstMove = true;
            MAX_BOARD_SIZE = squareSize * squareSize;
            pieceCount = 0;
        }

        //Force Player 1 to place in the middle
        private void PlaceMid()
        {
            int middle = MidPoint();
            Canvas canvas = GetCanvas(middle, middle);
            Ellipse shape = CreateShape();
            ColorStone(shape);
            CenterStone(shape);
            canvas.Children.Add(shape);
            pieceCount += 1;
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

        //Gets Middle Point of the board
        private int MidPoint()
        {
            return gameBoard.ColumnDefinitions.Count / 2;
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
            //Is the game Over
            //If not is this the very first turn for player one
            ////If so the player is forced to place a piece in the middle
            //Is this the very first turn for the second player
            ////Player can only place a piece that is three away
            if (!HasWinner)
            {
                var selectedCanvas = e.Source as Canvas;
                if (pieceCount != MAX_BOARD_SIZE)
                {
                    if (selectedCanvas != null)
                    {
                        if (IsFirstMove && IsFirstPlayer)
                        {
                            PlaceMid();
                            if (IsCPUPlaying)
                            {
                                TakeCPUTurn();
                                MessageBox.Show("CPU has taken turn");
                            }
                            else
                            {
                                SwitchTurn();
                            }
                        }
                        else if (IsFirstMove && !IsFirstPlayer)
                        {
                            if (IsThreeAway(selectedCanvas) && selectedCanvas.Children.Count < 1)
                            {
                                PlaceStone(selectedCanvas);
                                SwitchTurn();
                                IsFirstMove = false;
                            }
                        }
                        else if (!IsFirstMove)
                        {
                            if (selectedCanvas.Children.Count < 1)
                            {
                                PlaceStone(selectedCanvas);
                                if (IsCPUPlaying && !_HasWinner)
                                {
                                    TakeCPUTurn();
                                    MessageBox.Show("CPU Has Taken Turn");
                                }
                                if (!_HasWinner && !IsCPUPlaying)
                                {
                                    SwitchTurn();
                                }
                            }
                        }
                    }
                }
                else
                {
                    HasWinner = true;
                    MessageBox.Show("Game Ended in a DRAW!");
                }
            }
        }

        private void PlaceStone(Canvas selectedCanvas)
        {
            Ellipse shape = CreateShape();
            ColorStone(shape);
            CenterStone(shape);
            selectedCanvas.Children.Add(shape);
            CheckWin(selectedCanvas);
            pieceCount += 1;
        }

        //is this canvas away far enough
        private bool IsThreeAway(Canvas selected)
        {
            int midColumn = MidPoint();
            int midRow = MidPoint();
            int selectedColumn = Grid.GetColumn(selected);
            int selectedRow = Grid.GetRow(selected);

            int ColumnOffset = midColumn - selectedColumn;
            int RowOffset = midRow - selectedRow;

            if (IsThreeAway(ColumnOffset, RowOffset))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        //is this the offsets three away 
        private bool IsThreeAway(int col, int row)
        {
            if (col > 3 && row > 3)
            {
                return true;
            }
            else if (col > 3 && row == 0)
            {
                return true;
            }
            else if (col > 3 && row < -3)
            {
                return true;
            }
            else if (col == 0 && row > 3)
            {
                return true;
            }
            else if (col == 0 && row < -3)
            {
                return true;
            }
            else if (col < -3 && row > 3)
            {
                return true;
            }
            else if (col < -3 && row == 0)
            {
                return true;
            }
            else if (col < -3 && row < -3)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void TakeCPUTurn()
        {
            SwitchTurn();
            Random r = new Random();
            bool IsValidSpot = false;
            int selectedColumn;
            int selectedRow;
            do
            {
                selectedColumn = r.Next(0, gameBoard.ColumnDefinitions.Count);
                selectedRow = r.Next(0, gameBoard.RowDefinitions.Count);
                Canvas canvas = GetCanvas(selectedColumn, selectedRow);
                if (IsFirstMove && IsThreeAway(canvas))
                {
                    if (canvas.Children.Count < 1)
                    {
                        PlaceStone(canvas);
                        IsValidSpot = true;
                        IsFirstMove = false;
                    }
                }
                else if (!IsFirstMove)
                {
                    if (canvas.Children.Count < 1)
                    {
                        PlaceStone(canvas);
                        IsValidSpot = true;
                    }
                }
            } while (!IsValidSpot);
            SwitchTurn();
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
        private void CenterStone(Shape shape)
        {
            double left = shape.Width / 4.5;
            double top = shape.Height / 4.5;
            shape.Margin = new Thickness(left, top, 0, 0);
        }

        #endregion
        #region Taking Turn(Main,Helper,Check)
        #region Main
        //Check for capture and wins
        private void CheckWin(Canvas currentCanvas)
        {
            //Setup

            //Var to chech how many boxes we are away from the starting point
            int checkedPositions = 0;

            //What color is Considered Friendly
            SolidColorBrush friendlyColor = GetFriendlyColor();

            //Counters to setup win/capture
            int friendlyCounter = 0, enemyCounter = 0;

            //List of enemies positions(row,column) may end up being removed 
            List<int> enemyPositions = new List<int>();

            //Get Row and Column of the starting point
            int currentRow = Grid.GetRow(currentCanvas);
            int currentColumn = Grid.GetColumn(currentCanvas);

            CheckUpLeft(currentRow, currentColumn, checkedPositions, friendlyCounter, enemyCounter, enemyPositions, friendlyColor);
            CheckDownLeft(currentRow, currentColumn, checkedPositions, friendlyCounter, enemyCounter, enemyPositions, friendlyColor);
            CheckUpRight(currentRow, currentColumn, checkedPositions, friendlyCounter, enemyCounter, enemyPositions, friendlyColor);
            CheckDownRight(currentRow, currentColumn, checkedPositions, friendlyCounter, enemyCounter, enemyPositions, friendlyColor);
            CheckLeft(currentRow, currentColumn, checkedPositions, friendlyCounter, enemyCounter, enemyPositions, friendlyColor);
            CheckRight(currentRow, currentColumn, checkedPositions, friendlyCounter, enemyCounter, enemyPositions, friendlyColor);
            CheckUp(currentRow, currentColumn, checkedPositions, friendlyCounter, enemyCounter, enemyPositions, friendlyColor);
            CheckDown(currentRow, currentColumn, checkedPositions, friendlyCounter, enemyCounter, enemyPositions, friendlyColor);
            CheckConditions();
        }

        //Switch names and boolean
        public void SwitchTurn()
        {
            IsFirstPlayer = !IsFirstPlayer;

            if (_CurrentPlayerName == Player1Name)
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
                    pieceCount -= 1;
                    canvas.Children.Clear();
                }
                AddCapturePoints();
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

        //Gets a canvas from a certain Position
        private Canvas GetCanvas(int column, int row)
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

        //Adds to a players Count;
        private void AddCapturePoints()
        {
            if (IsFirstPlayer)
            {
                p1CaptureCount++;
            }
            else
            {
                p2CaptureCount++;
            }
        }

        //Check if a player either player has a count of 5 or 
        private void CheckConditions()
        {
            if (!_HasWinner)
            {
                if (p1CaptureCount >= 5 || p2CaptureCount >= 5)
                {
                    _HasWinner = true;
                }
            }
        }

        #endregion
        #region Check Methods

        //Go through Up a row and Left a column searching for an stone of the same color, enemy stone, or space
        private void CheckUpLeft(int startRow, int startColumn, int checkedPositions, int friendlyCounter, int enemyCounter, List<int> enemyPositions, SolidColorBrush friendlyColor)
        {

            HasTria = false;
            HasTessera = false;
            int columnOffset = startColumn - 1;
            int rowOffset = startRow - 1;
            checkedPositions = 0;
            enemyPositions.Clear();
            friendlyCounter = 0;
            enemyCounter = 0;

            while (checkedPositions < 5)
            {
                checkedPositions++;

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
                        if (checkedPositions == 3 && enemyCounter == 2 && friendlyCounter == 1)
                        {
                            CaptureStones(enemyPositions);
                        }
                        if (checkedPositions == 2 && friendlyCounter == 2)
                        {
                            HasTria = true;
                        }
                        if (HasTria && checkedPositions == 3)
                        {
                            if (friendlyCounter == 3)
                            {
                                HasTria = false;
                                HasTessera = true;
                            }
                        }
                        if (HasTessera && checkedPositions == 4)
                        {
                            if (friendlyCounter == 4)
                            {
                                HasWinner = true;
                                HasTessera = false;
                            }
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
                    checkedPositions = 6;
                }
            }
        }

        //Go through Up a row and Right a column searching for an stone of the same color, enemy stone, or space
        private void CheckUpRight(int startRow, int startColumn, int checkedPositions, int friendlyCounter, int enemyCounter, List<int> enemyPositions, SolidColorBrush friendlyColor)
        {
            HasTria = false;
            HasTessera = false;
            int columnOffset = startColumn + 1;
            int rowOffset = startRow - 1;
            checkedPositions = 0;
            enemyPositions.Clear();
            friendlyCounter = 0;
            enemyCounter = 0;

            while (checkedPositions < 5)
            {
                checkedPositions++;

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
                        if (checkedPositions == 3 && enemyCounter == 2 && friendlyCounter == 1)
                        {
                            CaptureStones(enemyPositions);
                        }
                        if (checkedPositions == 2 && friendlyCounter == 2)
                        {
                            HasTria = true;
                        }
                        if (HasTria && checkedPositions == 3)
                        {
                            if (friendlyCounter == 3)
                            {
                                HasTria = false;
                                HasTessera = true;
                            }
                        }
                        if (HasTessera && checkedPositions == 4)
                        {
                            if (friendlyCounter == 4)
                            {
                                HasWinner = true;
                                HasTessera = false;
                            }
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
                    checkedPositions = 6;
                }
            }
        }

        //Go through down a row and left a column searching for an stone of the same color, enemy stone, or space
        private void CheckDownLeft(int startRow, int startColumn, int checkedPositions, int friendlyCounter, int enemyCounter, List<int> enemyPositions, SolidColorBrush friendlyColor)
        {

            int columnOffset = startColumn - 1;
            int rowOffset = startRow + 1;
            checkedPositions = 0;
            enemyPositions.Clear();
            friendlyCounter = 0;
            enemyCounter = 0;

            while (checkedPositions < 5)
            {
                checkedPositions++;

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
                        if (checkedPositions == 3 && enemyCounter == 2 && friendlyCounter == 1)
                        {
                            CaptureStones(enemyPositions);
                        }
                        if (checkedPositions == 2 && friendlyCounter == 2)
                        {
                            HasTria = true;
                        }
                        if (HasTria && checkedPositions == 3)
                        {
                            if (friendlyCounter == 3)
                            {
                                HasTria = false;
                                HasTessera = true;
                            }
                        }
                        if (HasTessera && checkedPositions == 4)
                        {
                            if (friendlyCounter == 4)
                            {
                                HasWinner = true;
                                HasTessera = false;
                            }
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
                    checkedPositions = 6;
                }

            }
        }

        //Go through Updown a row and right a column searching for an stone of the same color, enemy stone, or space
        private void CheckDownRight(int startRow, int startColumn, int checkedPositions, int friendlyCounter, int enemyCounter, List<int> enemyPositions, SolidColorBrush friendlyColor)
        {

            int columnOffset = startColumn + 1;
            int rowOffset = startRow + 1;
            checkedPositions = 0;
            enemyPositions.Clear();
            friendlyCounter = 0;
            enemyCounter = 0;

            while (checkedPositions < 5)
            {
                checkedPositions++;

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
                        if (checkedPositions == 3 && enemyCounter == 2 && friendlyCounter == 1)
                        {
                            CaptureStones(enemyPositions);
                        }
                        if (checkedPositions == 2 && friendlyCounter == 2)
                        {
                            HasTria = true;
                        }
                        if (HasTria && checkedPositions == 3)
                        {
                            if (friendlyCounter == 3)
                            {
                                HasTria = false;
                                HasTessera = true;
                            }
                        }
                        if (HasTessera && checkedPositions == 4)
                        {
                            if (friendlyCounter == 4)
                            {
                                HasWinner = true;
                                HasTessera = false;
                            }
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
                    checkedPositions = 6;
                }

            }
        }

        //Go through directly left searching for an stone of the same color, enemy stone, or space
        private void CheckLeft(int startRow, int startColumn, int checkedPositions, int friendlyCounter, int enemyCounter, List<int> enemyPositions, SolidColorBrush friendlyColor)
        {

            int columnOffset = startColumn - 1;
            int rowOffset = startRow;
            checkedPositions = 0;
            enemyPositions.Clear();
            friendlyCounter = 0;
            enemyCounter = 0;

            while (checkedPositions < 5)
            {
                checkedPositions++;

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
                        if (checkedPositions == 3 && enemyCounter == 2 && friendlyCounter == 1)
                        {
                            CaptureStones(enemyPositions);
                        }
                        if (checkedPositions == 2 && friendlyCounter == 2)
                        {
                            HasTria = true;
                        }
                        if (HasTria && checkedPositions == 3)
                        {
                            if (friendlyCounter == 3)
                            {
                                HasTria = false;
                                HasTessera = true;
                            }
                        }
                        if (HasTessera && checkedPositions == 4)
                        {
                            if (friendlyCounter == 4)
                            {
                                HasWinner = true;
                                HasTessera = false;
                            }
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
                    checkedPositions = 6;
                }

            }
        }

        //Go through directly right searching for an stone of the same color, enemy stone, or space
        private void CheckRight(int startRow, int startColumn, int checkedPositions, int friendlyCounter, int enemyCounter, List<int> enemyPositions, SolidColorBrush friendlyColor)
        {

            int columnOffset = startColumn + 1;
            int rowOffset = startRow;
            checkedPositions = 0;
            enemyPositions.Clear();
            friendlyCounter = 0;
            enemyCounter = 0;

            while (checkedPositions < 5)
            {
                checkedPositions++;

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
                        if (checkedPositions == 3 && enemyCounter == 2 && friendlyCounter == 1)
                        {
                            CaptureStones(enemyPositions);
                        }
                        if (checkedPositions == 2 && friendlyCounter == 2)
                        {
                            HasTria = true;
                        }
                        if (HasTria && checkedPositions == 3)
                        {
                            if (friendlyCounter == 3)
                            {
                                HasTria = false;
                                HasTessera = true;
                            }
                        }
                        if (HasTessera && checkedPositions == 4)
                        {
                            if (friendlyCounter == 4)
                            {
                                HasWinner = true;
                                HasTessera = false;
                            }
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
                    checkedPositions = 6;
                }

            }
        }

        //Go through directly up searching for an stone of the same color, enemy stone, or space
        private void CheckUp(int startRow, int startColumn, int checkedPositions, int friendlyCounter, int enemyCounter, List<int> enemyPositions, SolidColorBrush friendlyColor)
        {

            int columnOffset = startColumn;
            int rowOffset = startRow - 1;
            checkedPositions = 0;
            enemyPositions.Clear();
            friendlyCounter = 0;
            enemyCounter = 0;

            while (checkedPositions < 5)
            {
                checkedPositions++;

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
                        if (checkedPositions == 3 && enemyCounter == 2 && friendlyCounter == 1)
                        {
                            CaptureStones(enemyPositions);
                        }
                        if (checkedPositions == 2 && friendlyCounter == 2)
                        {
                            HasTria = true;
                        }
                        if (HasTria && checkedPositions == 3)
                        {
                            if (friendlyCounter == 3)
                            {
                                HasTria = false;
                                HasTessera = true;
                            }
                        }
                        if (HasTessera && checkedPositions == 4)
                        {
                            if (friendlyCounter == 4)
                            {
                                HasWinner = true;
                                HasTessera = false;
                            }
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
                    checkedPositions = 6;
                }

            }
        }

        //Go through directly down searching for an stone of the same color, enemy stone, or space
        private void CheckDown(int startRow, int startColumn, int checkedPositions, int friendlyCounter, int enemyCounter, List<int> enemyPositions, SolidColorBrush friendlyColor)
        {

            int columnOffset = startColumn;
            int rowOffset = startRow + 1;
            checkedPositions = 0;
            enemyPositions.Clear();
            friendlyCounter = 0;
            enemyCounter = 0;

            while (checkedPositions < 5)
            {
                checkedPositions++;

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
                        if (checkedPositions == 3 && enemyCounter == 2 && friendlyCounter == 1)
                        {
                            CaptureStones(enemyPositions);
                        }
                        if (checkedPositions == 2 && friendlyCounter == 2)
                        {
                            HasTria = true;
                        }
                        if (HasTria && checkedPositions == 3)
                        {
                            if (friendlyCounter == 3)
                            {
                                HasTria = false;
                                HasTessera = true;
                            }
                        }
                        if (HasTessera && checkedPositions == 4)
                        {
                            if (friendlyCounter == 4)
                            {
                                HasWinner = true;
                                HasTessera = false;
                            }
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
                    checkedPositions = 6;
                }

            }
        }
        #endregion

        #endregion
    }
}
