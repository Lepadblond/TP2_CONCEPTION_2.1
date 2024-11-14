using Automate.Models;
using Automate.Utils;
using Automate.Enum;
using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Windows;
using MongoDB.Driver;
using static System.Runtime.CompilerServices.RuntimeHelpers;
using System.Collections.Generic;
using System.Windows.Media.Imaging;

namespace Automate.Views
{
    public partial class FormTache : Window
    {

        private TacheModel _tache;

        /// <summary>
        /// L'état du formulaire.
        /// </summary>
        private EtatFormulaire _etat;

        public EtatFormulaire Etat
        {
            get { return _etat; }
            private set { _etat = value; }
        }

        public TacheModel Tache
        {
            get { return _tache; }
            private set { _tache = value; }
        }
        public FormTache(TacheModel pTache = null, EtatFormulaire pEtat = EtatFormulaire.Ajouter)
        {
            InitializeComponent();
            Tache = pTache ?? new TacheModel();  
            Etat = pEtat;
            InitialiserFormulaire();
        }

        private void InitialiserFormulaire()
        {
            RemplirComboBoxType();
            RemplirComboBoxStatut();

            btnAjouterModifierSupprimer.Content = Etat;
            lblTitre.Text = $"{Etat} un produit";

            if (Etat == EtatFormulaire.Ajouter)
            {
                Title = "Ajouter un produit";
                txtTitre.Clear();
                txtDescription.Clear();
                dpDateDebut.SelectedDate = null;
                dpDateFin.SelectedDate = null;
                cboType.SelectedItem = null;
                cboStatus.SelectedItem = null;

            }
            else
            {
                txtTitre.Clear();
                txtDescription.Clear();
                dpDateDebut.SelectedDate = null;
                cboType.SelectedItem = null;
                cboStatus.SelectedItem = null;

                if (Etat == EtatFormulaire.Modifier)
                {
                    Title = "Modifier un produit";
                }
                else
                {
                    Title = "Supprimer un produit";
                    txtTitre.IsEnabled = false;
                    txtDescription.IsEnabled = false;
                    dpDateDebut.IsEnabled = false;
                    cboType.IsEnabled = false;
                    cboStatus.IsEnabled = false;
                }
            }
        }


        public static string ObtenirDescriptionEnum(System.Enum value)
        {
            FieldInfo field = value.GetType().GetField(value.ToString());
            DescriptionAttribute attribute = (DescriptionAttribute)Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute));

            return attribute == null ? value.ToString() : attribute.Description;
        }

        private void RemplirComboBoxType()
        {
            cboType.ItemsSource = System.Enum.GetValues(typeof(TaskType))
                .Cast<TaskType>()
                .Select(x => new { Value = x, Description = ObtenirDescriptionEnum(x) })
                .ToList();

            cboType.DisplayMemberPath = "Description";
            cboType.SelectedValuePath = "Value";
        }

        private void RemplirComboBoxStatut()
        {
            cboStatus.ItemsSource = System.Enum.GetValues(typeof(TaskStatus))
                .Cast<TaskStatus>()
                .Select(x => new { Value = x, Description = ObtenirDescriptionEnum(x) })
                .ToList();

            cboStatus.DisplayMemberPath = "Description";
            cboStatus.SelectedValuePath = "Value";
        }
        private void btnAjouterModifierSupprimer_Click(object sender, RoutedEventArgs e)
        {
            if (Etat == EtatFormulaire.Ajouter)
            {
                try
                {
                    MessageBoxResult confirmation = MessageBox.Show("Voulez-vous vraiment ajouter la tâche?", "Ajout d'une tâche", MessageBoxButton.YesNo);
                    if (confirmation == MessageBoxResult.Yes)
                    {
                        if (ValiderTache(Etat))
                        {
                            TacheModel nouvelleTache = new TacheModel
                            {
                                Titre = txtTitre.Text,
                                Description = txtDescription.Text,
                                DateDebut = dpDateDebut.SelectedDate,
                                DateFin = dpDateFin.SelectedDate,
                                Type = (TaskType)cboType.SelectedValue,
                                Status = (TaskStatus)cboStatus.SelectedValue,
                                DateAjout = DateTime.Now,
                                DateDerniereModification = DateTime.Now
                            };

                            Tache = nouvelleTache;

                            this.DialogResult = true;
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
                if (Etat == EtatFormulaire.Modifier)
                {
                    try
                    {
                        MessageBoxResult confirmation = MessageBox.Show("Voulez-vous vraiment modifier la tâche?", "Modification d'une tâche", MessageBoxButton.YesNo);
                        if (confirmation == MessageBoxResult.Yes)
                        {

                            if (ValiderTache(Etat))
                            {
                                Tache.Titre = txtTitre.Text;
                                Tache.Description = txtDescription.Text;
                                Tache.DateDebut = dpDateDebut.SelectedDate;
                                Tache.DateFin = dpDateFin.SelectedDate;
                                Tache.Type = (TaskType)cboType.SelectedValue;
                                Tache.Status = (TaskStatus)cboStatus.SelectedValue;
                                Tache.DateDerniereModification = DateTime.Now;

                                this.DialogResult = true;
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
                        this.DialogResult = true;
                    }
                }
            }
        }

        private void btnAnnuler_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private bool ValiderTache(EtatFormulaire pEtat)
        {
            try
            {
                string msgErreur = "";

                if (string.IsNullOrWhiteSpace(txtTitre.Text))
                {
                    msgErreur += "- Le champ 'Titre' ne doit pas être vide.\n";
                }

                if (string.IsNullOrWhiteSpace(txtDescription.Text))
                {
                    msgErreur += "- Le champ 'Description' ne doit pas être vide.\n";
                }

                if (!dpDateDebut.SelectedDate.HasValue)
                {
                    msgErreur += "- Le champ 'Date de départ' doit être sélectionné.\n";
                }

                if (!dpDateFin.SelectedDate.HasValue)
                {
                    msgErreur += "- Le champ 'Date de fin' doit être sélectionné.\n";
                }
                else
                {
                    if (dpDateFin.SelectedDate < dpDateDebut.SelectedDate)
                    {
                        msgErreur += "- La 'Date de fin' ne peut pas être antérieure à la 'Date de début'.\n";
                    }
                }

                if (cboType.SelectedItem == null)
                {
                    msgErreur += "- Le champ 'Type' doit être sélectionné.\n";
                }

                if (cboStatus.SelectedItem == null)
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