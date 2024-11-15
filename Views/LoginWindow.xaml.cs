using Automate.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace Automate
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
            DataContext = new LoginViewModel(this);
        }

        //pas le choix d'utiliser un événement, on ne peut pas BIND un password pour des raisons de sécurité
        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            PasswordBox? passwordBox = sender as PasswordBox;
            if (DataContext is LoginViewModel viewModel)
            {
                viewModel.Password = passwordBox.Password; // Met à jour la propriété Password dans le ViewModel
            }
        }
    }
}
