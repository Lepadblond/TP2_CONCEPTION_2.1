using Automate.Models;
using Automate.Enum;
using System.Windows;
using Automate.ViewModels;

namespace Automate.Views
{
    public partial class FormTache : Window
    {
        private TacheViewModel _viewModel;

        public FormTache(TacheModel pTache = null, EtatFormulaire pEtat = EtatFormulaire.Ajouter)
        {
            InitializeComponent();
            _viewModel = new TacheViewModel(pTache, pEtat);
            DataContext = _viewModel;
        }

        public TacheModel Tache => _viewModel.Tache;
    }

}
