using Automate.Enum;
using Automate.Models;
using Automate.Utils;
using Automate.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace Automate.ViewModels
{
    public class AccueilViewModel : INotifyPropertyChanged
    {
        private readonly MongoDBService _mongoService;
        private List<TacheModel> _taches;
        private ObservableCollection<TacheModel> _observableCollectionDeTaches;
        private TacheModel _tacheActuelle;
        private DateTime? _dateSelectionnee;
        private UserModel _utilisateurConnecte;

        public event PropertyChangedEventHandler? PropertyChanged;

        public AccueilViewModel(UserModel utilisateurConnecte)
        {
            {
                _mongoService = new MongoDBService("AutomateDB");
                _taches = new List<TacheModel>(); // Initialize _taches to avoid null
                _observableCollectionDeTaches = new ObservableCollection<TacheModel>();
                ChargerTaches();
                // Initialisation des commandes
                UtilisateurConnecte = utilisateurConnecte;

                AjouterTacheCommand = new RelayCommand(param => AjouterTache());
                ModifierUneTacheCommand = new RelayCommand(param => ModifierUneTache());
            }
        }
        // Propri�t�s
        public ObservableCollection<TacheModel> ObservableCollectionDeTaches
        {
            get => _observableCollectionDeTaches;
            set
            {
                _observableCollectionDeTaches = value;
                OnPropertyChanged();
            }
        }

        public UserModel UtilisateurConnecte
        {
            get => _utilisateurConnecte;
            set
            {
                _utilisateurConnecte = value;
                OnPropertyChanged();
            }
        }

        public DateTime? DateSelectionnee
        {
            get => _dateSelectionnee;
            set
            {
                _dateSelectionnee = value;
                OnPropertyChanged();
                FiltrerTachesParDate();
            }
        }

        public TacheModel TacheActuelle
        {
            get => _tacheActuelle;
            set
            {
                _tacheActuelle = value;
                OnPropertyChanged();
            }
        }

        // Commandes
        public ICommand AjouterTacheCommand { get; }
        public ICommand ModifierUneTacheCommand { get; }

        // M�thodes
        private void ChargerTaches()
        {
            try
            {
                _taches = _mongoService.ObtenirToutesLesTaches();
                if (_taches == null || _taches.Count == 0)
                {
                    MessageBox.Show("Aucune t�che n'a �t� trouv�e.");
                    return;
                }
                ObservableCollectionDeTaches = new ObservableCollection<TacheModel>(_taches);

                _tacheActuelle = _taches[0];
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de la r�cup�ration des t�ches : {ex.Message}");
            }
        }
        private void AjouterTache()
        {
            try
            {
                if (!estAdmin())
                {
                    MessageBox.Show("Vous n'avez pas les droits pour ajouter une t�che.");
                    return;
                }
                else
                {
                    FormTache formTache = new FormTache();

                    if (formTache.ShowDialog() == true && formTache != null)
                    {

                        TacheModel tacheAjoutee = formTache.Tache;

                        _mongoService.AjouterTache(formTache.Tache);
                        ObservableCollectionDeTaches.Add(formTache.Tache);
                    }
                    else
                    {
                        MessageBox.Show("La t�che n'a pas �t� ajout�e.");
                    }
                }


            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de la r�cup�ration des t�ches : {ex.Message}");
            }

        }

        private void ModifierUneTache()
        {

            //try
            //{
            //    if (!estAdmin())
            //    {
            //        MessageBox.Show("Vous n'avez pas les droits pour ajouter une t�che.");
            //        return;
            //    }
            //    else
            //    {
            //        if (TacheActuelle == null)
            //        {
            //            MessageBox.Show("Veuillez s�lectionner une t�che � modifier.");
            //            return;
            //        }
            //        FormTache formTache = new FormTache(TacheActuelle);
            //        if (formTache.ShowDialog() == true && formTache.Tache != null)
            //        {
            //            _mongoService.ModifierTache(formTache.Tache);
            //            ChargerTaches();
            //        }
            //        else
            //        {
            //            MessageBox.Show("La t�che n'a pas �t� modifi�e.");
            //        }

            //    }


         

            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show($"Erreur lors de la modification de la t�ches : {ex.Message}");

            //}



        }

        private void FiltrerTachesParDate()
        {
            if (DateSelectionnee.HasValue)
            {
                var tachesFiltrees = _mongoService.FiltrerTachesParDate(DateSelectionnee.Value);
                ObservableCollectionDeTaches = new ObservableCollection<TacheModel>(tachesFiltrees);
            }
            else
            {
                ObservableCollectionDeTaches = new ObservableCollection<TacheModel>(_taches);
            }
        }

        private bool IsUserAuthenticated()
        {
            // V�rifie si l'utilisateur est connect�
            return UtilisateurConnecte != null;
        }
        private bool estAdmin()
        {
            return UtilisateurConnecte.Role == "admin";
        }
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}