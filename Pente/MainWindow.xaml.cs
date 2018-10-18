using Pente.XAML;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
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
        Timer time;
        public event PropertyChangedEventHandler PropertyChanged;

        public static int maxTimerValue = 20;
        public int timerValue = 0;

        public int TimerValue
        {
            get { return timerValue; }
            set
            {
                timerValue = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("TimerValue"));
            }
        }

        public MainWindow(int GridSize, string player1Name, string player2Name, bool isCpuEnabled)
        {
            InitializeComponent();
            game = new GameBoard(GridSize, player1Name, player2Name,isCpuEnabled);
            game.PropertyChanged += NotifyNameChange;
            game.PropertyChanged += NotifyGameEnded;
            CreateTimer();
            time.Elapsed += UpdateTimer;
            PlaceBoard(game);
            SetNames(player1Name, player2Name);

            TxtBx_Notifications.Text = "Welcome Player 1 Take Turn";
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

        private void UpdateTimer(object sender = null, ElapsedEventArgs e = null)
        {
            if (timerValue < maxTimerValue)
            {
                timerValue++;
            }
            else
            {
                RestartTimer();
                game.SwitchTurn();
                StartTimer();
            }

            TxtBx_Timer.Text = timerValue + "";
        }

        private void NotifyNameChange(object sender, EventArgs e)
        {
            if (game.CurrentPlayerName == TxtBx_FirstPlayer.Text)
            {
                TxtBx_Notifications.Text = $"{TxtBx_SecondPlayer.Text} Take your turn";
            }
            else
            {
                TxtBx_Notifications.Text = $"{TxtBx_FirstPlayer.Text} Take your turn";
            }
        }

        private void NotifyGameEnded(object sender, EventArgs e)
        {
            if (game.HasWinner)
            {
                game.SwitchTurn();
                TxtBx_Notifications.Text = $"Game has Ended! Winnner{game.CurrentPlayerName}";
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
            time.Stop();
            timerValue = 0;
        }

        public void CreateTimer()
        {
            time = new Timer();
        }

        private void Return(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void GetHelp(object sender, RoutedEventArgs e)
        {
            Help helpPage = new Help();
            this.Visibility = Visibility.Hidden;
            helpPage.ShowDialog();
            this.Visibility = Visibility.Visible;
        }
    }
}
