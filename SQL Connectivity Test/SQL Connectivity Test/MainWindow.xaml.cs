using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
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

        private async void Btn_TestConnectivity_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ToggleFieldsUsability();
                ValidateFields();

                string server = Txt_DbInstance.Text;
                string dbname = Txt_DbName.Text;
                bool useWindowsAuth = Chk_WindowsAuthentication.IsChecked == true;
                string username = useWindowsAuth ? string.Empty : Txt_DbUsername.Text;
                string password = useWindowsAuth ? string.Empty : Txt_DbPassword.Password;
                var result = await Task.Run(() => ValidateConnectivity(server, dbname, useWindowsAuth, username, password));
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

            ToggleFieldsUsability();
        }

        private void Chk_WindowsAuthentication_Toggle(object sender, RoutedEventArgs e)
        {
            try
            {
                var chkBox = (CheckBox)sender;
                var toEnable = chkBox.IsChecked == false;
                Txt_DbUsername.IsEnabled = toEnable;
                Txt_DbPassword.IsEnabled = toEnable;
                Btn_ViewPassword.IsEnabled = toEnable;
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex.Message, ex.GetType().Name);
            }
        }

        private bool ValidateConnectivity(string server, string dbName, bool useWindowsAuth, string username, string password)
        {
            bool connectionEstablished = false;

            using (var connection = SQLConnectivityHelper.GetSQLConnection(server, dbName, useWindowsAuth, username, password))
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
                if (string.IsNullOrWhiteSpace(Txt_DbPassword.Password))
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

        private void ToggleFieldsUsability()
        {
            bool enableOperation = !Txt_DbInstance.IsEnabled;
            Txt_DbInstance.IsEnabled = !Txt_DbInstance.IsEnabled;
            Txt_DbName.IsEnabled = !Txt_DbName.IsEnabled;
            Chk_WindowsAuthentication.IsEnabled = !Chk_WindowsAuthentication.IsEnabled;
            Txt_DbUsername.IsEnabled = enableOperation && (Chk_WindowsAuthentication.IsChecked != true);
            Txt_DbPassword.IsEnabled = enableOperation && (Chk_WindowsAuthentication.IsChecked != true);
            Btn_ViewPassword.IsEnabled = enableOperation && (Chk_WindowsAuthentication.IsChecked != true);
            Btn_TestConnectivity.IsEnabled = !Btn_TestConnectivity.IsEnabled;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Txt_DbInstance.Focus();
        }

        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Btn_TestConnectivity.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
            }
        }

        private void Btn_ViewPassword_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(Txt_DbPassword.Password, "Password", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
