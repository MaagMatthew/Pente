using Pente.XAML;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Pente
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private GameBoard game;
        private DispatcherTimer time;

        private static int countdownFrom = 20;

        private int timerValue = countdownFrom;

        public MainWindow(int GridSize, string player1Name, string player2Name, bool isCpuEnabled)
        {
            InitializeComponent();
            game = new GameBoard(GridSize, player1Name, player2Name,isCpuEnabled);
            game.PropertyChanged += NotifyNameChange;
            game.PropertyChanged += NotifyGameEnded;
            CreateTimer();
            PlaceBoard(game);
            SetNames(player1Name, player2Name);
            TxtBx_Notifications.Text = $"{TxtBx_FirstPlayer.Text} Take your turn";
            StartTimer();

            TxtBx_Timer.Text = countdownFrom.ToString();
        }

        //Put a new board in the middle of the grid
        private void PlaceBoard(GameBoard game)
        {
            Grid.SetColumn(game,1);
            Grid.SetRow(game,1);
            game.Height = double.NaN;
            game.Width = double.NaN;
            boardView.Children.Add(game);            
        }

        //Display Names
        private void SetNames(string p1,string p2)
        {
            TxtBx_FirstPlayer.Text = p1;
            TxtBx_SecondPlayer.Text = p2;
        }

        //Start timer
        private void StartTimer()
        {
            time.Start();
        }

        //Make the timer tick and end turn if at 0 seconds
        private void UpdateTimer(object sender = null, EventArgs e = null)
        {
            if (timerValue > 0)
            {
                timerValue--;
            }
            
            if (timerValue == 0)
            {
                game.SwitchTurn();
                RestartTimer();
            }

            TxtBx_Timer.Text = timerValue.ToString();
        }

        //Recieves information from the GameBoard class and display whos turn it is now and if there is tessera/tria
        private void NotifyNameChange(object sender, EventArgs e)
        {
            TxtBx_Notifications.Text = "";
            if (game.HasTessera)
            {
                TxtBx_Notifications.Text += "Tessera is present. ";
            }
            if (game.HasTria)
            {
                TxtBx_Notifications.Text += "Tria is present. ";
            }
            if (game._CurrentPlayerName == TxtBx_FirstPlayer.Text)
            {
                TxtBx_Notifications.Text += $"{TxtBx_FirstPlayer.Text} Take your turn. ";
            }
            else
            {
                TxtBx_Notifications.Text += $"{TxtBx_SecondPlayer.Text} Take your turn. ";
            }

            RestartTimer();
        }

        //Recieves information that the game has ended and declare a winner
        private void NotifyGameEnded(object sender, EventArgs e)
        {
            if (game._HasWinner)
            {
                if (game._CurrentPlayerName == TxtBx_FirstPlayer.Text)
                {
                    TxtBx_Notifications.Text = $"{game._CurrentPlayerName} has won and {TxtBx_SecondPlayer.Text} is a loser";
                }
                else
                {
                    TxtBx_Notifications.Text = $"{game._CurrentPlayerName} has won and {TxtBx_FirstPlayer.Text} is a loser";
                }
            }
        }

        //Set the Timer back to 20
        public void RestartTimer()
        {
            if (game._HasWinner == false)
            {
                time.Stop();
                timerValue = countdownFrom;
                StartTimer();
            }
        }

        //Create a timer
        private void CreateTimer()
        {
            time = new DispatcherTimer();
            time.Tick += new EventHandler(UpdateTimer);
            time.Interval = new TimeSpan(0, 0, 0, 1);
        }

        //Return back to settings
        private void Return(object sender, RoutedEventArgs e)
        {
            Close();
        }

        //Show help page
        private void GetHelp(object sender, RoutedEventArgs e)
        {
            Help helpPage = new Help();
            helpPage.ShowDialog();
        }
    }
}
