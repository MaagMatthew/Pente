using Pente.XAML;
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
        public MainWindow(int GridSize, string player1Name, string player2Name, bool isCpuEnabled)
        {
            InitializeComponent();
            game = new GameBoard(GridSize, player1Name, player2Name,isCpuEnabled);
            game.PropertyChanged += NotifyNameChange;
            PlaceBoard(game);
            SetNames(player1Name, player2Name);
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

        }
        private void NotifyNameChange(object sender, EventArgs e)
        {
            MessageBox.Show("Keep");
        }
        private void TimerEnded()
        {
            TxtBx_Notifications.Text = $"Turn has Ended for {game.CurrentPlayerName}. ";
            game.SwitchTurn();
            TxtBx_Notifications.Text += $"{game.CurrentPlayerName} Take your Turn!";
        }

        public void RestartTimer()
        {
            
        }

        public void CreateTimer()
        {
            
        }
    }
}
