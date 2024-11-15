using Automate.Models;
using Automate.Enum;
using System;
using System.ComponentModel;
using System.Windows.Input;
using System.Windows;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using Automate.Utils;

namespace Automate.ViewModels
{
    public class TacheViewModel : INotifyPropertyChanged
    {
        private string _titre;
        private string _description;
        private DateTime _dateDebut;
        private DateTime _dateFin;
        private TaskType _type;
        private Enum.TaskStatus _status;
        private EtatFormulaire _etat;
        private TacheModel _tache;

        public string Titre
        {
            get => _titre;
            set
            {
                _titre = value;
                OnPropertyChanged(nameof(Titre));
            }
        }

        public string Description
        {
            get => _description;
            set
            {
                _description = value;
                OnPropertyChanged(nameof(Description));
            }
        }

        public DateTime DateDebut
        {
            get => _dateDebut;
            set
            {
                _dateDebut = value;
                OnPropertyChanged(nameof(DateDebut));
            }
        }

        public DateTime DateFin
        {
            get => _dateFin;
            set
            {
                _dateFin = value;
                OnPropertyChanged(nameof(DateFin));
            }
        }

        public TaskType Type
        {
            get => _type;
            set
            {
                _type = value;
                OnPropertyChanged(nameof(Type));
            }
        }

        public Enum.TaskStatus Status
        {
            get => _status;
            set
            {
                _status = value;
                OnPropertyChanged(nameof(Status));
            }
        }
        
        public EtatFormulaire Etat
        {
            get => _etat;
            set
            {
                _etat = value;
                OnPropertyChanged(nameof(Etat));
            }
        }

        public TacheModel Tache
        {
            get => _tache;
            set
            {
                _tache = value;
                OnPropertyChanged(nameof(Tache));
            }
        }

        public List<TaskType> TaskTypes { get; set; }
        public List<Enum.TaskStatus> TaskStatuses { get; set; }


        public string AjouterModifierSupprimerButtonText { get; set; }
        public string TitreLabelText { get; set; }

        public ICommand AjouterModifierSupprimerCommand { get; set; }
        public ICommand AnnulerCommand { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        private readonly MongoDBService _mongoService;

        public TacheViewModel(TacheModel pTache = null, EtatFormulaire pEtat = EtatFormulaire.Ajouter)
        {
            Tache = pTache;
            Etat = pEtat;
            InitialiserFormulaire();
        }

        private void InitialiserFormulaire()
        {

            TaskTypes = System.Enum.GetValues(typeof(TaskType)).Cast<TaskType>().ToList();
            TaskStatuses = System.Enum.GetValues(typeof(Enum.TaskStatus)).Cast<Enum.TaskStatus>().ToList();

            AjouterModifierSupprimerCommand = new RelayCommand(AjouterModifierSupprimer);
            AnnulerCommand = new RelayCommand(Annuler);

            if (Etat == EtatFormulaire.Ajouter)
            {
                AjouterModifierSupprimerButtonText = "Ajouter";
                TitreLabelText = "Ajouter une tâche";

                Titre = string.Empty;
                Description = string.Empty;
                DateDebut = DateTime.Now;
                DateFin = DateTime.Now;
                Type = TaskType.Semis;
                Status = Enum.TaskStatus.AFaire;
            }
            //else if (Etat == EtatFormulaire.Modifier)
            //{
            //    // Logique pour modifier la tâche
            //    MessageBox.Show("Modifier la tâche !");
            //}
            //else if (Etat == EtatFormulaire.Supprimer)
            //{
            //    // Logique pour supprimer la tâche
            //    MessageBox.Show("Supprimer la tâche !");
            //}

            OnPropertyChanged(nameof(AjouterModifierSupprimerButtonText));
            OnPropertyChanged(nameof(TitreLabelText));
        }

        private void AjouterModifierSupprimer(object parameter)
        {

            if (Etat == EtatFormulaire.Ajouter)
            {
                MessageBoxResult confirmation = MessageBox.Show("Voulez-vous vraiment ajouter la tâche?", "Ajout d'une tâche", MessageBoxButton.YesNo);
                if (confirmation == MessageBoxResult.Yes)
                {
                    if (ValiderTache())
                    {
                        try
                        {
                            // TODO : ID EMPLOYÉ
                            var nouvelleTache = new TacheModel
                            {
                                Titre = Titre,
                                Description = Description,
                                DateDebut = DateDebut,
                                DateFin = DateFin,
                                Type = Type,
                                Status = Status,
                                DateAjout = DateTime.Now,
                                DateDerniereModification = DateTime.Now
                            };

                            Tache = nouvelleTache;

                            MessageBox.Show("Tâche ajoutée avec succès.");

                            FermerFenetre(parameter, true);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Erreur: {ex.Message}");
                        }
                    }
                }
            }
        }

        private void Annuler(object parameter)
        {
            MessageBoxResult result = MessageBox.Show("Êtes-vous certain de vouloir fermer la fenêtre ?", "Confirmation", MessageBoxButton.YesNo);

            if (result == MessageBoxResult.Yes)
            {
                FermerFenetre(parameter, false);
            }
        }

        private void FermerFenetre(object parameter, bool pDialogResult)
        {
            if (parameter is Window window)
            {
                window.DialogResult = pDialogResult;
                window.Close();
            }
        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private bool ValiderTache()
        {
            string msgErreur = "";

            if (string.IsNullOrWhiteSpace(Titre))
            {
                msgErreur += "- Le champ 'Nom' ne doit pas être vide.\n";
            }

            if (string.IsNullOrWhiteSpace(Description))
            {
                msgErreur += "- Le champ 'Description' ne doit pas être vide.\n";
            }

            if (DateDebut == null)
            {
                msgErreur += "- Le champ 'Date Début' doit être sélectionné.\n";
            }

            if (Type == null)
            {
                msgErreur += "- Le champ 'Type' doit être sélectionné.\n";
            }

            if (Status == null)
            {
                msgErreur += "- Le champ 'Statut' doit être sélectionné.\n";
            }

            if (!string.IsNullOrEmpty(msgErreur))
            {
                MessageBox.Show(msgErreur, "Erreur");
                return false;
            }
            return true;
        }
    }
}
