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
        public ICommand SupprimerTacheCommand { get; }

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


            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de la r�cup�ration des t�ches : {ex.Message}");
            }


        private void ModifierUneTache()
        {
            try
            {
                if (!estAdmin())
                {
                    MessageBox.Show("Vous n'avez pas les droits pour modifier une t�che.");
                    return;
                }
                else
                {
                    if (TacheActuelle == null)
                    {
                        MessageBox.Show("Veuillez s�lectionner une t�che � modifier.");
                        return;
                    }
                    FormTache formTache = new FormTache(TacheActuelle, EtatFormulaire.Modifier);
                    if (formTache.ShowDialog() == true && formTache.Tache != null)
                    {
                        _mongoService.ModifierTache(formTache.Tache.Id, formTache.Tache);
                        ChargerTaches();

                        MessageBox.Show("T�che modifi�e avec succ�s.");
                    }
                    else
                    {
                        MessageBox.Show("La t�che n'a pas �t� modifi�e.");
                    }

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de la modification de la t�ches : {ex.Message}");

            }
        }

        private void SupprimerUneTache()
        {
            try
            {
                if (!estAdmin())
                {
                    MessageBox.Show("Vous n'avez pas les droits pour supprimer une t�che.");
                    return;
                }
                else
                {
                    if (TacheActuelle == null)
                    {
                        MessageBox.Show("Veuillez s�lectionner une t�che � supprimer.");
                        return;
                    }
                    FormTache formTache = new FormTache(TacheActuelle, EtatFormulaire.Supprimer);
                    if (formTache.ShowDialog() == true && formTache.Tache != null)
                    {
                        _mongoService.SupprimerTache(formTache.Tache);
                        ChargerTaches();

                        MessageBox.Show("T�che supprim�e avec succ�s.");
                    }
                    else
                    {
                        MessageBox.Show("La t�che n'a pas �t� supprim�e.");
                    }

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de la suppression de la t�ches : {ex.Message}");

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
    /// Permet de savoir si l'utilisateur connect� est un admin
    /// </summary>
    /// <returns></returns>
    private bool estAdmin()
    {
        return UtilisateurConnecte.Role == "admin";
    }


    /// <summary>
    /// Permet de notifier le changement de propri�t�
    /// </summary>
    /// <param name="propertyName"></param>
    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}