using Automate.Enum;
using Automate.Models;
using Automate.Utils;
using System;
using System.ComponentModel;
using System.Windows.Input;

namespace Automate.ViewModels
{
    public class TacheViewModel : INotifyPropertyChanged
    {
        private readonly MongoDBService _mongoService;
        private TacheModel _tache;
        private EtatFormulaire _etat;

        public event PropertyChangedEventHandler PropertyChanged;

        public ICommand AddOrUpdateCommand { get; private set; }

        public TacheViewModel()
        {
            _mongoService = new MongoDBService("AutomateDB");
            _tache = new TacheModel();
            _etat = EtatFormulaire.Ajouter;

            AddOrUpdateCommand = new RelayCommand(AjouterTache);
        }

        public TacheModel Tache
        {
            get { return _tache; }
            set
            {
                _tache = value;
                OnPropertyChanged(nameof(Tache));
            }
        }

        public EtatFormulaire Etat
        {
            get { return _etat; }
            set
            {
                _etat = value;
                OnPropertyChanged(nameof(Etat));
            }
        }

        public string Titre
        {
            get { return _tache.Titre; }
            set
            {
                _tache.Titre = value;
                OnPropertyChanged(nameof(Titre));
            }
        }

        public string Description
        {
            get { return _tache.Description; }
            set
            {
                _tache.Description = value;
                OnPropertyChanged(nameof(Description));
            }
        }

        public DateTime? DateDebut
        {
            get { return _tache.DateDebut; }
            set
            {
                _tache.DateDebut = value;
                OnPropertyChanged(nameof(DateDebut));
            }
        }

        public DateTime? DateFin
        {
            get { return _tache.DateFin; }
            set
            {
                _tache.DateFin = value;
                OnPropertyChanged(nameof(DateFin));
            }
        }

        public TaskType Type
        {
            get { return _tache.Type; }
            set
            {
                _tache.Type = value;
                OnPropertyChanged(nameof(Type));
            }
        }

        public Enum.TaskStatus Status
        {
            get { return _tache.Status; }
            set
            {
                _tache.Status = value;
                OnPropertyChanged(nameof(Status));
            }
        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void AjouterTache()
        {
            _mongoService.AjouterTache(_tache);
        }
    }
}
