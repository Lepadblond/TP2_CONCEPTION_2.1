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

        public event PropertyChangedEventHandler? PropertyChanged;

        public AccueilViewModel()
        {
            {
                _mongoService = new MongoDBService("AutomateDB");
                _taches = new List<TacheModel>(); // Initialize _taches to avoid null
                _observableCollectionDeTaches = new ObservableCollection<TacheModel>();
                ChargerTaches();
                // Initialisation des commandes
                AjouterTacheCommand = new RelayCommand(param => AjouterTache());
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

        // M�thodes
        private void ChargerTaches()
        {
            try
            {
                _taches = _mongoService.ObtenirToutLesTaches();
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
            FormTache formTache = new FormTache();
            if (formTache.ShowDialog() == true && formTache.Tache != null)
            {
                _mongoService.AjouterTache(formTache.Tache);
                ObservableCollectionDeTaches.Add(formTache.Tache);
            }
            else
            {
                MessageBox.Show("La t�che n'a pas �t� ajout�e.");
            }
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

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}