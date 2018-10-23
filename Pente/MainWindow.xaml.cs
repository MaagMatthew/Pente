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
        GameBoard game;
        DispatcherTimer time;

        public static int countdownFrom = 20;

        public int timerValue = countdownFrom;

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

        public void PlaceBoard(GameBoard game)
        {
            Grid.SetColumn(game,1);
            Grid.SetRow(game,1);
            game.Height = double.NaN;
            game.Width = double.NaN;
            boardView.Children.Add(game);            
        }

        public  void SetNames(string p1,string p2)
        {
            TxtBx_FirstPlayer.Text = p1;
            TxtBx_SecondPlayer.Text = p2;
        }

        public void StartTimer()
        {
            time.Start();
        }

        private void UpdateTimer(object sender = null, EventArgs e = null)
        {
            if (timerValue > 0)
            {
                timerValue--;
            }
            
            if (timerValue == 0)
            {
                RestartTimer();
            }

            TxtBx_Timer.Text = timerValue.ToString();
        }

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

        private void TimerEnded()
        {
            TxtBx_Notifications.Text = $"Turn has Ended for {game.CurrentPlayerName}. ";
            game.SwitchTurn();
            TxtBx_Notifications.Text += $"{game.CurrentPlayerName} Take your Turn!";
        }

        public void RestartTimer()
        {
            if (game._HasWinner == false)
            {
                time.Stop();
                timerValue = countdownFrom;
                StartTimer();
            }
        }

        public void CreateTimer()
        {
            time = new DispatcherTimer();
            time.Tick += new EventHandler(UpdateTimer);
            time.Interval = new TimeSpan(0, 0, 0, 1);
        }

        private void Return(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void GetHelp(object sender, RoutedEventArgs e)
        {
            Help helpPage = new Help();
            helpPage.ShowDialog();
        }
    }
}
