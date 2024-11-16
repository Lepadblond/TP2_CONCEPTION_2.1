using Microsoft.VisualStudio.TestTools.UnitTesting;
using Automate.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Automate.Enum;
using Automate.Models;
using MongoDB.Bson;

namespace Automate.ViewModels.Tests
{
    [TestClass()]
    public class TacheViewModelTests
    {
        private TacheViewModel _viewModel;

        [TestInitialize]
        public void TestInitialize()
        {
            _viewModel = new TacheViewModel();
        }

        [TestMethod()]
        public void TacheViewModel_Initialise_Liste_Types()
        {
            Assert.IsNotNull(_viewModel.TaskTypes);
        }

        [TestMethod()]
        public void TacheViewModel_Initialise_Liste_Types_Non_Vide()
        {
            Assert.IsTrue(_viewModel.TaskTypes.Count > 0);
        }

        [TestMethod()]
        public void TacheViewModel_Initialise_Liste_Statuts()
        {
            Assert.IsNotNull(_viewModel.TaskStatuses);
        }

        [TestMethod()]
        public void TacheViewModel_Initialise_Liste_Statuts_Non_Vide()
        {
            Assert.IsTrue(_viewModel.TaskStatuses.Count > 0);
        }

        [TestMethod()]
        public void Champs_Titre_Est_Vide_Lorsque_EtatFormulaire_Ajouter()
        {
            Assert.AreEqual(string.Empty, _viewModel.Titre);
        }

        [TestMethod()]
        public void Champs_Description_Est_Vide_Lorsque_EtatFormulaire_Ajouter()
        {
            Assert.AreEqual(string.Empty, _viewModel.Description);
        }

        [TestMethod()]
        public void Champs_DateDebut_Affiche_Date_Aujourdhui_Lorsque_EtatFormulaire_Ajouter()
        {
            Assert.AreEqual(DateTime.Now.Date, _viewModel.DateDebut.Date);
        }

        [TestMethod()]
        public void Champs_DateFin_Affiche_Date_Aujourdhui_Lorsque_EtatFormulaire_Ajouter()
        {
            Assert.AreEqual(DateTime.Now.Date, _viewModel.DateFin.Date);
        }

        [TestMethod()]
        public void Champs_Type_Affiche_Semis_Lorsque_EtatFormulaire_Ajouter()
        {
            Assert.AreEqual(TaskType.Semis, _viewModel.Type);
        }

        [TestMethod()]
        public void Champs_Status_Affiche_AFaire_Lorsque_EtatFormulaire_Ajouter()
        {
            Assert.AreEqual(Enum.TaskStatus.AFaire, _viewModel.Status);
        }

        [TestMethod()]
        public void ValiderTache_Retourne_True_Quand_Champs_Corrects()
        {
            _viewModel.Titre = "Test Tache";
            _viewModel.Description = "Description valide";
            _viewModel.UtilisateurSelectionne = new UserModel { Id = ObjectId.GenerateNewId() };
            _viewModel.DateDebut = DateTime.Now;
            _viewModel.DateFin = DateTime.Now.AddDays(1);
            _viewModel.Type = TaskType.Semis;
            _viewModel.Status = Enum.TaskStatus.AFaire;

            var methode = typeof(TacheViewModel).GetMethod("ValiderTache", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            Assert.IsNotNull(methode); 

            var result = (bool)methode.Invoke(_viewModel, new object[] { EtatFormulaire.Ajouter });

            Assert.IsTrue(result);
        }

        [TestMethod()]
        public void ValiderTache_Retourne_False_Quand_Champs_Vides()
        {
            var methode = typeof(TacheViewModel).GetMethod("ValiderTache", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            Assert.IsNotNull(methode);

            var resultat = (bool)methode.Invoke(_viewModel, new object[] { EtatFormulaire.Ajouter });

            Assert.IsFalse(resultat);
        }

        [TestMethod()]
        public void ValiderTache_Retourne_False_Quand_DateFin_Avant_DateDebut()
        {
            _viewModel.Titre = "test";
            _viewModel.Description = "test";
            _viewModel.UtilisateurSelectionne = new UserModel { Id = ObjectId.GenerateNewId() };
            _viewModel.DateDebut = DateTime.Now.AddDays(1);
            _viewModel.DateFin = DateTime.Now;
            _viewModel.Type = TaskType.Semis;
            _viewModel.Status = Enum.TaskStatus.EnCours;

            var methode = typeof(TacheViewModel).GetMethod("ValiderTache", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            Assert.IsNotNull(methode);

            var resultat = (bool)methode.Invoke(_viewModel, new object[] { EtatFormulaire.Ajouter });

            Assert.IsFalse(resultat);
        }

    }
}