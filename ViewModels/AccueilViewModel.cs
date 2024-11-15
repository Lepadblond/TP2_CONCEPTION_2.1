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
using Automate.Models;
using Automate.Utils;
using Automate.Views;

namespace Automate.ViewModels;

public class AccueilViewModel : INotifyPropertyChanged
{
    private readonly MongoDBService _mongoService;
    private DateTime? _dateSelectionnee;
    private ObservableCollection<TacheModel> _observableCollectionDeTaches;
    private TacheModel _tacheActuelle;
    private List<TacheModel> _taches;
    private UserModel _utilisateurConnecte;

    public AccueilViewModel(UserModel utilisateurConnecte)
    {
        {
            _mongoService = new MongoDBService("AutomateDB");
            _taches = new List<TacheModel>();
            _observableCollectionDeTaches = new ObservableCollection<TacheModel>();
            UtilisateurConnecte = utilisateurConnecte;
            ChargerTaches();

                AjouterTacheCommand = new RelayCommand(param => AjouterTache());
                ModifierUneTacheCommand = new RelayCommand(param => ModifierUneTache());
                SupprimerTacheCommand = new RelayCommand(param => SupprimerUneTache());
            }
        }
        // Propriétés
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
        public ICommand SupprimerTacheCommand { get; }

        // Méthodes
        private void ChargerTaches()
        {
            try
            {
                _taches = _mongoService.ObtenirToutesLesTaches();
                if (_taches == null || _taches.Count == 0)
                {
                    MessageBox.Show("Aucune tâche n'a été trouvée.");
                    return;
                }
                ObservableCollectionDeTaches = new ObservableCollection<TacheModel>(_taches);

                _tacheActuelle = _taches[0];
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de la récupération des tâches : {ex.Message}");
            }
        }
        private void AjouterTache()
        {

            try
            {

                if (!estAdmin())
                {
                    MessageBox.Show("Vous n'avez pas les droits pour ajouter une tâche.");
                    return;
                }
                else
                {
                    FormTache formTache = new FormTache();
                    if (formTache.ShowDialog() == true && formTache.Tache != null)
                    {
                        _mongoService.AjouterTache(formTache.Tache);
                        ObservableCollectionDeTaches.Add(formTache.Tache);
                    }
                    else
                    {
                        MessageBox.Show("La tâche n'a pas été ajoutée.");
                    }
                }


            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de la récupération des tâches : {ex.Message}");
            }


        private void ModifierUneTache()
        {
            try
            {
                if (!estAdmin())
                {
                    MessageBox.Show("Vous n'avez pas les droits pour modifier une tâche.");
                    return;
                }
                else
                {
                    if (TacheActuelle == null)
                    {
                        MessageBox.Show("Veuillez sélectionner une tâche à modifier.");
                        return;
                    }
                    FormTache formTache = new FormTache(TacheActuelle, EtatFormulaire.Modifier);
                    if (formTache.ShowDialog() == true && formTache.Tache != null)
                    {
                        _mongoService.ModifierTache(formTache.Tache.Id, formTache.Tache);
                        ChargerTaches();

                        MessageBox.Show("Tâche modifiée avec succès.");
                    }
                    else
                    {
                        MessageBox.Show("La tâche n'a pas été modifiée.");
                    }

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de la modification de la tâches : {ex.Message}");

            }
        }

        private void SupprimerUneTache()
        {
            try
            {
                if (!estAdmin())
                {
                    MessageBox.Show("Vous n'avez pas les droits pour supprimer une tâche.");
                    return;
                }
                else
                {
                    if (TacheActuelle == null)
                    {
                        MessageBox.Show("Veuillez sélectionner une tâche à supprimer.");
                        return;
                    }
                    FormTache formTache = new FormTache(TacheActuelle, EtatFormulaire.Supprimer);
                    if (formTache.ShowDialog() == true && formTache.Tache != null)
                    {
                        _mongoService.SupprimerTache(formTache.Tache);
                        ChargerTaches();

                        MessageBox.Show("Tâche supprimée avec succès.");
                    }
                    else
                    {
                        MessageBox.Show("La tâche n'a pas été supprimée.");
                    }

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de la suppression de la tâches : {ex.Message}");

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

    /// <summary>
    /// Permet de savoir si l'utilisateur connecté est un admin
    /// </summary>
    /// <returns></returns>
    private bool estAdmin()
    {
        return UtilisateurConnecte.Role == "admin";
    }


    /// <summary>
    /// Permet de notifier le changement de propriété
    /// </summary>
    /// <param name="propertyName"></param>
    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}