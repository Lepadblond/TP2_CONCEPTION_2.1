using Automate.ViewModels;
using System.Windows;

namespace Automate.Views
{
    /// <summary>
    /// Interaction logic for AccueilWindow.xaml
    /// </summary>
    public partial class AccueilWindow : Window
    {
        public AccueilWindow()
        {
            InitializeComponent();
            DataContext = new AccueilViewModel();
        }
    }
}