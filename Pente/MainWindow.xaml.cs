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
        public MainWindow(int GridSize, string player1Name, string player2Name, bool isCpuEnabled)
        {
            InitializeComponent();
            GameBoard game = new GameBoard(GridSize, player1Name, player2Name,isCpuEnabled);
            Grid.SetColumn(game,1);
            Grid.SetRow(game,1);
            game.Height = double.NaN;
            game.Width = double.NaN;
        }
    }
}
