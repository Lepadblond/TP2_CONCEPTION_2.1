using Automate.Models;
using Automate.ViewModels;
using System.Windows;

namespace Automate.Views
{
    /// <summary>
    /// Interaction logic for AccueilWindow.xaml
    /// </summary>
    public partial class AccueilWindow : Window
    {
        public AccueilWindow(UserModel utilisateurConnecte)
        {

            InitializeComponent();

            DataContext = new AccueilViewModel(utilisateurConnecte);
        }
    }
}