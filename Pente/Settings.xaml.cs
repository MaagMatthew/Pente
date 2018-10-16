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

namespace Pente
{
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class Settings : Window
    {
        public Settings()
        {
            InitializeComponent();
            SetGridOptions();
        }

        private void SetGridOptions()
        {
            for (int i = 9; i <= 39; i++)
            {
                CmboBx_Size.Items.Add(i);
            }
            CmboBx_Size.SelectedItem = 19;
        }

        private void GoHome(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Play(object sender, RoutedEventArgs e)
        {
            bool CPU = RdioBtn_EnabledCPU.IsEnabled;
            string firstName = TxtBx_FName.Text;
            string secondName = TxtBx_SName.Text;
            int gridSize = (int)CmboBx_Size.SelectedItem;

            if (string.IsNullOrWhiteSpace(firstName))
            {
                firstName = "Player 1";
            }
            if (string.IsNullOrWhiteSpace(secondName))
            {
                secondName = "Player 2";
            }

            MainWindow main = new MainWindow(gridSize, firstName, secondName, CPU);
            main.ShowDialog();
        }
    }
}
