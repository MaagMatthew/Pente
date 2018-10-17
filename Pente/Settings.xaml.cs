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

        //sets the options for grid size
        private void SetGridOptions()
        {
            for (int i = 9; i <= 39; i+=2)
            {
                CmboBx_Size.Items.Add(i);
            }
            CmboBx_Size.SelectedItem = 19;
        }

        //if go home is clicked close window
        private void GoHome(object sender, RoutedEventArgs e)
        {
            Close();
        }

        //if pressed create a new window with these variables
        private void Play(object sender, RoutedEventArgs e)
        {
            bool CPU = GetCPUStatus();
            string firstName = GetFirstPlayerName();
            string secondName = GetSecondPlayerName();
            int gridSize = GetGridSize();

            MainWindow main = new MainWindow(gridSize, firstName, secondName, CPU);
            main.ShowDialog();
        }

        //Returns Int Value From the ComboBox
        private int GetGridSize()
        {
            return (int)CmboBx_Size.SelectedItem;
        }

        //Returns bool of the Radio Button
        private bool GetCPUStatus()
        {
            return (bool)RdioBtn_EnabledCPU.IsChecked;
        }

        //If empty set default name to player 1        
        //Returns text from input of the textbox
        //Or return input if valid
        private string GetFirstPlayerName()
        {
            string result = TxtBx_FName.Text;
            if (string.IsNullOrWhiteSpace(result))
            {
                return "Player 1";
            }
            else
            {
                return result;
            }
        }
        //Returns text from input of the textbox
        //If empty set default name to player CPU if  radio buttion is checked
        //Or Set Default Name if no valid input is given
        //Or return whatever input is recieved
        private string GetSecondPlayerName()
        {
            string result = TxtBx_SName.Text;
            if (string.IsNullOrWhiteSpace(result) & GetCPUStatus())
            {
                return "CPU";
            }
            else if (string.IsNullOrWhiteSpace(result) & !GetCPUStatus())
            {
                return "Player 2";
            }
            else
            {
                return result;
            }
        }
    }
}
