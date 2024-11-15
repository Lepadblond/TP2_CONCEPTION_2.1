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
        }
    }

    public AccueilViewModel(MongoDBService mongoDBService, UserModel user)
    {
        _mongoService = mongoDBService;
        _utilisateurConnecte = user;
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

    public ICommand AjouterTacheCommand { get; }
    public ICommand ModifierUneTacheCommand { get; }

    public event PropertyChangedEventHandler? PropertyChanged;


    /// <summary>
    /// Charge les t�ches depuis la base de donn�es
    /// </summary>
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


    /// <summary>
    /// Permet d'ajouter une t�che
    /// </summary>
    private void AjouterTache()
    {
        try
        {
            if (!estAdmin())
            {
                MessageBox.Show("Vous n'avez pas les droits pour ajouter une t�che.");
            }
            else
            {
                var formTache = new FormTache();
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
    }


    /// <summary>
    /// Permet de modifier une t�che
    /// </summary>
    private void ModifierUneTache()
    {
        try
        {
            if (!estAdmin())
            {
                MessageBox.Show("Vous n'avez pas les droits pour ajouter une t�che.");
            }
            else
            {
                if (TacheActuelle == null)
                {
                    MessageBox.Show("Veuillez s�lectionner une t�che � modifier.");
                    return;
                }

                var formTache = new FormTache(TacheActuelle);
                if (formTache.ShowDialog() == true && formTache.Tache != null)
                {
                    _mongoService.ModifierTache(formTache.Tache);
                    ChargerTaches();
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


    /// <summary>
    /// Permet de supprimer une t�che
    /// </summary>
    private void SupprimerUneTache()
    {
    }



    /// <summary>
    /// Permet de filtrer les t�ches par date
    /// </summary>
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