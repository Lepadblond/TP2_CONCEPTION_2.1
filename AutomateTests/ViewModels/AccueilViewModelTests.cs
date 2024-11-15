using Microsoft.VisualStudio.TestTools.UnitTesting;
using Automate.ViewModels;
using Automate.Models;
using Automate.Utils;
using Moq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using MongoDB.Bson;

namespace Automate.Tests
{
    [TestClass]
    public class AccueilViewModelTests
    {
        private Mock<MongoDBService> _mockMongoService;
        private UserModel _testUser;
        private AccueilViewModel _viewModel;

        [TestInitialize]
        public void Setup()
        {
            _mockMongoService = new Mock<MongoDBService>("AutomateDB");

            _testUser = new UserModel { Username = "testuser", Role = "admin" };
            _viewModel = new AccueilViewModel(_testUser);
        }

        [TestMethod]
        public void Constructor_ShouldInitializeProperties()
        {
            // Arrange
            var expectedUser = _testUser;

            // Act
            var viewModel = new AccueilViewModel(expectedUser);

            // Assert
            Assert.IsNotNull(viewModel.ObservableCollectionDeTaches);
            Assert.AreEqual(expectedUser, viewModel.UtilisateurConnecte);
        }

        [TestMethod]
        public void AjouterTache_DoitAjouterTache_QuandUtilisateurEstAdmin()
        {
            // Arrange
            var nouvelleTache = new TacheModel { Titre = "Nouvelle Tâche" };
            _mockMongoService.Setup(s => s.AjouterTache(It.IsAny<TacheModel>())).Verifiable();

            // Act
            _viewModel.AjouterTacheCommand.Execute(null);

            // Assert
            _mockMongoService.Verify(s => s.AjouterTache(It.IsAny<TacheModel>()), Times.Once);
            Assert.IsTrue(_viewModel.ObservableCollectionDeTaches.Contains(nouvelleTache));
        }

        [TestMethod]
        public void ModifierUneTache_ShouldModifyTask_WhenUserIsAdmin()
        {
            // Arrange
            var existingTask = new TacheModel { Titre = "Existing Task" };
            _viewModel.TacheActuelle = existingTask;
            _mockMongoService.Setup(s => s.ModifierTache(It.IsAny<ObjectId>(), It.IsAny<TacheModel>()));

            // Act
            _viewModel.ModifierUneTacheCommand.Execute(null);

            // Assert
            _mockMongoService.Verify(s => s.ModifierTache(It.IsAny<ObjectId>(), It.IsAny<TacheModel>()), Times.Once);
        }

        [TestMethod]
        public void FiltrerTachesParDate_ShouldFilterTasksByDate()
        {
            // Arrange
            var date = new DateTime(2023, 10, 1);
            var filteredTasks = new List<TacheModel> { new TacheModel { Titre = "Filtered Task" } };
            _mockMongoService.Setup(s => s.FiltrerTachesParDate(date)).Returns(filteredTasks);

            // Act
            _viewModel.DateSelectionnee = date;

            // Assert
            CollectionAssert.AreEqual(filteredTasks, _viewModel.ObservableCollectionDeTaches);
        }
    }
}