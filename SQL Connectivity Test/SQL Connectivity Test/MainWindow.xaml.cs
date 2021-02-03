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

namespace SQL_Connectivity_Test
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Btn_TestConnectivity_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ValidateFields();
                var result = ValidateConnectivity();
                if (result)
                {
                    MessageBox.Show($"SQL connectivity is OK.", "Connection Successful", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show($"SQL Connection could not be established.", "Connection Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }    
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex.Message, ex.GetType().Name);
            }
        }

        private bool ValidateConnectivity()
        {
            string server = Txt_DbInstance.Text;
            string dbname = Txt_DbName.Text;
            bool useWindowsAuth = Chk_WindowsAuthentication.IsChecked == true;
            string username = useWindowsAuth ? string.Empty : Txt_DbUsername.Text;
            string password = useWindowsAuth ? string.Empty : Txt_DbPassword.Text;
            bool connectionEstablished = false;

            using (var connection = SQLConnectivityHelper.GetSQLConnection(server, dbname, useWindowsAuth, username, password))
            {
                connection.Open();
                connectionEstablished = true;
                connection.Close();
            }

            return connectionEstablished;
        }

        private void ValidateFields()
        {
            if (string.IsNullOrWhiteSpace(Txt_DbInstance.Text))
            {
                throw new Exception($"Database server name cannot be empty.");
            }

            if (Chk_WindowsAuthentication.IsChecked != true)
            {
                if (string.IsNullOrWhiteSpace(Txt_DbPassword.Text))
                {
                    throw new Exception($"Database password cannot be empty.");
                }

                if (string.IsNullOrWhiteSpace(Txt_DbUsername.Text))
                {
                    throw new Exception($"Database username cannot be empty.");
                }
            }
        }

        private void ShowErrorMessage(string message, string title = "Error")
        {
            MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
