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
using System.Data;

namespace Automate.ViewModels
{
    public class TacheViewModel : INotifyPropertyChanged
    {
        private string _titre;
        private string _description;
        private UserModel _utilisateurSelectionne;
        private DateTime _dateDebut;
        private DateTime _dateFin;
        private TaskType _type;
        private Enum.TaskStatus _status;
        private EtatFormulaire _etat;
        private TacheModel _tache;
        private bool _texteNonModifiable;
        private bool _boiteModifiable;

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

        public UserModel UtilisateurSelectionne
        {
            get => _utilisateurSelectionne;
            set
            {
                _utilisateurSelectionne = value;
                OnPropertyChanged(nameof(UtilisateurSelectionne));
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

        public bool TexteNonModifiable
        {
            get { return _texteNonModifiable; }
            set
            {
                if (_texteNonModifiable != value)
                {
                    _texteNonModifiable = value;
                    OnPropertyChanged(nameof(TexteNonModifiable));
                }
            }
        }

        public bool BoiteModifiable
        {
            get { return _boiteModifiable; }
            set
            {
                if (_boiteModifiable != value)
                {
                    _boiteModifiable = value;
                    OnPropertyChanged(nameof(BoiteModifiable));
                }
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

        public List<UserModel> Utilisateurs { get; set; }

        public TacheViewModel(TacheModel pTache = null, EtatFormulaire pEtat = EtatFormulaire.Ajouter)
        {

            _mongoService = new MongoDBService("AutomateDB");
            Tache = pTache;
            Etat = pEtat;
            InitialiserFormulaire();
        }

        private void InitialiserFormulaire()
        {
            Utilisateurs = _mongoService.ObtenirTousLesUtilisateurs().OrderBy(u => u.Username).ToList(); ;

            TaskTypes = System.Enum.GetValues(typeof(TaskType)).Cast<TaskType>().ToList();
            TaskStatuses = System.Enum.GetValues(typeof(Enum.TaskStatus)).Cast<Enum.TaskStatus>().ToList();

            AjouterModifierSupprimerCommand = new RelayCommand(AjouterModifierSupprimer);
            AnnulerCommand = new RelayCommand(Annuler);

            if (Etat == EtatFormulaire.Ajouter)
            {
                gererEtatChamps(Etat);
                AjouterModifierSupprimerButtonText = "Ajouter";
                TitreLabelText = "Ajouter une tâche";

                Titre = string.Empty;
                Description = string.Empty;
                DateDebut = DateTime.Now;
                DateFin = DateTime.Now;
                Type = TaskType.Semis;
                Status = Enum.TaskStatus.AFaire;
            } else
            {
                Titre = Tache.Titre ?? string.Empty;
                Description = Tache.Description ?? string.Empty;
                DateDebut = Tache.DateDebut ?? DateTime.Now;
                DateFin = Tache.DateFin ?? DateTime.Now;
                Type = Tache.Type;
                Status = Tache.Status;

                if (Tache.IdEmployeAffecte.HasValue)
                {
                    UtilisateurSelectionne = Utilisateurs.FirstOrDefault(u => u.Id == Tache.IdEmployeAffecte.Value);
                }

                if (Etat == EtatFormulaire.Modifier)
                {
                    gererEtatChamps(Etat);
                    AjouterModifierSupprimerButtonText = "Modifier";
                    TitreLabelText = "Modifier une tâche";
                }
                else
                {
                    gererEtatChamps(Etat);
                    AjouterModifierSupprimerButtonText = "Supprimer";
                    TitreLabelText = "Supprimer une tâche";
                }
            }

            OnPropertyChanged(nameof(AjouterModifierSupprimerButtonText));
            OnPropertyChanged(nameof(TitreLabelText));
        }

        private void gererEtatChamps(EtatFormulaire pEtat)
        {
            if (pEtat == EtatFormulaire.Supprimer)
            {
                TexteNonModifiable = true;
                BoiteModifiable = false;
            }
            else
            {
                TexteNonModifiable = false;
                BoiteModifiable = true;
            }
        }

        private void AjouterModifierSupprimer(object parameter)
        {

            if (Etat == EtatFormulaire.Ajouter)
            {
                if (ValiderTache(Etat))
                {
                    MessageBoxResult confirmation = MessageBox.Show("Voulez-vous vraiment ajouter la tâche?", "Ajout d'une tâche", MessageBoxButton.YesNo);
                    if (confirmation == MessageBoxResult.Yes)
                    {
                        try
                        {
                            var nouvelleTache = new TacheModel
                            {
                                Titre = Titre,
                                Description = Description,
                                IdEmployeAffecte = UtilisateurSelectionne.Id,
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
            else
            {
                if (Etat == EtatFormulaire.Modifier)
                {
                    try
                    {
                        if (ValiderTache(Etat))
                        {
                            MessageBoxResult confirmation = MessageBox.Show("Voulez-vous vraiment modifier la tâche?", "Modification d'une tâche", MessageBoxButton.YesNo);
                            if (confirmation == MessageBoxResult.Yes)
                            {
                                Tache.Titre = Titre;
                                Tache.Description = Description;
                                Tache.IdEmployeAffecte = UtilisateurSelectionne.Id;
                                Tache.DateDebut = DateDebut;
                                Tache.DateFin = DateFin;
                                Tache.Type = Type;
                                Tache.Status = Status;
                                Tache.DateDerniereModification = DateTime.Now;

                                FermerFenetre(parameter, true);
                            }
                        }
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                }
                else
                {
                    MessageBoxResult confirmation = MessageBox.Show("Voulez-vous vraiment supprimer la tâche?", "Suppression d'une tâche", MessageBoxButton.YesNo);
                    if (confirmation == MessageBoxResult.Yes)
                    {
                        FermerFenetre(parameter, true);
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

        private bool ValiderTache(EtatFormulaire pEtat)
        {
            try
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

                if (UtilisateurSelectionne == null)
                {
                    msgErreur += "- Le champ 'Employé' doit être sélectionné.\n";
                }

                if (DateDebut == null)
                {
                    msgErreur += "- Le champ 'Date de départ' doit être sélectionné.\n";
                }

                if (DateFin == null)
                {
                    msgErreur += "- Le champ 'Date de fin' doit être sélectionné.\n";
                }

                // Vérifer DateDebut < DateFin

                if (Type == null)
                {
                    msgErreur += "- Le champ 'Type' doit être sélectionné.\n";
                }

                if (Status == null)
                {
                    msgErreur += "- Le champ 'Statut' doit être sélectionné.\n";
                }

                if (msgErreur == "")
                {
                    return true;
                }
                else
                {
                    if (pEtat == EtatFormulaire.Ajouter)
                    {
                        MessageBox.Show(msgErreur, "Ajout d'un produit");
                    }
                    else
                    {
                        MessageBox.Show(msgErreur, "Modification d'un produit");
                    }
                    return false;
                }
            }
            catch (Exception)
            {
                throw;
            }

        }
    }
}
